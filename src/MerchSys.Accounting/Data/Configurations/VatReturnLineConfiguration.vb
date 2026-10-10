Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Accounting.Entities

Namespace Data.Configurations
    Public Class VatReturnLineConfiguration
        Implements IEntityTypeConfiguration(Of VatReturnLine)

        Public Sub Configure(builder As EntityTypeBuilder(Of VatReturnLine)) Implements IEntityTypeConfiguration(Of VatReturnLine).Configure
            builder.ToTable("Acc_VatReturnLines")

            builder.Property(Function(x) x.Amount).HasPrecision(18, 2)
            builder.Property(Function(x) x.TaxAmount).HasPrecision(18, 2)
            builder.Property(Function(x) x.TaxRate).HasPrecision(6, 4)

            builder.HasIndex(Function(x) x.VatReturnId)
            builder.HasIndex(Function(x) x.LineNumber)
        End Sub
    End Class
End Namespace