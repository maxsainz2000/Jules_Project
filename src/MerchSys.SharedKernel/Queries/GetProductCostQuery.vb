Imports MediatR

Namespace Queries

    Public Class GetProductCostQuery
        Implements IRequest(Of GetProductCostResult)

        Public Property ProductId As Integer

    End Class

End Namespace
