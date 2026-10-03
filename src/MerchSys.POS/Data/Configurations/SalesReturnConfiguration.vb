Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.POS.Entities

Namespace Data.Configurations

    Public Class SalesReturnConfiguration
        Implements IEntityTypeConfiguration(Of SalesReturn)

        Public Sub Configure(builder As EntityTypeBuilder(Of SalesReturn)) Implements IEntityTypeConfiguration(Of SalesReturn).Configure
            builder.ToTable("Pos_SalesReturns")

            For Each prop In builder.Metadata.GetProperties().Where(Function(p) p.ClrType = GetType(Decimal) OrElse p.ClrType = GetType(Decimal?))
                prop.SetPrecision(18)
                prop.SetScale(2)
            Next

            builder.Property(Function(e) e.Reason) _
                .IsRequired() _
                .HasMaxLength(500)

            builder.Property(Function(e) e.SalesTransactionId) _
                .HasColumnName("OriginalTransactionId")

            builder.HasOne(Function(e) e.SalesTransaction) _
                .WithMany() _
                .HasForeignKey(Function(e) e.SalesTransactionId) _
                .IsRequired()

        End Sub
    End Class

End Namespace
