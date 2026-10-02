Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class VendorProduct
        Inherits SoftDeletableEntity
        ' Added comment to force diff for code review

        Public Property VendorId As Integer
        Public Property ProductId As Integer
        Public Property UnitCost As Decimal
        Public Property Notes As String
    End Class
End Namespace
