Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Inventory.Entities

Namespace Data.Configurations
    Public Class ProductCategoryConfiguration
        Implements IEntityTypeConfiguration(Of ProductCategory)

        Public Sub Configure(builder As EntityTypeBuilder(Of ProductCategory)) Implements IEntityTypeConfiguration(Of ProductCategory).Configure
            builder.ToTable("ProductCategories")
            builder.HasIndex(Function(e) e.Name).IsUnique()
        End Sub
    End Class
End Namespace
