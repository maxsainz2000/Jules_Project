Imports MerchSys.POS.Entities
Imports System.Threading.Tasks
Imports System.Collections.Generic

Namespace Services
    Public Interface ICreditService
        Function CreateAccountAsync(customerName As String) As Task(Of CreditAccount)
        Function UpdateAccountAsync(account As CreditAccount) As Task(Of CreditAccount)
        Function GetAccountAsync(accountId As Integer) As Task(Of CreditAccount)
        Function ListAccountsAsync() As Task(Of List(Of CreditAccount))
        Function GetOverdueAccountsAsync() As Task(Of List(Of CreditAccount))
        Function CheckCreditExtensionAsync(accountId As Integer) As Task(Of Boolean)
        Function ChargeAccountAsync(accountId As Integer, amount As Decimal) As Task
        Function RecordPaymentAsync(accountId As Integer, amount As Decimal, paymentMethod As String) As Task
        Function GetAccountHistoryAsync(accountId As Integer) As Task(Of List(Of CreditPayment))
        Function GetTotalOutstandingBalanceAsync() As Task(Of Decimal)
    End Interface
End Namespace
