Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class ReorderSuggestion
        Inherits AuditableEntity

        Public Property ProductId As Integer
        Public Property SuggestedQuantity As Integer
        Public Property Status As String = "Pending"
        Public Property ResultingPurchaseOrderId As Integer?
        Public Property Reason As String
        Public Property RowVersion As DateTime
    End Class
End Namespace
