Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class GoodsReceipt
        Inherits AuditableEntity

        Public Property ReceiptNumber As String
        Public Property PurchaseOrderId As Integer

        Public Property Lines As List(Of GoodsReceiptLine) = New List(Of GoodsReceiptLine)()
    End Class
End Namespace
