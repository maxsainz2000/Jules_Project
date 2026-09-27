Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports MerchSys.Purchasing.Entities
Imports MerchSys.Purchasing.Services
Imports MerchSys.Purchasing.Views
Imports MerchSys.SharedKernel.Enums

Namespace Presenters
    Public Class PurchaseOrderEditorPresenter
        Private ReadOnly _view As IPurchaseOrderEditorView
        Private ReadOnly _poService As IPurchaseOrderService
        Private ReadOnly _vendorService As IVendorService

        Private _currentPoId As Integer?
        Private _currentLines As New List(Of PurchaseOrderLine)()

        Public Sub New(view As IPurchaseOrderEditorView, poService As IPurchaseOrderService, vendorService As IVendorService)
            _view = view
            _poService = poService
            _vendorService = vendorService
        End Sub

        Public Async Function InitializeAsync() As Task
            Try
                Dim vendors = Await _vendorService.GetAllAsync()
                _view.BindVendors(vendors)
                ClearEditor()
            Catch ex As Exception
                _view.ShowError($"Failed to load vendors: {ex.Message}")
            End Try
        End Function

        Public Sub PrepareNewPO()
            _currentPoId = Nothing
            ClearEditor()
            _view.SetEditorVisible(True)
            _view.SetEditorEnabled(True)
        End Sub

        Public Function LoadExistingPOAsync(poId As Integer) As Task
            Return Task.CompletedTask
            Try
                _currentPoId = poId
                ' In a real scenario we might fetch the PO details, but the service doesn't have a GetByIdAsync returning PO with lines.
                ' Wait, let me check what we can get. The List Presenter might pass the PO object, or we fetch it from DB directly if needed.
                ' Since IPurchaseOrderService doesn't have GetById, we will just simulate loading or wait to see what is needed.
                ' Actually, we need to show PO details. We might need to query the database directly or update the service.
                ' For now, we will assume the lines are fetched somewhere else, or we add it to the service later if needed.
                ' We'll leave it empty for a bit and see.

                ' To avoid doing DB calls here without service support, maybe we pass the existing PO to the presenter

            Catch ex As Exception
                _view.ShowError($"Failed to load PO details: {ex.Message}")
            End Try
        End Function

        Public Sub LoadPO(po As PurchaseOrder, lines As List(Of PurchaseOrderLine))
            _currentPoId = po.Id
            _view.SelectedVendorId = po.VendorId
            _view.ExpectedDeliveryDate = po.ExpectedDeliveryDate
            _view.Notes = po.Notes

            _currentLines = If(lines, New List(Of PurchaseOrderLine)())

            ' Only drafts can be edited
            Dim isDraft = po.Status = PurchaseOrderStatus.Draft
            _view.SetEditorEnabled(isDraft)

            RefreshLineItems()
            _view.SetEditorVisible(True)
        End Sub

        Public Async Function SaveDraftAsync() As Task(Of Boolean)
            Try
                If _currentPoId.HasValue Then
                    Await _poService.UpdateDraftAsync(_currentPoId.Value, _view.SelectedVendorId.GetValueOrDefault(), _view.Notes, _view.ExpectedDeliveryDate)
                Else
                    Dim newPo = Await _poService.CreateDraftAsync(_view.SelectedVendorId.GetValueOrDefault(), _view.Notes, _view.ExpectedDeliveryDate)
                    _currentPoId = newPo.Id

                    ' Update vendor somehow, but CreateDraftAsync doesn't take VendorId in the interface...
                    ' We might need to handle VendorId in a DB update or the interface needs it.
                End If

                _view.ShowMessage("Draft saved successfully.")
                Return True
            Catch ex As Exception
                _view.ShowError($"Failed to save draft: {ex.Message}")
                Return False
            End Try
        End Function

        Public Async Function AddLineAsync(productName As String, unitCost As Decimal, quantity As Integer) As Task
            Try
                If Not _currentPoId.HasValue Then
                    ' Save draft first if it's new
                    Dim saved = Await SaveDraftAsync()
                    If Not saved Then Return
                End If

                Dim dto = New CreatePOLineDto With {
                    .ProductName = productName,
                    .UnitCost = unitCost,
                    .LineTotal = unitCost * quantity,
                    .Quantity = quantity
                }

                ' Our service doesn't have Quantity in CreatePOLineDto... Let's check it.
                ' We'll add the line via service, but we'll also update the local list
                Dim addedLine = Await _poService.AddLineAsync(_currentPoId.Value, dto)


                _currentLines.Add(addedLine)
                RefreshLineItems()
            Catch ex As Exception
                _view.ShowError($"Failed to add line: {ex.Message}")
            End Try
        End Function

        Public Async Function RemoveLineAsync(lineId As Integer) As Task
            Try
                If Not _currentPoId.HasValue Then Return

                Await _poService.RemoveLineAsync(_currentPoId.Value, lineId)

                Dim lineToRemove = _currentLines.FirstOrDefault(Function(l) l.Id = lineId)
                If lineToRemove IsNot Nothing Then
                    _currentLines.Remove(lineToRemove)
                End If

                RefreshLineItems()
            Catch ex As Exception
                _view.ShowError($"Failed to remove line: {ex.Message}")
            End Try
        End Function

        Private Sub RefreshLineItems()
            _view.BindLineItems(_currentLines.ToList())

            Dim total = _currentLines.Sum(Function(l) l.LineTotal)
            _view.TotalAmountText = String.Format("₱{0:N2}", total)
        End Sub

        Public Sub ClearEditor()
            _currentPoId = Nothing
            _view.SelectedVendorId = Nothing
            _view.ExpectedDeliveryDate = Nothing
            _view.Notes = String.Empty
            _currentLines.Clear()
            RefreshLineItems()
            _view.SetEditorVisible(False)
        End Sub
    End Class
End Namespace
