Imports System.Collections.Generic
Imports System.Threading.Tasks
Imports MerchSys.Purchasing.Entities
Imports MerchSys.SharedKernel.Enums

Namespace Views
    Public Interface IPurchaseOrderEditorView
        Property SelectedVendorId As Integer
        Property Notes As String
        Property ExpectedDeliveryDate As DateTime?
        Property POStatus As PurchaseOrderStatus

        Property OnSaveDraft As Func(Of Task)
        Property OnSubmit As Func(Of Task)
        Property OnAddLine As Func(Of String, Integer, Decimal, Decimal, Task)
        Property OnRemoveLine As Func(Of Integer, Task)
        Property OnLoadData As Func(Of Task)

        Sub BindVendors(vendors As List(Of Vendor))
        Sub BindLines(lines As List(Of PurchaseOrderLine))
        Sub UpdateTotal(total As Decimal)
        Sub SetReadOnly(isReadOnly As Boolean)
        Sub ShowError(message As String)
        Sub CloseDialog()

        Function ShowDialog() As System.Windows.Forms.DialogResult
    End Interface
End Namespace
