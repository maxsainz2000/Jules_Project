Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class PriceChangeAlert
        Inherits AuditableEntity

        Public Property ProductId As Integer
        Public Property VendorId As Integer
        Public Property PreviousUnitCost As Decimal
        Public Property NewUnitCost As Decimal
        Public Property ChangePercent As Decimal
        Public Property ChangeDirection As String
        Public Property GoodsReceiptId As Integer
        Public Property IsAcknowledged As Boolean
        Public Property AcknowledgedAt As DateTime?
    End Class
End Namespace
