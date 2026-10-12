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

            ' The entity declares RowVersion, so it must be mapped explicitly. EF's conventions
            ' already treat a byte[] property named RowVersion as a concurrency token, which means
            ' IgnoreNonTokenRowVersionConvention leaves it in place — and an unmapped byte[]
            ' concurrency token then fails model validation for the whole module.
            builder.Property(Function(e) e.RowVersion).
                IsRowVersion().
                IsConcurrencyToken()
        End Sub
    End Class
End Namespace
