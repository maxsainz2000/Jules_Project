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

        ''' <summary>
        ''' Splits a migration script into individual statements.
        ''' Quote- and comment-aware: a delimiter that appears inside a string literal
        ''' (<c>'...'</c>), a quoted identifier (<c>"..."</c> or <c>`...`</c>), a line comment
        ''' (<c>--</c>, <c>#</c>) or a block comment (<c>/* ... */</c>) does NOT terminate a
        ''' statement. The <c>DELIMITER</c> directive (used to wrap trigger/procedure bodies) is
        ''' honoured and may appear on its own line anywhere in the script.
        ''' </summary>
        Friend Shared Function SplitSqlStatements(sql As String) As List(Of String)
            Dim statements As New List(Of String)()
            Dim sb As New StringBuilder()
            Dim delimiter As String = ";"

            Dim i As Integer = 0
            Dim n As Integer = sql.Length
            Dim inSingle As Boolean = False
            Dim inDouble As Boolean = False
            Dim inBacktick As Boolean = False
            Dim inLineComment As Boolean = False
            Dim inBlockComment As Boolean = False
            Dim atLineStart As Boolean = True

            While i < n
                Dim c As Char = sql(i)

                ' --- DELIMITER directive (line-level, outside literals/comments) ---
                If atLineStart AndAlso Not inSingle AndAlso Not inDouble AndAlso Not inBacktick AndAlso Not inLineComment AndAlso Not inBlockComment Then
                    Dim lineEndIdx = sql.IndexOfAny(New Char() {ChrW(10), ChrW(13)}, i)
                    Dim lineText = If(lineEndIdx < 0, sql.Substring(i), sql.Substring(i, lineEndIdx - i))
                    Dim trimmedLine = lineText.Trim()
                    If trimmedLine.StartsWith("DELIMITER ", StringComparison.OrdinalIgnoreCase) Then
                        delimiter = trimmedLine.Substring(10).Trim()
                        i = If(lineEndIdx < 0, n, lineEndIdx)
                        Continue While
                    End If
                End If

                ' --- newline: ends any line comment ---
                If c = ChrW(10) OrElse c = ChrW(13) Then
                    inLineComment = False
                    sb.Append(c)
                    i += 1
                    atLineStart = True
                    Continue While
                End If

                ' --- inside a line comment ---
                If inLineComment Then
                    sb.Append(c)
                    i += 1
                    atLineStart = False
                    Continue While
                End If

                ' --- inside a block comment ---
                If inBlockComment Then
                    If c = "*"c AndAlso i + 1 < n AndAlso sql(i + 1) = "/"c Then
                        inBlockComment = False
                        sb.Append("*/")
                        i += 2
                    Else
                        sb.Append(c)
                        i += 1
                    End If
                    atLineStart = False
                    Continue While
                End If

                ' --- inside a single-quoted string literal ---
                If inSingle Then
                    sb.Append(c)
                    If c = "\"c AndAlso i + 1 < n Then
                        sb.Append(sql(i + 1))
                        i += 2
                    ElseIf c = "'"c Then
                        If i + 1 < n AndAlso sql(i + 1) = "'"c Then
                            sb.Append("'")
                            i += 2
                        Else
                            inSingle = False
                            i += 1
                        End If
                    Else
                        i += 1
                    End If
                    atLineStart = False
                    Continue While
                End If

                ' --- inside a double-quoted identifier ---
                If inDouble Then
                    sb.Append(c)
                    i += 1
                    If c = """"c Then inDouble = False
                    atLineStart = False
                    Continue While
                End If

                ' --- inside a backtick identifier ---
                If inBacktick Then
                    sb.Append(c)
                    i += 1
                    If c = "`"c Then inBacktick = False
                    atLineStart = False
                    Continue While
                End If

                ' --- normal context: detect comment/literal openers ---
                If c = "-"c AndAlso i + 1 < n AndAlso sql(i + 1) = "-"c Then
                    inLineComment = True
                    sb.Append("--")
                    i += 2
                    atLineStart = False
                    Continue While
                End If
                If c = "#"c Then
                    inLineComment = True
                    sb.Append(c)
                    i += 1
                    atLineStart = False
                    Continue While
                End If
                If c = "/"c AndAlso i + 1 < n AndAlso sql(i + 1) = "*"c Then
                    inBlockComment = True
                    sb.Append("/*")
                    i += 2
                    atLineStart = False
                    Continue While
                End If
                If c = "'"c Then
                    inSingle = True
                    sb.Append(c)
                    i += 1
                    atLineStart = False
                    Continue While
                End If
                If c = """"c Then
                    inDouble = True
                    sb.Append(c)
                    i += 1
                    atLineStart = False
                    Continue While
                End If
                If c = "`"c Then
                    inBacktick = True
                    sb.Append(c)
                    i += 1
                    atLineStart = False
                    Continue While
                End If

                ' --- delimiter check (only reached outside literals/comments) ---
                If delimiter.Length > 0 AndAlso MatchesAt(sql, i, delimiter) Then
                    Dim stmt = sb.ToString().Trim()
                    If stmt.Length > 0 Then statements.Add(stmt)
                    sb.Clear()
                    i += delimiter.Length
                    atLineStart = False
                    Continue While
                End If

                sb.Append(c)
                If c <> " "c AndAlso c <> ChrW(9) Then atLineStart = False
                i += 1
            End While

            Dim remaining = sb.ToString().Trim()
            If remaining.Length > 0 Then statements.Add(remaining)

            Return statements
        End Function

        Private Shared Function MatchesAt(text As String, index As Integer, token As String) As Boolean
            If token.Length = 0 OrElse index + token.Length > text.Length Then Return False
            Return String.Compare(text, index, token, 0, token.Length, StringComparison.Ordinal) = 0
        End Function

        Private Shared Function ComputeSha256(input As String) As String
            Using shaAlg = SHA256.Create()
                Dim bytes = Encoding.UTF8.GetBytes(input)
                Dim hashBytes = shaAlg.ComputeHash(bytes)
                Return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant()
            End Using
        End Function

        Private Shared Sub EnsureDeveloperAccount(conn As MySqlConnection)
            ' Idempotent: INSERT IGNORE means two clients starting simultaneously cannot
            ' raise a duplicate-key error and abort startup (M14).
            Dim hash = PasswordHashHelper.HashPassword("VistaDeveloper2026!")
            Dim insertSql = "
                INSERT IGNORE INTO `UserAccounts` (`Username`, `PasswordHash`, `Role`, `CreatedAt`, `IsActive`)
                VALUES ('developer', @hash, 3, @now, 1);"
            Using cmd = New MySqlCommand(insertSql, conn)
                cmd.Parameters.AddWithValue("@hash", hash)
                cmd.Parameters.AddWithValue("@now", DateTime.UtcNow)
                cmd.ExecuteNonQuery()
            End Using
        End Sub

        Private Shared Sub EnsureDefaultAccounts(conn As MySqlConnection)
            Dim checkSql = "SELECT COUNT(*) FROM `UserAccounts` WHERE `Username` IN ('manager', 'owner')"
            Dim count As Integer
            Using cmd = New MySqlCommand(checkSql, conn)
                count = Convert.ToInt32(cmd.ExecuteScalar())
            End Using

            If count < 2 Then
                ' Each account gets its own salt: hashing once and reusing the value would
                ' give manager and owner identical stored hashes (H8).
                Dim managerHash = PasswordHashHelper.HashPassword("Vista2026!")
                Dim ownerHash = PasswordHashHelper.HashPassword("Vista2026!")

                Dim insertSqlManager = "
                    INSERT IGNORE INTO `UserAccounts` (`Username`, `PasswordHash`, `Role`, `CreatedAt`, `IsActive`)
                    VALUES ('manager', @hash, 1, @now, 1);"
                Using cmd = New MySqlCommand(insertSqlManager, conn)
                    cmd.Parameters.AddWithValue("@hash", managerHash)
                    cmd.Parameters.AddWithValue("@now", DateTime.UtcNow)
                    cmd.ExecuteNonQuery()
                End Using

                Dim insertSqlOwner = "
                    INSERT IGNORE INTO `UserAccounts` (`Username`, `PasswordHash`, `Role`, `CreatedAt`, `IsActive`)
                    VALUES ('owner', @hash, 2, @now, 1);"
                Using cmd = New MySqlCommand(insertSqlOwner, conn)
                    cmd.Parameters.AddWithValue("@hash", ownerHash)
                    cmd.Parameters.AddWithValue("@now", DateTime.UtcNow)
                    cmd.ExecuteNonQuery()
                End Using
            End If
        End Sub

    End Class
End Namespace
