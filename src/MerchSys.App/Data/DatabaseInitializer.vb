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

                ' Initialize PurchasingDbContext
                Dim purchasingDb = services.GetRequiredService(Of PurchasingDbContext)()
                purchasingDb.Database.EnsureCreated()

                ' Initialize InventoryDbContext
                Dim inventoryDb = services.GetRequiredService(Of InventoryDbContext)()
                inventoryDb.Database.EnsureCreated()

                ' Initialize POSDbContext
                Dim posDb = services.GetRequiredService(Of POSDbContext)()
                posDb.Database.EnsureCreated()

                ' Initialize AccountingDbContext
                Dim accountingDb = services.GetRequiredService(Of AccountingDbContext)()
                accountingDb.Database.EnsureCreated()
            End Using
        End Sub

    End Class
End Namespace
