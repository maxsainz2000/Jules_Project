Imports System
Imports System.Collections.Generic

Namespace Dtos

    Public Class ReceiveGoodsDto
        Public Property PurchaseOrderId As Integer
        Public Property Lines As List(Of ReceiveGoodsLineDto) = New List(Of ReceiveGoodsLineDto)()
    End Class

    Public Class ReceiveGoodsLineDto
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property QuantityOrdered As Integer
        Public Property QuantityReceived As Integer
        Public Property UnitCost As Decimal
        Public Property ExpiryDate As DateTime?
        Public Property DiscrepancyNotes As String
    End Class

End Namespace
