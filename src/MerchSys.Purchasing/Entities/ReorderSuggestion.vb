Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class ReorderSuggestion
        Inherits BaseEntity

        Public Property ProductId As Integer
        Public Property SuggestedQuantity As Integer
        Public Property Status As String = "Pending"
        Public Property ResultingPurchaseOrderId As Integer?
        Public Property Reason As String
        Public Property RowVersion As Byte()
    End Class
End Namespace
