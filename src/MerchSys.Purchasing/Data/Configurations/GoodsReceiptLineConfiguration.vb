Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Purchasing.Entities

Namespace Data.Configurations
    Public Class GoodsReceiptLineConfiguration
        Implements IEntityTypeConfiguration(Of GoodsReceiptLine)

        Public Sub Configure(builder As EntityTypeBuilder(Of GoodsReceiptLine)) Implements IEntityTypeConfiguration(Of GoodsReceiptLine).Configure
            builder.ToTable("GoodsReceiptLines")

            builder.Property(Function(e) e.UnitCost).HasPrecision(18, 4)
        End Sub
    End Class
End Namespace
