Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Reflection
Imports Microsoft.Extensions.Logging
Imports MySqlConnector

Namespace Data

    ''' <summary>
    ''' Layer 3 of startup schema preparation: the immutability triggers on the append-only tables —
    ''' the two BIR retention archives (<c>Pos_OfficialReceiptArchive</c>,
    ''' <c>Pos_ReceiptIntegrityArchive</c>) and the audit log (<c>App_AuditLog</c>).
    '''
    ''' The class name predates the audit log and is now narrower than its job; the DDL it applies lives
    ''' in <c>Plans/VISTA_Modules/Infrastructure/sql/02-pos-receipt-integrity-triggers.sql</c>.
    '''
    ''' EF Core cannot create triggers, so they live in an embedded SQL script. They must run
    ''' <em>after</em> the EF model has created the archive tables, which is why this is a separate
    ''' step from <see cref="MariaDbSchemaInitializer"/> (Layer 1 — the versioned central
    ''' migrations) and runs after the module schemas (Layer 2).
    '''
    ''' Previously an operator had to run the script by hand after the first launch, and nothing
    ''' checked whether they had. A production database could therefore run with archive
    ''' immutability silently unenforced — a BIR compliance failure with no signal. The application
    ''' now applies the triggers itself (idempotently, only when they are missing) and verifies the
    ''' result, failing startup loudly if they still cannot be created (H3).
    '''
    ''' The triggers are the <b>only</b> enforcement of immutability that survives a client which
    ''' does not go through EF: <c>ImmutableReceiptInterceptor</c> is application code, bypassed by
    ''' direct SQL, by another client, and by EF bulk operations (which raise no SaveChanges
    ''' interceptor).
    '''
    ''' <c>Pos_OfficialReceiptArchive</c> was covered first. <c>Pos_ReceiptIntegrityArchive</c> is
    ''' the second BIR retention table and previously had no database-level protection at all — only
    ''' the interceptor, which is not enforcement.
    ''' </summary>
    Public NotInheritable Class ReceiptArchiveTriggerBootstrapper

        Private Sub New()
        End Sub

        ''' <summary>
        ''' The append-only tables these triggers guard: the two BIR retention archives and the audit
        ''' log.
        ''' </summary>
        Friend Shared ReadOnly ArchiveTables As String() = {
            "Pos_OfficialReceiptArchive",
            "Pos_ReceiptIntegrityArchive",
            "App_AuditLog"
        }

        ''' <summary>
        ''' The embedded DDL. The <c>.ddl</c> logical name deliberately does not end in <c>.sql</c>
        ''' so <see cref="MariaDbSchemaInitializer"/>'s "every embedded .sql is a versioned
        ''' migration" sweep does not apply it during Layer 1 — before the archive tables exist.
        ''' </summary>
        Friend Const TriggerScriptResource As String = "02-pos-receipt-integrity-triggers.ddl"

        ''' <summary>The triggers the append-only tables must carry.</summary>
        Public Shared ReadOnly RequiredTriggers As String() = {
            "pos_receipt_archive_no_update",
            "pos_receipt_archive_no_delete",
            "pos_receipt_integrity_archive_no_update",
            "pos_receipt_integrity_archive_no_delete",
            "app_audit_log_no_update",
            "app_audit_log_no_delete"
        }

        Public Shared Sub EnsureTriggers(connectionString As String, logger As ILogger)
            If String.IsNullOrWhiteSpace(connectionString) Then
                Throw New ArgumentException("A non-empty MariaDB connection string is required.", NameOf(connectionString))
            End If

            Using conn As New MySqlConnection(connectionString)
                conn.Open()

                If HasAllRequiredTriggers(ReadTriggerNames(conn)) Then
                    If logger IsNot Nothing Then
                        logger.LogInformation("BIR receipt-archive immutability triggers are present on {Tables}.", TablesForLog())
                    End If
                    Return
                End If

                If logger IsNot Nothing Then
                    logger.LogWarning("BIR receipt-archive immutability triggers are missing on {Tables}; applying them.", TablesForLog())
                End If

                ApplyTriggerScript(conn)

                Dim afterApply = ReadTriggerNames(conn)
                Dim missing = RequiredTriggers.Where(Function(t) Not afterApply.Contains(t)).ToList()
                If missing.Count > 0 Then
                    Dim message = $"BIR receipt-archive immutability is NOT enforced: trigger(s) {String.Join(", ", missing)} could not be created on {TablesForLog()}. NIRC sec. 235 immutability is unenforced — check that the database account holds the TRIGGER privilege."
                    If logger IsNot Nothing Then logger.LogError(message)
                    Throw New InvalidOperationException(message)
                End If

                If logger IsNot Nothing Then
                    logger.LogInformation("Applied BIR receipt-archive immutability triggers on {Tables}.", TablesForLog())
                End If
            End Using
        End Sub

        ''' <summary>
        ''' True when every required trigger is present. Pure, so the startup decision is testable
        ''' without a database. Name comparison is case-insensitive, matching MariaDB's treatment
        ''' of trigger names.
        ''' </summary>
        Public Shared Function HasAllRequiredTriggers(present As IEnumerable(Of String)) As Boolean
            If present Is Nothing Then Return False
            Dim presentSet = New HashSet(Of String)(present, StringComparer.OrdinalIgnoreCase)
            Return RequiredTriggers.All(Function(t) presentSet.Contains(t))
        End Function

        ''' <summary>
        ''' The trigger names currently defined in the connection's own schema, read from
        ''' <c>information_schema.TRIGGERS</c>. Trigger names are unique per schema, so the names
        ''' alone identify which of the required triggers are present.
        ''' </summary>
        Friend Shared Function ReadTriggerNames(conn As MySqlConnection) As HashSet(Of String)
            Dim names As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Using cmd = conn.CreateCommand()
                cmd.CommandText = "SELECT TRIGGER_NAME FROM information_schema.TRIGGERS WHERE TRIGGER_SCHEMA = DATABASE()"
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        names.Add(reader.GetString(0))
                    End While
                End Using
            End Using
            Return names
        End Function

        Private Shared Function TablesForLog() As String
            Return String.Join(", ", ArchiveTables)
        End Function

        Private Shared Sub ApplyTriggerScript(conn As MySqlConnection)
            Dim sql = ReadEmbeddedScript()
            For Each statement In MariaDbSchemaInitializer.SplitSqlStatements(sql)
                Using cmd = conn.CreateCommand()
                    cmd.CommandText = statement
                    cmd.ExecuteNonQuery()
                End Using
            Next
        End Sub

        ''' <summary>
        ''' Reads the trigger DDL from the embedded resource. <c>Friend</c> so a test can assert the
        ''' resource is actually embedded under the expected logical name — the failure mode when it
        ''' is not is a startup exception, which is exactly what this step exists to prevent.
        ''' </summary>
        Friend Shared Function ReadEmbeddedScript() As String
            Dim asm = Assembly.GetExecutingAssembly()
            Using stream = asm.GetManifestResourceStream(TriggerScriptResource)
                If stream Is Nothing Then
                    Throw New InvalidOperationException($"Embedded trigger script '{TriggerScriptResource}' was not found.")
                End If
                Using reader = New StreamReader(stream)
                    Return reader.ReadToEnd()
                End Using
            End Using
        End Function

    End Class

End Namespace
