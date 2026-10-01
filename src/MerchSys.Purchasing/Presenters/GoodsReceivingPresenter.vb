Imports System.Threading.Tasks
Imports System.Linq
Imports MerchSys.Purchasing.Services
Imports MerchSys.Purchasing.Views
Imports MerchSys.Purchasing.Dtos
Imports MerchSys.SharedKernel.Enums

Namespace Presenters
    Public Class POSelectorItem
        Public Property Id As Integer
        Public Property DisplayText As String

        Public Overrides Function ToString() As String
            Return DisplayText
        End Function
    End Class

    Public Class GRLineItem
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property QuantityOrdered As Integer
        Public Property QuantityReceived As Integer
        Public Property UnitCost As Decimal
        Public Property ExpiryDate As DateTime?
        Public Property DiscrepancyNotes As String

        Public ReadOnly Property HasDiscrepancy As Boolean
            Get
                Return QuantityOrdered <> QuantityReceived OrElse Not String.IsNullOrWhiteSpace(DiscrepancyNotes)
            End Get
        End Property
    End Class

    Public Class GoodsReceivingPresenter
        Private ReadOnly _view As IGoodsReceivingView
        Private ReadOnly _goodsReceivingService As IGoodsReceivingService
        Private ReadOnly _purchaseOrderService As IPurchaseOrderService

        Private _selectedPOId As Integer?
        Private _lines As List(Of GRLineItem)

        Public Sub New(view As IGoodsReceivingView, goodsReceivingService As IGoodsReceivingService, purchaseOrderService As IPurchaseOrderService)
            _view = view
            _goodsReceivingService = goodsReceivingService
            _purchaseOrderService = purchaseOrderService

            _lines = New List(Of GRLineItem)()

            _view.OnLoadPendingPOs = AddressOf LoadPendingPOsAsync
            _view.OnPOSelected = AddressOf POSelectedAsync
            _view.OnConfirmReceipt = AddressOf ConfirmReceiptAsync
        End Sub

        Private Async Function LoadPendingPOsAsync() As Task
            Try
                _view.SetStatus("Loading Submitted POs...")
                Dim submittedPOs = Await _purchaseOrderService.SearchAsync("", PurchaseOrderStatus.Submitted)

                Dim selectorItems = submittedPOs.Select(Function(po) New POSelectorItem With {
                    .Id = po.Id,
                    .DisplayText = $"PO #{po.OrderNumber} - {po.ExpectedDeliveryDate:d}"
                }).ToList()

                _view.BindPOSelector(selectorItems)
                _view.SetStatus("Ready")
            Catch ex As Exception
                _view.ShowError($"Failed to load POs: {ex.Message}")
                _view.SetStatus("Error loading POs")
            End Try
        End Function

        Private Async Function POSelectedAsync(poId As Integer) As Task
            Try
                _view.SetStatus("Loading PO details...")
                _selectedPOId = poId
                _view.IsPOSelected = True

                Dim lines = Await _purchaseOrderService.GetLinesAsync(poId)

                _lines = lines.Select(Function(l) New GRLineItem With {
                    .ProductId = l.ProductId,
                    .ProductName = l.ProductName,
                    .QuantityOrdered = l.Quantity,
                    .QuantityReceived = l.Quantity, ' Pre-fill
                    .UnitCost = l.UnitCost
                }).ToList()

                _view.BindReceivingLines(_lines)
                _view.SetStatus("Ready to receive goods")
            Catch ex As Exception
                _view.ShowError($"Failed to load PO details: {ex.Message}")
                _view.SetStatus("Error loading details")
                _selectedPOId = Nothing
                _view.IsPOSelected = False
                _lines.Clear()
                _view.BindReceivingLines(_lines)
            End Try
        End Function

        Private Async Function ConfirmReceiptAsync() As Task
            If Not _selectedPOId.HasValue Then
                _view.ShowError("Please select a PO first.")
                Return
            End If

            ' Validation
            For Each line In _lines
                If line.QuantityOrdered <> line.QuantityReceived AndAlso String.IsNullOrWhiteSpace(line.DiscrepancyNotes) Then
                    _view.ShowError($"Product {line.ProductName} has a quantity discrepancy. Please provide discrepancy notes.")
                    Return
                End If
                If line.QuantityReceived < 0 Then
                    _view.ShowError($"Quantity received for {line.ProductName} cannot be negative.")
                    Return
                End If
            Next

            If Not _view.ConfirmAction("Are you sure you want to confirm receipt of these goods?", "Confirm Receipt") Then
                Return
            End If

            Try
                _view.SetStatus("Saving receipt...")
                Dim dto = New ReceiveGoodsDto With {
                    .PurchaseOrderId = _selectedPOId.Value,
                    .Lines = _lines.Select(Function(l) New ReceiveGoodsLineDto With {
                        .ProductId = l.ProductId,
                        .ProductName = l.ProductName,
                        .QuantityOrdered = l.QuantityOrdered,
                        .QuantityReceived = l.QuantityReceived,
                        .UnitCost = l.UnitCost,
                        .ExpiryDate = l.ExpiryDate,
                        .DiscrepancyNotes = l.DiscrepancyNotes
                    }).ToList()
                }

                Await _goodsReceivingService.ReceiveGoodsAsync(dto)

                _view.ShowMessage("Goods received successfully.")

                ' Reset state
                _selectedPOId = Nothing
                _view.IsPOSelected = False
                _lines.Clear()
                _view.BindReceivingLines(_lines)
                Await LoadPendingPOsAsync()

            Catch ex As Exception
                _view.ShowError($"Failed to receive goods: {ex.Message}")
                _view.SetStatus("Error saving receipt")
            End Try
        End Function
    End Class
End Namespace
