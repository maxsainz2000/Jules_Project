Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Inventory.Entities

Namespace Data.Configurations
    Public Class ProductConfiguration
        Implements IEntityTypeConfiguration(Of Product)

        Public Sub Configure(builder As EntityTypeBuilder(Of Product)) Implements IEntityTypeConfiguration(Of Product).Configure
            builder.ToTable("Products")
            builder.HasIndex(Function(e) e.SKU).IsUnique()
        End Sub
    End Class
End Namespace
