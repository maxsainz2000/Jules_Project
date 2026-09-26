Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Inventory.Entities

Namespace Data.Configurations
    Public Class StockBatchConfiguration
        Implements IEntityTypeConfiguration(Of StockBatch)

        Public Sub Configure(builder As EntityTypeBuilder(Of StockBatch)) Implements IEntityTypeConfiguration(Of StockBatch).Configure
            builder.ToTable("StockBatches")

            builder.Property(Function(e) e.UnitCost).HasPrecision(18, 4)

            builder.HasIndex(Function(e) New With {e.ProductId, e.ReceiptDate})

            builder.Ignore(Function(e) e.IsExpired)
            builder.Ignore(Function(e) e.IsFullyConsumed)
        End Sub
    End Class
End Namespace
