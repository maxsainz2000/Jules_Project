Imports System.Collections
Imports System.Threading.Tasks

Namespace Views.Purchasing
    Public Interface IVendorCatalogView
        Property SelectedVendorId As Integer?
        Property SelectedVendorProductId As Integer?
        Property ProductIdInput As Integer
        Property UnitCostInput As Decimal
        Property NotesInput As String

        Sub BindVendors(items As IEnumerable)
        Sub BindVendorProducts(items As IEnumerable)
        Sub ShowError(message As String)
        Function ConfirmAction(message As String, title As String) As Boolean

        Property OnAdd As Func(Of Task)
        Property OnEdit As Func(Of Task)
        Property OnSave As Func(Of Task)
        Property OnDelete As Func(Of Task)
        Property OnVendorSelected As Func(Of Task)
    End Interface
End Namespace
