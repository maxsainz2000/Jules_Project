Imports System.Data
Imports MerchSys.SharedKernel.Data
Imports MerchSys.SharedKernel.Entities
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Interfaces
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.Logging
Imports MySqlConnector

Namespace Services
    Public Class AuthenticationService
        Implements IAuthenticationService

        Private ReadOnly _connectionString As String
        Private ReadOnly _logger As ILogger(Of AuthenticationService)
        Private ReadOnly _sessionService As ISessionService
        Private ReadOnly _writeContextScope As IWriteContextScope

        Public Sub New(configuration As IConfiguration, logger As ILogger(Of AuthenticationService), sessionService As ISessionService, writeContextScope As IWriteContextScope)
            ' Use the single resolved, pooling-enabled connection string so authentication
            ' hits exactly the same database as the EF contexts.
            _connectionString = MerchSys.App.Configuration.ConnectionStringLoader.GetMariaDbConnectionString(configuration, logger)
            _logger = logger
            _sessionService = sessionService
            _writeContextScope = writeContextScope
        End Sub

        Private Function MapUser(reader As MySqlDataReader) As UserAccount
            Return New UserAccount With {
                .Id = reader.GetInt32("Id"),
                .Username = reader.GetString("Username"),
                .PasswordHash = reader.GetString("PasswordHash"),
                .Role = CType(reader.GetInt32("Role"), UserRole),
                .IsActive = reader.GetBoolean("IsActive"),
                .FailedLoginAttempts = reader.GetInt32("FailedLoginAttempts"),
                .LockedUntil = If(reader.IsDBNull(reader.GetOrdinal("LockedUntil")), CType(Nothing, DateTime?), reader.GetDateTime("LockedUntil")),
                .LastPasswordChangeAt = If(reader.IsDBNull(reader.GetOrdinal("LastPasswordChangeAt")), CType(Nothing, DateTime?), reader.GetDateTime("LastPasswordChangeAt")),
                .CreatedAt = reader.GetDateTime("CreatedAt"),
                .ModifiedAt = If(reader.IsDBNull(reader.GetOrdinal("ModifiedAt")), CType(Nothing, DateTime?), reader.GetDateTime("ModifiedAt"))
            }
        End Function

        Private Const GenericCredentialsError As String = "Invalid username or password."
        Private Const MaxFailedAttempts As Integer = 5
        Private Const LockoutMinutes As Integer = 15

        Public Async Function AuthenticateAsync(username As String, password As String) As Task(Of AuthenticationResult) Implements IAuthenticationService.AuthenticateAsync
            If String.IsNullOrWhiteSpace(username) OrElse String.IsNullOrWhiteSpace(password) Then
                Return New AuthenticationResult With {.Success = False, .ErrorMessage = "Username and password are required."}
            End If

            Try
                Using conn As New MySqlConnection(_connectionString)
                    Await conn.OpenAsync()
                    Dim user As UserAccount = Nothing

                    Using cmd As New MySqlCommand("SELECT * FROM UserAccounts WHERE Username = @username", conn)
                        cmd.Parameters.AddWithValue("@username", username)
                        Using reader = Await cmd.ExecuteReaderAsync()
                            If Await reader.ReadAsync() Then
                                user = MapUser(reader)
                            End If
                        End Using
                    End Using

                    ' Never reveal whether an account exists (OWASP DA2 / verification Test 15).
                    If user Is Nothing Then
                        Return New AuthenticationResult With {.Success = False, .ErrorMessage = GenericCredentialsError}
                    End If

                    ' Lockout is evaluated before password verification so that even a correct
                    ' password during lockout reports the remaining time (verification Test 13).
                    If user.LockedUntil.HasValue AndAlso user.LockedUntil.Value > DateTime.UtcNow Then
                        Dim minutesLeft = Math.Max(1, CInt(Math.Ceiling((user.LockedUntil.Value - DateTime.UtcNow).TotalMinutes)))
                        Return New AuthenticationResult With {.Success = False, .ErrorMessage = $"Account locked. Try again in {minutesLeft} minute(s)."}
                    End If

                    If Not PasswordHashHelper.VerifyPassword(password, user.PasswordHash) Then
                        Dim attempts = user.FailedLoginAttempts + 1
                        Dim lockedUntil As DateTime? = Nothing
                        If attempts >= MaxFailedAttempts Then
                            lockedUntil = DateTime.UtcNow.AddMinutes(LockoutMinutes)
                        End If

                        Using scope = _writeContextScope.Enter(WriteContextKind.System)
                            DataWriteGuard.EnsureCanWrite(_sessionService, _writeContextScope, "failed login attempt counter")
                            Using updateCmd As New MySqlCommand("UPDATE UserAccounts SET FailedLoginAttempts = @attempts, LockedUntil = @lockedUntil WHERE Id = @id", conn)
                                updateCmd.Parameters.AddWithValue("@attempts", attempts)
                                updateCmd.Parameters.AddWithValue("@lockedUntil", If(lockedUntil.HasValue, lockedUntil.Value, CObj(DBNull.Value)))
                                updateCmd.Parameters.AddWithValue("@id", user.Id)
                                Await updateCmd.ExecuteNonQueryAsync()
                            End Using
                        End Using

                        If lockedUntil.HasValue Then
                            Return New AuthenticationResult With {.Success = False, .ErrorMessage = $"Account locked. Try again in {LockoutMinutes} minute(s)."}
                        End If
                        Return New AuthenticationResult With {.Success = False, .ErrorMessage = GenericCredentialsError}
                    End If

                    ' Password is correct. Only now is it safe to reject a disabled account
                    ' without leaking that the account is disabled (verification Test 15).
                    If Not user.IsActive Then
                        Return New AuthenticationResult With {.Success = False, .ErrorMessage = GenericCredentialsError}
                    End If

                    If user.FailedLoginAttempts > 0 OrElse user.LockedUntil.HasValue Then
                        Using scope = _writeContextScope.Enter(WriteContextKind.System)
                            DataWriteGuard.EnsureCanWrite(_sessionService, _writeContextScope, "login attempt counter reset")
                            Using updateCmd As New MySqlCommand("UPDATE UserAccounts SET FailedLoginAttempts = 0, LockedUntil = NULL WHERE Id = @id", conn)
                                updateCmd.Parameters.AddWithValue("@id", user.Id)
                                Await updateCmd.ExecuteNonQueryAsync()
                            End Using
                        End Using
                    End If

                    Dim requiresChange = Not user.LastPasswordChangeAt.HasValue
                    Return New AuthenticationResult With {
                        .Success = True,
                        .User = user,
                        .RequiresPasswordChange = requiresChange
                    }
                End Using
            Catch ex As MySqlException
                _logger.LogError(ex, "Database connection error during authentication")
                Return New AuthenticationResult With {.Success = False, .ErrorMessage = "Unable to reach central server. Please check your network connection."}
            Catch ex As Exception
                _logger.LogError(ex, "Error during authentication")
                Return New AuthenticationResult With {.Success = False, .ErrorMessage = "An error occurred during authentication."}
            End Try
        End Function

        Public Async Function ChangePasswordAsync(userId As Integer, newPassword As String) As Task(Of PasswordChangeResult) Implements IAuthenticationService.ChangePasswordAsync
            Try
                ' If the user is an owner, they can only change their own password
                If _sessionService.CurrentUserRole = UserRole.Owner Then
                    ' Verify that the ID corresponds to the logged-in user
                    Dim isOwnPassword As Boolean = False
                    Using connCheck As New MySqlConnection(_connectionString)
                        Await connCheck.OpenAsync()
                        Using checkCmd As New MySqlCommand("SELECT Username FROM UserAccounts WHERE Id = @id", connCheck)
                            checkCmd.Parameters.AddWithValue("@id", userId)
                            Dim usernameObj = Await checkCmd.ExecuteScalarAsync()
                            If usernameObj IsNot Nothing AndAlso Not DBNull.Value.Equals(usernameObj) Then
                                Dim targetUsername = usernameObj.ToString()
                                If String.Equals(targetUsername, _sessionService.CurrentUsername, StringComparison.OrdinalIgnoreCase) Then
                                    isOwnPassword = True
                                End If
                            End If
                        End Using
                    End Using

                    If Not isOwnPassword Then
                        Return New PasswordChangeResult With {.Success = False, .ErrorMessage = "Owner accounts are not permitted to change credentials of other users."}
                    End If
                End If

                Dim hash = PasswordHashHelper.HashPassword(newPassword)
                Using scope = _writeContextScope.Enter(WriteContextKind.AuthSelfService)
                    DataWriteGuard.EnsureCanWrite(_sessionService, _writeContextScope, "password change")
                    Using conn As New MySqlConnection(_connectionString)
                        Await conn.OpenAsync()
                        Using cmd As New MySqlCommand("UPDATE UserAccounts SET PasswordHash = @hash, LastPasswordChangeAt = @now, ModifiedAt = @now WHERE Id = @id", conn)
                            cmd.Parameters.AddWithValue("@hash", hash)
                            cmd.Parameters.AddWithValue("@now", DateTime.UtcNow)
                            cmd.Parameters.AddWithValue("@id", userId)
                            Dim rows = Await cmd.ExecuteNonQueryAsync()
                            If rows > 0 Then
                                Return New PasswordChangeResult With {.Success = True}
                            Else
                                Return New PasswordChangeResult With {.Success = False, .ErrorMessage = "User not found."}
                            End If
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                _logger.LogError(ex, "Error during password change")
                Return New PasswordChangeResult With {.Success = False, .ErrorMessage = "An error occurred."}
            End Try
        End Function

        Public Async Function ChangePasswordForFirstLoginAsync(username As String, currentPassword As String, newPassword As String) As Task(Of PasswordChangeResult) Implements IAuthenticationService.ChangePasswordForFirstLoginAsync
            Dim authResult = Await AuthenticateAsync(username, currentPassword)
            If Not authResult.Success Then
                Return New PasswordChangeResult With {.Success = False, .ErrorMessage = "Current password is incorrect."}
            End If

            Return Await ChangePasswordAsync(authResult.User.Id, newPassword)
        End Function

    End Class
End Namespace
