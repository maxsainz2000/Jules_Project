Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Purchasing.Entities

Namespace Data.Configurations
    Public Class VendorProductConfiguration
        Implements IEntityTypeConfiguration(Of VendorProduct)

        Public Sub Configure(builder As EntityTypeBuilder(Of VendorProduct)) Implements IEntityTypeConfiguration(Of VendorProduct).Configure
            ' Note: prefix is added dynamically
            builder.ToTable("Pur_VendorProducts")

            builder.HasIndex(Function(vp) New With {vp.VendorId, vp.ProductId}).
                IsUnique().
                HasFilter("IsDeleted = 0")
        End Sub
    End Class
End Namespace
