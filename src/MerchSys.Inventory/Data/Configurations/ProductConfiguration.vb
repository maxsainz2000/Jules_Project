Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Inventory.Entities

Namespace Data.Configurations
    Public Class ProductConfiguration
        Implements IEntityTypeConfiguration(Of Product)

        Public Sub Configure(builder As EntityTypeBuilder(Of Product)) Implements IEntityTypeConfiguration(Of Product).Configure
            builder.ToTable("Products")

            builder.Property(Function(e) e.Name).HasMaxLength(200)
            builder.Property(Function(e) e.SKU).HasMaxLength(50)
            builder.Property(Function(e) e.RetailPrice).HasPrecision(18, 2)

            builder.HasIndex(Function(e) e.SKU).IsUnique()
        End Sub
    End Class
End Namespace
