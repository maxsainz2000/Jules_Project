Imports MerchSys.SharedKernel.Entities

Namespace Entities
    ''' <summary>
    ''' Represents a sales return record.
    ''' </summary>
    Public Class SalesReturn
        Inherits AuditableEntity

        ''' <summary>
        ''' Gets or sets the ID of the returned sales transaction.
        ''' </summary>
        Public Property SalesTransactionId As Integer

        ''' <summary>
        ''' Gets or sets the associated sales transaction.
        ''' </summary>
        Public Property SalesTransaction As SalesTransaction

        ''' <summary>
        ''' Gets or sets the reason for the return.
        ''' This field is required.
        ''' </summary>
        Public Property Reason As String

        ''' <summary>
        ''' Gets or sets a value indicating whether the returned items are restocked.
        ''' True signals the service layer to restore inventory.
        ''' </summary>
        Public Property IsRestocked As Boolean
    End Class
End Namespace
