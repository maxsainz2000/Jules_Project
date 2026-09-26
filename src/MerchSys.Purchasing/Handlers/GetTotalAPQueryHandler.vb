Imports System.Threading
Imports System.Threading.Tasks
Imports MediatR
Imports MerchSys.SharedKernel.Queries

Namespace Handlers

    Public Class GetTotalAPQueryHandler
        Implements IRequestHandler(Of GetTotalAPQuery, Decimal)

        Public Function Handle(request As GetTotalAPQuery, cancellationToken As CancellationToken) As Task(Of Decimal) Implements IRequestHandler(Of GetTotalAPQuery, Decimal).Handle
            Return Task.FromResult(0D)
        End Function

    End Class

End Namespace
