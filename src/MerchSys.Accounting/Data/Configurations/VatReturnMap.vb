Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Accounting.Entities

Namespace Data.Configurations
    Public Class VatReturnMap
        Implements IEntityTypeConfiguration(Of VatReturn)

        Public Sub Configure(builder As EntityTypeBuilder(Of VatReturn)) Implements IEntityTypeConfiguration(Of VatReturn).Configure
            builder.ToTable("Acc_VatReturns")
            builder.Property(Function(x) x.RowVersion).IsRowVersion()
        End Sub
    End Class
End Namespace
