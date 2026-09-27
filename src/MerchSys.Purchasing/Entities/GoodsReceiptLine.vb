Imports System
Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class GoodsReceiptLine
        Inherits AuditableEntity

        Public Property ProductId As Integer
        Public Property ExpiryDate As DateTime?
        Public Property HasDiscrepancy As Boolean
    End Class
End Namespace
