Imports MerchSys.SharedKernel.Enums

Namespace Dtos
    Public Class PendingDeliveryRowDto
        Public Property POId As Integer
        Public Property OrderNumber As String
        Public Property VendorName As String
        Public Property OrderDate As DateTime
        Public Property ExpectedDeliveryDate As DateTime?
        Public Property Status As PurchaseOrderStatus
        Public Property TotalAmount As Decimal
    End Class
End Namespace