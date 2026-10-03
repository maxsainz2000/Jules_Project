Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.POS.Entities

Namespace Data.Configurations

    Public Class SalesTransactionConfiguration
        Implements IEntityTypeConfiguration(Of SalesTransaction)

        Public Sub Configure(builder As EntityTypeBuilder(Of SalesTransaction)) Implements IEntityTypeConfiguration(Of SalesTransaction).Configure
            builder.ToTable("Pos_SalesTransactions")

            For Each prop In builder.Metadata.GetProperties().Where(Function(p) p.ClrType = GetType(Decimal) OrElse p.ClrType = GetType(Decimal?))
                prop.SetPrecision(18)
                prop.SetScale(2)
            Next

            builder.Property(Function(e) e.TransactionNumber) _
                .IsRequired() _
                .HasMaxLength(20)

            builder.HasIndex(Function(e) e.TransactionNumber) _
                .IsUnique()

            builder.HasIndex(Function(e) e.TransactionDate)

            builder.Property(Function(e) e.TotalAmount) _
                .HasPrecision(18, 2)

            builder.HasMany(Function(e) e.Lines) _
                .WithOne() _
                .HasForeignKey("SalesTransactionId") _
                .OnDelete(DeleteBehavior.Cascade)

            builder.HasOne(Function(e) e.Receipt) _
                .WithOne() _
                .HasForeignKey(Of OfficialReceipt)("SalesTransactionId") _
                .IsRequired(False)

            ' optional shadow FK CreditAccountId
            builder.Property(Of Integer?)("CreditAccountId")

            builder.HasOne(Function(e) e.CreditAccount) _
                .WithMany() _
                .HasForeignKey("CreditAccountId") _
                .IsRequired(False)

        End Sub
    End Class

End Namespace
