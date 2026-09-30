Imports System.Linq
Imports System.Threading.Tasks
Imports Microsoft.Extensions.DependencyInjection
Imports MerchSys.Purchasing.Entities

Namespace Views.Shell.Modules.Purchasing
    Public Class PurchaseOrderListPresenter
        Public ReadOnly Property View As IPurchaseOrderListView
        Private ReadOnly _poService As MerchSys.Purchasing.Services.IPurchaseOrderService
        Private ReadOnly _serviceProvider As IServiceProvider

        Private _allOrders As List(Of PurchaseOrder)

        Public Sub New(view As IPurchaseOrderListView, poService As MerchSys.Purchasing.Services.IPurchaseOrderService, serviceProvider As IServiceProvider)
            Me.View = view
            _poService = poService
            _serviceProvider = serviceProvider
            _allOrders = New List(Of PurchaseOrder)()

            AddHandler Me.View.RefreshRequested, AddressOf OnRefreshRequested
            AddHandler Me.View.CreateRequested, AddressOf OnCreateRequested
            AddHandler Me.View.EditRequested, AddressOf OnEditRequested
            AddHandler Me.View.SubmitRequested, AddressOf OnSubmitRequested
            AddHandler Me.View.DeleteRequested, AddressOf OnDeleteRequested

            Dim statuses = New List(Of String) From {"Draft", "Submitted", "Approved", "Rejected"}
            Me.View.InitializeStatusFilter(statuses)
        End Sub

        Private Async Sub OnRefreshRequested(sender As Object, e As EventArgs)
            Await LoadDataAsync()
        End Sub

        Public Async Function LoadDataAsync() As Task
            Try
                Dim request As New MerchSys.SharedKernel.Paging.PageRequest With {
                    .IsFirstPage = True,
                    .PageSize = 100 ' Simplify for now
                }

                Dim result = Await _poService.GetHistoryAsync(request)
                _allOrders = result.Items

                ApplyFilters()
            Catch ex As Exception
                View.ShowError("Failed to load purchase orders: " & ex.Message)
            End Try
        End Function

        Private Sub ApplyFilters()
            Dim filtered = _allOrders.AsEnumerable()

            If View.FilterStatus >= 0 Then
                filtered = filtered.Where(Function(po) po.Status = CType(View.FilterStatus, MerchSys.SharedKernel.Enums.PurchaseOrderStatus))
            End If

            If Not String.IsNullOrWhiteSpace(View.SearchTerm) Then
                Dim term = View.SearchTerm.ToLower()
                filtered = filtered.Where(Function(po) (po.OrderNumber IsNot Nothing AndAlso po.OrderNumber.ToLower().Contains(term)))
            End If

            View.DataSource = filtered.ToList()
        End Sub

        Private Async Sub OnCreateRequested(sender As Object, e As EventArgs)
            Try
                Dim newPo = Await _poService.CreateDraftAsync()

                Using scope = _serviceProvider.CreateScope()
                    Dim editorPresenter = scope.ServiceProvider.GetRequiredService(Of PurchaseOrderEditorPresenter)()
                    Dim editorView = editorPresenter.View
                    editorView.AttachPresenter(editorPresenter)

                    editorPresenter.LoadPurchaseOrder(newPo)

                    editorView.ShowDialog()
                End Using

                Await LoadDataAsync()
            Catch ex As Exception
                View.ShowError("Failed to create purchase order: " & ex.Message)
            End Try
        End Sub

        Private Async Sub OnEditRequested(sender As Object, e As EventArgs)
            Dim po = View.SelectedPO
            If po Is Nothing Then Return

            ' STRICT VALIDATION: Prevent editing non-drafts
            If po.Status <> MerchSys.SharedKernel.Enums.PurchaseOrderStatus.Draft Then
                View.ShowError("Only Draft purchase orders can be edited.")
                Return
            End If

            Try
                Using scope = _serviceProvider.CreateScope()
                    Dim editorPresenter = scope.ServiceProvider.GetRequiredService(Of PurchaseOrderEditorPresenter)()
                    Dim editorView = editorPresenter.View
                    editorView.AttachPresenter(editorPresenter)

                    editorPresenter.LoadPurchaseOrder(po)

                    editorView.ShowDialog()
                End Using

                Await LoadDataAsync()
            Catch ex As Exception
                View.ShowError("Failed to open purchase order: " & ex.Message)
            End Try
        End Sub

        Private Async Sub OnSubmitRequested(sender As Object, e As EventArgs)
            Dim po = View.SelectedPO
            If po Is Nothing Then Return

            If po.Status <> MerchSys.SharedKernel.Enums.PurchaseOrderStatus.Draft Then
                View.ShowError("Only Draft purchase orders can be submitted.")
                Return
            End If

            If View.Confirm($"Are you sure you want to submit Purchase Order {po.OrderNumber}?", "Submit Order") Then
                Try
                    Await _poService.SubmitAsync(po.Id)
                    View.ShowMessage("Purchase Order submitted successfully.")
                    Await LoadDataAsync()
                Catch ex As Exception
                    View.ShowError("Failed to submit purchase order: " & ex.Message)
                End Try
            End If
        End Sub

        Private Sub OnDeleteRequested(sender As Object, e As EventArgs)
            View.ShowMessage("Delete functionality is not implemented in this mock.")
        End Sub
    End Class
End Namespace
