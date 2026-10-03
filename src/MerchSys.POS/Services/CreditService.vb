Imports System
Imports System.Threading.Tasks
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.POS.Data
Imports MerchSys.POS.Entities
Imports MerchSys.SharedKernel.Events
Imports MerchSys.SharedKernel.Interfaces

Namespace Services

    Public Class CreditService
        Implements ICreditService

        Private ReadOnly _db As POSDbContext
        Private ReadOnly _eventBus As IEventBus

        Public Sub New(db As POSDbContext, eventBus As IEventBus)
            _db = db
            _eventBus = eventBus
        End Sub

        Public Async Function CreateAccountAsync(account As CreditAccount) As Task Implements ICreditService.CreateAccountAsync
            _db.CreditAccounts.Add(account)
            Await _db.SaveChangesAsync()
        End Function

        Public Async Function UpdateAccountAsync(account As CreditAccount) As Task Implements ICreditService.UpdateAccountAsync
            _db.CreditAccounts.Update(account)
            Await _db.SaveChangesAsync()
        End Function

        Public Async Function GetAccountAsync(accountId As Integer) As Task(Of CreditAccount) Implements ICreditService.GetAccountAsync
            Return Await _db.CreditAccounts.FindAsync(accountId)
        End Function

        Public Async Function ListAccountsAsync() As Task(Of List(Of CreditAccount)) Implements ICreditService.ListAccountsAsync
            Return Await _db.CreditAccounts.ToListAsync()
        End Function

        Public Async Function CheckCreditExtensionAsync(accountId As Integer) As Task Implements ICreditService.CheckCreditExtensionAsync
            ' Implementation for credit extension check
            Dim account = Await _db.CreditAccounts.FindAsync(accountId)
            If account Is Nothing Then
                Throw New ArgumentException("Account not found.")
            End If

            If account.IsBlocked OrElse account.CurrentBalance > 0 Then
                Throw New CreditBlockedException($"Account for {account.CustomerName} is blocked from further credit until outstanding balance is settled.")
            End If
        End Function

        Public Async Function ChargeAccountAsync(accountId As Integer, amount As Decimal) As Task Implements ICreditService.ChargeAccountAsync
            Dim account = Await _db.CreditAccounts.FindAsync(accountId)
            If account Is Nothing Then
                Throw New ArgumentException("Account not found.")
            End If

            ' Evaluate zero-tolerance hard blocking rule
            If account.IsBlocked OrElse account.CurrentBalance > 0 Then
                Throw New CreditBlockedException($"Account for {account.CustomerName} is blocked from further credit until outstanding balance is settled.")
            End If

            account.CurrentBalance += amount
            account.TotalCreditExtended += amount
            account.LastTransactionDate = DateTime.Now

            ' Re-evaluate block status
            If account.CurrentBalance > 0 Then
                account.IsBlocked = True
            End If

            Await _db.SaveChangesAsync()
        End Function

        Public Async Function RecordPaymentAsync(accountId As Integer, amount As Decimal, paymentMethod As String) As Task Implements ICreditService.RecordPaymentAsync
            If String.Equals(paymentMethod, "Credit", StringComparison.OrdinalIgnoreCase) Then
                Throw New ArgumentException("Credit cannot be used as a payment method for credit payments.")
            End If

            Dim account = Await _db.CreditAccounts.FindAsync(accountId)
            If account Is Nothing Then
                Throw New ArgumentException("Account not found.")
            End If

            If amount <= 0 Then
                Throw New ArgumentException("Payment amount must be greater than zero.")
            End If

            If amount > account.CurrentBalance Then
                Throw New ArgumentException("Payment amount cannot exceed current balance.")
            End If

            account.CurrentBalance -= amount
            account.LastTransactionDate = DateTime.Now

            If account.CurrentBalance <= 0 Then
                account.IsBlocked = False
            End If

            Dim payment As New CreditPayment With {
                .CreditAccountId = accountId,
                .Amount = amount,
                .PaymentMethod = paymentMethod
            }

            _db.CreditPayments.Add(payment)
            Await _db.SaveChangesAsync()

            ' Publish CreditPaymentEvent via IEventBus
            Await _eventBus.PublishAsync(New CreditPaymentEvent(accountId, amount))
        End Function

        Public Async Function GetAccountHistoryAsync(accountId As Integer) As Task(Of List(Of CreditPayment)) Implements ICreditService.GetAccountHistoryAsync
            Return Await _db.CreditPayments.Where(Function(p) p.CreditAccountId = accountId).OrderByDescending(Function(p) p.Id).ToListAsync()
        End Function

        Public Async Function GetTotalsAsync() As Task(Of (TotalCreditExtended As Decimal, TotalOutstandingBalance As Decimal)) Implements ICreditService.GetTotalsAsync
            Dim totalCredit = If(Await _db.CreditAccounts.SumAsync(Function(a) CType(a.TotalCreditExtended, Decimal?)), 0D)
            Dim totalBalance = If(Await _db.CreditAccounts.SumAsync(Function(a) CType(a.CurrentBalance, Decimal?)), 0D)
            Return (totalCredit, totalBalance)
        End Function

        Public Async Function GetOverdueAccountsAsync() As Task(Of List(Of CreditAccount)) Implements ICreditService.GetOverdueAccountsAsync
            ' For now, just returning blocked accounts or accounts with balance > 0
            Return Await _db.CreditAccounts.Where(Function(a) a.CurrentBalance > 0).ToListAsync()
        End Function

    End Class

End Namespace
