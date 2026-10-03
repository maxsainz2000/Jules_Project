Imports MerchSys.SharedKernel.Entities

Namespace Entities
    ''' <summary>
    ''' Represents a core sales transaction in the POS system.
    ''' </summary>
    Public Class SalesTransaction
        Inherits SoftDeletableEntity

        ''' <summary>
        ''' Gets or sets the transaction number.
        ''' </summary>
        Public Property TransactionNumber As String

        ''' <summary>
        ''' Gets or sets the transaction date.
        ''' </summary>
        Public Property TransactionDate As DateTime

        ''' <summary>
        ''' Gets or sets the total amount of the transaction.
        ''' </summary>
        Public Property TotalAmount As Decimal

        ''' <summary>
        ''' Gets or sets the payment method used for the transaction.
        ''' </summary>
        Public Property PaymentMethod As String

        ''' <summary>
        ''' Gets or sets a value indicating whether the transaction is voided.
        ''' </summary>
        Public Property IsVoid As Boolean

        ''' <summary>
        ''' Gets or sets the collection of lines associated with this transaction.
        ''' </summary>
        Public Property Lines As List(Of SalesTransactionLine) = New List(Of SalesTransactionLine)()

        ''' <summary>
        ''' Gets or sets the official receipt associated with this transaction.
        ''' </summary>
        Public Property Receipt As OfficialReceipt

        ''' <summary>
        ''' Gets or sets the credit account associated with this transaction, if applicable.
        ''' </summary>
        Public Property CreditAccount As CreditAccount
    End Class
End Namespace
