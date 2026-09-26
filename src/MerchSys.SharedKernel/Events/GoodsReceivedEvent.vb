Imports MediatR

Namespace Events

    Public Class GoodsReceivedEvent
        Implements INotification

        Public Property SourcePurchaseOrderId As Integer
        Public Property ReceiptDate As DateTime
        Public Property Items As List(Of GoodsReceivedItem) = New List(Of GoodsReceivedItem)()

        Public Class GoodsReceivedItem
            Public Property ProductId As Integer
            Public Property QuantityReceived As Integer
            Public Property UnitCost As Decimal
            Public Property ExpiryDate As DateTime?
        End Class
    End Class

End Namespace
