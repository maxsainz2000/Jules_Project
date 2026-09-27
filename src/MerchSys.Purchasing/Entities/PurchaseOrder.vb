Imports MerchSys.SharedKernel.Entities
Imports MerchSys.SharedKernel.Enums

Namespace Entities
    Public Class PurchaseOrder
        Inherits SoftDeletableEntity

        Public Property OrderNumber As String
        Public Property Status As PurchaseOrderStatus
    End Class
End Namespace
