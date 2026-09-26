Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Inventory.Entities

Namespace Data.Configurations
    Public Class CategoryConfiguration
        Implements IEntityTypeConfiguration(Of Category)

        Public Sub Configure(builder As EntityTypeBuilder(Of Category)) Implements IEntityTypeConfiguration(Of Category).Configure
            builder.ToTable("Categories")
            builder.HasIndex(Function(e) e.Name).IsUnique()
        End Sub
    End Class
End Namespace
