Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Accounting.Entities

Namespace Data.Configurations
    Public Class RevenueRecordConfiguration
        Implements IEntityTypeConfiguration(Of RevenueRecord)

        Public Sub Configure(builder As EntityTypeBuilder(Of RevenueRecord)) Implements IEntityTypeConfiguration(Of RevenueRecord).Configure
            builder.ToTable("Acc_RevenueRecords")

            builder.Property(Function(x) x.UnitPrice).HasPrecision(18, 2)
            builder.Property(Function(x) x.TotalRevenue).HasPrecision(18, 2)
            builder.Property(Function(x) x.UnitCost).HasPrecision(18, 2)
            builder.Property(Function(x) x.TotalCogs).HasPrecision(18, 2)
            builder.Property(Function(x) x.GrossProfit).HasPrecision(18, 2)
            builder.Property(Function(x) x.QuantitySold).HasPrecision(18, 2)
            builder.Property(Function(x) x.VatableAmount).HasPrecision(18, 2)
            builder.Property(Function(x) x.VatExemptAmount).HasPrecision(18, 2)
            builder.Property(Function(x) x.ZeroRatedAmount).HasPrecision(18, 2)
            builder.Property(Function(x) x.OutputVat).HasPrecision(18, 2)
            builder.Property(Function(x) x.InputVat).HasPrecision(18, 2)

            builder.HasIndex(Function(x) x.RecordDate)
            builder.HasIndex(Function(x) x.ProductId)
        End Sub
    End Class
End Namespace
