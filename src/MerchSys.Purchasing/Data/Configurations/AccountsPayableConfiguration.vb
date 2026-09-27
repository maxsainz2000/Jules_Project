Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Purchasing.Entities

Namespace Data.Configurations
    Public Class AccountsPayableConfiguration
        Implements IEntityTypeConfiguration(Of AccountsPayableEntry)

        Public Sub Configure(builder As EntityTypeBuilder(Of AccountsPayableEntry)) Implements IEntityTypeConfiguration(Of AccountsPayableEntry).Configure
            builder.ToTable("AccountsPayable")

            builder.Property(Function(e) e.TotalAmount).HasPrecision(18, 2)
            builder.Property(Function(e) e.AmountPaid).HasPrecision(18, 2)

            builder.HasIndex(Function(e) New With { e.VendorId, e.IsPaid })
        End Sub
    End Class
End Namespace
