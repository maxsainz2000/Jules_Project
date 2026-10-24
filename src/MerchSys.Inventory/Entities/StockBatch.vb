Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class StockBatch
        Inherits AuditableEntity
        Implements MerchSys.SharedKernel.Interfaces.IAuditedEntity

        Public Property ProductId As Integer
        Public Property QuantityReceived As Integer
        Public Property QuantityRemaining As Integer
        Public Property UnitCost As Decimal
        Public Property ReceiptDate As DateTime
        Public Property ExpiryDate As DateTime?
        Public Property SourcePurchaseOrderId As Integer

        ''' <summary>
        ''' Optimistic concurrency token (mapped to MariaDB <c>TIMESTAMP(6)</c>).
        ''' Guards concurrent decrements of <see cref="QuantityRemaining"/> across clients.
        ''' </summary>
        Public Property RowVersion As DateTime

        Public ReadOnly Property IsExpired As Boolean
            Get
                If Not ExpiryDate.HasValue Then Return False
                Return ExpiryDate.Value.Date < DateTime.Today
            End Get
        End Property

        Public ReadOnly Property IsFullyConsumed As Boolean
            Get
                Return QuantityRemaining <= 0
            End Get
        End Property
    End Class
End Namespace
