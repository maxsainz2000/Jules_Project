Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class PurchaseOrderLine
        Inherits AuditableEntity

        Public Property PurchaseOrderId As Integer
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property Quantity As Integer
        Public Property UnitCost As Decimal
        Public Property LineTotal As Decimal
        Public Property RowVersion As Byte()
    End Class
End Namespace
