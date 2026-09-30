Imports MerchSys.App.Views.Shell.Modules.Purchasing

Namespace Views.Shell.Modules.Purchasing
    Public Interface IPurchaseOrderEditorView
        Sub AttachPresenter(presenter As PurchaseOrderEditorPresenter)
        Function ShowDialog() As System.Windows.Forms.DialogResult
        Sub CloseView()
        Sub Close()
        Property SelectedVendorId As Integer?
        Property Vendors As Object
        Property OrderNumber As String
        Property Status As Integer
        Property ExpectedDeliveryDate As Date?
        Property Notes As String
        Property Lines As Object
        Property SelectedLineIndex As Integer?
        Property SelectedLine As MerchSys.Purchasing.Entities.PurchaseOrderLine
        Property TotalAmount As Decimal
        Property CanEdit As Boolean
        Property CanAddLine As Boolean
        Property CanRemoveLine As Boolean
        Property CanSave As Boolean
        Property CanSubmit As Boolean
        Sub ShowError(message As String)
        Sub ShowMessage(message As String)

        Event SaveRequested As System.EventHandler
        Event SubmitRequested As System.EventHandler
        Event AddLineRequested As System.EventHandler
        Event RemoveLineRequested As System.EventHandler
    End Interface
End Namespace
