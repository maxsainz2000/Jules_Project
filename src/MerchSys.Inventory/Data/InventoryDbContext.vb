Imports Microsoft.EntityFrameworkCore
Imports MerchSys.SharedKernel.Data

Namespace Data

    Public Class InventoryDbContext
        Inherits BaseDbContext

        Public Property ProductCategories As DbSet(Of Entities.ProductCategory)
        Public Property Products As DbSet(Of Entities.Product)
        Public Property StockBatches As DbSet(Of Entities.StockBatch)
        Public Property ShrinkageRecords As DbSet(Of Entities.ShrinkageRecord)
        Public Property StockAlertConfigs As DbSet(Of Entities.StockAlertConfig)

        Public Sub New(options As DbContextOptions(Of InventoryDbContext))
            MyBase.New(options)
        End Sub

        Protected Overrides Sub OnModelCreating(modelBuilder As ModelBuilder)
            MyBase.OnModelCreating(modelBuilder)
            modelBuilder.ApplyConfigurationsFromAssembly(GetType(InventoryDbContext).Assembly)

            For Each entityType In modelBuilder.Model.GetEntityTypes()
                entityType.SetTableName("Inv_" & entityType.GetTableName())
            Next

            SeedData.InventorySeedData.Seed(modelBuilder)
        End Sub

    End Class

End Namespace
