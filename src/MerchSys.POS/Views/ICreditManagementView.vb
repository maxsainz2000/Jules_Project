Imports System.Collections.Generic
Imports MerchSys.POS.Entities
Imports MerchSys.POS.Presenters

Namespace Views

    Public Class AddAccountEventArgs
        Inherits EventArgs

        Public Property CustomerName As String
        Public Property Details As String
    End Class

    Public Class RecordPaymentEventArgs
        Inherits EventArgs

        Public Property AccountId As Integer
        Public Property Amount As Decimal
        Public Property PaymentMethod As String
    End Class

    Public Class AccountSelectedEventArgs
        Inherits EventArgs

        Public Property AccountId As Integer
    End Class

    Public Interface ICreditManagementView
        Event Load As EventHandler
        Event FilterChanged As EventHandler
        Event SearchTextChanged As EventHandler
        Event AccountSelected As EventHandler(Of AccountSelectedEventArgs)
        Event AddAccount As EventHandler(Of AddAccountEventArgs)
        Event RecordPayment As EventHandler(Of RecordPaymentEventArgs)

        Property TotalAR As Decimal
        Property BlockedCount As Integer
        Property OverdueCount As Integer

        Property FilterMode As String
        Property SearchTerm As String

        Property Accounts As IEnumerable(Of CreditAccount)
        Property SelectedAccount As CreditAccount
        Property TransactionHistory As IEnumerable(Of CreditTransactionItem)

        Property Message As String
        Property ErrorMessage As String
    End Interface

End Namespace
