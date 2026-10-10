Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.POS.Entities

Namespace Data.Configurations
    Public Class ReceiptSequenceConfiguration
        Implements IEntityTypeConfiguration(Of ReceiptSequence)

        Public Sub Configure(builder As EntityTypeBuilder(Of ReceiptSequence)) Implements IEntityTypeConfiguration(Of ReceiptSequence).Configure
            builder.ToTable("Pos_ReceiptSequence")

            builder.HasKey(Function(rs) rs.Id)

            builder.HasIndex(Function(rs) rs.Year).IsUnique()

            builder.Property(Function(rs) rs.RowVersion).
                IsRowVersion().
                IsConcurrencyToken()
        End Sub
    End Class
End Namespace