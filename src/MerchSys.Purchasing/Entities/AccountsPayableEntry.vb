Imports System
Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class AccountsPayableEntry
        Inherits AuditableEntity

        Public Property VendorId As Integer
        Public Property TotalAmount As Decimal
        Public Property AmountPaid As Decimal
        Public Property IsPaid As Boolean
    End Class
End Namespace
