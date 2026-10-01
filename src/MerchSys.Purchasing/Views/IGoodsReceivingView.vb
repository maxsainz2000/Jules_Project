Imports System.Collections
Imports System.Threading.Tasks

Namespace Views
    Public Interface IGoodsReceivingView
        ' Events mapped to presenter
        Property OnLoadPendingPOs As Func(Of Task)
        Property OnPOSelected As Func(Of Integer, Task)
        Property OnConfirmReceipt As Func(Of Task)

        ' Properties manipulated by presenter
        Property IsPOSelected As Boolean

        ' Methods to update view
        Sub BindPOSelector(items As IEnumerable)
        Sub BindReceivingLines(items As IEnumerable)
        Sub ShowError(message As String)
        Sub ShowMessage(message As String)
        Function ConfirmAction(message As String, title As String) As Boolean
        Sub SetStatus(status As String)
        Sub RefreshLines()
    End Interface
End Namespace
