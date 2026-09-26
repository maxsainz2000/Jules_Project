Imports MediatR

Namespace Events

    Public Class SaleCompletedEvent
        Implements INotification

        Public Property SaleId As Integer
        Public Property SaleDate As DateTime
        Public Property Items As List(Of SaleCompletedItem) = New List(Of SaleCompletedItem)()

        Public Class SaleCompletedItem
            Public Property ProductId As Integer
            Public Property QuantitySold As Integer
        End Class
    End Class

End Namespace
