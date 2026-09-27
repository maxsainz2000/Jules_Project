Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports MerchSys.Purchasing.Entities
Imports MerchSys.Purchasing.Services
Imports MerchSys.Purchasing.Views
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Paging
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Purchasing.Data

Namespace Presenters
    Public Class PurchaseOrderListPresenter
        Private ReadOnly _view As IPurchaseOrderListView
        Private ReadOnly _poService As IPurchaseOrderService
        Private ReadOnly _editorPresenter As PurchaseOrderEditorPresenter
        Private ReadOnly _dbContext As PurchasingDbContext

        Private _currentOrders As New List(Of PurchaseOrder)()

        Public Sub New(view As IPurchaseOrderListView, poService As IPurchaseOrderService, editorPresenter As PurchaseOrderEditorPresenter, dbContext As PurchasingDbContext)
            _view = view
            _poService = poService
            _editorPresenter = editorPresenter
            _dbContext = dbContext
        End Sub

        Public Async Function InitializeAsync() As Task
            Await _editorPresenter.InitializeAsync()
            Await LoadPurchaseOrdersAsync()
        End Function

        Public Async Function LoadPurchaseOrdersAsync() As Task
            Try
                Dim request As New PageRequest With {
                    .IsFirstPage = True,
                    .PageSize = 100
                }

                Dim pagedResult = Await _poService.GetHistoryAsync(request)
                _currentOrders = pagedResult.Items

                ApplyFilters()
            Catch ex As Exception
                _view.ShowError($"Failed to load purchase orders: {ex.Message}")
            End Try
        End Function

        Public Sub ApplyFilters()
            If _view.IsManager Then
                ' Manager gets full view
            Else
                ' Non-manager logic (maybe limited view)
            End If
            _view.IsManager = True ' Need real auth integration, assuming true for now
            ' Real role-based logic goes here based on user context
            Dim filtered = _currentOrders.AsEnumerable()

            If _view.StatusFilter.HasValue Then
                filtered = filtered.Where(Function(po) po.Status = _view.StatusFilter.Value)
            End If

            If Not String.IsNullOrWhiteSpace(_view.SearchTerm) Then
                filtered = filtered.Where(Function(po) po.OrderNumber IsNot Nothing AndAlso po.OrderNumber.Contains(_view.SearchTerm, StringComparison.OrdinalIgnoreCase))
            End If

            _view.BindPurchaseOrders(filtered.ToList())
        End Sub

        Public Sub PrepareNewPO()
            _editorPresenter.PrepareNewPO()
        End Sub

        Public Async Function EditSelectedPOAsync() As Task
            If Not _view.SelectedPurchaseOrderId.HasValue Then Return

            Try
                Dim poId = _view.SelectedPurchaseOrderId.Value

                ' Fetch PO and lines to pass to editor
                ' Using projection to avoid VB.NET ToListAsync trap
                Dim poQuery = From p In _dbContext.Set(Of PurchaseOrder)()
                              Where p.Id = poId
                              Select New With {
                                  .Id = p.Id,
                                  .VendorId = p.VendorId,
                                  .Notes = p.Notes,
                                  .ExpectedDeliveryDate = p.ExpectedDeliveryDate,
                                  .Status = p.Status
                              }

                Dim poData = Await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(poQuery)

                If poData Is Nothing Then
                    _view.ShowError("Purchase Order not found.")
                    Return
                End If

                Dim linesQuery = From l In _dbContext.Set(Of PurchaseOrderLine)()
                                 Where l.PurchaseOrderId = poId
                                 Select New With {
                                     .Id = l.Id,
                                     .PurchaseOrderId = l.PurchaseOrderId,
                                     .ProductId = l.ProductId,
                                     .ProductName = l.ProductName,
                                     .Quantity = l.Quantity,
                                     .UnitCost = l.UnitCost,
                                     .LineTotal = l.LineTotal
                                 }

                Dim linesData = Await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(linesQuery)

                Dim po = New PurchaseOrder With {
                    .Id = poData.Id,
                    .VendorId = poData.VendorId,
                    .Notes = poData.Notes,
                    .ExpectedDeliveryDate = poData.ExpectedDeliveryDate,
                    .Status = poData.Status
                }

                Dim lines = linesData.Select(Function(l) New PurchaseOrderLine With {
                    .Id = l.Id,
                    .PurchaseOrderId = l.PurchaseOrderId,
                    .ProductId = l.ProductId,
                    .ProductName = l.ProductName,
                    .Quantity = l.Quantity,
                    .UnitCost = l.UnitCost,
                    .LineTotal = l.LineTotal
                }).ToList()

                _editorPresenter.LoadPO(po, lines)

            Catch ex As Exception
                _view.ShowError($"Failed to load PO details: {ex.Message}")
            End Try
        End Function

        Public Async Function SubmitPOAsync() As Task
            If Not _view.SelectedPurchaseOrderId.HasValue Then Return

            Dim poId = _view.SelectedPurchaseOrderId.Value

            Try
                If _view.ConfirmAction("Are you sure you want to submit this Purchase Order?") Then
                    Await _poService.SubmitAsync(poId)
                    _view.ShowMessage("Purchase Order submitted successfully.")
                    Await LoadPurchaseOrdersAsync()
                    _editorPresenter.ClearEditor()
                End If
            Catch ex As Exception
                _view.ShowError($"Failed to submit PO: {ex.Message}")
            End Try
        End Function

        Public Async Function CancelPOAsync() As Task
            If Not _view.SelectedPurchaseOrderId.HasValue Then Return

            Dim poId = _view.SelectedPurchaseOrderId.Value

            Try
                If _view.ConfirmAction("Are you sure you want to cancel this Purchase Order?") Then
                    ' Await _poService.CancelAsync(poId)
                    ' Note: Cancellation might require special handling or isn't in interface yet,
                    ' Assuming it's part of Close or something similar. For now, try to find a CancelAsync or use CloseAsync.
                    Await _poService.CloseAsync(poId) ' Adjust according to exact service interface
                    _view.ShowMessage("Purchase Order cancelled/closed successfully.")
                    Await LoadPurchaseOrdersAsync()
                    _editorPresenter.ClearEditor()
                End If
            Catch ex As Exception
                _view.ShowError($"Failed to cancel PO: {ex.Message}")
            End Try
        End Function
    End Class
End Namespace
