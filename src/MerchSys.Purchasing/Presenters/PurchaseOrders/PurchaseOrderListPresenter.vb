Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports MerchSys.Purchasing.Entities
Imports MerchSys.Purchasing.Services
Imports MerchSys.Purchasing.Views.Dialogs
Imports MerchSys.Purchasing.Views.PurchaseOrders
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.SharedKernel.Paging
Imports Microsoft.Extensions.DependencyInjection

Namespace Presenters.PurchaseOrders
    Public Class PurchaseOrderListPresenter
        Private ReadOnly _view As IPurchaseOrderListView
        Private ReadOnly _purchaseOrderService As IPurchaseOrderService
        Private ReadOnly _sessionService As ISessionService
        Private ReadOnly _serviceProvider As IServiceProvider

        Public Sub New(view As IPurchaseOrderListView, purchaseOrderService As IPurchaseOrderService, sessionService As ISessionService, serviceProvider As IServiceProvider)
            _view = view
            _purchaseOrderService = purchaseOrderService
            _sessionService = sessionService
            _serviceProvider = serviceProvider

            _view.IsOwnerRole = (_sessionService.CurrentUserRole = UserRole.Owner)

            Dim statusOptions As New List(Of String) From {"All"}
            statusOptions.AddRange([Enum].GetNames(GetType(PurchaseOrderStatus)))

            _view.SetStatusOptions(statusOptions)
            _view.StatusFilter = "All"

            If TypeOf _view Is PurchaseOrderListView Then
                DirectCast(_view, PurchaseOrderListView).SetPresenter(Me)
            End If
        End Sub

        Public Async Function OnLoadAsync() As Task
            Await LoadDataAsync()
        End Function

        Public Async Function LoadDataAsync() As Task
            Dim request As New PageRequest With {.PageSize = 1000}
            Dim result = Await _purchaseOrderService.GetHistoryAsync(request)

            Dim filtered = result.Items.AsEnumerable()

            If Not String.IsNullOrEmpty(_view.StatusFilter) AndAlso _view.StatusFilter <> "All" Then
                Dim status As PurchaseOrderStatus
                If [Enum].TryParse(_view.StatusFilter, status) Then
                    filtered = filtered.Where(Function(p) p.Status = status)
                End If
            End If

            If Not String.IsNullOrWhiteSpace(_view.SearchTerm) Then
                Dim term = _view.SearchTerm.ToLower()
                filtered = filtered.Where(Function(p) p.OrderNumber?.ToLower().Contains(term) OrElse p.Notes?.ToLower().Contains(term))
            End If

            _view.Orders = filtered.ToList()
        End Function

        Public Async Function OnFilterChangedAsync() As Task
            Await LoadDataAsync()
        End Function

        Public Async Function OnNewClickedAsync() As Task
            Dim editorPresenter = _serviceProvider.GetRequiredService(Of PurchaseOrderEditorPresenter)()
            Await editorPresenter.LoadDataAsync(Nothing)
            If editorPresenter.View.ShowDialog(DirectCast(_view, UserControl)) = DialogResult.OK Then
                Await LoadDataAsync()
            End If
        End Function

        Public Async Function OnEditClickedAsync() As Task
            If _view.SelectedOrder Is Nothing Then Return
            Dim editorPresenter = _serviceProvider.GetRequiredService(Of PurchaseOrderEditorPresenter)()
            Await editorPresenter.LoadDataAsync(_view.SelectedOrder.Id)
            If editorPresenter.View.ShowDialog(DirectCast(_view, UserControl)) = DialogResult.OK Then
                Await LoadDataAsync()
            End If
        End Function

        Public Async Function OnSubmitClickedAsync() As Task
            If _view.SelectedOrder Is Nothing Then Return
            If _view.SelectedOrder.Status <> PurchaseOrderStatus.Draft Then
                MessageBox.Show("Only draft purchase orders can be submitted.")
                Return
            End If

            Await _purchaseOrderService.SubmitAsync(_view.SelectedOrder.Id)
            Await LoadDataAsync()
        End Function

        Public Async Function OnDeleteClickedAsync() As Task
            If _view.SelectedOrder Is Nothing Then Return

            Dim confirmResult = MessageBox.Show("Are you sure to delete this item?", "Confirm Delete", MessageBoxButtons.YesNo)
            If confirmResult = DialogResult.Yes Then
                Await _purchaseOrderService.DeleteAsync(_view.SelectedOrder.Id)
                Await LoadDataAsync()
            End If
        End Function
    End Class
End Namespace