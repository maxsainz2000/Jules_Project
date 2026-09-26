Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class StockBatch
        Inherits AuditableEntity

        Public Property QuantityReceived As Integer
        Public Property QuantityRemaining As Integer
        Public Property UnitCost As Decimal
        Public Property ReceiptDate As DateTime
        Public Property ExpiryDate As DateTime?
        Public Property SourcePurchaseOrderId As Integer

        Public ReadOnly Property IsExpired As Boolean
            Get
                Return ExpiryDate.HasValue AndAlso ExpiryDate.Value.Date < DateTime.UtcNow.Date
            End Get
        End Property

        Public ReadOnly Property IsFullyConsumed As Boolean
            Get
                Return QuantityRemaining <= 0
            End Get
        End Property
    End Class
End Namespace
