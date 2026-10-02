Imports System.Threading.Tasks
Imports MerchSys.Purchasing.Services
Imports MerchSys.Purchasing.Views
Imports MerchSys.SharedKernel.Interfaces

Namespace Presenters
    Public Class VendorListPresenter
        Private _view As IVendorDirectoryView
        Private ReadOnly _vendorService As IVendorService
        Private ReadOnly _sessionService As ISessionService
        Private ReadOnly _editorPresenter As VendorEditorPresenter

        Public Property View As IVendorDirectoryView
            Get
                Return _view
            End Get
            Set(value As IVendorDirectoryView)
                _view = value
                If _editorPresenter IsNot Nothing Then
                    _editorPresenter.View = _view
                End If
                If _view IsNot Nothing Then
                    _view.OnSearch = AddressOf SearchVendorsAsync
                    _view.OnSelectionChanged = AddressOf LoadVendorDetailsAsync
                    _view.OnAdd = AddressOf AddVendorAsync
                    _view.OnEdit = AddressOf EditVendorAsync
                    _view.OnSave = AddressOf SaveVendorAsync
                    _view.OnDelete = AddressOf DeleteVendorAsync
                End If
            End Set
        End Property

        Public Sub New(vendorService As IVendorService, sessionService As ISessionService, editorPresenter As VendorEditorPresenter)
            _vendorService = vendorService
            _sessionService = sessionService
            _editorPresenter = editorPresenter
        End Sub

        Public Async Function LoadVendorsAsync() As Task
            Try
                Dim vendors = Await _vendorService.GetAllAsync()
                _view.BindVendors(vendors)
            Catch ex As Exception
                _view.ShowError("Failed to load vendors: " & ex.Message)
            End Try
        End Function

        Public Async Function SearchVendorsAsync(term As String) As Task
            Try
                Dim vendors = Await _vendorService.SearchAsync(term)
                _view.BindVendors(vendors)
            Catch ex As Exception
                _view.ShowError("Search failed: " & ex.Message)
            End Try
        End Function

        Public Async Function LoadVendorDetailsAsync(vendorId As Integer?) As Task
            If Not vendorId.HasValue Then
                _view.BindRecentPOs(Nothing)
                Return
            End If

            Try
                Dim history = Await _vendorService.GetPurchaseHistoryAsync(vendorId.Value)
                If history IsNot Nothing Then
                    _view.BindRecentPOs(history.RecentPOs)
                End If
            Catch ex As Exception
                _view.ShowError("Failed to load vendor details: " & ex.Message)
            End Try
        End Function

        Private Function IsManager() As Boolean
            Return _sessionService.CurrentUserRole = MerchSys.SharedKernel.Enums.UserRole.Manager OrElse
                   _sessionService.CurrentUserRole = MerchSys.SharedKernel.Enums.UserRole.Developer
        End Function

        Public Sub ToggleEditor(isOpen As Boolean)
            If isOpen AndAlso Not IsManager() Then
                _view.ShowError("You do not have permission to modify vendors.")
                Return
            End If
            _view.IsEditorOpen = isOpen
        End Sub

        Private Async Function AddVendorAsync() As Task
            ToggleEditor(True)
            _editorPresenter.Clear()
            _view.SelectedVendorId = Nothing
            Await Task.CompletedTask
        End Function

        Private Async Function EditVendorAsync() As Task
            If Not _view.SelectedVendorId.HasValue Then
                _view.ShowError("Please select a vendor to edit.")
                Return
            End If

            Try
                Dim vendor = Await _vendorService.GetByIdAsync(_view.SelectedVendorId.Value)
                _editorPresenter.LoadVendor(vendor)
                ToggleEditor(True)
            Catch ex As Exception
                _view.ShowError("Failed to load vendor: " & ex.Message)
            End Try
        End Function

        Private Async Function SaveVendorAsync() As Task
            If Not _editorPresenter.Validate() Then
                Return
            End If

            Try
                If _view.SelectedVendorId.HasValue Then
                    Dim dto = _editorPresenter.ToUpdateDto(_view.SelectedVendorId.Value)
                    Await _vendorService.UpdateAsync(dto.Id, dto)
                Else
                    Dim dto = _editorPresenter.ToCreateDto()
                    Await _vendorService.CreateAsync(dto)
                End If

                ToggleEditor(False)
                Await LoadVendorsAsync()
            Catch ex As Exception
                _view.ShowError("Failed to save vendor: " & ex.Message)
            End Try
        End Function

        Private Async Function DeleteVendorAsync() As Task
            If Not IsManager() Then
                _view.ShowError("You do not have permission to delete vendors.")
                Return
            End If

            If Not _view.SelectedVendorId.HasValue Then
                _view.ShowError("Please select a vendor to delete.")
                Return
            End If

            If _view.ConfirmAction("Are you sure you want to delete this vendor?", "Confirm Delete") Then
                Try
                    Await _vendorService.DeleteAsync(_view.SelectedVendorId.Value)
                    Await LoadVendorsAsync()
                Catch ex As Exception
                    _view.ShowError("Failed to delete vendor: " & ex.Message)
                End Try
            End If
        End Function
    End Class
End Namespace
