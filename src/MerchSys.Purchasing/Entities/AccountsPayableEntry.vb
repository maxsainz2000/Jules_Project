Imports System
Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class AccountsPayableEntry
        Inherits AuditableEntity

        Public Property VendorId As Integer
        Public Property PurchaseOrderId As Integer?
        Public Property InvoiceDate As DateTime
        Public Property DueDate As DateTime
        Public Property TotalAmount As Decimal
        Public Property AmountPaid As Decimal
        Public Property IsPaid As Boolean

        ''' <summary>
        ''' Optimistic concurrency token (mapped to MariaDB <c>TIMESTAMP(6)</c>).
        ''' Guards concurrent payment recording against <see cref="AmountPaid"/> across clients
        ''' (AGENTS.md).
        ''' </summary>
        Public Property RowVersion As DateTime
    End Class
End Namespace
