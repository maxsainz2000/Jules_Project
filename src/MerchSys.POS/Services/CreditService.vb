Imports MerchSys.POS.Entities
Imports MerchSys.POS.Exceptions
Imports MerchSys.POS.Data
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.SharedKernel.Events
Imports System.Threading.Tasks
Imports System.Collections.Generic
Imports Microsoft.EntityFrameworkCore

Namespace Services
    Public Class CreditService
        Implements ICreditService

        Private ReadOnly _dbContext As POSDbContext
        Private ReadOnly _eventBus As IEventBus

        Public Sub New(dbContext As POSDbContext, eventBus As IEventBus)
            _dbContext = dbContext
            _eventBus = eventBus
        End Sub

        Public Async Function CreateAccountAsync(customerName As String) As Task(Of CreditAccount) Implements ICreditService.CreateAccountAsync
            Dim account = New CreditAccount With {
                .CustomerName = customerName,
                .CurrentBalance = 0D,
                .IsBlocked = False,
                .TotalCreditExtended = 0D
            }
            _dbContext.CreditAccounts.Add(account)
            Await _dbContext.SaveChangesAsync()
            Return account
        End Function

        Public Async Function UpdateAccountAsync(account As CreditAccount) As Task(Of CreditAccount) Implements ICreditService.UpdateAccountAsync
            _dbContext.CreditAccounts.Update(account)
            Await _dbContext.SaveChangesAsync()
            Return account
        End Function

        Public Async Function GetAccountAsync(accountId As Integer) As Task(Of CreditAccount) Implements ICreditService.GetAccountAsync
            Return Await _dbContext.CreditAccounts.FindAsync(accountId)
        End Function

        Public Async Function ListAccountsAsync() As Task(Of List(Of CreditAccount)) Implements ICreditService.ListAccountsAsync
            Dim dummy = Await _dbContext.CreditAccounts.Select(Function(x) New With {x.Id, x.CustomerName, x.CurrentBalance, x.IsBlocked, x.TotalCreditExtended, x.LastTransactionDate, x.IsDeleted}).ToListAsync()
            Dim result = New List(Of CreditAccount)
            For Each item In dummy
                result.Add(New CreditAccount With {
                    .Id = item.Id,
                    .CustomerName = item.CustomerName,
                    .CurrentBalance = item.CurrentBalance,
                    .IsBlocked = item.IsBlocked,
                    .TotalCreditExtended = item.TotalCreditExtended,
                    .LastTransactionDate = item.LastTransactionDate,
                    .IsDeleted = item.IsDeleted
                })
            Next
            Return result
        End Function

        Public Async Function GetOverdueAccountsAsync() As Task(Of List(Of CreditAccount)) Implements ICreditService.GetOverdueAccountsAsync
            ' Assuming overdue means balance > 0 and last transaction date is older than 30 days
            Dim cutoffDate = DateTime.UtcNow.AddDays(-30)
            Dim dummy = Await _dbContext.CreditAccounts.Where(Function(a) a.CurrentBalance > 0 AndAlso a.LastTransactionDate < cutoffDate).Select(Function(x) New With {x.Id, x.CustomerName, x.CurrentBalance, x.IsBlocked, x.TotalCreditExtended, x.LastTransactionDate, x.IsDeleted}).ToListAsync()
            Dim result = New List(Of CreditAccount)
            For Each item In dummy
                result.Add(New CreditAccount With {
                    .Id = item.Id,
                    .CustomerName = item.CustomerName,
                    .CurrentBalance = item.CurrentBalance,
                    .IsBlocked = item.IsBlocked,
                    .TotalCreditExtended = item.TotalCreditExtended,
                    .LastTransactionDate = item.LastTransactionDate,
                    .IsDeleted = item.IsDeleted
                })
            Next
            Return result
        End Function

        Public Async Function CheckCreditExtensionAsync(accountId As Integer) As Task(Of Boolean) Implements ICreditService.CheckCreditExtensionAsync
            Dim account = Await _dbContext.CreditAccounts.FindAsync(accountId)
            If account Is Nothing Then Return False
            Return Not account.IsBlocked
        End Function

        Public Async Function ChargeAccountAsync(accountId As Integer, amount As Decimal) As Task Implements ICreditService.ChargeAccountAsync
            Dim account = Await _dbContext.CreditAccounts.FindAsync(accountId)
            If account Is Nothing Then Throw New System.ArgumentException("Account not found", NameOf(accountId))

            If account.IsBlocked Then
                Throw New CreditBlockedException("Credit account is blocked.")
            End If

            account.CurrentBalance += amount
            account.TotalCreditExtended += amount
            account.LastTransactionDate = DateTime.UtcNow
            account.IsBlocked = True ' Non-negotiable rule: Maintained as True when CurrentBalance > 0.

            _dbContext.CreditAccounts.Update(account)
            Await _dbContext.SaveChangesAsync()
        End Function

        Public Async Function RecordPaymentAsync(accountId As Integer, amount As Decimal, paymentMethod As String) As Task Implements ICreditService.RecordPaymentAsync
            If String.Equals(paymentMethod, "Credit", StringComparison.OrdinalIgnoreCase) Then
                Throw New System.ArgumentException("Credit cannot be used as a payment method for credit payments.", NameOf(paymentMethod))
            End If

            Dim account = Await _dbContext.CreditAccounts.FindAsync(accountId)
            If account Is Nothing Then Throw New System.ArgumentException("Account not found", NameOf(accountId))

            Dim payment = New CreditPayment With {
                .CreditAccountId = accountId,
                .Amount = amount,
                .PaymentMethod = paymentMethod
            }
            _dbContext.CreditPayments.Add(payment)

            account.CurrentBalance -= amount
            account.LastTransactionDate = DateTime.UtcNow

            If account.CurrentBalance <= 0 Then
                account.IsBlocked = False ' Unblock when balance is paid off
            End If

            _dbContext.CreditAccounts.Update(account)
            Await _dbContext.SaveChangesAsync()

            Await _eventBus.PublishAsync(New CreditPaymentEvent())
        End Function

        Public Async Function GetAccountHistoryAsync(accountId As Integer) As Task(Of List(Of CreditPayment)) Implements ICreditService.GetAccountHistoryAsync
            Dim dummy = Await _dbContext.CreditPayments.Where(Function(p) p.CreditAccountId = accountId).Select(Function(x) New With {x.Id, x.Amount, x.PaymentMethod, x.CreditAccountId, x.CreatedAt, x.CreatedBy, x.ModifiedAt, x.ModifiedBy}).ToListAsync()
            Dim result = New List(Of CreditPayment)
            For Each item In dummy
                result.Add(New CreditPayment With {
                    .Id = item.Id,
                    .Amount = item.Amount,
                    .PaymentMethod = item.PaymentMethod,
                    .CreditAccountId = item.CreditAccountId,
                    .CreatedAt = item.CreatedAt,
                    .CreatedBy = item.CreatedBy,
                    .ModifiedAt = item.ModifiedAt,
                    .ModifiedBy = item.ModifiedBy
                })
            Next
            Return result
        End Function

        Public Async Function GetTotalOutstandingBalanceAsync() As Task(Of Decimal) Implements ICreditService.GetTotalOutstandingBalanceAsync
            Return Await _dbContext.CreditAccounts.SumAsync(Function(a) a.CurrentBalance)
        End Function

    End Class
End Namespace
