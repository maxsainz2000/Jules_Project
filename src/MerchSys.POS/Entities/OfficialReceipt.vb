Imports MerchSys.SharedKernel.Entities

Namespace Entities
    ''' <summary>
    ''' Represents a BIR-compliant official receipt for a sales transaction.
    ''' The receipt numbering format must follow OR-YYYY-XXXX.
    ''' </summary>
    Public Class OfficialReceipt
        Inherits AuditableEntity

        ''' <summary>
        ''' Gets or sets the receipt number (format: OR-YYYY-XXXX).
        ''' </summary>
        Public Property ReceiptNumber As String

        ''' <summary>
        ''' Gets or sets the serialized line items for reprint purposes.
        ''' </summary>
        Public Property SerializedLineItems As String
    End Class
End Namespace
