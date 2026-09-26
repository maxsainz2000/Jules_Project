Imports System.Threading
Imports MediatR
Imports MerchSys.SharedKernel.Events
Imports MerchSys.Inventory.Services

Namespace Handlers

    Public Class GoodsReceivedHandler
        Implements INotificationHandler(Of GoodsReceivedEvent)

        Private ReadOnly _stockService As IStockService

        Public Sub New(stockService As IStockService)
            _stockService = stockService
        End Sub

        Public Async Function Handle(notification As GoodsReceivedEvent, cancellationToken As CancellationToken) As Task Implements INotificationHandler(Of GoodsReceivedEvent).Handle
            For Each item In notification.Items
                Await _stockService.AddStockBatchAsync(
                    item.ProductId,
                    item.QuantityReceived,
                    item.UnitCost,
                    notification.ReceiptDate,
                    notification.SourcePurchaseOrderId,
                    item.ExpiryDate
                )
            Next
        End Function
    End Class

End Namespace
