Imports System.Collections.Generic

Namespace Queries

    Public Class ProductCatalogItem
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property Sku As String
        Public Property UnitPrice As Decimal
        Public Property AvailableStock As Integer
        Public Property IsLowStock As Boolean
    End Class

    Public Class GetProductCatalogResult
        Public Property Items As List(Of ProductCatalogItem) = New List(Of ProductCatalogItem)()
    End Class

End Namespace
