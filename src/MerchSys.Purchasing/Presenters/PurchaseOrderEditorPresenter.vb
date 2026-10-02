Imports System.Threading.Tasks
Imports System.Linq
Imports MerchSys.Purchasing.Services
Imports MerchSys.Purchasing.Views
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Paging
Imports System
Imports System.ComponentModel
Imports MerchSys.SharedKernel.Queries

Namespace Presenters
    Public Class PurchaseOrderEditorPresenter
        Private ReadOnly _poService As IPurchaseOrderService
        Private ReadOnly _vendorService As IVendorService
        Private ReadOnly _sessionService As ISessionService
        Private ReadOnly _mediator As MediatR.IMediator
        Private ReadOnly _vendorProductService As IVendorProductService
        Private _poId As Integer?

        Public ReadOnly Property View As IPurchaseOrderEditorView

        Public Sub New(view As IPurchaseOrderEditorView, poService As IPurchaseOrderService, vendorService As IVendorService, sessionService As ISessionService, mediator As MediatR.IMediator, vendorProductService As IVendorProductService)
            Me.View = view
            _poService = poService
            _vendorService = vendorService
            _sessionService = sessionService
            _mediator = mediator
            _vendorProductService = vendorProductService

            Me.View.OnLoadData = AddressOf LoadDataAsync
            Me.View.OnVendorChanged = AddressOf VendorChangedAsync
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
                        Await VendorChangedAsync(po.VendorId)

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
                    If Me.View.SelectedVendorId > 0 Then
                        Await VendorChangedAsync(Me.View.SelectedVendorId)
                    End If
                End If
            Catch ex As Exception
                Me.View.ShowError("Failed to load data: " & ex.Message)
            End Try
        End Function

        Private Async Function VendorChangedAsync(vendorId As Integer) As Task
            Try
                Dim request As New PageRequest With { .PageSize = 10000, .IsFirstPage = True }
                Dim allProducts = Await _vendorProductService.GetHistoryAsync(request)
                Dim vendorProducts = allProducts.Items.Where(Function(p) p.VendorId = vendorId AndAlso Not p.IsDeleted).ToList()

                Dim catalogProducts = Await _mediator.Send(New GetProductsForCatalogQuery())

                Dim catalog = New BindingList(Of VendorCatalogItem)()
                For Each vp In vendorProducts
                    Dim product = catalogProducts.FirstOrDefault(Function(p) p.Id = vp.ProductId)
                    If product IsNot Nothing Then
                        catalog.Add(New VendorCatalogItem With {
                            .ProductId = vp.ProductId,
                            .ProductName = product.Name,
                            .UnitCost = vp.UnitCost
                        })
                    End If
                Next

                Me.View.VendorCatalog = catalog
            Catch ex As Exception
                Me.View.ShowError("Failed to load vendor catalog: " & ex.Message)
            End Try
        End Function

        Private Async Function AddLineAsync(productId As Integer, productName As String, qty As Integer, unitCost As Decimal, lineTotal As Decimal) As Task
            Try
                If Not _poId.HasValue Then
                    ' Save the draft first if this is a new PO and they are adding lines
                    Await SaveDraftAsync(False)
                    If Not _poId.HasValue Then Return ' Save failed
                End If

                Dim dto As New CreatePOLineDto With {
                    .ProductId = productId,
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
                    Dim lines = Await _poService.GetLinesAsync(_poId.Value)
                    If lines.Any(Function(l) l.ProductId = 0) Then
                        Me.View.ShowError("Cannot save purchase order: one or more lines have an invalid product selection (ProductId = 0).")
                        Return
                    End If
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
