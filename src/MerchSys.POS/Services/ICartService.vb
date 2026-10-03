Imports MerchSys.POS.Entities

Namespace Services

    Public Class CartLineDto
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property UnitPrice As Decimal
        Public Property Quantity As Integer
        Public Property LineTotal As Decimal
    End Class

    Public Class CartDto
        Public Property CartId As String
        Public Property Lines As List(Of CartLineDto) = New List(Of CartLineDto)()
        Public Property TotalAmount As Decimal
        Public Property VatableSales As Decimal
        Public Property VatAmount As Decimal
    End Class

    Public Interface ICartService
        Function GetCart(cartId As String) As CartDto
        Sub AddToCart(cartId As String, productId As Integer, productName As String, unitPrice As Decimal, quantity As Integer)
        Sub RemoveFromCart(cartId As String, productId As Integer)
        Sub ClearCart(cartId As String)
        Function FinalizeTransactionAsync(cartId As String, paymentMethod As String, Optional creditAccountId As Integer? = Nothing) As Task(Of SalesTransaction)
        Function VoidTransactionAsync(transactionId As Integer) As Task
        Function GetTransactionHistoryAsync() As Task(Of List(Of SalesTransaction))
    End Interface

End Namespace
