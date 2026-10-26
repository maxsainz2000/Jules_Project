Imports System.Threading
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.SharedKernel.Events
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities

Namespace Handlers
    ''' <summary>
    ''' Books COGS for a completed sale from the cost Inventory reports in
    ''' <see cref="StockDeductedEvent"/>.
    '''
    ''' The cost has to arrive this way. Nothing in the model records which batches a sale consumed,
    ''' so the FIFO cost is only knowable at deduction time — a query against *remaining* stock
    ''' returns the cost of the batches that are left, which is both biased (it is the next cost, not
    ''' the consumed one) and dependent on whether Inventory or Accounting happens to run first.
    '''
    ''' <see cref="SaleCompletedAccountingHandler"/> owns the revenue columns on the same row; this
    ''' handler owns the cost columns. Both upsert on (SourceTransactionId, ProductId) and recompute
    ''' gross profit from the persisted row, so the final value is correct whichever runs last.
    ''' </summary>
    Public Class StockDeductedAccountingHandler
        Implements INotificationHandler(Of StockDeductedEvent)

        Private ReadOnly _dbContext As AccountingDbContext
        Private ReadOnly _mediator As IMediator

        Public Sub New(dbContext As AccountingDbContext, Optional mediator As IMediator = Nothing)
            _dbContext = dbContext
            _mediator = mediator
        End Sub

        Public Async Function Handle(notification As StockDeductedEvent, cancellationToken As CancellationToken) As Task Implements INotificationHandler(Of StockDeductedEvent).Handle
            Dim breakdown As List(Of SharedKernel.Queries.SaleCogsBreakdownDto) = Nothing
            If _mediator IsNot Nothing Then
                Try
                    breakdown = Await _mediator.Send(New SharedKernel.Queries.GetSaleCogsBreakdownQuery With {.SaleId = notification.SaleId}, cancellationToken)
                Catch ex As Exception
                    ' Defensively fall back
                End Try
            End If

            For Each item In notification.Items
                If item.QuantityDeducted <= 0 Then Continue For

                Dim trueCost = item.TotalCost
                If breakdown IsNot Nothing Then
                    Dim itemBreakdown = breakdown.Where(Function(b) b.ProductId = item.ProductId).ToList()
                    If itemBreakdown.Any() Then
                        trueCost = itemBreakdown.Sum(Function(b) b.TotalCost)
                    End If
                End If

                Dim revenueRecord = Await RevenueRecordUpsert.FindOrCreateAsync(_dbContext, notification.SaleId, item.ProductId, cancellationToken)

                revenueRecord.UnitCost = trueCost / CDec(item.QuantityDeducted)
                revenueRecord.TotalCogs = trueCost

                ' Revenue may not have been recorded yet if this handler won the race — in that case
                ' TotalRevenue is still 0 here and the revenue handler corrects gross profit when it
                ' runs. Whichever finishes second leaves the row consistent.
                revenueRecord.GrossProfit = revenueRecord.TotalRevenue - revenueRecord.TotalCogs

                ' One COGS expense per (sale, product), keyed on the same natural identity so a repeat
                ' publication does not book the expense twice. SourceModule stays "POS" — the sale is
                ' a POS transaction even though the cost is reported by Inventory.
                Dim description = $"COGS for Sale #{notification.SaleId} Product #{item.ProductId}"
                Dim expenseRecord = Await _dbContext.ExpenseRecords.FirstOrDefaultAsync(
                    Function(e) e.SourceReferenceId.HasValue AndAlso
                                 e.SourceReferenceId.Value = notification.SaleId AndAlso
                                 e.Category = "COGS" AndAlso
                                 e.Description = description,
                    cancellationToken)

                If expenseRecord Is Nothing Then
                    expenseRecord = New ExpenseRecord With {
                        .SourceReferenceId = notification.SaleId,
                        .Category = "COGS",
                        .Description = description
                    }
                    _dbContext.ExpenseRecords.Add(expenseRecord)
                End If

                expenseRecord.ExpenseDate = notification.DeductedAt
                expenseRecord.Amount = trueCost
                expenseRecord.SourceModule = "POS"
            Next

            Await _dbContext.SaveChangesAsync(cancellationToken)
        End Function
    End Class
End Namespace
