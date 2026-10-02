Imports System.Threading.Tasks
Imports System.Linq
Imports MerchSys.Purchasing.Services
Imports MerchSys.Purchasing.Views.Purchasing
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Paging
Imports System

Namespace Presenters
    Public Class VendorCatalogPresenter
        Private _view As IVendorCatalogView
        Private ReadOnly _vendorProductService As IVendorProductService
        Private ReadOnly _vendorService As IVendorService
        Private ReadOnly _sessionService As ISessionService
        Private _isEditing As Boolean = False
        Private _isAdding As Boolean = False
        Private _editingId As Integer? = Nothing

        Public Property View As IVendorCatalogView
            Get
                Return _view
            End Get
            Set(value As IVendorCatalogView)
                _view = value
                If _view IsNot Nothing Then
                    _view.OnVendorSelected = AddressOf LoadVendorProductsAsync
                    _view.OnAdd = AddressOf AddAsync
                    _view.OnEdit = AddressOf EditAsync
                    _view.OnSave = AddressOf SaveAsync
                    _view.OnDelete = AddressOf DeleteAsync
                End If
            End Set
        End Property

        Public Sub New(vendorProductService As IVendorProductService, vendorService As IVendorService, sessionService As ISessionService)
            _vendorProductService = vendorProductService
            _vendorService = vendorService
            _sessionService = sessionService
        End Sub

        Public Async Function LoadVendorsAsync() As Task
            Try
                Dim request As New PageRequest With { .PageSize = 1000, .IsFirstPage = True }
                Dim vendorsResult = Await _vendorService.GetHistoryAsync(request)
                _view.BindVendors(vendorsResult.Items.ToList())
            Catch ex As Exception
                _view.ShowError("Failed to load vendors: " & ex.Message)
            End Try
        End Function

        Private Async Function LoadVendorProductsAsync() As Task
            If Not _view.SelectedVendorId.HasValue Then
                _view.BindVendorProducts(Nothing)
                Return
            End If

            Try
                Dim request As New PageRequest With { .PageSize = 1000, .IsFirstPage = True }
                Dim vpResult = Await _vendorProductService.GetHistoryAsync(request)
                Dim vendorProducts = vpResult.Items.Where(Function(vp) vp.VendorId = _view.SelectedVendorId.Value AndAlso Not vp.IsDeleted).ToList()
                _view.BindVendorProducts(vendorProducts)
            Catch ex As Exception
                _view.ShowError("Failed to load vendor products: " & ex.Message)
            End Try
        End Function

        Private Function IsManager() As Boolean
            Return _sessionService.CurrentUserRole = UserRole.Manager OrElse
                   _sessionService.CurrentUserRole = UserRole.Developer
        End Function

        Private Async Function AddAsync() As Task
            If Not IsManager() Then
                _view.ShowError("Only managers can modify the vendor product catalog.")
                Return
            End If
            If Not _view.SelectedVendorId.HasValue Then
                _view.ShowError("Please select a vendor first.")
                Return
            End If

            _isAdding = True
            _isEditing = False
            _editingId = Nothing

            Await Task.CompletedTask
        End Function

        Private Async Function EditAsync() As Task
            If Not IsManager() Then
                _view.ShowError("Only managers can modify the vendor product catalog.")
                Return
            End If
            If Not _view.SelectedVendorProductId.HasValue Then
                _view.ShowError("Please select a vendor product to edit.")
                Return
            End If

            _isAdding = False
            _isEditing = True
            _editingId = _view.SelectedVendorProductId

            Await Task.CompletedTask
        End Function

        Private Async Function SaveAsync() As Task
            Try
                If Not _isAdding AndAlso Not _isEditing Then
                    Return
                End If

                If _isAdding Then
                    Await _vendorProductService.AddCatalogEntryAsync(_view.SelectedVendorId.Value, _view.ProductIdInput, _view.UnitCostInput, _view.NotesInput)
                ElseIf _isEditing AndAlso _editingId.HasValue Then
                    Await _vendorProductService.UpdateCatalogEntryAsync(_editingId.Value, _view.UnitCostInput, _view.NotesInput)
                End If

                _isAdding = False
                _isEditing = False
                _editingId = Nothing
                Await LoadVendorProductsAsync()
            Catch ex As Exception
                _view.ShowError("Failed to save entry: " & ex.Message)
            End Try
        End Function

        Private Async Function DeleteAsync() As Task
            If Not IsManager() Then
                _view.ShowError("Only managers can modify the vendor product catalog.")
                Return
            End If

            If Not _view.SelectedVendorProductId.HasValue Then
                _view.ShowError("Please select a vendor product to delete.")
                Return
            End If

            If _view.ConfirmAction("Are you sure you want to delete this catalog entry?", "Confirm Delete") Then
                Try
                    Await _vendorProductService.DeleteCatalogEntryAsync(_view.SelectedVendorProductId.Value)
                    Await LoadVendorProductsAsync()
                Catch ex As Exception
                    _view.ShowError("Failed to delete entry: " & ex.Message)
                End Try
            End If
        End Function
    End Class
End Namespace
