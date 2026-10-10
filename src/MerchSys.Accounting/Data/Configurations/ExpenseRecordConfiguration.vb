Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Accounting.Entities

Namespace Data.Configurations
    Public Class ExpenseRecordConfiguration
        Implements IEntityTypeConfiguration(Of ExpenseRecord)

        Public Sub Configure(builder As EntityTypeBuilder(Of ExpenseRecord)) Implements IEntityTypeConfiguration(Of ExpenseRecord).Configure
            builder.ToTable("Acc_ExpenseRecords")

            builder.Property(Function(x) x.Amount).HasPrecision(18, 2)

            builder.Property(Function(x) x.Category).IsRequired().HasMaxLength(50)
            builder.Property(Function(x) x.Description).HasMaxLength(500)
            builder.Property(Function(x) x.SourceModule).HasMaxLength(50)
        End Sub
    End Class
End Namespace
