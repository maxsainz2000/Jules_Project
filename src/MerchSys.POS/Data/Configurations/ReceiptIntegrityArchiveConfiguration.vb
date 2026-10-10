Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.POS.Entities

Namespace Data.Configurations
    Public Class ReceiptIntegrityArchiveConfiguration
        Implements IEntityTypeConfiguration(Of ReceiptIntegrityArchive)

        Public Sub Configure(builder As EntityTypeBuilder(Of ReceiptIntegrityArchive)) Implements IEntityTypeConfiguration(Of ReceiptIntegrityArchive).Configure
            builder.ToTable("Pos_ReceiptIntegrityArchive")

            builder.HasKey(Function(ri) ri.Id)
        End Sub
    End Class
End Namespace
