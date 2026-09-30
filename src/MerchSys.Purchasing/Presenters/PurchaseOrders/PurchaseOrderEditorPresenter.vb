Imports System
Imports System.Threading.Tasks
Imports MerchSys.Purchasing.Entities
Imports MerchSys.Purchasing.Services
Imports MerchSys.Purchasing.Views.Dialogs

Namespace Presenters.PurchaseOrders
    Public Class PurchaseOrderEditorPresenter
        Public ReadOnly Property View As IPurchaseOrderEditorView

        Private ReadOnly _purchaseOrderService As IPurchaseOrderService
        Private ReadOnly _vendorService As IVendorService
        Private _poId As Integer?

        Public Sub New(view As IPurchaseOrderEditorView, purchaseOrderService As IPurchaseOrderService, vendorService As IVendorService)
            Me.View = view
            _purchaseOrderService = purchaseOrderService
            _vendorService = vendorService

            Me.View.SetPresenter(Me)
        End Sub

        Public Async Function LoadDataAsync(poId As Integer?) As Task
            _poId = poId
            Me.View.Vendors = Await _vendorService.GetAllAsync()

            If poId.HasValue Then
                Dim po = Await _purchaseOrderService.GetByIdAsync(poId.Value)
                If po IsNot Nothing Then
                    Me.View.SelectedVendorId = po.VendorId
                    Me.View.Notes = po.Notes
                    Me.View.ExpectedDeliveryDate = po.ExpectedDeliveryDate
                    Me.View.Lines = po.Lines
                End If
            Else
                Me.View.SelectedVendorId = Nothing
                Me.View.Notes = ""
                Me.View.ExpectedDeliveryDate = Nothing
                Me.View.Lines = New System.Collections.Generic.List(Of PurchaseOrderLine)()
            End If
        End Function

        Public Async Function SaveAsync() As Task
            Dim po As PurchaseOrder
            Dim vendorId = If(Me.View.SelectedVendorId, 0)

            If _poId.HasValue Then
                po = Await _purchaseOrderService.UpdateDraftAsync(_poId.Value, vendorId, Me.View.Notes, Me.View.ExpectedDeliveryDate)
            Else
                po = Await _purchaseOrderService.CreateDraftAsync(vendorId, Me.View.Notes, Me.View.ExpectedDeliveryDate)
            End If

            Dim currentLines = If(po.Lines, New System.Collections.Generic.List(Of PurchaseOrderLine)())
            Dim viewLines = Me.View.Lines

            For Each viewLine In viewLines
                If viewLine.Id = 0 Then
                    Await _purchaseOrderService.AddLineAsync(po.Id, New CreatePOLineDto With {
                        .ProductName = viewLine.ProductName,
                        .UnitCost = viewLine.UnitCost,
                        .LineTotal = viewLine.LineTotal
                    })
                End If
            Next

            For Each currentLine In currentLines
                Dim exists = False
                For Each viewLine In viewLines
                    If viewLine.Id = currentLine.Id Then
                        exists = True
                        Exit For
                    End If
                Next
                If Not exists Then
                    Await _purchaseOrderService.RemoveLineAsync(po.Id, currentLine.Id)
                End If
            Next
        End Function
    End Class
End Namespace