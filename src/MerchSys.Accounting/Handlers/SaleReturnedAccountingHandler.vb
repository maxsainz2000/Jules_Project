Imports System.Threading
Imports System.Threading.Tasks
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Logging
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities
Imports MerchSys.SharedKernel.Events

Namespace Handlers

    ''' <summary>
    ''' Reverses the revenue and COGS for a returned sale.
    '''
    ''' Previously nothing did this at all: <c>SalesReturnService</c> restocked inventory and adjusted
    ''' the customer's credit balance, but the sale stayed on the books in full — so gross profit was
    ''' overstated by the returned margin, and POS and Accounting disagreed about the same transaction.
    '''
    ''' <b>The net position is set absolutely, not subtracted.</b> Each line carries both the quantity
    ''' returned now and the cumulative quantity returned including earlier returns, so the handler can
    ''' compute net = original − cumulative and assign it. Subtracting instead would mean a repeated
    ''' publication reversed twice; assigning cannot, so this is idempotent by construction.
    '''
    ''' The original sale is untouched in <c>Pos_SalesTransactions</c>, so gross activity is still
    ''' recoverable; the RevenueRecord reflects the net position for the line.
    '''
    ''' Three things are set on that net position:
    ''' <list type="bullet">
    '''   <item>revenue, COGS and gross profit, as above;</item>
    '''   <item>the output VAT and the VAT base it is reported with, reversed in proportion to the
    '''     units returned — otherwise the VAT return keeps reporting the original sale's VAT;</item>
    '''   <item>for a <b>non-restocked</b> return (goods damaged and discarded) a
    '''     <c>"Damaged Goods"</c> expense for the cost of the discarded units. COGS has already been
    '''     reduced to the net quantity, so without that expense the cost of the discarded goods
    '''     leaves the P&amp;L entirely and profit is overstated. Restocked returns need no such
    '''     expense: the goods are back in inventory.</item>
    ''' </list>
    ''' </summary>
    Public Class SaleReturnedAccountingHandler
        Implements INotificationHandler(Of SaleReturnedEvent)

        Private ReadOnly _dbContext As AccountingDbContext
        Private ReadOnly _logger As ILogger(Of SaleReturnedAccountingHandler)

        Public Sub New(dbContext As AccountingDbContext, logger As ILogger(Of SaleReturnedAccountingHandler))
            _dbContext = dbContext
            _logger = logger
        End Sub

        Public Async Function Handle(notification As SaleReturnedEvent, cancellationToken As CancellationToken) As Task Implements INotificationHandler(Of SaleReturnedEvent).Handle
            For Each item In notification.Items
                Dim revenueRecord = Await _dbContext.RevenueRecords.FirstOrDefaultAsync(
                    Function(r) r.SourceTransactionId = notification.SaleId AndAlso r.ProductId = item.ProductId,
                    cancellationToken)

                If revenueRecord Is Nothing Then
                    If _logger IsNot Nothing Then
                        _logger.LogWarning(
                            "No revenue record exists for sale {SaleId} product {ProductId}, so the return cannot be reflected in Accounting.",
                            notification.SaleId, item.ProductId)
                    End If
                    Continue For
                End If

                Dim netQuantity = item.OriginalQuantitySold - item.CumulativeQuantityReturned
                If netQuantity < 0 Then
                    netQuantity = 0
                End If

                revenueRecord.QuantitySold = CDec(netQuantity)
                revenueRecord.UnitPrice = item.UnitPrice
                revenueRecord.TotalRevenue = CDec(netQuantity) * item.UnitPrice
                revenueRecord.TotalCogs = CDec(netQuantity) * revenueRecord.UnitCost
                revenueRecord.GrossProfit = revenueRecord.TotalRevenue - revenueRecord.TotalCogs

                ' Reverse the output VAT (and the VAT base it is reported with) in proportion to the
                ' units returned. Assigned, not subtracted — the base comes from the event, never
                ' from this record's own columns, which the previous reversal already overwrote.
                ' Without this the VAT return kept reporting the whole original sale's output VAT
                ' after the goods came back, overstating net VAT payable.
                Dim netRatio As Decimal = If(item.OriginalQuantitySold > 0,
                                             CDec(netQuantity) / CDec(item.OriginalQuantitySold),
                                             0D)
                revenueRecord.VatableAmount = item.OriginalVatableAmount * netRatio
                revenueRecord.VatExemptAmount = item.OriginalVatExemptAmount * netRatio
                revenueRecord.ZeroRatedAmount = item.OriginalZeroRatedAmount * netRatio
                revenueRecord.OutputVat = item.OriginalOutputVat * netRatio

                ' The COGS expense was booked per (sale, product); bring it down to match.
                Dim description = $"COGS for Sale #{notification.SaleId} Product #{item.ProductId}"
                Dim expenseRecord = Await _dbContext.ExpenseRecords.FirstOrDefaultAsync(
                    Function(e) e.SourceReferenceId.HasValue AndAlso
                                 e.SourceReferenceId.Value = notification.SaleId AndAlso
                                 e.Category = "COGS" AndAlso
                                 e.Description = description,
                    cancellationToken)

                If expenseRecord IsNot Nothing Then
                    expenseRecord.Amount = revenueRecord.TotalCogs
                End If

                ' Goods returned damaged and discarded are a loss, not a cost of sale. COGS above
                ' has already been brought down to the net quantity, so without this the cost of the
                ' discarded units would drop out of the P&L altogether and profit would be
                ' overstated. Keyed per return (the description carries the return id) so a repeated
                ' publication re-sets the same amount instead of booking the loss twice.
                If Not notification.IsRestocked Then
                    Dim damagedDescription = $"Damaged goods for Sale #{notification.SaleId} Product #{item.ProductId} Return #{notification.SalesReturnId}"
                    Dim damagedExpense = Await _dbContext.ExpenseRecords.FirstOrDefaultAsync(
                        Function(e) e.SourceReferenceId.HasValue AndAlso
                                     e.SourceReferenceId.Value = notification.SaleId AndAlso
                                     e.Category = "Damaged Goods" AndAlso
                                     e.Description = damagedDescription,
                        cancellationToken)

                    If damagedExpense Is Nothing Then
                        damagedExpense = New ExpenseRecord With {
                            .ExpenseDate = notification.ReturnedAt,
                            .Category = "Damaged Goods",
                            .Description = damagedDescription,
                            .SourceModule = "POS",
                            .SourceReferenceId = notification.SaleId
                        }
                        _dbContext.ExpenseRecords.Add(damagedExpense)
                    End If

                    damagedExpense.Amount = CDec(item.QuantityReturned) * revenueRecord.UnitCost
                End If

                If _logger IsNot Nothing Then
                    _logger.LogInformation(
                        "Return on sale {SaleId} product {ProductId}: {Returned} returned ({Cumulative} cumulative of {Original}), restocked={Restocked}. Net revenue now {NetRevenue}, output VAT now {OutputVat}.",
                        notification.SaleId, item.ProductId, item.QuantityReturned,
                        item.CumulativeQuantityReturned, item.OriginalQuantitySold, notification.IsRestocked,
                        revenueRecord.TotalRevenue, revenueRecord.OutputVat)
                End If
            Next

            Await _dbContext.SaveChangesAsync(cancellationToken)
        End Function

    End Class

End Namespace
