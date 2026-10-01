Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports MerchSys.Purchasing.Services
Imports MerchSys.Purchasing.Views
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.SharedKernel.Enums
Imports System.Collections.Generic

Namespace Presenters
    Public Class VendorListPresenter
        Private ReadOnly _vendorService As IVendorService
        Private ReadOnly _sessionService As ISessionService
        Private ReadOnly _editorPresenter As VendorEditorPresenter

        Private _view As IVendorDirectoryView
        Public Property View As IVendorDirectoryView
            Get
                Return _view
            End Get
            Set(value As IVendorDirectoryView)
                _view = value
                If _view IsNot Nothing Then
                    WireUpEvents()
                End If
            End Set
        End Property

        Public Sub New(vendorService As IVendorService, sessionService As ISessionService)
            _vendorService = vendorService
            _sessionService = sessionService
            _editorPresenter = New VendorEditorPresenter()
        End Sub

        Private Sub WireUpEvents()
            AddHandler View.SearchTextChanged, AddressOf OnSearchTextChanged
            AddHandler View.RefreshClicked, AddressOf OnRefreshClicked
            AddHandler View.AddClicked, AddressOf OnAddClicked
            AddHandler View.EditClicked, AddressOf OnEditClicked
            AddHandler View.DeleteClicked, AddressOf OnDeleteClicked
            AddHandler View.SaveClicked, Async Sub(s, e) Await OnSaveClickedAsync()
            AddHandler View.CancelClicked, AddressOf OnCancelClicked
            AddHandler View.SelectionChanged, Async Sub(s, e) Await OnSelectionChangedAsync()
        End Sub

        Public Async Function InitializeAsync() As Task
            Await LoadVendorsAsync()
        End Function

        Private Async Sub OnSearchTextChanged(sender As Object, e As EventArgs)
            Await LoadVendorsAsync()
        End Sub

        Private Async Sub OnRefreshClicked(sender As Object, e As EventArgs)
            Await LoadVendorsAsync()
        End Sub

        Private Async Function LoadVendorsAsync() As Task
            Try
                Dim vendors As List(Of VendorDetailDto)
                If String.IsNullOrWhiteSpace(View.SearchTerm) Then
                    vendors = Await _vendorService.GetAllAsync()
                Else
                    vendors = Await _vendorService.SearchAsync(View.SearchTerm)
                End If
                View.Vendors = vendors
            Catch ex As Exception
                View.ShowError($"Failed to load vendors: {ex.Message}")
            End Try
        End Function

        Private Sub OnAddClicked(sender As Object, e As EventArgs)
            If Not IsManager() Then
                View.ShowError("You do not have permission to add vendors.")
                Return
            End If

            _editorPresenter.Clear()
            UpdateViewFromEditor("Add New Vendor")
            View.IsEditorOpen = True
        End Sub

        Private Sub OnEditClicked(sender As Object, e As EventArgs)
            If Not IsManager() Then
                View.ShowError("You do not have permission to edit vendors.")
                Return
            End If

            Dim selected = View.SelectedVendor
            If selected Is Nothing Then
                View.ShowError("Please select a vendor to edit.")
                Return
            End If

            _editorPresenter.Load(selected)
            UpdateViewFromEditor($"Edit Vendor: {selected.Name}")
            View.IsEditorOpen = True
        End Sub

        Private Async Sub OnDeleteClicked(sender As Object, e As EventArgs)
            If Not IsManager() Then
                View.ShowError("You do not have permission to delete vendors.")
                Return
            End If

            Dim selected = View.SelectedVendor
            If selected Is Nothing Then
                View.ShowError("Please select a vendor to delete.")
                Return
            End If

            If View.ConfirmDelete(selected.Name) Then
                Try
                    Await _vendorService.DeleteAsync(selected.Id)
                    View.ShowMessage("Vendor deleted successfully.")
                    View.IsEditorOpen = False
                    Await LoadVendorsAsync()
                Catch ex As Exception
                    View.ShowError($"Failed to delete vendor: {ex.Message}")
                End Try
            End If
        End Sub

        Private Async Function OnSaveClickedAsync() As Task
            UpdateEditorFromView()
            Dim validationError = _editorPresenter.Validate()
            If Not String.IsNullOrEmpty(validationError) Then
                View.ShowError(validationError)
                Return
            End If

            Try
                If _editorPresenter.Id.HasValue Then
                    Await _vendorService.UpdateAsync(_editorPresenter.Id.Value, _editorPresenter.ToUpdateDto())
                    View.ShowMessage("Vendor updated successfully.")
                Else
                    Await _vendorService.CreateAsync(_editorPresenter.ToCreateDto())
                    View.ShowMessage("Vendor created successfully.")
                End If

                View.IsEditorOpen = False
                Await LoadVendorsAsync()
            Catch ex As Exception
                View.ShowError($"Failed to save vendor: {ex.Message}")
            End Try
        End Function

        Private Sub OnCancelClicked(sender As Object, e As EventArgs)
            View.IsEditorOpen = False
            _editorPresenter.Clear()
        End Sub

        Private Async Function OnSelectionChangedAsync() As Task
            Dim selected = View.SelectedVendor
            If selected IsNot Nothing Then
                Try
                    View.PurchaseHistory = Await _vendorService.GetPurchaseHistoryAsync(selected.Id)
                Catch ex As Exception
                    View.ShowError($"Failed to load purchase history: {ex.Message}")
                End Try
            Else
                View.PurchaseHistory = Nothing
            End If
        End Function

        Private Sub UpdateViewFromEditor(title As String)
            View.EditorTitle = title
            View.EditorName = _editorPresenter.Name
            View.EditorPhone = _editorPresenter.Phone
            View.EditorEmail = _editorPresenter.Email
            View.EditorContactPerson = _editorPresenter.ContactPerson
            View.EditorAddress = _editorPresenter.Address
            View.EditorTaxId = _editorPresenter.TaxId
            View.EditorNotes = _editorPresenter.Notes
            View.EditorLeadTimeDays = _editorPresenter.LeadTimeDays
        End Sub

        Private Sub UpdateEditorFromView()
            _editorPresenter.Name = View.EditorName
            _editorPresenter.Phone = View.EditorPhone
            _editorPresenter.Email = View.EditorEmail
            _editorPresenter.ContactPerson = View.EditorContactPerson
            _editorPresenter.Address = View.EditorAddress
            _editorPresenter.TaxId = View.EditorTaxId
            _editorPresenter.Notes = View.EditorNotes
            _editorPresenter.LeadTimeDays = View.EditorLeadTimeDays
        End Sub

        Private Function IsManager() As Boolean
            Dim role = _sessionService.CurrentUserRole
            Return role = UserRole.Manager OrElse role = UserRole.Owner OrElse role = UserRole.Developer
        End Function
    End Class

    Public Class POSummaryRow
        Public Property PONumber As String
        Public Property CreatedAt As DateTime
        Public Property TotalAmount As Decimal
        Public Property Status As String
    End Class
End Namespace
