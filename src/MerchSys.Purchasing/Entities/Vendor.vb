Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class Vendor
        Inherits SoftDeletableEntity

        Public Property Name As String
        Public Property Phone As String
        Public Property LeadTimeDays As Integer
    End Class
End Namespace
