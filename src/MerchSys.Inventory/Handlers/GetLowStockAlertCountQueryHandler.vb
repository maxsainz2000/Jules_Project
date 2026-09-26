Imports System.Threading
Imports System.Threading.Tasks
Imports MediatR
Imports MerchSys.SharedKernel.Queries

Namespace Handlers

    Public Class GetLowStockAlertCountQueryHandler
        Implements IRequestHandler(Of GetLowStockAlertCountQuery, Integer)

        Public Function Handle(request As GetLowStockAlertCountQuery, cancellationToken As CancellationToken) As Task(Of Integer) Implements IRequestHandler(Of GetLowStockAlertCountQuery, Integer).Handle
            Return Task.FromResult(0)
        End Function

    End Class

End Namespace
