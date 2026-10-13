Imports System.Threading
Imports System.Threading.Tasks
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Logging
Imports MerchSys.Accounting.Data
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

                If _logger IsNot Nothing Then
                    _logger.LogInformation(
                        "Return on sale {SaleId} product {ProductId}: {Returned} returned ({Cumulative} cumulative of {Original}), net revenue now {NetRevenue}.",
                        notification.SaleId, item.ProductId, item.QuantityReturned,
                        item.CumulativeQuantityReturned, item.OriginalQuantitySold, revenueRecord.TotalRevenue)
                End If
            Next

            Await _dbContext.SaveChangesAsync(cancellationToken)
        End Function

    End Class

End Namespace
