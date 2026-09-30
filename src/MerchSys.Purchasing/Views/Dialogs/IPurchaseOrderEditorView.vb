Imports System.Collections.Generic
Imports System.Windows.Forms
Imports MerchSys.Purchasing.Entities
Imports MerchSys.Purchasing.Services

Namespace Views.Dialogs
    Public Interface IPurchaseOrderEditorView
        Property Vendors As List(Of VendorDetailDto)
        Property SelectedVendorId As Integer?
        Property Notes As String
        Property ExpectedDeliveryDate As DateTime?
        Property Lines As List(Of PurchaseOrderLine)
        Property RunningTotal As Decimal

        Function ShowDialog(owner As IWin32Window) As DialogResult
        Sub SetPresenter(presenter As Presenters.PurchaseOrders.PurchaseOrderEditorPresenter)
    End Interface
End Namespace