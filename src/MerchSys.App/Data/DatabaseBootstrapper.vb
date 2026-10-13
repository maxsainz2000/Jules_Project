Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Infrastructure
Imports Microsoft.EntityFrameworkCore.Storage
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Logging
Imports MySqlConnector
Imports MerchSys.Accounting.Data
Imports MerchSys.Inventory.Data
Imports MerchSys.POS.Data
Imports MerchSys.Purchasing.Data

Namespace Data

    ''' <summary>
    ''' The single startup schema entry point for the VISTA client-server deployment.
    '''
    ''' There is exactly one place that decides how the central database is created, so the
    ''' previous situation — two independent mechanisms (embedded versioned SQL *and* EF
    ''' <c>EnsureCreated</c> plus duplicated inline <c>ALTER TABLE</c> DDL) masking each other —
    ''' can no longer occur.
    '''
    ''' Two clearly separated layers:
    ''' <list type="number">
    '''   <item>Central reference schema (versioned, hash-checked embedded SQL) via
    '''     <see cref="MariaDbSchemaInitializer"/> — owns the central auth tables and seeding.</item>
    '''   <item>Module schemas, derived from the EF model, which is the single source of truth
    '''     for the <c>Pur_/Inv_/Pos_/Acc_</c> tables.</item>
    ''' </list>
    ''' </summary>
    Public NotInheritable Class DatabaseBootstrapper

        Private Sub New()
        End Sub

        Public Shared Sub Initialize(connectionString As String, serviceProvider As IServiceProvider, logger As ILogger)
            If String.IsNullOrWhiteSpace(connectionString) Then
                Throw New ArgumentException("A non-empty MariaDB connection string is required.", NameOf(connectionString))
            End If

            ' Layer 1 — central schema + reference data (versioned embedded SQL).
            MariaDbSchemaInitializer.Initialize(connectionString, logger)

            ' Layer 2 — module schemas from the EF model.
            EnsureModuleSchemas(connectionString, serviceProvider, logger)

            ' Layer 3 — BIR receipt-archive immutability triggers (H3). This must follow Layer 2:
            ' the archive table they guard is created there. EF Core cannot create triggers, so they
            ' are applied from an embedded script and then verified; a failure here is surfaced to
            ' the operator rather than leaving NIRC sec. 235 immutability silently unenforced.
            ReceiptArchiveTriggerBootstrapper.EnsureTriggers(connectionString, logger)
        End Sub

        ''' <summary>
        ''' Creates the module tables from the EF model.
        '''
        ''' <c>Database.EnsureCreated()</c> cannot be used for this. It only calls
        ''' <c>CreateTables()</c> when the database contains no tables at all, and Layer 1 always
        ''' creates the central tables first — so <c>EnsureCreated()</c> reported success while
        ''' silently creating nothing. Each context is therefore materialised through
        ''' <see cref="IRelationalDatabaseCreator.CreateTables()"/>, which has no such
        ''' short-circuit, guarded by a check that none of that context's tables exist yet.
        ''' </summary>
        Private Shared Sub EnsureModuleSchemas(connectionString As String, serviceProvider As IServiceProvider, logger As ILogger)
            ' Layer 2 failures must reach the log, not just the startup dialog: a client machine
            ' cannot be diagnosed from a MessageBox on someone else's screen.
            Try
                Dim existing = ReadExistingTableNames(connectionString)

                Using scope = serviceProvider.CreateScope()
                    Dim services = scope.ServiceProvider

                    If logger IsNot Nothing Then logger.LogInformation("Ensuring module database schemas from the EF model.")
                    EnsureContextTables(services.GetRequiredService(Of PurchasingDbContext)(), existing, logger)
                    EnsureContextTables(services.GetRequiredService(Of InventoryDbContext)(), existing, logger)
                    EnsureContextTables(services.GetRequiredService(Of POSDbContext)(), existing, logger)
                    EnsureContextTables(services.GetRequiredService(Of AccountingDbContext)(), existing, logger)
                End Using
            Catch ex As Exception
                If logger IsNot Nothing Then logger.LogError(ex, "Failed to ensure the module database schemas.")
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' Returns True when none of <paramref name="modelTables"/> already exists, i.e. the
        ''' context's schema has not been created yet. <c>CreateTables()</c> is not idempotent —
        ''' it aborts on the first table that already exists — so it must only be called then.
        ''' </summary>
        Friend Shared Function ShouldCreateTables(existing As HashSet(Of String), modelTables As IEnumerable(Of String)) As Boolean
            Return Not modelTables.Any(Function(t) existing.Contains(t))
        End Function

        Private Shared Sub EnsureContextTables(db As DbContext, existing As HashSet(Of String), logger As ILogger)
            ' Log which module failed. Without this a failure here is only ever visible in the
            ' startup MessageBox, which is useless for diagnosing a client machine remotely.
            Try
                EnsureContextTablesCore(db, existing, logger)
            Catch ex As Exception
                If logger IsNot Nothing Then
                    logger.LogError(ex, "Failed to create the module schema for {Context}.", db.GetType().Name)
                End If
                Throw
            End Try
        End Sub

        Private Shared Sub EnsureContextTablesCore(db As DbContext, existing As HashSet(Of String), logger As ILogger)
            Dim modelTables = db.Model.GetEntityTypes().
                Select(Function(et) et.GetTableName()).
                Where(Function(n) n IsNot Nothing).
                Distinct(StringComparer.OrdinalIgnoreCase).
                ToList()

            If modelTables.Count = 0 Then Return

            If Not ShouldCreateTables(existing, modelTables) Then
                ' ShouldCreateTables is False in two very different situations: the schema is
                ' already complete (nothing to do — the normal path on every launch after the
                ' first), or it is genuinely partial. Distinguish them, otherwise a healthy
                ' database logs a false failure.
                Dim missing = modelTables.
                    Where(Function(t) Not existing.Contains(t)).
                    OrderBy(Function(t) t).
                    ToList()

                If missing.Count = 0 Then
                    If logger IsNot Nothing Then
                        logger.LogInformation("Module schema for {Context} is already present ({TableCount} tables).", db.GetType().Name, modelTables.Count)
                    End If
                    Return
                End If

                ' A partial schema cannot be reconciled without migrations, which this deployment
                ' deliberately does not use — CreateTables() is not idempotent, so calling it here
                ' would abort on the first table that already exists. Name the missing tables: a
                ' module skipped silently only shows up much later as an unrelated
                ' "table doesn't exist" failure on the first query against it.
                If logger IsNot Nothing Then
                    logger.LogError(
                        "Module schema for {Context} is partially present: {MissingCount} of {TotalCount} tables are missing ({Missing}). Skipping table creation — this database cannot be reconciled automatically.",
                        db.GetType().Name, missing.Count, modelTables.Count, String.Join(", ", missing))
                End If
                Return
            End If

            db.Database.GetService(Of IRelationalDatabaseCreator)().CreateTables()

            For Each tableName In modelTables
                existing.Add(tableName)
            Next
        End Sub

        ''' <summary>
        ''' Reads the base table names of the current database.
        '''
        ''' Uses its own <see cref="MySqlConnection"/> rather than the EF connection: the EF
        ''' provider is Oracle's MySql.EntityFrameworkCore, whose connection is a
        ''' <c>MySql.Data</c> connection, not a <c>MySqlConnector</c> one.
        ''' </summary>
        Private Shared Function ReadExistingTableNames(connectionString As String) As HashSet(Of String)
            Dim names As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

            Using conn As New MySqlConnection(connectionString)
                conn.Open()

                Using cmd = conn.CreateCommand()
                    cmd.CommandText = "SELECT TABLE_NAME FROM information_schema.TABLES " &
                                      "WHERE TABLE_SCHEMA = DATABASE() AND TABLE_TYPE = 'BASE TABLE'"

                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            names.Add(reader.GetString(0))
                        End While
                    End Using
                End Using
            End Using

            Return names
        End Function

    End Class

End Namespace
