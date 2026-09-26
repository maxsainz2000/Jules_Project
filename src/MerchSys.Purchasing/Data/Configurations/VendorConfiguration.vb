Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Purchasing.Entities

Namespace Data.Configurations
    Public Class VendorConfiguration
        Implements IEntityTypeConfiguration(Of Vendor)

        Public Sub Configure(builder As EntityTypeBuilder(Of Vendor)) Implements IEntityTypeConfiguration(Of Vendor).Configure
            builder.ToTable("Vendors")
            builder.HasIndex(Function(e) e.Name).IsUnique()
        End Sub
    End Class
End Namespace
