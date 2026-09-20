Imports MerchSys.App.Configuration
Imports MerchSys.App.Services
Imports MerchSys.SharedKernel.Enums
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.Logging
Imports MySqlConnector

Namespace Data
    Public Class DatabaseInitializer
        Private ReadOnly _connectionString As String

        Public Sub New(config As IConfiguration, logger As ILogger(Of DatabaseInitializer))
            _connectionString = ConnectionStringLoader.GetMariaDbConnectionString(config, logger)
        End Sub

        Public Async Function InitializeAsync() As Task
            Using conn = New MySqlConnection(_connectionString)
                Await conn.OpenAsync()

                ' Ensure the _users table exists
                Dim createTableQuery = "
                    CREATE TABLE IF NOT EXISTS _users (
                        Id INT AUTO_INCREMENT PRIMARY KEY,
                        Username VARCHAR(255) NOT NULL UNIQUE,
                        PasswordHash VARCHAR(255) NOT NULL,
                        Role INT NOT NULL,
                        IsActive BOOLEAN NOT NULL DEFAULT 1,
                        FailedLoginAttempts INT NOT NULL DEFAULT 0,
                        LockedUntil DATETIME NULL,
                        LastPasswordChangeAt DATETIME NULL,
                        CreatedAt DATETIME NOT NULL,
                        CreatedBy VARCHAR(255),
                        ModifiedAt DATETIME NULL,
                        ModifiedBy VARCHAR(255)
                    )"
                Using cmd = New MySqlCommand(createTableQuery, conn)
                    Await cmd.ExecuteNonQueryAsync()
                End Using

                ' Seed Manager
                Await SeedUserAsync(conn, "manager", UserRole.Manager)
                ' Seed Owner
                Await SeedUserAsync(conn, "owner", UserRole.Owner)
            End Using
        End Function

        Private Async Function SeedUserAsync(conn As MySqlConnection, username As String, role As UserRole) As Task
            Dim checkQuery = "SELECT COUNT(1) FROM _users WHERE Username = @username"
            Using cmd = New MySqlCommand(checkQuery, conn)
                cmd.Parameters.AddWithValue("@username", username)
                Dim count = Convert.ToInt32(Await cmd.ExecuteScalarAsync())
                If count = 0 Then
                    Dim insertQuery = "
                        INSERT INTO _users (Username, PasswordHash, Role, IsActive, CreatedAt, CreatedBy)
                        VALUES (@username, @passwordHash, @role, 1, @now, 'System')"
                    Using insertCmd = New MySqlCommand(insertQuery, conn)
                        insertCmd.Parameters.AddWithValue("@username", username)
                        insertCmd.Parameters.AddWithValue("@passwordHash", PasswordHashHelper.HashPassword("Vista2026!"))
                        insertCmd.Parameters.AddWithValue("@role", CInt(role))
                        insertCmd.Parameters.AddWithValue("@now", DateTime.UtcNow)
                        Await insertCmd.ExecuteNonQueryAsync()
                    End Using
                End If
            End Using
        End Function
    End Class
End Namespace
