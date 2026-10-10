Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Accounting.Entities

Namespace Data.Configurations
    Public Class FinancialSnapshotConfiguration
        Implements IEntityTypeConfiguration(Of FinancialSnapshot)

        Public Sub Configure(builder As EntityTypeBuilder(Of FinancialSnapshot)) Implements IEntityTypeConfiguration(Of FinancialSnapshot).Configure
            builder.ToTable("Acc_FinancialSnapshots")

            builder.Property(Function(x) x.TotalAR).HasPrecision(18, 2)
            builder.Property(Function(x) x.TotalAP).HasPrecision(18, 2)
            builder.Property(Function(x) x.InventoryValue).HasPrecision(18, 2)
            builder.Property(Function(x) x.TodayRevenue).HasPrecision(18, 2)
            builder.Property(Function(x) x.MtdRevenue).HasPrecision(18, 2)
            builder.Property(Function(x) x.YtdRevenue).HasPrecision(18, 2)

            builder.HasIndex(Function(s) s.SnapshotDate).IsUnique()
        End Sub
    End Class
End Namespace
