Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class PurchaseOrderLine
        Inherits AuditableEntity

        Public Property ProductName As String
        Public Property RowVersion As Byte()
    End Class
End Namespace
