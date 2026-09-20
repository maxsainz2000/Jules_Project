Imports MySqlConnector
Imports MerchSys.App.Services

Namespace Data
    Public Class DatabaseInitializer

        Public Shared Sub Initialize(connectionString As String)
            Try
                Using conn As New MySqlConnection(connectionString)
                    conn.Open()

                    Dim tableCheckQuery = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = 'UserAccounts';"
                    Dim tableExists As Boolean
                    Using cmd As New MySqlCommand(tableCheckQuery, conn)
                        tableExists = Convert.ToInt32(cmd.ExecuteScalar()) > 0
                    End Using

                    If Not tableExists Then
                        Dim createTableQuery = "
                        CREATE TABLE UserAccounts (
                            Id INT AUTO_INCREMENT PRIMARY KEY,
                            Username VARCHAR(255) NOT NULL UNIQUE,
                            PasswordHash VARCHAR(255) NOT NULL,
                            Role INT NOT NULL,
                            IsActive BOOLEAN NOT NULL DEFAULT 1,
                            FailedLoginAttempts INT NOT NULL DEFAULT 0,
                            LockedUntil DATETIME NULL,
                            LastPasswordChangeAt DATETIME NULL,
                            CreatedAt DATETIME NOT NULL,
                            ModifiedAt DATETIME NULL,
                            CreatedBy VARCHAR(255) NULL,
                            ModifiedBy VARCHAR(255) NULL
                        );"
                        Using cmd As New MySqlCommand(createTableQuery, conn)
                            cmd.ExecuteNonQuery()
                        End Using

                        Dim hash = PasswordHashHelper.HashPassword("Vista2026!")

                        Dim insertQuery = "
                        INSERT INTO UserAccounts (Username, PasswordHash, Role, CreatedAt, IsActive)
                        VALUES
                            ('manager', @hash, 1, @now, 1),
                            ('owner', @hash, 2, @now, 1);"
                        Using cmd As New MySqlCommand(insertQuery, conn)
                            cmd.Parameters.AddWithValue("@hash", hash)
                            cmd.Parameters.AddWithValue("@now", DateTime.UtcNow)
                            cmd.ExecuteNonQuery()
                        End Using
                    End If
                End Using
            Catch ex As Exception
                ' Let the caller handle it or log it
                Console.WriteLine("Database initialization error: " & ex.Message)
            End Try
        End Sub

    End Class
End Namespace
