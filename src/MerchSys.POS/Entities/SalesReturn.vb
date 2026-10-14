Imports MerchSys.SharedKernel.Entities

Namespace Entities
    ''' <summary>
    ''' Represents a sales return record.
    ''' </summary>
    Public Class SalesReturn
        Inherits AuditableEntity
        Implements MerchSys.SharedKernel.Interfaces.IAuditedEntity

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

          ''' <summary>
          ''' Gets or sets the returned lines as JSON: product id, quantity and unit price.
          '''
          ''' Returns used to be recorded with no line detail at all. That made the returned quantity
          ''' for a line impossible to reconstruct, so a customer could return more than they bought
          ''' across several returns, and the accounting reversal could not be made idempotent.
          ''' Serializing mirrors the existing convention on
          ''' <c>OfficialReceipt.SerializedLineItems</c>.
          ''' </summary>
          Public Property SerializedLines As String
      End Class
End Namespace
