Imports System.Collections
Imports System.Threading.Tasks

Namespace Views
    Public Interface IVendorDirectoryView
        Property IsEditorOpen As Boolean
        Property SelectedVendorId As Integer?
        Property EditorName As String
        Property EditorPhone As String
        Property EditorLeadTimeDays As Integer

        Sub BindVendors(items As IEnumerable)
        Sub BindRecentPOs(items As IEnumerable)
        Sub ShowError(message As String)
        Function ConfirmAction(message As String, title As String) As Boolean

        Property OnAdd As Func(Of Task)
        Property OnEdit As Func(Of Task)
        Property OnSave As Func(Of Task)
        Property OnDelete As Func(Of Task)
        Property OnSearch As Func(Of String, Task)
        Property OnSelectionChanged As Func(Of Integer?, Task)
    End Interface
End Namespace
