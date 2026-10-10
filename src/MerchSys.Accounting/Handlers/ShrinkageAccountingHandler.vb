Imports System.Threading
Imports MediatR
Imports MerchSys.SharedKernel.Events
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities

Namespace Handlers
    Public Class ShrinkageAccountingHandler
        Implements INotificationHandler(Of ShrinkageRecordedEvent)

        Private ReadOnly _dbContext As AccountingDbContext

        Public Sub New(dbContext As AccountingDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function Handle(notification As ShrinkageRecordedEvent, cancellationToken As CancellationToken) As Task Implements INotificationHandler(Of ShrinkageRecordedEvent).Handle
            Dim expenseRecord = New ExpenseRecord With {
                .ExpenseDate = DateTime.UtcNow,
                .Category = "Shrinkage",
                .Description = $"Inventory shrinkage for Product #{notification.ProductId}: {notification.Reason}",
                .Amount = notification.TotalValue,
                .SourceModule = "Inventory",
                .SourceReferenceId = notification.ProductId
            }
            _dbContext.ExpenseRecords.Add(expenseRecord)

            Await _dbContext.SaveChangesAsync(cancellationToken)
        End Function
    End Class
End Namespace
