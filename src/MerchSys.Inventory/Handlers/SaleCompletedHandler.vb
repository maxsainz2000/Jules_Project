Imports System.Threading
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Logging
Imports MerchSys.Inventory.Data
Imports MerchSys.Inventory.Services
Imports MerchSys.SharedKernel.Events
Imports MerchSys.SharedKernel.Exceptions

Namespace Handlers

    ''' <summary>
    ''' Deducts stock for a completed sale.
    '''
    ''' The whole item set runs in <b>one</b> transaction. It previously saved each item
    ''' independently, so a failure on item 3 left items 1 and 2 deducted with no rollback — and
    ''' because <c>StockDeductedEvent</c> is only published after the loop, those deductions were
    ''' never reported to Accounting either, leaving their revenue with no COGS against it. Wrapping
    ''' the loop and publishing after the commit fixes both halves.
    ''' </summary>
    Public Class SaleCompletedHandler
        Implements INotificationHandler(Of SaleCompletedEvent)

        Private ReadOnly _stockService As IStockService
        Private ReadOnly _dbContext As InventoryDbContext
        Private ReadOnly _logger As ILogger(Of SaleCompletedHandler)
        Private ReadOnly _mediator As IMediator

        Public Sub New(stockService As IStockService, dbContext As InventoryDbContext, logger As ILogger(Of SaleCompletedHandler), mediator As IMediator)
            _stockService = stockService
            _dbContext = dbContext
            _logger = logger
            _mediator = mediator
        End Sub

        Public Async Function Handle(notification As SaleCompletedEvent, cancellationToken As CancellationToken) As Task Implements INotificationHandler(Of SaleCompletedEvent).Handle
            Dim deductedEvent = New StockDeductedEvent With {
                .SaleId = notification.SaleId,
                .DeductedAt = DateTime.UtcNow
            }

            ' DeductStockFIFOAsync joins this transaction instead of starting its own, so every item
            ' shares one unit of work. Disposing without committing is the rollback path — that is
            ' what makes a failure on any item undo the whole set.
            Using transaction = Await _dbContext.Database.BeginTransactionAsync(cancellationToken)
                For Each item In notification.Items
                    Dim result As FIFODeductionResult
                    Try
                        result = Await _stockService.DeductStockFIFOAsync(item.ProductId, item.QuantitySold)
                    Catch ex As DbUpdateConcurrencyException
                        ' AGENTS.md: surface the conflict, never silently overwrite. With FOR UPDATE in
                        ' place this should not happen for the batch rows any more, but a writer that
                        ' does not take the lock can still move them.
                        _logger.LogError(ex, "Concurrent modification while deducting stock for Sale {SaleId}, Product {ProductId}", notification.SaleId, item.ProductId)
                        Throw New ConcurrentModificationException($"Stock for product {item.ProductId} was changed by another client while processing sale {notification.SaleId}.")
                    Catch ex As InsufficientStockException
                        _logger.LogError(ex, "Failed to deduct stock for Sale {SaleId}, Product {ProductId}", notification.SaleId, item.ProductId)
                        Throw
                    End Try

                    ' Capture the cost of what was actually consumed. This is the only point at which
                    ' the true FIFO cost of the sale is knowable — nothing records which batches a
                    ' sale consumed, so once the batches are decremented it cannot be reconstructed.
                    deductedEvent.Items.Add(New StockDeductedEvent.StockDeductedItem With {
                        .ProductId = item.ProductId,
                        .QuantityDeducted = result.DeductedQuantity,
                        .TotalCost = result.TotalCost
                    })
                Next

                Await transaction.CommitAsync(cancellationToken)
            End Using

            ' Published only after the commit: a deduction that rolled back must never be announced.
            If deductedEvent.Items.Count > 0 Then
                Await _mediator.Publish(deductedEvent, cancellationToken)
            End If
        End Function
    End Class

End Namespace
