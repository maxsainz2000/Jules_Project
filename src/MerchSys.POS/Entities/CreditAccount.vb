Imports MerchSys.SharedKernel.Entities

Namespace Entities
    ''' <summary>
    ''' Represents an informal credit (utang) account for a customer.
    ''' </summary>
    Public Class CreditAccount
        Inherits SoftDeletableEntity

        ''' <summary>
        ''' Gets or sets the customer's name.
        ''' </summary>
        Public Property CustomerName As String

        ''' <summary>
        ''' Gets or sets the current balance of the credit account.
        ''' </summary>
        Public Property CurrentBalance As Decimal

        ''' <summary>
        ''' Gets or sets a value indicating whether the account is hard-blocked.
        ''' Non-negotiable rule: Maintained as True when CurrentBalance > 0.
        ''' </summary>
        Public Property IsBlocked As Boolean
    End Class
End Namespace
