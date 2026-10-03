Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.POS.Entities

Namespace Data.Configurations

    Public Class SalesTransactionLineConfiguration
        Implements IEntityTypeConfiguration(Of SalesTransactionLine)

        Public Sub Configure(builder As EntityTypeBuilder(Of SalesTransactionLine)) Implements IEntityTypeConfiguration(Of SalesTransactionLine).Configure
            builder.ToTable("Pos_SalesTransactionLines")

            For Each prop In builder.Metadata.GetProperties().Where(Function(p) p.ClrType = GetType(Decimal) OrElse p.ClrType = GetType(Decimal?))
                prop.SetPrecision(18)
                prop.SetScale(2)
            Next

            builder.Property(Function(e) e.ProductName) _
                .IsRequired() _
                .HasMaxLength(200)

            builder.Property(Function(e) e.UnitPrice) _
                .HasPrecision(18, 2)

            builder.Property(Function(e) e.LineTotal) _
                .HasPrecision(18, 2)

        End Sub
    End Class

End Namespace
