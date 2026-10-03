Imports System.Threading.Tasks
Imports System.Collections.Generic
Imports MerchSys.POS.Entities

Namespace Services

    Public Interface ICreditService
        Function CreateAccountAsync(account As CreditAccount) As Task
        Function UpdateAccountAsync(account As CreditAccount) As Task
        Function GetAccountAsync(accountId As Integer) As Task(Of CreditAccount)
        Function ListAccountsAsync() As Task(Of List(Of CreditAccount))
        ' Checks if credit can be extended.
        Function CheckCreditExtensionAsync(accountId As Integer) As Task
        Function ChargeAccountAsync(accountId As Integer, amount As Decimal) As Task
        Function RecordPaymentAsync(accountId As Integer, amount As Decimal, paymentMethod As String) As Task
        Function GetAccountHistoryAsync(accountId As Integer) As Task(Of List(Of CreditPayment))
        Function GetTotalsAsync() As Task(Of (TotalCreditExtended As Decimal, TotalOutstandingBalance As Decimal))
        Function GetOverdueAccountsAsync() As Task(Of List(Of CreditAccount))
    End Interface

End Namespace
