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
        Public Property ProductPriceHistories As DbSet(Of Entities.ProductPriceHistory)

        Public Sub New(options As DbContextOptions(Of InventoryDbContext), Optional currentUserProvider As MerchSys.SharedKernel.Interfaces.ICurrentUserProvider = Nothing, Optional writeContextScope As MerchSys.SharedKernel.Interfaces.IWriteContextScope = Nothing)
            MyBase.New(options, currentUserProvider, writeContextScope)
        End Sub

        ''' <summary>Tag written into the audit log for records this context writes.</summary>
        Protected Overrides ReadOnly Property AuditModuleTag As String
            Get
                Return "Inv"
            End Get
        End Property

        Protected Overrides Sub OnModelCreating(modelBuilder As ModelBuilder)
            MyBase.OnModelCreating(modelBuilder)
            modelBuilder.ApplyConfigurationsFromAssembly(GetType(InventoryDbContext).Assembly)

            ApplyTablePrefix(modelBuilder, "Inv_")

            SeedData.InventorySeedData.Seed(modelBuilder)
        End Sub

    End Class

End Namespace
