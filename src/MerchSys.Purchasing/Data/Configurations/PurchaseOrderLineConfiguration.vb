Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Purchasing.Entities

Namespace Data.Configurations
    Public Class PurchaseOrderLineConfiguration
        Implements IEntityTypeConfiguration(Of PurchaseOrderLine)

        Public Sub Configure(builder As EntityTypeBuilder(Of PurchaseOrderLine)) Implements IEntityTypeConfiguration(Of PurchaseOrderLine).Configure
            builder.ToTable("Pur_PurchaseOrderLines")
        End Sub
    End Class
End Namespace
