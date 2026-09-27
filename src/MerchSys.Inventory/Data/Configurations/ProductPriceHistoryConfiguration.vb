Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Inventory.Entities

Namespace Data.Configurations
    Public Class ProductPriceHistoryConfiguration
        Implements IEntityTypeConfiguration(Of ProductPriceHistory)

        Public Sub Configure(builder As EntityTypeBuilder(Of ProductPriceHistory)) Implements IEntityTypeConfiguration(Of ProductPriceHistory).Configure
            builder.ToTable("ProductPriceHistories")

            builder.Property(Function(e) e.OldPrice).HasPrecision(18, 2)
            builder.Property(Function(e) e.NewPrice).HasPrecision(18, 2)
            builder.Property(Function(e) e.ChangedBy).HasMaxLength(100)
            builder.Property(Function(e) e.Reason).HasMaxLength(255)

            builder.HasIndex(Function(e) New With { e.ProductId, e.ChangedAt })
        End Sub
    End Class
End Namespace
