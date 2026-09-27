Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Purchasing.Entities

Namespace Data.Configurations
    Public Class PurchaseOrderConfiguration
        Implements IEntityTypeConfiguration(Of PurchaseOrder)

        Public Sub Configure(builder As EntityTypeBuilder(Of PurchaseOrder)) Implements IEntityTypeConfiguration(Of PurchaseOrder).Configure
            builder.ToTable("PurchaseOrders")
            builder.HasIndex(Function(e) e.OrderNumber).IsUnique()

            builder.HasMany(Of PurchaseOrderLine)().
                WithOne().
                HasForeignKey(Function(l) l.PurchaseOrderId).
                OnDelete(DeleteBehavior.Cascade)

            builder.HasMany(Of GoodsReceipt)().
                WithOne().
                HasForeignKey(Function(g) g.PurchaseOrderId).
                OnDelete(DeleteBehavior.Cascade)
        End Sub
    End Class
End Namespace
