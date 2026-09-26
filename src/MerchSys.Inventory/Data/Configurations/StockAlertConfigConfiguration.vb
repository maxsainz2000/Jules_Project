Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Inventory.Entities

Namespace Data.Configurations
    Public Class StockAlertConfigConfiguration
        Implements IEntityTypeConfiguration(Of StockAlertConfig)

        Public Sub Configure(builder As EntityTypeBuilder(Of StockAlertConfig)) Implements IEntityTypeConfiguration(Of StockAlertConfig).Configure
            builder.ToTable("StockAlertConfigs")
            builder.Property(Function(x) x.RowVersion).IsRowVersion()
        End Sub
    End Class
End Namespace
