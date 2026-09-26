Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Purchasing.Entities

Namespace Data.Configurations
    Public Class ReorderConfigConfiguration
        Implements IEntityTypeConfiguration(Of ReorderConfig)

        Public Sub Configure(builder As EntityTypeBuilder(Of ReorderConfig)) Implements IEntityTypeConfiguration(Of ReorderConfig).Configure
            builder.ToTable("Pur_ReorderConfigs")
            builder.Property(Function(x) x.RowVersion).IsRowVersion()
        End Sub
    End Class
End Namespace
