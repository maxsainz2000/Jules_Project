Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Purchasing.Entities

Namespace Data.Configurations
    Public Class GoodsReceiptConfiguration
        Implements IEntityTypeConfiguration(Of GoodsReceipt)

        Public Sub Configure(builder As EntityTypeBuilder(Of GoodsReceipt)) Implements IEntityTypeConfiguration(Of GoodsReceipt).Configure
            builder.ToTable("GoodsReceipts")
            builder.HasIndex(Function(e) e.ReceiptNumber).IsUnique()

            ' Use the Lines navigation explicitly. The previous nav-less form —
            ' HasMany(Of GoodsReceiptLine)() — left GoodsReceipt.Lines to be discovered by
            ' convention as a SECOND relationship, which created a shadow GoodsReceiptId1 column
            ' and made Include(Function(r) r.Lines) join on that always-null key, so the
            ' collection always came back empty.
            builder.HasMany(Function(r) r.Lines).
                WithOne().
                HasForeignKey(Function(l) l.GoodsReceiptId).
                OnDelete(DeleteBehavior.Cascade)
        End Sub
    End Class
End Namespace
