Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.POS.Entities

Namespace Data.Configurations
    Public Class OfficialReceiptArchiveConfiguration
        Implements IEntityTypeConfiguration(Of OfficialReceiptArchive)

        Public Sub Configure(builder As EntityTypeBuilder(Of OfficialReceiptArchive)) Implements IEntityTypeConfiguration(Of OfficialReceiptArchive).Configure
            builder.ToTable("Pos_OfficialReceiptArchive")

            builder.HasKey(Function(ora) ora.Id)

            builder.HasIndex(Function(ora) ora.OriginalReceiptId)
        End Sub
    End Class
End Namespace