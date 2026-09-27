Imports System
Imports System.Collections.Generic
Imports MerchSys.Purchasing.Entities
Imports MerchSys.Purchasing.Services
Imports MerchSys.SharedKernel.Enums

Namespace Views
    Public Interface IPurchaseOrderListView
        Property StatusFilter As PurchaseOrderStatus?
        Property SearchTerm As String
        Property SelectedPurchaseOrderId As Integer?
        Property IsManager As Boolean

        Sub BindPurchaseOrders(orders As List(Of PurchaseOrder))
        Sub ShowMessage(message As String)
        Sub ShowError(errorMessage As String)
        Function ConfirmAction(message As String) As Boolean
    End Interface

    Public Interface IPurchaseOrderEditorView
        Property SelectedVendorId As Integer?
        Property ExpectedDeliveryDate As DateTime?
        Property Notes As String
        Property TotalAmountText As String

        Sub BindVendors(vendors As List(Of VendorDetailDto))
        Sub BindLineItems(lines As List(Of PurchaseOrderLine))

        Sub SetEditorVisible(visible As Boolean)
        Sub SetEditorEnabled(enabled As Boolean)

        Sub ShowMessage(message As String)
        Sub ShowError(errorMessage As String)
        Function ConfirmAction(message As String) As Boolean
    End Interface
End Namespace
