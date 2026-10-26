Imports MediatR

Namespace Queries

    Public Class GetSaleCogsBreakdownQuery
        Implements IRequest(Of List(Of SaleCogsBreakdownDto))

        Public Property SaleId As Integer
    End Class

    Public Class SaleCogsBreakdownDto
        Public Property SaleId As Integer
        Public Property ProductId As Integer
        Public Property BatchId As Integer
        Public Property QuantityDeducted As Integer
        Public Property UnitCost As Decimal
        Public Property TotalCost As Decimal
        Public Property DeductedAt As DateTime
    End Class

End Namespace
