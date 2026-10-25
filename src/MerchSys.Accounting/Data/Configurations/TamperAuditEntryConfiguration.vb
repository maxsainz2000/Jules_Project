Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Accounting.Entities

Namespace Data.Configurations

    Public Class TamperAuditEntryConfiguration
        Implements IEntityTypeConfiguration(Of TamperAuditEntry)

        Public Sub Configure(builder As EntityTypeBuilder(Of TamperAuditEntry)) Implements IEntityTypeConfiguration(Of TamperAuditEntry).Configure
            builder.ToTable("Acc_TamperAuditLog")
            builder.HasKey(Function(x) x.Id)
            builder.HasIndex(Function(x) New With {x.DetectedAt, x.TamperKind}).HasDatabaseName("IX_Acc_TamperAuditLog_DetectedAt_TamperKind")
        End Sub

    End Class

End Namespace