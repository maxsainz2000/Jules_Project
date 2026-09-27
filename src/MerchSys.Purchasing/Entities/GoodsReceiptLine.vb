Imports System
Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class GoodsReceiptLine
        Inherits AuditableEntity

        Public Property GoodsReceiptId As Integer
        Public Property ProductId As Integer
        Public Property UnitCost As Decimal
        Public Property ExpiryDate As DateTime?
        Public Property HasDiscrepancy As Boolean
        Public Property QuantityReceived As Integer
        Public Property DiscrepancyNotes As String
    End Class
End Namespace
