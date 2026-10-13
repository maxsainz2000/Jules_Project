Imports System.Threading
Imports System.Threading.Tasks
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Logging
Imports MerchSys.Inventory.Data
Imports MerchSys.Inventory.Services
Imports MerchSys.SharedKernel.Events

Namespace Handlers

    ''' <summary>
    ''' Returns stock to inventory when a sale is returned.
    '''
    ''' <c>SalesReturnService</c> publishes <see cref="StockReturnedEvent"/> but no handler existed, so
    ''' a return decremented nothing and the goods were simply lost from inventory.
    '''
    ''' <b>Valuation is an estimate, deliberately.</b> <see cref="StockReturnedEvent"/> carries only a
    ''' product and a quantity — no cost — and Inventory has no record of which batches the original
    ''' sale consumed, so the true cost of the returned units cannot be recovered. The batch is
    ''' restocked at the current weighted-average cost of that product's remaining batches. That keeps
    ''' inventory quantity exact and valuation approximately continuous; a product with no batches
    ''' left is restocked at zero and logged, because there is nothing to average.
    '''
    ''' Restocked batches are dated now, so under FIFO they are consumed after existing stock rather
    ''' than ahead of it.
    ''' </summary>
    Public Class StockReturnedHandler
        Implements INotificationHandler(Of StockReturnedEvent)

        Private ReadOnly _dbContext As InventoryDbContext
        Private ReadOnly _stockService As IStockService
        Private ReadOnly _logger As ILogger(Of StockReturnedHandler)

        Public Sub New(dbContext As InventoryDbContext, stockService As IStockService, logger As ILogger(Of StockReturnedHandler))
            _dbContext = dbContext
            _stockService = stockService
            _logger = logger
        End Sub

        Public Async Function Handle(notification As StockReturnedEvent, cancellationToken As CancellationToken) As Task Implements INotificationHandler(Of StockReturnedEvent).Handle
            If notification.Quantity <= 0 Then Return

            Dim unitCost = Await EstimateUnitCostAsync(notification.ProductId, cancellationToken)

            If _logger IsNot Nothing Then
                _logger.LogInformation(
                    "Restocking {Quantity} of product {ProductId} from a sales return at an estimated unit cost of {UnitCost}.",
                    notification.Quantity, notification.ProductId, unitCost)
            End If

            ' No purchase order behind a customer return.
            Await _stockService.AddStockBatchAsync(
                notification.ProductId,
                notification.Quantity,
                unitCost,
                DateTime.UtcNow,
                0,
                Nothing)
        End Function

        Private Async Function EstimateUnitCostAsync(productId As Integer, cancellationToken As CancellationToken) As Task(Of Decimal)
            Dim batches = Await _dbContext.StockBatches.
                Where(Function(b) b.ProductId = productId AndAlso b.QuantityRemaining > 0).
                Select(Function(b) New With {b.QuantityRemaining, b.UnitCost}).
                ToListAsync(cancellationToken)

            Dim quantity = batches.Sum(Function(b) b.QuantityRemaining)
            If quantity <= 0 Then
                If _logger IsNot Nothing Then
                    _logger.LogWarning(
                        "Product {ProductId} has no stock on hand, so a returned quantity cannot be valued. Restocking at zero cost.",
                        productId)
                End If
                Return 0D
            End If

            Dim totalValue = batches.Sum(Function(b) CDec(b.QuantityRemaining) * b.UnitCost)
            Return totalValue / CDec(quantity)
        End Function

    End Class

End Namespace
