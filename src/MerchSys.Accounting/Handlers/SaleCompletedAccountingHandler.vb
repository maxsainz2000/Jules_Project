Imports System.Threading
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.SharedKernel.Events
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities

Namespace Handlers
    ''' <summary>
    ''' Records the revenue side of a completed sale: when it happened, how much was sold, at what
    ''' price, and how it was paid.
    '''
    ''' This handler owns the quantity/price/payment columns. COGS is deliberately NOT written here —
    ''' only Inventory knows the true FIFO cost of the units consumed, and it arrives separately via
    ''' <see cref="StockDeductedEvent"/>. The VAT columns are owned by
    ''' <see cref="SaleCompletedWithVatHandler"/>. Three handlers write the same row, so each touches
    ''' only its own columns and upserts on (SourceTransactionId, ProductId); whichever runs last
    ''' leaves the row complete rather than clobbering the others.
    ''' </summary>
    Public Class SaleCompletedAccountingHandler
        Implements INotificationHandler(Of SaleCompletedEvent)

        Private ReadOnly _dbContext As AccountingDbContext

        Public Sub New(dbContext As AccountingDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function Handle(notification As SaleCompletedEvent, cancellationToken As CancellationToken) As Task Implements INotificationHandler(Of SaleCompletedEvent).Handle
            For Each item In notification.Items
                Dim revenueRecord = Await RevenueRecordUpsert.FindOrCreateAsync(_dbContext, notification.SaleId, item.ProductId, cancellationToken)

                revenueRecord.RecordDate = notification.SaleDate
                revenueRecord.QuantitySold = CDec(item.QuantitySold)
                revenueRecord.UnitPrice = item.UnitPrice
                revenueRecord.TotalRevenue = CDec(item.QuantitySold) * item.UnitPrice
                revenueRecord.PaymentMethod = PaymentMethod.Cash

                ' Recompute from whatever is currently persisted. The COGS handler may not have run
                ' yet, in which case TotalCogs is still 0 and this is corrected when it does — and
                ' symmetrically, the COGS handler recomputes after this one has set revenue.
                revenueRecord.GrossProfit = revenueRecord.TotalRevenue - revenueRecord.TotalCogs
            Next

            Await _dbContext.SaveChangesAsync(cancellationToken)
        End Function
    End Class
End Namespace
