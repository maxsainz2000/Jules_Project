Imports System.Threading
Imports MediatR
Imports MerchSys.SharedKernel.Events
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities

Namespace Handlers
    Public Class GoodsReceivedAccountingHandler
        Implements INotificationHandler(Of GoodsReceivedEvent)

        Private ReadOnly _dbContext As AccountingDbContext

        Public Sub New(dbContext As AccountingDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function Handle(notification As GoodsReceivedEvent, cancellationToken As CancellationToken) As Task Implements INotificationHandler(Of GoodsReceivedEvent).Handle
            For Each item In notification.Items
                Dim expenseRecord = New ExpenseRecord With {
                    .ExpenseDate = notification.ReceiptDate,
                    .Category = "Purchase",
                    .Description = $"Purchase PO #{notification.SourcePurchaseOrderId} Product #{item.ProductId}",
                    .Amount = CDec(item.QuantityReceived) * item.UnitCost,
                    .SourceModule = "Purchasing",
                    .SourceReferenceId = notification.SourcePurchaseOrderId
                }
                _dbContext.ExpenseRecords.Add(expenseRecord)
            Next

            Await _dbContext.SaveChangesAsync(cancellationToken)
        End Function
    End Class
End Namespace
