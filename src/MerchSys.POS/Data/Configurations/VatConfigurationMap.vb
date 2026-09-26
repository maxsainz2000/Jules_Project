Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.POS.Entities

Namespace Data.Configurations
    Public Class VatConfigurationMap
        Implements IEntityTypeConfiguration(Of VatConfiguration)

        Public Sub Configure(builder As EntityTypeBuilder(Of VatConfiguration)) Implements IEntityTypeConfiguration(Of VatConfiguration).Configure
            builder.ToTable("Pos_VatConfigurations")
            builder.Property(Function(x) x.RowVersion).IsRowVersion()
        End Sub
    End Class
End Namespace
