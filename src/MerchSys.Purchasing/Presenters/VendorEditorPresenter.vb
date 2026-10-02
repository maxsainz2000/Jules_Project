Imports MerchSys.Purchasing.Views
Imports MerchSys.Purchasing.Services

Namespace Presenters
    Public Class VendorEditorPresenter
        Public Property View As IVendorDirectoryView

        Public Sub New()
        End Sub

        Public Function Validate() As Boolean
            If View Is Nothing Then Return False

            If String.IsNullOrWhiteSpace(View.EditorName) Then
                View.ShowError("Vendor Name is required.")
                Return False
            End If

            If String.IsNullOrWhiteSpace(View.EditorPhone) Then
                View.ShowError("Vendor Phone is required.")
                Return False
            End If

            If View.EditorLeadTimeDays <= 0 Then
                View.ShowError("Lead Time Days must be greater than zero.")
                Return False
            End If

            Return True
        End Function

        Public Function ToCreateDto() As CreateVendorDto
            Return New CreateVendorDto With {
                .Name = View.EditorName.Trim(),
                .Phone = View.EditorPhone.Trim(),
                .LeadTimeDays = View.EditorLeadTimeDays
            }
        End Function

        Public Function ToUpdateDto(id As Integer) As UpdateVendorDto
            Return New UpdateVendorDto With {
                .Id = id,
                .Name = View.EditorName.Trim(),
                .Phone = View.EditorPhone.Trim(),
                .LeadTimeDays = View.EditorLeadTimeDays
            }
        End Function

        Public Sub LoadVendor(vendor As VendorDetailDto)
            If vendor IsNot Nothing Then
                View.EditorName = vendor.Name
                View.EditorPhone = vendor.Phone
                View.EditorLeadTimeDays = vendor.LeadTimeDays
            Else
                Clear()
            End If
        End Sub

        Public Sub Clear()
            If View IsNot Nothing Then
                View.EditorName = String.Empty
                View.EditorPhone = String.Empty
                View.EditorLeadTimeDays = 0
            End If
        End Sub
    End Class
End Namespace
