Imports System
Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class GoodsReceiptLine
        Inherits AuditableEntity

        Public Property GoodsReceiptId As Integer
        Public Property ProductId As Integer
        Public Property UnitCost As Decimal
        Public Property ExpiryDate As DateTime?
        Public Property QuantityOrdered As Integer
        Public Property QuantityReceived As Integer
        Public Property DiscrepancyNotes As String
        Public Property HasDiscrepancy As Boolean
    End Class
End Namespace
