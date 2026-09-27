Imports Microsoft.EntityFrameworkCore
Imports MerchSys.SharedKernel.Data
Imports MerchSys.Purchasing.Entities
Imports MerchSys.Purchasing.Data.SeedData

Namespace Data

    Public Class PurchasingDbContext
        Inherits BaseDbContext

        Public Property Vendors As DbSet(Of Vendor)
        Public Property PurchaseOrders As DbSet(Of PurchaseOrder)
        Public Property PurchaseOrderLines As DbSet(Of PurchaseOrderLine)
        Public Property GoodsReceipts As DbSet(Of GoodsReceipt)
        Public Property GoodsReceiptLines As DbSet(Of GoodsReceiptLine)
        Public Property AccountsPayable As DbSet(Of AccountsPayableEntry)

        Public Sub New(options As DbContextOptions(Of PurchasingDbContext))
            MyBase.New(options)
        End Sub

        Protected Overrides Sub OnModelCreating(modelBuilder As ModelBuilder)
            MyBase.OnModelCreating(modelBuilder)
            modelBuilder.ApplyConfigurationsFromAssembly(GetType(PurchasingDbContext).Assembly)

            For Each entityType In modelBuilder.Model.GetEntityTypes()
                entityType.SetTableName("Pur_" & entityType.GetTableName())
            Next

            PurchasingSeedData.Seed(modelBuilder)
        End Sub

    End Class

End Namespace
