Imports System.Threading.Tasks
Imports System.Linq
Imports MerchSys.Purchasing.Services
Imports MerchSys.Purchasing.Views
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Paging
Imports Microsoft.Extensions.DependencyInjection
Imports System.ComponentModel
Imports System.Windows.Forms

Namespace Presenters
    Public Class PurchaseOrderListPresenter
        Public ReadOnly Property View As IPurchaseOrderListView
        Private ReadOnly _poService As IPurchaseOrderService
        Private ReadOnly _sessionService As ISessionService
        Private ReadOnly _serviceProvider As IServiceProvider
        Private ReadOnly _vendorService As IVendorService
        Private _currentPage As Integer = 1
        Private _pageSize As Integer = 50

        Public Sub New(view As IPurchaseOrderListView, poService As IPurchaseOrderService, sessionService As ISessionService, serviceProvider As IServiceProvider, vendorService As IVendorService)
            Me.View = view
            _poService = poService
            _sessionService = sessionService
            _serviceProvider = serviceProvider
            _vendorService = vendorService

            Me.View.OnLoadPage = AddressOf LoadDataAsync
            Me.View.OnSearch = AddressOf SearchAsync
            Me.View.OnNewPO = AddressOf NewPOAsync
            Me.View.OnEditPO = AddressOf EditPOAsync
            Me.View.OnSubmitPO = AddressOf SubmitPOAsync
            Me.View.OnDeletePO = AddressOf DeleteAsync

            SetRoleBasedPermissions()
        End Sub

        Private Sub SetRoleBasedPermissions()
            If _sessionService.CurrentUserRole = UserRole.Owner Then
                Me.View.CanEdit = False
            Else
                Me.View.CanEdit = True
            End If
        End Sub

        Private Async Function LoadDataAsync() As Task
            Await LoadPageAsync(1, Nothing, Nothing)
        End Function

        Private Async Function SearchAsync(term As String, status As PurchaseOrderStatus?) As Task
            Await LoadPageAsync(1, term, status)
        End Function

        Private Async Function LoadPageAsync(page As Integer, term As String, status As PurchaseOrderStatus?) As Task
            Try
                Dim items = Await _poService.SearchAsync(term, status)
                Dim vendors = Await _vendorService.GetAllAsync()

                Dim displayItems = items.Select(Function(p) New With {
                    .Id = p.Id,
                    .OrderNumber = p.OrderNumber,
                    .VendorName = vendors.FirstOrDefault(Function(v) v.Id = p.VendorId)?.Name,
                    .Status = p.Status,
                    .ExpectedDeliveryDate = p.ExpectedDeliveryDate,
                    .TotalAmount = p.TotalAmount
                }).ToList()

                Me.View.BindData(displayItems)
            Catch ex As Exception
                Me.View.ShowError("Failed to load purchase orders: " & ex.Message)
            End Try
        End Function

        Private Async Function NewPOAsync() As Task
            Using scope = _serviceProvider.CreateScope()
                Dim editor = DirectCast(scope.ServiceProvider.GetService(GetType(PurchaseOrderEditorPresenter)), PurchaseOrderEditorPresenter)
                editor.SetPurchaseOrderId(Nothing)
                If editor.View.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                    Await LoadDataAsync()
                End If
            End Using
        End Function

        Private Async Function EditPOAsync(id As Integer) As Task
            Dim po = Await _poService.GetByIdAsync(id)
            If po Is Nothing Then Return

            Using scope = _serviceProvider.CreateScope()
                Dim editor = DirectCast(scope.ServiceProvider.GetService(GetType(PurchaseOrderEditorPresenter)), PurchaseOrderEditorPresenter)
                editor.SetPurchaseOrderId(id)
                If editor.View.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                    Await LoadDataAsync()
                End If
            End Using
        End Function

        Private Async Function SubmitPOAsync(id As Integer) As Task
            Try
                Dim po = Await _poService.GetByIdAsync(id)
                If po Is Nothing Then Return

                If po.Status <> PurchaseOrderStatus.Draft Then
                    Me.View.ShowError("Only Draft purchase orders can be submitted.")
                    Return
                End If

                Await _poService.SubmitAsync(id)
                Me.View.ShowMessage("Purchase order submitted.")
                Await LoadDataAsync()
            Catch ex As Exception
                Me.View.ShowError("Failed to submit purchase order: " & ex.Message)
            End Try
        End Function

        Private Async Function DeleteAsync(id As Integer) As Task
            Try
                Dim po = Await _poService.GetByIdAsync(id)
                If po Is Nothing Then Return

                If po.Status <> PurchaseOrderStatus.Draft Then
                    Me.View.ShowError("Only Draft purchase orders can be deleted.")
                    Return
                End If

                If Me.View.ConfirmAction($"Are you sure you want to delete purchase order {po.OrderNumber}?", "Confirm Delete") Then
                    Await _poService.DeleteAsync(id)
                    Me.View.ShowMessage("Purchase order deleted.")
                    Await LoadDataAsync()
                End If
            Catch ex As Exception
                Me.View.ShowError("Failed to delete purchase order: " & ex.Message)
            End Try
        End Function
    End Class
End Namespace
