Imports Microsoft.Extensions.DependencyInjection
Imports MerchSys.Purchasing.Data
Imports MerchSys.Inventory.Data
Imports MerchSys.POS.Data
Imports MerchSys.Accounting.Data

Namespace Data

    Public Class DatabaseInitializer

        Public Shared Sub Initialize(serviceProvider As IServiceProvider)
            Using scope = serviceProvider.CreateScope()
                Dim services = scope.ServiceProvider

                ' Resolve each DbContext and ensure the database is created
                Dim purchasingDb = services.GetRequiredService(Of PurchasingDbContext)()
                purchasingDb.Database.EnsureCreated()

                Dim inventoryDb = services.GetRequiredService(Of InventoryDbContext)()
                inventoryDb.Database.EnsureCreated()

                Dim posDb = services.GetRequiredService(Of POSDbContext)()
                posDb.Database.EnsureCreated()

                Dim accountingDb = services.GetRequiredService(Of AccountingDbContext)()
                accountingDb.Database.EnsureCreated()
            End Using
        End Sub

    End Class

End Namespace
