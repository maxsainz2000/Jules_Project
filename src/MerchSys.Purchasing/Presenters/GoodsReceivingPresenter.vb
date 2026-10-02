Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports MerchSys.Purchasing.Dtos
Imports MerchSys.Purchasing.Services
Imports MerchSys.Purchasing.Views
Imports MerchSys.SharedKernel.Enums

Namespace Presenters

    Public Class POSelectorItem
        Public Property Id As Integer
        Public Property PONumber As String
        Public Property VendorName As String

        Public ReadOnly Property DisplayText As String
            Get
                Return $"{PONumber} - {VendorName}"
            End Get
        End Property
    End Class

    Public Class GRLineItem
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property QuantityOrdered As Integer

        Private _quantityReceived As Integer
        Public Property QuantityReceived As Integer
            Get
                Return _quantityReceived
            End Get
            Set(value As Integer)
                _quantityReceived = value
                HasDiscrepancy = (_quantityReceived <> QuantityOrdered)
                ComputeValues()
            End Set
        End Property

        Private _unitCost As Decimal
        Public Property UnitCost As Decimal
            Get
                Return _unitCost
            End Get
            Set(value As Decimal)
                _unitCost = value
                ComputeValues()
            End Set
        End Property

        Private _vatClassification As VatTreatment
        Public Property VatClassification As VatTreatment
            Get
                Return _vatClassification
            End Get
            Set(value As VatTreatment)
                _vatClassification = value
                ComputeValues()
            End Set
        End Property

        Public Property ExpiryDate As DateTime?
        Public Property DiscrepancyNotes As String
        Public Property HasDiscrepancy As Boolean
        Public Property VatAmount As Decimal
        Public Property VatableSales As Decimal

        Private Sub ComputeValues()
            Dim totalAmount = UnitCost * QuantityReceived
            If VatClassification = VatTreatment.Vatable Then
                VatableSales = Math.Round(totalAmount / 1.12D, 2)
                VatAmount = totalAmount - VatableSales
            Else
                VatableSales = totalAmount
                VatAmount = 0
            End If
        End Sub
    End Class

    Public Class GoodsReceivingPresenter

        Private _view As IGoodsReceivingView
        Private ReadOnly _purchaseOrderService As IPurchaseOrderService
        Private ReadOnly _goodsReceivingService As IGoodsReceivingService
        Private ReadOnly _vendorService As IVendorService
        Private _currentLines As List(Of GRLineItem) = New List(Of GRLineItem)()

        Public ReadOnly Property VatTreatmentValues As List(Of VatTreatment)
            Get
                Return [Enum].GetValues(GetType(VatTreatment)).Cast(Of VatTreatment)().ToList()
            End Get
        End Property

        Public Sub New(purchaseOrderService As IPurchaseOrderService, goodsReceivingService As IGoodsReceivingService, vendorService As IVendorService)
            _purchaseOrderService = purchaseOrderService
            _goodsReceivingService = goodsReceivingService
            _vendorService = vendorService
        End Sub

        Public Property View As IGoodsReceivingView
            Get
                Return _view
            End Get
            Set(value As IGoodsReceivingView)
                If _view IsNot Nothing Then
                    RemoveHandler _view.PurchaseOrderSelected, AddressOf OnPurchaseOrderSelectedSync
                    RemoveHandler _view.ConfirmReceiptRequested, AddressOf OnConfirmReceiptRequestedSync
                End If
                _view = value
                If _view IsNot Nothing Then
                    AddHandler _view.PurchaseOrderSelected, AddressOf OnPurchaseOrderSelectedSync
                    AddHandler _view.ConfirmReceiptRequested, AddressOf OnConfirmReceiptRequestedSync
                End If
            End Set
        End Property

        Private Async Sub OnPurchaseOrderSelectedSync(s As Object, e As Integer?)
            Await OnPurchaseOrderSelectedAsync(e)
        End Sub

        Private Async Sub OnConfirmReceiptRequestedSync(s As Object, e As EventArgs)
            Await OnConfirmReceiptRequestedAsync()
        End Sub

        Public Async Function InitializeAsync() As Task
            Await LoadSubmittedPOsAsync()
        End Function

        Private Async Function LoadSubmittedPOsAsync() As Task
            Try
                Dim pos = Await _purchaseOrderService.SearchAsync("", PurchaseOrderStatus.Submitted)
                Dim vendors = Await _vendorService.GetAllAsync()
                Dim items = pos.Select(Function(p) New POSelectorItem With {
                    .Id = p.Id,
                    .PONumber = p.OrderNumber,
                    .VendorName = If(vendors.FirstOrDefault(Function(v) v.Id = p.VendorId)?.Name, p.VendorId.ToString())
                }).ToList()

                _view.SetPurchaseOrders(items)
                _view.CanConfirmReceipt = False
                _view.SetLineItems(New List(Of GRLineItem)())
            Catch ex As Exception
                _view.ShowError($"Error loading Purchase Orders: {ex.Message}")
            End Try
        End Function

        Private Async Function OnPurchaseOrderSelectedAsync(purchaseOrderId As Integer?) As Task
            If Not purchaseOrderId.HasValue Then
                _currentLines = New List(Of GRLineItem)()
                _view.SetLineItems(_currentLines)
                _view.CanConfirmReceipt = False
                Return
            End If

            Try
                Dim poLines = Await _purchaseOrderService.GetLinesAsync(purchaseOrderId.Value)
                _currentLines = poLines.Select(Function(l) New GRLineItem With {
                    .ProductId = l.ProductId,
                    .ProductName = l.ProductName,
                    .QuantityOrdered = l.Quantity,
                    .QuantityReceived = l.Quantity,
                    .UnitCost = l.UnitCost,
                    .HasDiscrepancy = False,
                    .VatClassification = VatTreatment.Vatable
                }).ToList()

                _view.SetLineItems(_currentLines)
                _view.CanConfirmReceipt = True
            Catch ex As Exception
                _view.ShowError($"Error loading PO Lines: {ex.Message}")
            End Try
        End Function

        Private Async Function OnConfirmReceiptRequestedAsync() As Task
            If Not _view.SelectedPurchaseOrderId.HasValue Then
                _view.ShowError("Please select a Purchase Order first.")
                Return
            End If

            If _currentLines.Count = 0 Then
                _view.ShowError("No items to receive.")
                Return
            End If

            ' Validate discrepancy notes
            For Each line In _currentLines
                If line.HasDiscrepancy AndAlso String.IsNullOrWhiteSpace(line.DiscrepancyNotes) Then
                    _view.ShowError($"Discrepancy notes are required for {line.ProductName} because the received quantity ({line.QuantityReceived}) differs from the ordered quantity ({line.QuantityOrdered}).")
                    Return
                End If
            Next

            Try
                Dim dto = New ReceiveGoodsDto With {
                    .PurchaseOrderId = _view.SelectedPurchaseOrderId.Value,
                    .Lines = _currentLines.Select(Function(l) New ReceiveGoodsLineDto With {
                        .ProductId = l.ProductId,
                        .ProductName = l.ProductName,
                        .QuantityOrdered = l.QuantityOrdered,
                        .QuantityReceived = l.QuantityReceived,
                        .UnitCost = l.UnitCost,
                        .ExpiryDate = l.ExpiryDate,
                        .DiscrepancyNotes = l.DiscrepancyNotes,
                        .VatClassification = l.VatClassification,
                        .VatableSales = l.VatableSales,
                        .VatAmount = l.VatAmount
                    }).ToList()
                }

                Await _goodsReceivingService.ReceiveGoodsAsync(dto)
                _view.ShowMessage("Goods received successfully.", "Success")

                ' Reset state
                Await LoadSubmittedPOsAsync()

            Catch ex As Exception
                _view.ShowError($"Error receiving goods: {ex.Message}")
            End Try
        End Function

    End Class

End Namespace
