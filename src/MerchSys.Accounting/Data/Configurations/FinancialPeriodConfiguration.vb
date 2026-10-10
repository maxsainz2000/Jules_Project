Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Accounting.Entities

Namespace Data.Configurations
    Public Class FinancialPeriodConfiguration
        Implements IEntityTypeConfiguration(Of FinancialPeriod)

        Public Sub Configure(builder As EntityTypeBuilder(Of FinancialPeriod)) Implements IEntityTypeConfiguration(Of FinancialPeriod).Configure
            builder.ToTable("Acc_FinancialPeriods")

            builder.Property(Function(x) x.TotalRevenue).HasPrecision(18, 2)
            builder.Property(Function(x) x.TotalCogs).HasPrecision(18, 2)
            builder.Property(Function(x) x.GrossProfit).HasPrecision(18, 2)
            builder.Property(Function(x) x.OperatingExpenses).HasPrecision(18, 2)
            builder.Property(Function(x) x.NetIncome).HasPrecision(18, 2)
            builder.Property(Function(x) x.GrossMarginPercent).HasPrecision(10, 4)

            builder.Property(Function(x) x.RowVersion).IsRowVersion()
        End Sub
    End Class
End Namespace
