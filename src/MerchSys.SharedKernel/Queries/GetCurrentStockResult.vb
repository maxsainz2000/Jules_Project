Imports MediatR

Namespace Queries

    Public Class GetCurrentStockResult
        Public Property StockLevels As List(Of ProductStockLevel) = New List(Of ProductStockLevel)()
    End Class

    Public Class ProductStockLevel
        Public Property ProductId As Integer
        Public Property CurrentQuantity As Integer
        Public Property ThresholdQuantity As Integer
        Public Property IsBelowThreshold As Boolean
    End Class

End Namespace
