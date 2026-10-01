Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class Vendor
        Inherits SoftDeletableEntity

        Public Property Name As String
        Public Property Phone As String
        Public Property Email As String
        Public Property ContactPerson As String
        Public Property Address As String
        Public Property TaxId As String
        Public Property Notes As String
        Public Property LeadTimeDays As Integer
        ' Dummy comment to force a diff
    End Class
End Namespace
