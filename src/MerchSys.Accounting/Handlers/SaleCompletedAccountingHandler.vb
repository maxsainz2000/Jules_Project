Imports System.Threading
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.SharedKernel.Events
Imports MerchSys.SharedKernel.Queries
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities

Namespace Handlers
    ''' <summary>
    ''' Records revenue and COGS for a completed sale.
    '''
    ''' This handler is deliberately idempotent. Two different events can describe the same sale —
    ''' <see cref="SaleCompletedEvent"/> (published by PaymentService) and
    ''' <see cref="SaleCompletedWithVatEvent"/> (published by VatAwareReceiptService) — and MediatR
    ''' gives no ordering guarantee between handlers. The original implementation called
    ''' <c>RevenueRecords.Add(...)</c> unconditionally, so whichever handler ran second inserted a
    ''' SECOND row for the same (sale, product) and revenue was double-counted. Upserting on
    ''' (SourceTransactionId, ProductId) means whichever handler runs first, the other completes the
    ''' same row.
    '''
    ''' Each handler writes only the columns it owns: this one owns the quantity/cost/payment
    ''' columns, <see cref="SaleCompletedWithVatHandler"/> owns the VAT columns. Neither clobbers
    ''' the other.
    ''' </summary>
    Public Class SaleCompletedAccountingHandler
        Implements INotificationHandler(Of SaleCompletedEvent)

        Private ReadOnly _dbContext As AccountingDbContext
        Private ReadOnly _mediator As IMediator

        Public Sub New(dbContext As AccountingDbContext, mediator As IMediator)
            _dbContext = dbContext
            _mediator = mediator
        End Sub

        Public Async Function Handle(notification As SaleCompletedEvent, cancellationToken As CancellationToken) As Task Implements INotificationHandler(Of SaleCompletedEvent).Handle
            For Each item In notification.Items
                Dim query = New GetProductCostQuery With {.ProductId = item.ProductId}
                Dim costResult = Await _mediator.Send(query, cancellationToken)

                Dim unitCost As Decimal = 0D
                If costResult IsNot Nothing Then
                    unitCost = costResult.FifoUnitCost
                End If

                Dim totalCogs As Decimal = CDec(item.QuantitySold) * unitCost

                Dim revenueRecord = Await _dbContext.RevenueRecords.FirstOrDefaultAsync(
                    Function(r) r.SourceTransactionId = notification.SaleId AndAlso r.ProductId = item.ProductId,
                    cancellationToken)

                If revenueRecord Is Nothing Then
                    revenueRecord = New RevenueRecord With {
                        .SourceTransactionId = notification.SaleId,
                        .ProductId = item.ProductId
                    }
                    _dbContext.RevenueRecords.Add(revenueRecord)
                End If

                revenueRecord.RecordDate = notification.SaleDate
                revenueRecord.QuantitySold = CDec(item.QuantitySold)
                revenueRecord.UnitCost = unitCost
                revenueRecord.TotalCogs = totalCogs
                revenueRecord.PaymentMethod = PaymentMethod.Cash

                ' The COGS expense is keyed on the same natural identity so a repeat publication does
                ' not book the expense twice. Description is deterministic per (sale, product).
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

                expenseRecord.ExpenseDate = notification.SaleDate
                expenseRecord.Amount = totalCogs
                expenseRecord.SourceModule = "POS"
            Next

            Await _dbContext.SaveChangesAsync(cancellationToken)
        End Function
    End Class
End Namespace
