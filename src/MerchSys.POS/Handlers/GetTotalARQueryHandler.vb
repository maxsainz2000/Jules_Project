Imports System.Threading
Imports System.Threading.Tasks
Imports MediatR
Imports MerchSys.SharedKernel.Queries

Namespace Handlers

    Public Class GetTotalARQueryHandler
        Implements IRequestHandler(Of GetTotalARQuery, Decimal)

        Public Function Handle(request As GetTotalARQuery, cancellationToken As CancellationToken) As Task(Of Decimal) Implements IRequestHandler(Of GetTotalARQuery, Decimal).Handle
            Return Task.FromResult(0D)
        End Function

    End Class

End Namespace
