Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Purchasing.Entities

Namespace Data.Configurations
    Public Class PriceChangeAlertConfiguration
        Implements IEntityTypeConfiguration(Of PriceChangeAlert)

        Public Sub Configure(builder As EntityTypeBuilder(Of PriceChangeAlert)) Implements IEntityTypeConfiguration(Of PriceChangeAlert).Configure
            builder.ToTable("Pur_PriceChangeAlerts")
            builder.HasKey(Function(e) e.Id)

            builder.Property(Function(e) e.PreviousUnitCost).HasPrecision(18, 4)
            builder.Property(Function(e) e.NewUnitCost).HasPrecision(18, 4)
            builder.Property(Function(e) e.ChangePercent).HasPrecision(18, 4)

            builder.HasIndex(Function(e) e.IsAcknowledged)
            builder.HasIndex(Function(e) e.ProductId)
        End Sub
    End Class
End Namespace
