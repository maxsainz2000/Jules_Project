Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Purchasing.Entities

Namespace Data.Configurations
    Public Class PurchaseOrderLineConfiguration
        Implements IEntityTypeConfiguration(Of PurchaseOrderLine)

        Public Sub Configure(builder As EntityTypeBuilder(Of PurchaseOrderLine)) Implements IEntityTypeConfiguration(Of PurchaseOrderLine).Configure
            builder.ToTable("PurchaseOrderLines")

            builder.Property(Function(e) e.UnitCost).HasPrecision(18, 4)
            builder.Property(Function(e) e.LineTotal).HasPrecision(18, 2)
        End Sub
    End Class
End Namespace
