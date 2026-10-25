Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Accounting.Entities

Namespace Data.Configurations

    Public Class TamperAuditEntryConfiguration
        Implements IEntityTypeConfiguration(Of TamperAuditEntry)

        Public Sub Configure(builder As EntityTypeBuilder(Of TamperAuditEntry)) Implements IEntityTypeConfiguration(Of TamperAuditEntry).Configure
            builder.ToTable("Acc_TamperAuditLog")
            builder.HasKey(Function(x) x.Id)

            builder.Property(Function(x) x.ReceiptNumber).HasMaxLength(50).IsRequired()
            builder.Property(Function(x) x.ExpectedHash).HasMaxLength(128).IsRequired()
            builder.Property(Function(x) x.ActualHash).HasMaxLength(128).IsRequired()
            builder.Property(Function(x) x.TamperKind).HasMaxLength(50).IsRequired()
            builder.Property(Function(x) x.DetectedBy).HasMaxLength(100).IsRequired(False)
            builder.Property(Function(x) x.MachineName).HasMaxLength(100).IsRequired(False)
            builder.Property(Function(x) x.WindowsUser).HasMaxLength(100).IsRequired(False)

            builder.HasIndex(Function(x) New With {x.DetectedAt, x.TamperKind}).HasDatabaseName("IX_Acc_TamperAuditLog_DetectedAt_TamperKind")
        End Sub

    End Class

End Namespace
