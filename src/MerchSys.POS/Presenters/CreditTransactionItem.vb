Imports System

Namespace Presenters

    ''' <summary>
    ''' Represents a unified transaction item for a credit account's history.
    ''' </summary>
    Public Class CreditTransactionItem
        Public Property TransactionDate As DateTime
        Public Property Type As String
        Public Property ReferenceNumber As String
        Public Property Amount As Decimal
        Public Property Details As String
    End Class

End Namespace
