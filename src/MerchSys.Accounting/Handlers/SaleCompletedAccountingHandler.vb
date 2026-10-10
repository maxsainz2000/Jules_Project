Imports System.Threading
Imports MediatR
Imports MerchSys.SharedKernel.Events
Imports MerchSys.SharedKernel.Queries
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities

Namespace Handlers
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

                Dim revenueRecord = New RevenueRecord With {
                    .RecordDate = notification.SaleDate,
                    .ProductId = item.ProductId,
                    .QuantitySold = CDec(item.QuantitySold),
                    .TotalCogs = totalCogs,
                    .SourceTransactionId = notification.SaleId,
                    .PaymentMethod = PaymentMethod.Cash
                }
                _dbContext.RevenueRecords.Add(revenueRecord)

                Dim expenseRecord = New ExpenseRecord With {
                    .ExpenseDate = notification.SaleDate,
                    .Category = "COGS",
                    .Description = $"COGS for Sale #{notification.SaleId} Product #{item.ProductId}",
                    .Amount = totalCogs,
                    .SourceModule = "POS",
                    .SourceReferenceId = notification.SaleId
                }
                _dbContext.ExpenseRecords.Add(expenseRecord)
            Next

            Await _dbContext.SaveChangesAsync(cancellationToken)
        End Function
    End Class
End Namespace
