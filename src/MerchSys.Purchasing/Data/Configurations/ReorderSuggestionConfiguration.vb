Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.Purchasing.Entities

Namespace Data.Configurations
    Public Class ReorderSuggestionConfiguration
        Implements IEntityTypeConfiguration(Of ReorderSuggestion)

        Public Sub Configure(builder As EntityTypeBuilder(Of ReorderSuggestion)) Implements IEntityTypeConfiguration(Of ReorderSuggestion).Configure
            builder.ToTable("ReorderSuggestions") ' Will be prefixed with Pur_ by DbContext

            builder.HasKey(Function(e) e.Id)

            builder.HasIndex(Function(e) New With {e.ProductId, e.Status})

            builder.Property(Function(e) e.RowVersion).IsRowVersion()
        End Sub
    End Class
End Namespace
