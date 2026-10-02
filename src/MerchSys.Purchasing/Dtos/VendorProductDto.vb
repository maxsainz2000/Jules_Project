Namespace Dtos
    Public Class VendorProductDto
        Public Property Id As Integer
        Public Property VendorId As Integer
        Public Property ProductId As Integer
        Public Property UnitCost As Decimal
        Public Property Notes As String
        Public Property CreatedAt As DateTime
        Public Property CreatedBy As String
        Public Property ModifiedAt As DateTime?
        Public Property ModifiedBy As String
    End Class
End Namespace
