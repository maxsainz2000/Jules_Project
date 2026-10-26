Imports System.Threading
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities
Imports MerchSys.SharedKernel.Events

Namespace Handlers
    ''' <summary>
    ''' Consolidated handler for revenue and VAT records of a completed sale.
    '''
    ''' This handler owns the quantity/price/payment columns and the VAT columns. COGS is
    ''' deliberately NOT written here — only Inventory knows the true FIFO cost of the units
    ''' consumed, and it arrives separately via <see cref="StockDeductedEvent"/>. Both upsert on
    ''' (SourceTransactionId, ProductId) and recompute gross profit from the persisted row, so the
    ''' final value is correct whichever runs last.
    ''' </summary>
    Public Class SaleRevenueHandler
        Implements INotificationHandler(Of SaleCompletedEvent)
        Implements INotificationHandler(Of SaleCompletedWithVatEvent)

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
                revenueRecord.PaymentMethod = notification.PaymentMethod

                ' Recompute from whatever is currently persisted. The COGS handler may not have run
                ' yet, in which case TotalCogs is still 0 and this is corrected when it does — and
                ' symmetrically, the COGS handler recomputes after this one has set revenue.
                revenueRecord.GrossProfit = revenueRecord.TotalRevenue - revenueRecord.TotalCogs

                If revenueRecord.TotalCogs > 0 Then
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
                            .SourceModule = "POS",
                            .Description = description
                        }
                        _dbContext.ExpenseRecords.Add(expenseRecord)
                    End If

                    expenseRecord.ExpenseDate = notification.SaleDate
                    expenseRecord.Amount = revenueRecord.TotalCogs
                End If
            Next

            Await _dbContext.SaveChangesAsync(cancellationToken)
        End Function

        Public Async Function Handle(notification As SaleCompletedWithVatEvent, cancellationToken As CancellationToken) As Task Implements INotificationHandler(Of SaleCompletedWithVatEvent).Handle
            For Each item In notification.Items
                Dim record = Await RevenueRecordUpsert.FindOrCreateAsync(_dbContext, notification.SaleId, item.ProductId, cancellationToken)

                record.VatableAmount = item.VatableAmount
                record.VatExemptAmount = item.VatExemptAmount
                record.ZeroRatedAmount = item.ZeroRatedAmount
                record.OutputVat = item.OutputVat
                record.VatTreatment = item.Treatment.ToString()
            Next

            Await _dbContext.SaveChangesAsync(cancellationToken)
        End Function
    End Class
End Namespace
