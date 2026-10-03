Imports MerchSys.SharedKernel.Entities

Namespace Entities
    ''' <summary>
    ''' Represents a payment record for a credit account.
    ''' </summary>
    Public Class CreditPayment
        Inherits AuditableEntity

        ''' <summary>
        ''' Gets or sets the payment amount.
        ''' </summary>
        Public Property Amount As Decimal

        ''' <summary>
        ''' Gets or sets the payment method.
        ''' Explicitly prohibited: "Credit" cannot be used as a payment method for credit payments.
        ''' </summary>
        Public Property PaymentMethod As String

        ''' <summary>
        ''' Gets or sets the associated credit account ID.
        ''' </summary>
        Public Property CreditAccountId As Integer

        ''' <summary>
        ''' Gets or sets the associated credit account.
        ''' </summary>
        Public Property CreditAccount As CreditAccount
    End Class
End Namespace
