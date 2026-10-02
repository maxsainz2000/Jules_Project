Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Purchasing.Entities

Namespace Data.Configurations
    Public Class VendorProductConfiguration
        Implements IEntityTypeConfiguration(Of VendorProduct)

        Public Sub Configure(builder As EntityTypeBuilder(Of VendorProduct)) Implements IEntityTypeConfiguration(Of VendorProduct).Configure
            builder.ToTable("VendorProducts")
            builder.HasIndex(Function(e) New With { e.VendorId, e.ProductId }).IsUnique()
        End Sub
    End Class
End Namespace
