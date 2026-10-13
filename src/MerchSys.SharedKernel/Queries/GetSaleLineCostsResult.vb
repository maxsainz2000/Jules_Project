Namespace Queries

    Public Class GetSaleLineCostsResult
        Public Property Costs As List(Of SaleLineCost) = New List(Of SaleLineCost)()
    End Class

    Public Class SaleLineCost
        Public Property ProductId As Integer
        Public Property UnitCost As Decimal
    End Class

End Namespace
