Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Accounting.Entities

Namespace Data.Configurations
    Public Class VatReturnConfiguration
        Implements IEntityTypeConfiguration(Of VatReturn)

        Public Sub Configure(builder As EntityTypeBuilder(Of VatReturn)) Implements IEntityTypeConfiguration(Of VatReturn).Configure
            builder.ToTable("Acc_VatReturns")

            builder.Property(Function(x) x.TotalSales).HasPrecision(18, 2)
            builder.Property(Function(x) x.TotalPurchases).HasPrecision(18, 2)
            builder.Property(Function(x) x.OutputVatTotal).HasPrecision(18, 2)
            builder.Property(Function(x) x.InputVatTotal).HasPrecision(18, 2)
            builder.Property(Function(x) x.NetVatPayable).HasPrecision(18, 2)

            builder.Property(Function(x) x.RowVersion).IsRowVersion()

            builder.HasMany(Function(x) x.Lines).
                    WithOne(Function(x) x.VatReturn).
                    HasForeignKey(Function(x) x.VatReturnId).
                    OnDelete(DeleteBehavior.Cascade)

            builder.HasIndex(Function(x) New With {x.Year, x.Period, x.PeriodType, x.FormType}).
                    IsUnique()
        End Sub
    End Class
End Namespace