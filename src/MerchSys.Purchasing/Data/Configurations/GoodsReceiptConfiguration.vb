Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Purchasing.Entities

Namespace Data.Configurations
    Public Class GoodsReceiptConfiguration
        Implements IEntityTypeConfiguration(Of GoodsReceipt)

        Public Sub Configure(builder As EntityTypeBuilder(Of GoodsReceipt)) Implements IEntityTypeConfiguration(Of GoodsReceipt).Configure
            builder.ToTable("GoodsReceipts")
            builder.HasIndex(Function(e) e.ReceiptNumber).IsUnique()

            builder.HasMany(Of GoodsReceiptLine)().
                WithOne().
                HasForeignKey(Function(l) l.GoodsReceiptId).
                OnDelete(DeleteBehavior.Cascade)
        End Sub
    End Class
End Namespace
