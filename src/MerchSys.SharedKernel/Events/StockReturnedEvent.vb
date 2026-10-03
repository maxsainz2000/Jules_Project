Imports MediatR

Namespace Events
    ''' <summary>
    ''' Event triggered when stock is returned in the POS module.
    ''' </summary>
    Public Class StockReturnedEvent
        Implements INotification

        ''' <summary>
        ''' Gets or sets the ID of the product returned.
        ''' </summary>
        Public Property ProductId As Integer

        ''' <summary>
        ''' Gets or sets the quantity of the product returned.
        ''' </summary>
        Public Property Quantity As Integer

        Public Sub New(productId As Integer, quantity As Integer)
            Me.ProductId = productId
            Me.Quantity = quantity
        End Sub
    End Class
End Namespace