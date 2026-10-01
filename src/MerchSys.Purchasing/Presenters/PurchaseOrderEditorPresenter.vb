Imports System.Threading.Tasks
Imports System.Linq
Imports MerchSys.Purchasing.Services
Imports MerchSys.Purchasing.Views
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Paging
Imports System

Namespace Presenters
    Public Class PurchaseOrderEditorPresenter
        Private ReadOnly _poService As IPurchaseOrderService
        Private ReadOnly _vendorService As IVendorService
        Private ReadOnly _sessionService As ISessionService
        Private _poId As Integer?

        Public ReadOnly Property View As IPurchaseOrderEditorView

        Public Sub New(view As IPurchaseOrderEditorView, poService As IPurchaseOrderService, vendorService As IVendorService, sessionService As ISessionService)
            Me.View = view
            _poService = poService
            _vendorService = vendorService
            _sessionService = sessionService

            Me.View.OnLoadData = AddressOf LoadDataAsync
            Me.View.OnAddLine = AddressOf AddLineAsync
            Me.View.OnRemoveLine = AddressOf RemoveLineAsync
            Me.View.OnSaveDraft = AddressOf SaveDraftActionAsync
            Me.View.OnSubmit = AddressOf SubmitAsync
        End Sub

        Public Sub SetPurchaseOrderId(id As Integer?)
            _poId = id
        End Sub

        Private Async Function LoadDataAsync() As Task
            Try
                ' Load Vendors
                Dim request As New PageRequest With { .PageSize = 1000, .IsFirstPage = True }
                Dim vendorsResult = Await _vendorService.GetHistoryAsync(request)
                Me.View.BindVendors(vendorsResult.Items.ToList())

                If _poId.HasValue Then
                    ' Load existing PO
                    Dim po = Await _poService.GetByIdAsync(_poId.Value)
                    If po IsNot Nothing Then
                        Me.View.SelectedVendorId = po.VendorId
                        Me.View.Notes = po.Notes
                        Me.View.ExpectedDeliveryDate = po.ExpectedDeliveryDate
                        Me.View.POStatus = po.Status
                        Me.View.UpdateTotal(po.TotalAmount)

                        Dim lines = Await _poService.GetLinesAsync(po.Id)
                        Me.View.BindLines(lines)
                        
                        ' Role-based logic and Status check for read-only
                        If _sessionService.CurrentUserRole = UserRole.Owner OrElse po.Status <> PurchaseOrderStatus.Draft Then
                            Me.View.SetReadOnly(True)
                        End If
                    End If
                Else
                    ' New PO
                    Me.View.POStatus = PurchaseOrderStatus.Draft
                    Me.View.UpdateTotal(0D)
                    If _sessionService.CurrentUserRole = UserRole.Owner Then
                        Me.View.SetReadOnly(True)
                    End If
                End If
            Catch ex As Exception
                Me.View.ShowError("Failed to load data: " & ex.Message)
            End Try
        End Function

        Private Async Function AddLineAsync(productName As String, qty As Integer, unitCost As Decimal, lineTotal As Decimal) As Task
            Try
                If Not _poId.HasValue Then
                    ' Save the draft first if this is a new PO and they are adding lines
                    Await SaveDraftAsync(False)
                    If Not _poId.HasValue Then Return ' Save failed
                End If

                Dim dto As New CreatePOLineDto With {
                    .ProductName = productName,
                    .Quantity = qty,
                    .UnitCost = unitCost,
                    .LineTotal = lineTotal
                }
                
                Await _poService.AddLineAsync(_poId.Value, dto)
                Await ReloadLinesAndTotalAsync()
            Catch ex As Exception
                Me.View.ShowError("Failed to add line: " & ex.Message)
            End Try
        End Function

        Private Async Function RemoveLineAsync(lineId As Integer) As Task
            Try
                If _poId.HasValue Then
                    Await _poService.RemoveLineAsync(_poId.Value, lineId)
                    Await ReloadLinesAndTotalAsync()
                End If
            Catch ex As Exception
                Me.View.ShowError("Failed to remove line: " & ex.Message)
            End Try
        End Function

        Private Async Function ReloadLinesAndTotalAsync() As Task
            Dim po = Await _poService.GetByIdAsync(_poId.Value)
            Dim lines = Await _poService.GetLinesAsync(_poId.Value)
            Me.View.BindLines(lines)
            Me.View.UpdateTotal(po.TotalAmount)
        End Function
        
        Private Async Function SaveDraftActionAsync() As Task
            Await SaveDraftAsync(True)
        End Function

        Private Async Function SaveDraftAsync(Optional closeAfterSave As Boolean = True) As Task
            Try
                Dim vendorId = Me.View.SelectedVendorId
                If vendorId = 0 Then
                    Me.View.ShowError("Please select a vendor.")
                    Return
                End If

                If _poId.HasValue Then
                    ' Update existing
                    Await _poService.UpdateDraftAsync(_poId.Value, vendorId, Me.View.Notes, Me.View.ExpectedDeliveryDate)
                Else
                    ' Create new
                    Dim po = Await _poService.CreateDraftAsync(vendorId, Me.View.Notes, Me.View.ExpectedDeliveryDate)
                    _poId = po.Id
                End If
                
                If closeAfterSave Then
                    Me.View.CloseDialog()
                End If
            Catch ex As Exception
                Me.View.ShowError("Failed to save draft: " & ex.Message)
            End Try
        End Function

        Private Async Function SubmitAsync() As Task
            Try
                Await SaveDraftAsync(False) ' Ensure changes are saved
                If _poId.HasValue Then
                    Await _poService.SubmitAsync(_poId.Value)
                    Me.View.CloseDialog()
                End If
            Catch ex As Exception
                Me.View.ShowError("Failed to submit PO: " & ex.Message)
            End Try
        End Function
    End Class
End Namespace
