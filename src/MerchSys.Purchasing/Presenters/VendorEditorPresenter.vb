Imports MerchSys.Purchasing.Services

Namespace Presenters
    Public Class VendorEditorPresenter
        Public Property Id As Integer?
        Public Property Name As String
        Public Property Phone As String
        Public Property Email As String
        Public Property ContactPerson As String
        Public Property Address As String
        Public Property TaxId As String
        Public Property Notes As String
        Public Property LeadTimeDays As Integer

        Public Sub Load(vendor As VendorDetailDto)
            If vendor IsNot Nothing Then
                Id = vendor.Id
                Name = vendor.Name
                Phone = vendor.Phone
                Email = vendor.Email
                ContactPerson = vendor.ContactPerson
                Address = vendor.Address
                TaxId = vendor.TaxId
                Notes = vendor.Notes
                LeadTimeDays = vendor.LeadTimeDays
            Else
                Clear()
            End If
        End Sub

        Public Sub Clear()
            Id = Nothing
            Name = String.Empty
            Phone = String.Empty
            Email = String.Empty
            ContactPerson = String.Empty
            Address = String.Empty
            TaxId = String.Empty
            Notes = String.Empty
            LeadTimeDays = 0
        End Sub

        Public Function Validate() As String
            If String.IsNullOrWhiteSpace(Name) Then Return "Vendor Name is required."
            If String.IsNullOrWhiteSpace(Phone) Then Return "Phone is required."
            If LeadTimeDays <= 0 Then Return "Lead Time must be greater than 0."
            Return Nothing
        End Function

        Public Function ToCreateDto() As CreateVendorDto
            Return New CreateVendorDto With {
                .Name = Name.Trim(),
                .Phone = Phone?.Trim(),
                .Email = Email?.Trim(),
                .ContactPerson = ContactPerson?.Trim(),
                .Address = Address?.Trim(),
                .TaxId = TaxId?.Trim(),
                .Notes = Notes?.Trim(),
                .LeadTimeDays = LeadTimeDays
            }
        End Function

        Public Function ToUpdateDto() As UpdateVendorDto
            Return New UpdateVendorDto With {
                .Id = Id.GetValueOrDefault(),
                .Name = Name.Trim(),
                .Phone = Phone?.Trim(),
                .Email = Email?.Trim(),
                .ContactPerson = ContactPerson?.Trim(),
                .Address = Address?.Trim(),
                .TaxId = TaxId?.Trim(),
                .Notes = Notes?.Trim(),
                .LeadTimeDays = LeadTimeDays
            }
        End Function
    End Class
End Namespace
