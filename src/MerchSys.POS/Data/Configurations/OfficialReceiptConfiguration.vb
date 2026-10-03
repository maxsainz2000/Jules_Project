Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.POS.Entities

Namespace Data.Configurations

    Public Class OfficialReceiptConfiguration
        Implements IEntityTypeConfiguration(Of OfficialReceipt)

        Public Sub Configure(builder As EntityTypeBuilder(Of OfficialReceipt)) Implements IEntityTypeConfiguration(Of OfficialReceipt).Configure
            builder.ToTable("Pos_OfficialReceipts")

            For Each prop In builder.Metadata.GetProperties().Where(Function(p) p.ClrType = GetType(Decimal) OrElse p.ClrType = GetType(Decimal?))
                prop.SetPrecision(18)
                prop.SetScale(2)
            Next

            builder.Property(Function(e) e.ReceiptNumber) _
                .IsRequired() _
                .HasMaxLength(20)

            builder.HasIndex(Function(e) e.ReceiptNumber) _
                .IsUnique()

        End Sub
    End Class

End Namespace
