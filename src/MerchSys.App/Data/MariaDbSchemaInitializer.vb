Imports System.IO
Imports System.Reflection
Imports System.Security.Cryptography
Imports System.Text
Imports Microsoft.Extensions.Logging
Imports MySqlConnector
Imports System.Windows.Forms
Imports MerchSys.App.Services

Namespace Data
    Public Class MariaDbSchemaInitializer

        Public Shared Sub Initialize(connectionString As String, logger As ILogger)
            Try
                Using conn As New MySqlConnection(connectionString)
                    conn.Open()

                    ' Create migration history table if not exists
                    Dim historyTableCmd As New MySqlCommand("
                        CREATE TABLE IF NOT EXISTS `__SchemaHistory` (
                            `Id` INT NOT NULL AUTO_INCREMENT,
                            `ScriptName` VARCHAR(255) NOT NULL,
                            `Checksum` VARCHAR(64) NOT NULL,
                            `AppliedAt` DATETIME(6) NOT NULL,
                            PRIMARY KEY (`Id`),
                            UNIQUE KEY `UX_SchemaHistory_ScriptName` (`ScriptName`)
                        );", conn)
                    historyTableCmd.ExecuteNonQuery()

                    Dim assembly As Assembly = Assembly.GetExecutingAssembly()
                    Dim resourceNames = assembly.GetManifestResourceNames().
                        Where(Function(n) n.Contains("000")).
                        Where(Function(n) n.EndsWith(".sql")).
                        OrderBy(Function(n) n).
                        ToList()

                    For Each resourceName In resourceNames
                        Dim scriptName = resourceName.Substring(resourceName.LastIndexOf("."c, resourceName.Length - 5) + 1)

                        Dim scriptContent As String
                        Using stream As Stream = assembly.GetManifestResourceStream(resourceName)
                            Using reader As New StreamReader(stream)
                                scriptContent = reader.ReadToEnd()
                            End Using
                        End Using

                        Dim checksum = ComputeSha256(scriptContent.Replace(vbCrLf, vbLf))

                        ' Check if already applied
                        Dim checkCmd As New MySqlCommand("SELECT Checksum FROM `__SchemaHistory` WHERE ScriptName = @scriptName", conn)
                        checkCmd.Parameters.AddWithValue("@scriptName", scriptName)
                        Dim existingChecksum = TryCast(checkCmd.ExecuteScalar(), String)

                        If existingChecksum IsNot Nothing Then
                            If existingChecksum <> checksum Then
                                Dim msg = $"Schema drift detected in {scriptName}. Expected {existingChecksum}, found {checksum}."
                                logger?.LogCritical(msg)
                                MessageBox.Show(msg, "Schema Integrity Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Environment.Exit(1)
                            End If
                            Continue For ' Already applied and checksum matches
                        End If

                        ' Apply script in transaction
                        Using transaction = conn.BeginTransaction()
                            Try
                                ExecuteScript(conn, transaction, scriptContent)

                                Dim insertCmd As New MySqlCommand("INSERT INTO `__SchemaHistory` (ScriptName, Checksum, AppliedAt) VALUES (@scriptName, @checksum, @appliedAt)", conn, transaction)
                                insertCmd.Parameters.AddWithValue("@scriptName", scriptName)
                                insertCmd.Parameters.AddWithValue("@checksum", checksum)
                                insertCmd.Parameters.AddWithValue("@appliedAt", DateTime.UtcNow)
                                insertCmd.ExecuteNonQuery()

                                transaction.Commit()
                                logger?.LogInformation($"Successfully applied schema script: {scriptName}")
                            Catch ex As Exception
                                transaction.Rollback()
                                Dim msg = $"Failed to apply schema script: {scriptName}. Error: {ex.Message}"
                                logger?.LogCritical(ex, msg)
                                MessageBox.Show(msg, "Schema Bootstrap Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Environment.Exit(1)
                            End Try
                        End Using
                    Next

                    SeedSystemUsers(conn, logger)

                End Using
            Catch ex As Exception
                Dim msg = $"Database initialization error: {ex.Message}"
                logger?.LogCritical(ex, msg)
                MessageBox.Show(msg, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Environment.Exit(1)
            End Try
        End Sub

        Private Shared Sub ExecuteScript(conn As MySqlConnection, transaction As MySqlTransaction, scriptContent As String)
            Dim delimiter As String = ";"
            Dim statements = scriptContent.Split(New String() {delimiter}, StringSplitOptions.RemoveEmptyEntries)

            For Each statement In statements
                Dim trimmed = statement.Trim()
                If Not String.IsNullOrEmpty(trimmed) Then
                    Using cmd As New MySqlCommand(trimmed, conn, transaction)
                        cmd.ExecuteNonQuery()
                    End Using
                End If
            Next
        End Sub

        Private Shared Function ComputeSha256(input As String) As String
            Using sha256 As SHA256 = SHA256.Create()
                Dim bytes = Encoding.UTF8.GetBytes(input)
                Dim hashBytes = sha256.ComputeHash(bytes)
                Return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant()
            End Using
        End Function

        Private Shared Sub SeedSystemUsers(conn As MySqlConnection, logger As ILogger)
            Try
                ' Ensure UserAccounts table exists first by checking
                Dim tableCheckQuery = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = 'Sys_UserAccounts';"
                Dim tableExists As Boolean
                Using cmd As New MySqlCommand(tableCheckQuery, conn)
                    tableExists = Convert.ToInt32(cmd.ExecuteScalar()) > 0
                End Using

                If tableExists Then
                    Dim countCmd As New MySqlCommand("SELECT COUNT(*) FROM Sys_UserAccounts WHERE Username IN ('manager', 'owner')", conn)
                    Dim count As Integer = Convert.ToInt32(countCmd.ExecuteScalar())

                    If count = 0 Then
                        Dim hash = PasswordHashHelper.HashPassword("Vista2026!")

                        Dim insertQuery = "
                        INSERT INTO Sys_UserAccounts (Username, PasswordHash, Role, CreatedAt, IsActive)
                        VALUES
                            ('manager', @hash, 1, @now, 1),
                            ('owner', @hash, 2, @now, 1);"
                        Using cmd As New MySqlCommand(insertQuery, conn)
                            cmd.Parameters.AddWithValue("@hash", hash)
                            cmd.Parameters.AddWithValue("@now", DateTime.UtcNow)
                            cmd.ExecuteNonQuery()
                        End Using
                        logger?.LogInformation("Successfully seeded default system users.")
                    End If
                End If
            Catch ex As Exception
                logger?.LogWarning(ex, "Failed to seed system users.")
            End Try
        End Sub

    End Class
End Namespace
