Imports MerchSys.SharedKernel.Entities
Imports MerchSys.SharedKernel.Enums

Namespace Entities
    Public Class PurchaseOrder
        Inherits SoftDeletableEntity

        Public Property OrderNumber As String
        Public Property VendorId As Integer
        Public Property Status As PurchaseOrderStatus
        Public Property Notes As String
        Public Property ExpectedDeliveryDate As DateTime?
        Public Property TotalAmount As Decimal
    End Class
End Namespace
