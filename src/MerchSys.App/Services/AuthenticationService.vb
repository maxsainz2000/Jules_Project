Imports System.Security.Cryptography
Imports System.Text
Imports Konscious.Security.Cryptography
Imports MerchSys.App.Configuration
Imports MerchSys.SharedKernel.Entities
Imports MerchSys.SharedKernel.Enums
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.Logging
Imports MySqlConnector

Namespace Services
    Friend Module PasswordHashHelper
        Public Function HashPassword(password As String) As String
            Dim salt As Byte() = New Byte(15) {}
            Using rng = RandomNumberGenerator.Create()
                rng.GetBytes(salt)
            End Using

            Dim argon2 = New Argon2id(Encoding.UTF8.GetBytes(password))
            argon2.Salt = salt
            argon2.DegreeOfParallelism = 1
            argon2.MemorySize = 19456
            argon2.Iterations = 2

            Dim hash As Byte() = argon2.GetBytes(32)
            Return Convert.ToBase64String(salt) & ":" & Convert.ToBase64String(hash)
        End Function

        Public Function VerifyPassword(password As String, hashString As String) As Boolean
            Try
                Dim parts = hashString.Split(":"c)
                If parts.Length <> 2 Then Return False

                Dim salt = Convert.FromBase64String(parts(0))
                Dim expectedHash = Convert.FromBase64String(parts(1))

                Dim argon2 = New Argon2id(Encoding.UTF8.GetBytes(password))
                argon2.Salt = salt
                argon2.DegreeOfParallelism = 1
                argon2.MemorySize = 19456
                argon2.Iterations = 2

                Dim actualHash As Byte() = argon2.GetBytes(32)
                Return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash)
            Catch
                Return False
            End Try
        End Function
    End Module

    Public Class AuthenticationService
        Implements IAuthenticationService

        Private ReadOnly _connectionString As String

        Public Sub New(config As IConfiguration, logger As ILogger(Of AuthenticationService))
            _connectionString = ConnectionStringLoader.GetMariaDbConnectionString(config, logger)
        End Sub

        Public Async Function AuthenticateAsync(username As String, password As String) As Task(Of AuthenticationResult) Implements IAuthenticationService.AuthenticateAsync
            Using conn = New MySqlConnection(_connectionString)
                Await conn.OpenAsync()

                Dim query = "SELECT Id, Username, PasswordHash, Role, IsActive, FailedLoginAttempts, LockedUntil, LastPasswordChangeAt, CreatedAt, ModifiedAt FROM _users WHERE Username = @username"
                Using cmd = New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@username", username)
                    Using reader = Await cmd.ExecuteReaderAsync()
                        If Not Await reader.ReadAsync() Then
                            Return New AuthenticationResult With {.Success = False, .ErrorMessage = "Invalid username or password."}
                        End If

                        Dim user = New UserAccount() With {
                            .Id = reader.GetInt32("Id"),
                            .Username = reader.GetString("Username"),
                            .PasswordHash = reader.GetString("PasswordHash"),
                            .Role = CType(reader.GetInt32("Role"), UserRole),
                            .IsActive = reader.GetBoolean("IsActive"),
                            .FailedLoginAttempts = reader.GetInt32("FailedLoginAttempts"),
                            .LockedUntil = If(reader.IsDBNull(reader.GetOrdinal("LockedUntil")), DirectCast(Nothing, DateTime?), reader.GetDateTime("LockedUntil")),
                            .LastPasswordChangeAt = If(reader.IsDBNull(reader.GetOrdinal("LastPasswordChangeAt")), DirectCast(Nothing, DateTime?), reader.GetDateTime("LastPasswordChangeAt"))
                        }

                        If Not user.IsActive Then
                            Return New AuthenticationResult With {.Success = False, .ErrorMessage = "Account is disabled."}
                        End If

                        If user.LockedUntil.HasValue AndAlso user.LockedUntil.Value > DateTime.UtcNow Then
                            Return New AuthenticationResult With {.Success = False, .ErrorMessage = "Account is locked."}
                        End If

                        If PasswordHashHelper.VerifyPassword(password, user.PasswordHash) Then
                            Return New AuthenticationResult With {
                                .Success = True,
                                .User = user,
                                .RequiresPasswordChange = Not user.LastPasswordChangeAt.HasValue
                            }
                        Else
                            Return New AuthenticationResult With {.Success = False, .ErrorMessage = "Invalid username or password."}
                        End If
                    End Using
                End Using
            End Using
        End Function

        Public Async Function ChangePasswordAsync(userId As Integer, newPassword As String) As Task(Of PasswordChangeResult) Implements IAuthenticationService.ChangePasswordAsync
            Try
                Dim newHash = PasswordHashHelper.HashPassword(newPassword)
                Using conn = New MySqlConnection(_connectionString)
                    Await conn.OpenAsync()
                    Dim cmd = New MySqlCommand("UPDATE _users SET PasswordHash = @hash, LastPasswordChangeAt = @now WHERE Id = @id", conn)
                    cmd.Parameters.AddWithValue("@hash", newHash)
                    cmd.Parameters.AddWithValue("@now", DateTime.UtcNow)
                    cmd.Parameters.AddWithValue("@id", userId)
                    Await cmd.ExecuteNonQueryAsync()
                End Using
                Return New PasswordChangeResult With {.Success = True}
            Catch ex As Exception
                Return New PasswordChangeResult With {.Success = False, .ErrorMessage = "Failed to change password."}
            End Try
        End Function
    End Class
End Namespace
