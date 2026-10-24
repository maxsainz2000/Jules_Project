Imports MerchSys.SharedKernel.Entities
Imports MerchSys.SharedKernel.Enums

Namespace Entities
    Public Class PurchaseOrder
        Inherits SoftDeletableEntity
        Implements MerchSys.SharedKernel.Interfaces.IAuditedEntity
        Implements MerchSys.SharedKernel.Interfaces.IPaginatedEntity

        Public Property OrderNumber As String
        Public Property VendorId As Integer
        Public Property Status As PurchaseOrderStatus
        Public Property Notes As String
        Public Property ExpectedDeliveryDate As DateTime?
        Public Property TotalAmount As Decimal

        ''' <summary>
        ''' Optimistic concurrency token on the order's mutable state (AGENTS.md). Without it the
        ''' status transitions are check-then-act, so two clients closing the same order both pass
        ''' the status check and both book an accounts-payable entry — the liability is counted
        ''' twice. Must be mapped explicitly; <c>BaseDbContext</c> strips an unmapped one.
        ''' </summary>
        Public Property RowVersion As DateTime
    End Class
End Namespace
