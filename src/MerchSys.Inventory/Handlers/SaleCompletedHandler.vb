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
        Private ReadOnly _mediator As IMediator

        Public Sub New(stockService As IStockService, logger As ILogger(Of SaleCompletedHandler), mediator As IMediator)
            _stockService = stockService
            _logger = logger
            _mediator = mediator
        End Sub

        Public Async Function Handle(notification As SaleCompletedEvent, cancellationToken As CancellationToken) As Task Implements INotificationHandler(Of SaleCompletedEvent).Handle
            Dim deductedEvent = New StockDeductedEvent With {
                .SaleId = notification.SaleId,
                .DeductedAt = DateTime.UtcNow
            }

            For Each item In notification.Items
                Try
                    Dim result = Await _stockService.DeductStockFIFOAsync(item.ProductId, item.QuantitySold)

                    ' Capture the cost of what was actually consumed. This is the only point at which
                    ' the true FIFO cost of the sale is knowable — nothing records which batches a
                    ' sale consumed, so once the batches are decremented it cannot be reconstructed.
                    deductedEvent.Items.Add(New StockDeductedEvent.StockDeductedItem With {
                        .ProductId = item.ProductId,
                        .QuantityDeducted = result.DeductedQuantity,
                        .TotalCost = result.TotalCost
                    })
                Catch ex As InsufficientStockException
                    _logger.LogError(ex, "Failed to deduct stock for Sale {SaleId}, Product {ProductId}", notification.SaleId, item.ProductId)
                    Throw
                End Try
            Next

            If deductedEvent.Items.Count > 0 Then
                Await _mediator.Publish(deductedEvent, cancellationToken)
            End If
        End Function
    End Class

End Namespace
