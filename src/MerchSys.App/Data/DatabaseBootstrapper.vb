Imports System
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Logging
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
    '''   <item>Module schemas, derived from the EF model via <c>EnsureCreated</c>, which is the
    '''     single source of truth for the <c>Pur_/Inv_/Pos_/Acc_</c> tables.</item>
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
            EnsureModuleSchemas(serviceProvider, logger)
        End Sub

        ''' <summary>
        ''' Creates the module tables from the EF model. This replaces the former
        ''' <c>DatabaseInitializer</c>, which additionally ran duplicated inline ALTER/DDL
        ''' statements that shadowed the EF model as a second source of truth.
        ''' </summary>
        Private Shared Sub EnsureModuleSchemas(serviceProvider As IServiceProvider, logger As ILogger)
            Using scope = serviceProvider.CreateScope()
                Dim services = scope.ServiceProvider

                If logger IsNot Nothing Then logger.LogInformation("Ensuring module database schemas from the EF model.")
                services.GetRequiredService(Of PurchasingDbContext)().Database.EnsureCreated()
                services.GetRequiredService(Of InventoryDbContext)().Database.EnsureCreated()
                services.GetRequiredService(Of POSDbContext)().Database.EnsureCreated()
                services.GetRequiredService(Of AccountingDbContext)().Database.EnsureCreated()
            End Using
        End Sub

    End Class

End Namespace
