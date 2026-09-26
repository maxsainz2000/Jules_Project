Imports System.Threading
Imports MediatR
Imports Microsoft.Extensions.Logging
Imports MerchSys.SharedKernel.Events
Imports MerchSys.Inventory.Services

Namespace Handlers

    Public Class SaleCompletedHandler
        Implements INotificationHandler(Of SaleCompletedEvent)

        Private ReadOnly _stockService As IStockService
        Private ReadOnly _logger As ILogger(Of SaleCompletedHandler)

        Public Sub New(stockService As IStockService, logger As ILogger(Of SaleCompletedHandler))
            _stockService = stockService
            _logger = logger
        End Sub

        Public Async Function Handle(notification As SaleCompletedEvent, cancellationToken As CancellationToken) As Task Implements INotificationHandler(Of SaleCompletedEvent).Handle
            For Each item In notification.Items
                Try
                    Await _stockService.DeductStockFIFOAsync(item.ProductId, item.QuantitySold)
                Catch ex As InsufficientStockException
                    _logger.LogError(ex, "Failed to deduct stock for Sale {SaleId}, Product {ProductId}", notification.SaleId, item.ProductId)
                    Throw
                End Try
            Next
        End Function
    End Class

End Namespace
