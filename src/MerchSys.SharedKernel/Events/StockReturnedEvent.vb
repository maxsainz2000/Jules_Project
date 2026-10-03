Imports MediatR

Namespace Events

    Public Class StockReturnedEvent
        Implements INotification

        Public Property ReturnId As Integer
        Public Property ReturnDate As DateTime
        Public Property Items As List(Of StockReturnedItem) = New List(Of StockReturnedItem)()

        Public Class StockReturnedItem
            Public Property ProductId As Integer
            Public Property Quantity As Integer
        End Class

    End Class

End Namespace
