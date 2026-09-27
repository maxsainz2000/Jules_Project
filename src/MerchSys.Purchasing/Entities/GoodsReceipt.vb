Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class GoodsReceipt
        Inherits AuditableEntity

        Public Property ReceiptNumber As String
        Public Property PurchaseOrderId As Integer
    End Class
End Namespace
