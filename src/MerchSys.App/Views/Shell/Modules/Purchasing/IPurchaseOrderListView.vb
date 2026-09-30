Imports MerchSys.App.Views.Shell.Modules.Purchasing

Namespace Views.Shell.Modules.Purchasing
    Public Interface IPurchaseOrderListView
        Sub AttachPresenter(presenter As PurchaseOrderListPresenter)
        Property SearchText As String
        Property StatusFilter As String
        Property SelectedOrderId As Integer?
        Property Orders As Object
        Property CanAdd As Boolean
        Property CanEdit As Boolean
        Property CanSubmit As Boolean
        Property CanDelete As Boolean
        Sub ShowError(message As String)
        Sub ShowMessage(message As String)
        Sub InitializeStatusFilter(statuses As System.Collections.Generic.List(Of String))

        ' Properties required by the current Presenter implementation
        Property FilterStatus As Integer
        Property SearchTerm As String
        Property DataSource As Object
        Property SelectedPO As MerchSys.Purchasing.Entities.PurchaseOrder
        Property CanCreate As Boolean
        Event RefreshRequested As System.EventHandler
        Event CreateRequested As System.EventHandler
        Event EditRequested As System.EventHandler
        Event SubmitRequested As System.EventHandler
        Event DeleteRequested As System.EventHandler
        Function Confirm(message As String, title As String) As Boolean
    End Interface
End Namespace
