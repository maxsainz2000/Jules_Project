Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Purchasing.Entities

Namespace Data.Configurations
    Public Class ReorderConfigConfiguration
        Implements IEntityTypeConfiguration(Of ReorderConfig)

        Public Sub Configure(builder As EntityTypeBuilder(Of ReorderConfig)) Implements IEntityTypeConfiguration(Of ReorderConfig).Configure
            builder.ToTable("ReorderConfigs") ' Will be prefixed with Pur_ by DbContext

            builder.HasKey(Function(e) e.Id)

            builder.HasIndex(Function(e) e.ProductId).IsUnique()

            builder.HasOne(Of Vendor)().
                WithMany().
                HasForeignKey(Function(e) e.PreferredVendorId).
                OnDelete(DeleteBehavior.SetNull)

            builder.Property(Function(e) e.RowVersion).IsRowVersion()
        End Sub
    End Class
End Namespace
