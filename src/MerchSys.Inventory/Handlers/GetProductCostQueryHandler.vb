Imports System.Threading
Imports System.Threading.Tasks
Imports MediatR
Imports MerchSys.SharedKernel.Queries

Namespace Handlers

    Public Class GetProductCostQueryHandler
        Implements IRequestHandler(Of GetProductCostQuery, GetProductCostResult)

        Public Function Handle(request As GetProductCostQuery, cancellationToken As CancellationToken) As Task(Of GetProductCostResult) Implements IRequestHandler(Of GetProductCostQuery, GetProductCostResult).Handle
            Return Task.FromResult(New GetProductCostResult With {
                .ProductId = request.ProductId,
                .FifoUnitCost = 0D
            })
        End Function

    End Class

End Namespace
