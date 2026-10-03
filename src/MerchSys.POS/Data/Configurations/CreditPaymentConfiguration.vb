Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.POS.Entities

Namespace Data.Configurations

    Public Class CreditPaymentConfiguration
        Implements IEntityTypeConfiguration(Of CreditPayment)

        Public Sub Configure(builder As EntityTypeBuilder(Of CreditPayment)) Implements IEntityTypeConfiguration(Of CreditPayment).Configure
            builder.ToTable("Pos_CreditPayments")

            For Each prop In builder.Metadata.GetProperties().Where(Function(p) p.ClrType = GetType(Decimal) OrElse p.ClrType = GetType(Decimal?))
                prop.SetPrecision(18)
                prop.SetScale(2)
            Next

            builder.Property(Function(e) e.Amount) _
                .HasColumnName("PaymentAmount") _
                .HasPrecision(18, 2)

            builder.HasOne(Function(e) e.CreditAccount) _
                .WithMany() _
                .HasForeignKey(Function(e) e.CreditAccountId) _
                .IsRequired()

        End Sub
    End Class

End Namespace
