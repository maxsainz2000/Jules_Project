Imports MediatR

Namespace Queries

    Public Class GetInventoryValuationResult
        Public Property Valuations As List(Of ProductValuation) = New List(Of ProductValuation)()
    End Class

    Public Class ProductValuation
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property Quantity As Integer
        Public Property UnitCost As Decimal
        Public Property TotalValue As Decimal
    End Class

End Namespace
