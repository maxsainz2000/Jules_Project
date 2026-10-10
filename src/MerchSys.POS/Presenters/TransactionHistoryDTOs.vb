Imports System

Namespace Presenters

    Public Class TransactionSummaryItem
        Public Property TransactionId As Integer
        Public Property TransactionNumber As String
        Public Property TransactionDate As DateTime
        Public Property PaymentMethod As String
        Public Property TotalAmount As Decimal
        Public Property IsVoid As Boolean
        Public Property ReturnStatus As String
    End Class

    Public Class TransactionDetailLine
        Public Property LineId As Integer
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property UnitPrice As Decimal
        Public Property Quantity As Integer
        Public Property LineTotal As Decimal
        Public Property ReturnedQuantity As Integer
        Public Property AvailableForReturn As Integer
    End Class

    Public Class ReturnSummaryItem
        Public Property ReturnId As Integer
        Public Property ReturnDate As DateTime
        Public Property Reason As String
        Public Property RefundAmount As Decimal
        Public Property IsRestocked As Boolean
    End Class

    Public Class ReturnLineSelection
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property MaxQuantity As Integer
        Public Property ReturnQuantity As Integer
    End Class

End Namespace
