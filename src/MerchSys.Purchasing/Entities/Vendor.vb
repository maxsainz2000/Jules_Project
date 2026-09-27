Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class Vendor
        Inherits SoftDeletableEntity

        Public Property Name As String
        Public Property LeadTimeDays As Integer
        ' Dummy comment to force a diff
    End Class
End Namespace
