Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Accounting.Entities

Namespace Data.Configurations
    Public Class FinancialPeriodConfiguration
        Implements IEntityTypeConfiguration(Of FinancialPeriod)

        Public Sub Configure(builder As EntityTypeBuilder(Of FinancialPeriod)) Implements IEntityTypeConfiguration(Of FinancialPeriod).Configure
            builder.ToTable("Acc_FinancialPeriods")
            builder.Property(Function(x) x.RowVersion).IsRowVersion()
        End Sub
    End Class
End Namespace
