Imports System.Collections.Generic
Imports System.ComponentModel
Imports MerchSys.Purchasing.Services

Namespace Views
    Public Interface IVendorDirectoryView
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Property Vendors As List(Of VendorDetailDto)

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Property SelectedVendor As VendorDetailDto

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Property PurchaseHistory As VendorPurchaseHistoryDto

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Property IsEditorOpen As Boolean

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Property EditorTitle As String

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Property EditorName As String

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Property EditorPhone As String

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Property EditorEmail As String

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Property EditorContactPerson As String

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Property EditorAddress As String

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Property EditorTaxId As String

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Property EditorNotes As String

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Property EditorLeadTimeDays As Integer

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Property SearchTerm As String

        Event SearchTextChanged As EventHandler
        Event RefreshClicked As EventHandler
        Event AddClicked As EventHandler
        Event EditClicked As EventHandler
        Event DeleteClicked As EventHandler
        Event SaveClicked As EventHandler
        Event CancelClicked As EventHandler
        Event SelectionChanged As EventHandler

        Sub ShowError(message As String)
        Sub ShowMessage(message As String)
        Function ConfirmDelete(vendorName As String) As Boolean
    End Interface
End Namespace
