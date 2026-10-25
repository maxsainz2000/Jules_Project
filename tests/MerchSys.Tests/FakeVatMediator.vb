Imports MediatR
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Collections.Generic
Imports MerchSys.SharedKernel.Queries

Public Class FakeVatMediator
    Implements IMediator

    Private ReadOnly _vatRate As Decimal

    Public Sub New(vatRate As Decimal)
        _vatRate = vatRate
    End Sub

    Public Function Send(Of TResponse)(request As IRequest(Of TResponse), Optional cancellationToken As CancellationToken = Nothing) As Task(Of TResponse) Implements ISender.Send
        If TypeOf request Is GetVatConfigurationQuery Then
            Dim result = New GetVatConfigurationResult With {
                .VatRate = _vatRate,
                .IsVatRegistered = True
            }
            Return Task.FromResult(DirectCast(DirectCast(result, Object), TResponse))
        End If
        Throw New NotSupportedException()
    End Function

    Public Function Send(Of TRequest As IRequest)(request As TRequest, Optional cancellationToken As CancellationToken = Nothing) As Task Implements ISender.Send
        Throw New NotSupportedException()
    End Function

    Public Function Send(request As Object, Optional cancellationToken As CancellationToken = Nothing) As Task(Of Object) Implements ISender.Send
        Throw New NotSupportedException()
    End Function

    Public Function Publish(notification As Object, Optional cancellationToken As CancellationToken = Nothing) As Task Implements IPublisher.Publish
        Return Task.CompletedTask
    End Function

    Public Function Publish(Of TNotification As INotification)(notification As TNotification, Optional cancellationToken As CancellationToken = Nothing) As Task Implements IPublisher.Publish
        Return Task.CompletedTask
    End Function

    Public Function CreateStream(Of TResponse)(request As IStreamRequest(Of TResponse), Optional cancellationToken As CancellationToken = Nothing) As IAsyncEnumerable(Of TResponse) Implements ISender.CreateStream
        Throw New NotSupportedException()
    End Function

    Public Function CreateStream(request As Object, Optional cancellationToken As CancellationToken = Nothing) As IAsyncEnumerable(Of Object) Implements ISender.CreateStream
        Throw New NotSupportedException()
    End Function
End Class
