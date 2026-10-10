Imports System.Threading
Imports MediatR
Imports MerchSys.SharedKernel.Events
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities

Namespace Handlers
    Public Class CreditPaymentAccountingHandler
        Implements INotificationHandler(Of CreditPaymentEvent)

        Private ReadOnly _dbContext As AccountingDbContext

        Public Sub New(dbContext As AccountingDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function Handle(notification As CreditPaymentEvent, cancellationToken As CancellationToken) As Task Implements INotificationHandler(Of CreditPaymentEvent).Handle
            Dim expenseRecord = New ExpenseRecord With {
                .ExpenseDate = DateTime.UtcNow,
                .Category = "AR Reduction",
                .Description = $"Credit repayment for account #{notification.CreditAccountId}",
                .Amount = notification.Amount,
                .SourceModule = "POS",
                .SourceReferenceId = notification.CreditAccountId
            }
            _dbContext.ExpenseRecords.Add(expenseRecord)

            Await _dbContext.SaveChangesAsync(cancellationToken)
        End Function
    End Class
End Namespace
