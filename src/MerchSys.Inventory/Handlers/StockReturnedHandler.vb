Imports System.Threading
Imports System.Threading.Tasks
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Logging
Imports MerchSys.Inventory.Data
Imports MerchSys.Inventory.Services
Imports MerchSys.SharedKernel.Events
Imports MerchSys.SharedKernel.Queries

Namespace Handlers

    ''' <summary>
    ''' Returns stock to inventory when a sale is returned.
    '''
    ''' The returned goods are valued at the cost the sale was actually booked at, fetched from
    ''' Accounting (<see cref="GetSaleLineCostsQuery"/>). That is the only correct source: the sale line
    ''' carries <c>UnitPrice</c> — the selling price — not cost, and recomputing from current stock
    ''' returns the cost of the batches still on hand rather than the ones the sale consumed.
    '''
    ''' The weighted average of remaining batches is kept only as a fallback for sales whose cost was
    ''' never recorded, and zero is a last resort that is logged, because a zero-cost batch silently
    ''' understates inventory value.
    '''
    ''' Restocked batches are dated at the return, so under FIFO they are consumed after existing stock.
    ''' </summary>
    Public Class StockReturnedHandler
        Implements INotificationHandler(Of SaleReturnedEvent)

        Private ReadOnly _dbContext As InventoryDbContext
        Private ReadOnly _stockService As IStockService
        Private ReadOnly _mediator As IMediator
        Private ReadOnly _logger As ILogger(Of StockReturnedHandler)

        Public Sub New(dbContext As InventoryDbContext, stockService As IStockService, mediator As IMediator, logger As ILogger(Of StockReturnedHandler))
            _dbContext = dbContext
            _stockService = stockService
            _mediator = mediator
            _logger = logger
        End Sub

        Public Async Function Handle(notification As SaleReturnedEvent, cancellationToken As CancellationToken) As Task Implements INotificationHandler(Of SaleReturnedEvent).Handle
            If Not notification.IsRestocked Then Return

            Dim recordedCosts = Await _mediator.Send(
                New GetSaleLineCostsQuery With {.SaleId = notification.SaleId}, cancellationToken)

            Dim costByProduct As New Dictionary(Of Integer, Decimal)()
            If recordedCosts IsNot Nothing Then
                For Each cost In recordedCosts.Costs
                    costByProduct(cost.ProductId) = cost.UnitCost
                Next
            End If

            For Each item In notification.Items
                If item.QuantityReturned <= 0 Then Continue For

                Dim unitCost As Decimal = 0D
                If costByProduct.ContainsKey(item.ProductId) Then
                    unitCost = costByProduct(item.ProductId)
                End If

                If unitCost <= 0 Then
                    unitCost = Await EstimateUnitCostAsync(item.ProductId, cancellationToken)
                End If

                If unitCost <= 0 AndAlso _logger IsNot Nothing Then
                    _logger.LogWarning(
                        "No cost is recorded for product {ProductId} on sale {SaleId}, and no stock is on hand to average. Restocking {Quantity} returned units at zero cost, which understates inventory value.",
                        item.ProductId, notification.SaleId, item.QuantityReturned)
                End If

                ' No purchase order behind a customer return.
                Await _stockService.AddStockBatchAsync(
                    item.ProductId,
                    item.QuantityReturned,
                    unitCost,
                    notification.ReturnedAt,
                    0,
                    Nothing)
            Next
        End Function

        ''' <summary>
        ''' Fallback only: the weighted-average cost of the batches still on hand.
        ''' </summary>
        Private Async Function EstimateUnitCostAsync(productId As Integer, cancellationToken As CancellationToken) As Task(Of Decimal)
            Dim batches = Await _dbContext.StockBatches.
                Where(Function(b) b.ProductId = productId AndAlso b.QuantityRemaining > 0).
                Select(Function(b) New With {b.QuantityRemaining, b.UnitCost}).
                ToListAsync(cancellationToken)

            Dim quantity = batches.Sum(Function(b) b.QuantityRemaining)
            If quantity <= 0 Then Return 0D

            Dim totalValue = batches.Sum(Function(b) CDec(b.QuantityRemaining) * b.UnitCost)
            Return totalValue / CDec(quantity)
        End Function

    End Class

End Namespace
