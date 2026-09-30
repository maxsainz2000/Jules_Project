Imports System.Linq
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports MerchSys.Purchasing.Entities
Imports MerchSys.Purchasing.Services

Namespace Views.Shell.Modules.Purchasing
    Public Class PurchaseOrderEditorPresenter
        Public ReadOnly Property View As IPurchaseOrderEditorView
        Private ReadOnly _poService As IPurchaseOrderService
        Private ReadOnly _vendorService As IVendorService

        Private _currentPo As PurchaseOrder
        Private _lines As List(Of PurchaseOrderLine)

        Public Sub New(view As IPurchaseOrderEditorView, poService As IPurchaseOrderService, vendorService As IVendorService)
            Me.View = view
            _poService = poService
            _vendorService = vendorService

            _lines = New List(Of PurchaseOrderLine)()

            AddHandler Me.View.SaveRequested, AddressOf OnSaveRequested
            AddHandler Me.View.SubmitRequested, AddressOf OnSubmitRequested
            AddHandler Me.View.AddLineRequested, AddressOf OnAddLineRequested
            AddHandler Me.View.RemoveLineRequested, AddressOf OnRemoveLineRequested

            LoadVendorsAsync().ConfigureAwait(False)
        End Sub

        Private Async Function LoadVendorsAsync() As Task
            Try
                Dim vendors = Await _vendorService.GetAllAsync()
                View.Vendors = vendors
            Catch ex As Exception
                View.ShowError("Failed to load vendors: " & ex.Message)
            End Try
        End Function

        Public Sub LoadPurchaseOrder(po As PurchaseOrder)
            _currentPo = po
            View.OrderNumber = po.OrderNumber
            View.Status = CInt(po.Status)
            View.Notes = po.Notes
            View.ExpectedDeliveryDate = po.ExpectedDeliveryDate
            View.TotalAmount = po.TotalAmount
            View.SelectedVendorId = po.VendorId

            ApplyDraftRules()
            ' Mock loading lines here since the entity doesn't have a navigation property for Lines
            ' In a real app, you'd fetch the lines from the DB using a query. For now, empty list is fine for new drafts.
            _lines = New List(Of PurchaseOrderLine)()
            UpdateLinesView()
        End Sub

        Private Sub ApplyDraftRules()
            If _currentPo IsNot Nothing AndAlso _currentPo.Status = MerchSys.SharedKernel.Enums.PurchaseOrderStatus.Draft Then
                View.CanAddLine = True
                View.CanRemoveLine = True
                View.CanSave = True
                View.CanSubmit = True
            Else
                View.CanAddLine = False
                View.CanRemoveLine = False
                View.CanSave = False
                View.CanSubmit = False
            End If
        End Sub

        Private Sub UpdateLinesView()
            View.Lines = _lines.ToList()
            Dim total = _lines.Sum(Function(l) l.LineTotal)
            View.TotalAmount = total
        End Sub

        Private Async Sub OnSaveRequested(sender As Object, e As EventArgs)
            If _currentPo Is Nothing OrElse _currentPo.Status <> MerchSys.SharedKernel.Enums.PurchaseOrderStatus.Draft Then Return

            Try
                ' We'd also update VendorId, but UpdateDraftAsync in the service currently only accepts notes and date.
                Await _poService.UpdateDraftAsync(_currentPo.Id, View.Notes, View.ExpectedDeliveryDate)
                View.Close()
            Catch ex As Exception
                View.ShowError("Failed to save draft: " & ex.Message)
            End Try
        End Sub

        Private Async Sub OnSubmitRequested(sender As Object, e As EventArgs)
            If _currentPo Is Nothing OrElse _currentPo.Status <> MerchSys.SharedKernel.Enums.PurchaseOrderStatus.Draft Then Return

            Try
                Await _poService.UpdateDraftAsync(_currentPo.Id, View.Notes, View.ExpectedDeliveryDate)
                Await _poService.SubmitAsync(_currentPo.Id)
                View.Close()
            Catch ex As Exception
                View.ShowError("Failed to submit purchase order: " & ex.Message)
            End Try
        End Sub

        Private Async Sub OnAddLineRequested(sender As Object, e As EventArgs)
            If _currentPo Is Nothing OrElse _currentPo.Status <> MerchSys.SharedKernel.Enums.PurchaseOrderStatus.Draft Then Return

            ' Use an InputBox or a simpler mechanism to get product name to avoid hardcoded mock
            Dim productName = Microsoft.VisualBasic.Interaction.InputBox("Enter Product Name:", "Add Line", "New Product")
            If String.IsNullOrWhiteSpace(productName) Then Return

            Dim unitCostStr = Microsoft.VisualBasic.Interaction.InputBox("Enter Unit Cost:", "Add Line", "100.00")
            Dim unitCost As Decimal
            If Not Decimal.TryParse(unitCostStr, unitCost) Then
                View.ShowError("Invalid unit cost.")
                Return
            End If

            Dim qtyStr = Microsoft.VisualBasic.Interaction.InputBox("Enter Quantity:", "Add Line", "1")
            Dim qty As Integer
            If Not Integer.TryParse(qtyStr, qty) OrElse qty <= 0 Then
                View.ShowError("Invalid quantity.")
                Return
            End If

            Dim total = unitCost * qty

            Dim dto As New CreatePOLineDto With {
                .ProductName = productName,
                .UnitCost = unitCost,
                .LineTotal = total
            }

            Try
                Dim newLine = Await _poService.AddLineAsync(_currentPo.Id, dto)
                _lines.Add(newLine)
                UpdateLinesView()
            Catch ex As Exception
                View.ShowError("Failed to add line: " & ex.Message)
            End Try
        End Sub

        Private Async Sub OnRemoveLineRequested(sender As Object, e As EventArgs)
            If _currentPo Is Nothing OrElse _currentPo.Status <> MerchSys.SharedKernel.Enums.PurchaseOrderStatus.Draft Then Return

            Dim line = View.SelectedLine
            If line Is Nothing Then Return

            Try
                Await _poService.RemoveLineAsync(_currentPo.Id, line.Id)
                _lines.Remove(line)
                UpdateLinesView()
            Catch ex As Exception
                View.ShowError("Failed to remove line: " & ex.Message)
            End Try
        End Sub
    End Class
End Namespace
