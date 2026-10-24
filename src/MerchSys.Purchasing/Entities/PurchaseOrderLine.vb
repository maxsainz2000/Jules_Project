Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class PurchaseOrderLine
        Inherits AuditableEntity
        Implements MerchSys.SharedKernel.Interfaces.IAuditedEntity

        Public Property PurchaseOrderId As Integer
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property Quantity As Integer
        Public Property UnitCost As Decimal
        Public Property LineTotal As Decimal
        Public Property RowVersion As DateTime
    End Class
End Namespace
