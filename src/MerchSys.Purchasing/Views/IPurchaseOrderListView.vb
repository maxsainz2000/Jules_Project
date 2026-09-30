Imports System.Collections
Imports System.Threading.Tasks
Imports MerchSys.Purchasing.Entities
Imports MerchSys.SharedKernel.Enums

Namespace Views
    Public Interface IPurchaseOrderListView
        ' Events mapped to presenter
        Property OnSearch As Func(Of String, PurchaseOrderStatus?, Task)
        Property OnNewPO As Func(Of Task)
        Property OnEditPO As Func(Of Integer, Task)
        Property OnSubmitPO As Func(Of Integer, Task)
        Property OnDeletePO As Func(Of Integer, Task)
        Property OnLoadPage As Func(Of Task)

        ' Properties manipulated by presenter
        Property CanEdit As Boolean

        ' Methods to update view
        Sub BindData(items As IEnumerable)
        Sub ShowError(message As String)
        Sub ShowMessage(message As String)
        Function ConfirmAction(message As String, title As String) As Boolean
    End Interface
End Namespace
