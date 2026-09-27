Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class ProductPriceHistory
        Inherits BaseEntity

        Public Property ProductId As Integer
        Public Property OldPrice As Decimal
        Public Property NewPrice As Decimal
        Public Property ChangedAt As DateTime
        Public Property ChangedBy As String
        Public Property Reason As String
    End Class
End Namespace
