Imports System.IO
Imports System.Reflection
Imports System.Security.Cryptography
Imports System.Text
Imports Microsoft.Extensions.Logging
Imports MySqlConnector
Imports MerchSys.App.Services

Namespace Data
    Public Class MariaDbSchemaInitializer
        Private Const SchemaTable As String = "App_SchemaHistory"

        Public Shared Sub Initialize(connectionString As String, logger As ILogger)
            Try
                Using conn As New MySqlConnection(connectionString)
                    conn.Open()

                    ' Ensure schema history table exists
                    EnsureSchemaHistoryTable(conn)

                    ' Get embedded SQL resources
                    Dim asm = Assembly.GetExecutingAssembly()
                    Dim resources = asm.GetManifestResourceNames().
                                    Where(Function(r) r.EndsWith(".sql")).
                                    OrderBy(Function(r) r).
                                    ToList()

                    If Not resources.Any() Then
                        If logger IsNot Nothing Then logger.LogWarning("No SQL migration files found in embedded resources.")
                        Return
                    End If

                    For Each resourceName In resources
                        Using stream = asm.GetManifestResourceStream(resourceName)
                            If stream Is Nothing Then Continue For
                            Using reader = New StreamReader(stream)
                                Dim sql = reader.ReadToEnd()
                                Dim hash = ComputeSha256(sql)

                                ' Check if already applied and hash matches
                                Dim existingHash = GetAppliedHash(conn, resourceName)
                                If existingHash IsNot Nothing Then
                                    If existingHash <> hash Then
                                        Throw New InvalidOperationException($"Tamper detection: Script '{resourceName}' hash mismatch. Expected {existingHash}, found {hash}.")
                                    End If
                                    Continue For ' Skip if already applied and hash matches
                                End If

                                ' Apply new script
                                ApplyScript(conn, sql, resourceName, hash, logger)
                            End Using
                        End Using
                    Next

                    ' Seed Accounts
                    EnsureDeveloperAccount(conn)
                    EnsureDefaultAccounts(conn)

                End Using
            Catch ex As Exception
                If logger IsNot Nothing Then logger.LogError(ex, "Failed to initialize MariaDB schema.")
                Throw ' Propagate to caller (Program.vb) to show UI error
            End Try
        End Sub

        Private Shared Sub EnsureSchemaHistoryTable(conn As MySqlConnection)
            Dim sql = $"
                CREATE TABLE IF NOT EXISTS `{SchemaTable}` (
                    `Id` INT AUTO_INCREMENT PRIMARY KEY,
                    `ScriptName` VARCHAR(255) NOT NULL UNIQUE,
                    `ScriptHash` VARCHAR(64) NOT NULL,
                    `AppliedAt` DATETIME(6) NOT NULL
                );"
            Using cmd = New MySqlCommand(sql, conn)
                cmd.ExecuteNonQuery()
            End Using
        End Sub

        Private Shared Function GetAppliedHash(conn As MySqlConnection, scriptName As String) As String
            Dim sql = $"SELECT `ScriptHash` FROM `{SchemaTable}` WHERE `ScriptName` = @name"
            Using cmd = New MySqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@name", scriptName)
                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso Not DBNull.Value.Equals(result) Then
                    Return result.ToString()
                End If
            End Using
            Return Nothing
        End Function

        Private Shared Sub ApplyScript(conn As MySqlConnection, sql As String, resourceName As String, hash As String, logger As ILogger)
            If logger IsNot Nothing Then logger.LogInformation($"Applying migration script: {resourceName}")

            Using transaction = conn.BeginTransaction()
                Try
                    ' Custom statement splitting to support DELIMITER blocks
                    Dim statements = SplitSqlStatements(sql)
                    For Each stmt In statements
                        Using cmd = New MySqlCommand(stmt, conn, transaction)
                            cmd.ExecuteNonQuery()
                        End Using
                    Next

                    ' Record execution
                    Dim recordSql = $"INSERT INTO `{SchemaTable}` (`ScriptName`, `ScriptHash`, `AppliedAt`) VALUES (@name, @hash, @now)"
                    Using cmd = New MySqlCommand(recordSql, conn, transaction)
                        cmd.Parameters.AddWithValue("@name", resourceName)
                        cmd.Parameters.AddWithValue("@hash", hash)
                        cmd.Parameters.AddWithValue("@now", DateTime.UtcNow)
                        cmd.ExecuteNonQuery()
                    End Using

                    transaction.Commit()
                Catch ex As Exception
                    transaction.Rollback()
                    Throw New InvalidOperationException($"Failed to apply script '{resourceName}'. Transaction rolled back.", ex)
                End Try
            End Using
        End Sub

        Private Shared Function SplitSqlStatements(sql As String) As List(Of String)
            Dim statements As New List(Of String)()
            Dim currentDelimiter As String = ";"
            Dim lines = sql.Split(New String() {Environment.NewLine, vbLf, vbCr}, StringSplitOptions.None)
            Dim currentStatement As New StringBuilder()

            For Each line In lines
                Dim trimmedLine = line.Trim()

                If trimmedLine.StartsWith("DELIMITER ", StringComparison.OrdinalIgnoreCase) Then
                    currentDelimiter = trimmedLine.Substring(10).Trim()
                    Continue For
                End If

                If trimmedLine.EndsWith(currentDelimiter, StringComparison.OrdinalIgnoreCase) Then
                    Dim withoutDelimiter = line.Substring(0, line.LastIndexOf(currentDelimiter))
                    currentStatement.AppendLine(withoutDelimiter)

                    Dim finalStmt = currentStatement.ToString().Trim()
                    If Not String.IsNullOrEmpty(finalStmt) Then
                        statements.Add(finalStmt)
                    End If
                    currentStatement.Clear()
                Else
                    currentStatement.AppendLine(line)
                End If
            Next

            Dim remainingStmt = currentStatement.ToString().Trim()
            If Not String.IsNullOrEmpty(remainingStmt) Then
                statements.Add(remainingStmt)
            End If

            Return statements
        End Function

        Private Shared Function ComputeSha256(input As String) As String
            Using shaAlg = SHA256.Create()
                Dim bytes = Encoding.UTF8.GetBytes(input)
                Dim hashBytes = shaAlg.ComputeHash(bytes)
                Return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant()
            End Using
        End Function

        Private Shared Sub EnsureDeveloperAccount(conn As MySqlConnection)
            ' Ensure the Developer role exists (Id 3 per UserRole enum)
            Dim checkSql = "SELECT COUNT(*) FROM `UserAccounts` WHERE `Username` = 'developer'"
            Dim count As Integer
            Using cmd = New MySqlCommand(checkSql, conn)
                count = Convert.ToInt32(cmd.ExecuteScalar())
            End Using

            If count = 0 Then
                Dim hash = PasswordHashHelper.HashPassword("VistaDeveloper2026!")
                Dim insertSql = "
                    INSERT INTO `UserAccounts` (`Username`, `PasswordHash`, `Role`, `CreatedAt`, `IsActive`)
                    VALUES ('developer', @hash, 3, @now, 1);"
                Using cmd = New MySqlCommand(insertSql, conn)
                    cmd.Parameters.AddWithValue("@hash", hash)
                    cmd.Parameters.AddWithValue("@now", DateTime.UtcNow)
                    cmd.ExecuteNonQuery()
                End Using
            End If
        End Sub

        Private Shared Sub EnsureDefaultAccounts(conn As MySqlConnection)
            Dim checkSql = "SELECT COUNT(*) FROM `UserAccounts` WHERE `Username` IN ('manager', 'owner')"
            Dim count As Integer
            Using cmd = New MySqlCommand(checkSql, conn)
                count = Convert.ToInt32(cmd.ExecuteScalar())
            End Using

            If count < 2 Then
                Dim hash = PasswordHashHelper.HashPassword("Vista2026!")

                Dim insertSqlManager = "
                    INSERT IGNORE INTO `UserAccounts` (`Username`, `PasswordHash`, `Role`, `CreatedAt`, `IsActive`)
                    VALUES ('manager', @hash, 1, @now, 1);"
                Using cmd = New MySqlCommand(insertSqlManager, conn)
                    cmd.Parameters.AddWithValue("@hash", hash)
                    cmd.Parameters.AddWithValue("@now", DateTime.UtcNow)
                    cmd.ExecuteNonQuery()
                End Using

                Dim insertSqlOwner = "
                    INSERT IGNORE INTO `UserAccounts` (`Username`, `PasswordHash`, `Role`, `CreatedAt`, `IsActive`)
                    VALUES ('owner', @hash, 2, @now, 1);"
                Using cmd = New MySqlCommand(insertSqlOwner, conn)
                    cmd.Parameters.AddWithValue("@hash", hash)
                    cmd.Parameters.AddWithValue("@now", DateTime.UtcNow)
                    cmd.ExecuteNonQuery()
                End Using
            End If
        End Sub

    End Class
End Namespace
