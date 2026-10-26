Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Inventory.Entities

Namespace Data.Configurations
    Public Class SaleCogsRecordConfiguration
        Implements IEntityTypeConfiguration(Of SaleCogsRecord)

        Public Sub Configure(builder As EntityTypeBuilder(Of SaleCogsRecord)) Implements IEntityTypeConfiguration(Of SaleCogsRecord).Configure
            builder.ToTable("Inv_SaleCogs")
            builder.HasKey(Function(r) r.Id)
            builder.HasIndex(Function(r) r.SaleId).HasDatabaseName("IX_Inv_SaleCogs_SaleId")

            builder.Property(Function(r) r.UnitCost).HasPrecision(18, 4)
            builder.Property(Function(r) r.TotalCost).HasPrecision(18, 4)
        End Sub
    End Class
End Namespace
