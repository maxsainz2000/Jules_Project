Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Inventory.Entities

Namespace Data.Configurations
    Public Class ShrinkageRecordConfiguration
        Implements IEntityTypeConfiguration(Of ShrinkageRecord)

        Public Sub Configure(builder As EntityTypeBuilder(Of ShrinkageRecord)) Implements IEntityTypeConfiguration(Of ShrinkageRecord).Configure
            builder.ToTable("ShrinkageRecords")

            builder.Property(Function(e) e.Reason).HasMaxLength(50).IsRequired()
            builder.Property(Function(e) e.UnitCost).HasPrecision(18, 4)
            builder.Property(Function(e) e.TotalValue).HasPrecision(18, 2)
        End Sub
    End Class
End Namespace
