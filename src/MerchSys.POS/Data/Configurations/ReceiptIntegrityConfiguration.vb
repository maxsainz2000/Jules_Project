Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.POS.Entities

Namespace Data.Configurations
    Public Class ReceiptIntegrityConfiguration
        Implements IEntityTypeConfiguration(Of ReceiptIntegrity)

        Public Sub Configure(builder As EntityTypeBuilder(Of ReceiptIntegrity)) Implements IEntityTypeConfiguration(Of ReceiptIntegrity).Configure
            builder.ToTable("Pos_ReceiptIntegrity")

            builder.HasKey(Function(ri) ri.Id)

            builder.HasIndex(Function(ri) ri.ReceiptId).IsUnique()
            builder.HasIndex(Function(ri) ri.RetentionExpiresAt)

            builder.HasOne(Function(ri) ri.OfficialReceipt).
                WithOne().
                HasForeignKey(Of ReceiptIntegrity)(Function(ri) ri.ReceiptId).
                OnDelete(DeleteBehavior.Restrict)
        End Sub
    End Class
End Namespace