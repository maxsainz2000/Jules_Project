Imports System
Imports System.Collections.Generic
Imports MerchSys.POS.Presenters

Namespace Views

    Public Class TransactionSelectedEventArgs
        Inherits EventArgs
        Public Property TransactionId As Integer
    End Class

    Public Class PrintReceiptRequestedEventArgs
        Inherits EventArgs
        Public Property TransactionId As Integer
    End Class

    Public Class ProcessReturnRequestedEventArgs
        Inherits EventArgs
        Public Property TransactionId As Integer
        Public Property ReturnQuantities As Dictionary(Of Integer, Integer)
        Public Property Reason As String
        Public Property IsRestocked As Boolean
    End Class

    Public Class VoidTransactionRequestedEventArgs
        Inherits EventArgs
        Public Property TransactionId As Integer
    End Class

    Public Interface ITransactionHistoryView

        Event Load As EventHandler
        Event SearchRequested As EventHandler
        Event TransactionSelected As EventHandler(Of TransactionSelectedEventArgs)
        Event PrintReceiptRequested As EventHandler(Of PrintReceiptRequestedEventArgs)
        Event ProcessReturnRequested As EventHandler(Of ProcessReturnRequestedEventArgs)
        Event VoidTransactionRequested As EventHandler(Of VoidTransactionRequestedEventArgs)

        Property SearchStartDate As DateTime?
        Property SearchEndDate As DateTime?
        Property SearchTransactionNumber As String
        Property SearchPaymentMethod As String

        Property Transactions As IEnumerable(Of TransactionSummaryItem)
        Property SelectedTransaction As TransactionSummaryItem
        Property SelectedTransactionLines As IEnumerable(Of TransactionDetailLine)
        Property SelectedTransactionReturns As IEnumerable(Of ReturnSummaryItem)

        Property ReceiptPreview As String
        Property ErrorMessage As String
        Property Message As String

    End Interface

End Namespace
