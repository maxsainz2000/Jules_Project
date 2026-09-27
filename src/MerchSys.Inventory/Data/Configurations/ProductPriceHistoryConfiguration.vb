Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Inventory.Entities

Namespace Data.Configurations

    Public Class ProductPriceHistoryConfiguration
        Implements IEntityTypeConfiguration(Of ProductPriceHistory)

        Public Sub Configure(builder As EntityTypeBuilder(Of ProductPriceHistory)) Implements IEntityTypeConfiguration(Of ProductPriceHistory).Configure
            builder.ToTable("ProductPriceHistory")
            builder.HasKey(Function(p) p.Id)

            builder.Property(Function(p) p.OldPrice).HasPrecision(18, 2)
            builder.Property(Function(p) p.NewPrice).HasPrecision(18, 2)
            builder.Property(Function(p) p.ChangedBy).HasMaxLength(100)
            builder.Property(Function(p) p.Reason).HasMaxLength(255)

            builder.HasIndex(Function(p) New With { p.ChangedAt, p.Id })
        End Sub

    End Class

End Namespace
