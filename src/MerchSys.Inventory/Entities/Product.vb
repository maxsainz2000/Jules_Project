Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class Product
        Inherits SoftDeletableEntity

        Public Property ProductCategoryId As Integer
        Public Property Name As String
        Public Property SKU As String
        Public Property RetailPrice As Decimal
        Public Property Unit As String
        Public Property HasExpiry As Boolean
        Public Property MinimumThreshold As Integer
    End Class
End Namespace
