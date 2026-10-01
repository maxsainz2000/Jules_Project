#!/bin/bash
# I lost the files doing hard reset. Need to recreate them quickly with fixes.

mkdir -p src/MerchSys.Purchasing/Views/
cat << 'INNER' > src/MerchSys.Purchasing/Views/IVendorDirectoryView.vb
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
INNER

cat << 'INNER' > src/MerchSys.Purchasing/Presenters/VendorEditorPresenter.vb
Imports MerchSys.Purchasing.Services

Namespace Presenters
    Public Class VendorEditorPresenter
        Public Property Id As Integer?
        Public Property Name As String
        Public Property Phone As String
        Public Property Email As String
        Public Property ContactPerson As String
        Public Property Address As String
        Public Property TaxId As String
        Public Property Notes As String
        Public Property LeadTimeDays As Integer

        Public Sub Load(vendor As VendorDetailDto)
            If vendor IsNot Nothing Then
                Id = vendor.Id
                Name = vendor.Name
                Phone = vendor.Phone
                Email = vendor.Email
                ContactPerson = vendor.ContactPerson
                Address = vendor.Address
                TaxId = vendor.TaxId
                Notes = vendor.Notes
                LeadTimeDays = vendor.LeadTimeDays
            Else
                Clear()
            End If
        End Sub

        Public Sub Clear()
            Id = Nothing
            Name = String.Empty
            Phone = String.Empty
            Email = String.Empty
            ContactPerson = String.Empty
            Address = String.Empty
            TaxId = String.Empty
            Notes = String.Empty
            LeadTimeDays = 0
        End Sub

        Public Function Validate() As String
            If String.IsNullOrWhiteSpace(Name) Then Return "Vendor Name is required."
            If String.IsNullOrWhiteSpace(Phone) Then Return "Phone is required."
            If LeadTimeDays <= 0 Then Return "Lead Time must be greater than 0."
            Return Nothing
        End Function

        Public Function ToCreateDto() As CreateVendorDto
            Return New CreateVendorDto With {
                .Name = Name.Trim(),
                .Phone = Phone?.Trim(),
                .Email = Email?.Trim(),
                .ContactPerson = ContactPerson?.Trim(),
                .Address = Address?.Trim(),
                .TaxId = TaxId?.Trim(),
                .Notes = Notes?.Trim(),
                .LeadTimeDays = LeadTimeDays
            }
        End Function

        Public Function ToUpdateDto() As UpdateVendorDto
            Return New UpdateVendorDto With {
                .Id = Id.GetValueOrDefault(),
                .Name = Name.Trim(),
                .Phone = Phone?.Trim(),
                .Email = Email?.Trim(),
                .ContactPerson = ContactPerson?.Trim(),
                .Address = Address?.Trim(),
                .TaxId = TaxId?.Trim(),
                .Notes = Notes?.Trim(),
                .LeadTimeDays = LeadTimeDays
            }
        End Function
    End Class
End Namespace
INNER

cat << 'INNER' > src/MerchSys.Purchasing/Presenters/VendorListPresenter.vb
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports MerchSys.Purchasing.Services
Imports MerchSys.Purchasing.Views
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.SharedKernel.Enums
Imports System.Collections.Generic

Namespace Presenters
    Public Class VendorListPresenter
        Private ReadOnly _vendorService As IVendorService
        Private ReadOnly _sessionService As ISessionService
        Private ReadOnly _editorPresenter As VendorEditorPresenter

        Private _view As IVendorDirectoryView
        Public Property View As IVendorDirectoryView
            Get
                Return _view
            End Get
            Set(value As IVendorDirectoryView)
                _view = value
                If _view IsNot Nothing Then
                    WireUpEvents()
                End If
            End Set
        End Property

        Public Sub New(vendorService As IVendorService, sessionService As ISessionService)
            _vendorService = vendorService
            _sessionService = sessionService
            _editorPresenter = New VendorEditorPresenter()
        End Sub

        Private Sub WireUpEvents()
            AddHandler View.SearchTextChanged, AddressOf OnSearchTextChanged
            AddHandler View.RefreshClicked, AddressOf OnRefreshClicked
            AddHandler View.AddClicked, AddressOf OnAddClicked
            AddHandler View.EditClicked, AddressOf OnEditClicked
            AddHandler View.DeleteClicked, AddressOf OnDeleteClicked
            AddHandler View.SaveClicked, Async Sub(s, e) Await OnSaveClickedAsync()
            AddHandler View.CancelClicked, AddressOf OnCancelClicked
            AddHandler View.SelectionChanged, Async Sub(s, e) Await OnSelectionChangedAsync()
        End Sub

        Public Async Function InitializeAsync() As Task
            Await LoadVendorsAsync()
        End Function

        Private Async Sub OnSearchTextChanged(sender As Object, e As EventArgs)
            Await LoadVendorsAsync()
        End Sub

        Private Async Sub OnRefreshClicked(sender As Object, e As EventArgs)
            Await LoadVendorsAsync()
        End Sub

        Private Async Function LoadVendorsAsync() As Task
            Try
                Dim vendors As List(Of VendorDetailDto)
                If String.IsNullOrWhiteSpace(View.SearchTerm) Then
                    vendors = Await _vendorService.GetAllAsync()
                Else
                    vendors = Await _vendorService.SearchAsync(View.SearchTerm)
                End If
                View.Vendors = vendors
            Catch ex As Exception
                View.ShowError($"Failed to load vendors: {ex.Message}")
            End Try
        End Function

        Private Sub OnAddClicked(sender As Object, e As EventArgs)
            If Not IsManager() Then
                View.ShowError("You do not have permission to add vendors.")
                Return
            End If

            _editorPresenter.Clear()
            UpdateViewFromEditor("Add New Vendor")
            View.IsEditorOpen = True
        End Sub

        Private Sub OnEditClicked(sender As Object, e As EventArgs)
            If Not IsManager() Then
                View.ShowError("You do not have permission to edit vendors.")
                Return
            End If

            Dim selected = View.SelectedVendor
            If selected Is Nothing Then
                View.ShowError("Please select a vendor to edit.")
                Return
            End If

            _editorPresenter.Load(selected)
            UpdateViewFromEditor($"Edit Vendor: {selected.Name}")
            View.IsEditorOpen = True
        End Sub

        Private Async Sub OnDeleteClicked(sender As Object, e As EventArgs)
            If Not IsManager() Then
                View.ShowError("You do not have permission to delete vendors.")
                Return
            End If

            Dim selected = View.SelectedVendor
            If selected Is Nothing Then
                View.ShowError("Please select a vendor to delete.")
                Return
            End If

            If View.ConfirmDelete(selected.Name) Then
                Try
                    Await _vendorService.DeleteAsync(selected.Id)
                    View.ShowMessage("Vendor deleted successfully.")
                    View.IsEditorOpen = False
                    Await LoadVendorsAsync()
                Catch ex As Exception
                    View.ShowError($"Failed to delete vendor: {ex.Message}")
                End Try
            End If
        End Sub

        Private Async Function OnSaveClickedAsync() As Task
            UpdateEditorFromView()
            Dim validationError = _editorPresenter.Validate()
            If Not String.IsNullOrEmpty(validationError) Then
                View.ShowError(validationError)
                Return
            End If

            Try
                If _editorPresenter.Id.HasValue Then
                    Await _vendorService.UpdateAsync(_editorPresenter.Id.Value, _editorPresenter.ToUpdateDto())
                    View.ShowMessage("Vendor updated successfully.")
                Else
                    Await _vendorService.CreateAsync(_editorPresenter.ToCreateDto())
                    View.ShowMessage("Vendor created successfully.")
                End If

                View.IsEditorOpen = False
                Await LoadVendorsAsync()
            Catch ex As Exception
                View.ShowError($"Failed to save vendor: {ex.Message}")
            End Try
        End Function

        Private Sub OnCancelClicked(sender As Object, e As EventArgs)
            View.IsEditorOpen = False
            _editorPresenter.Clear()
        End Sub

        Private Async Function OnSelectionChangedAsync() As Task
            Dim selected = View.SelectedVendor
            If selected IsNot Nothing Then
                Try
                    View.PurchaseHistory = Await _vendorService.GetPurchaseHistoryAsync(selected.Id)
                Catch ex As Exception
                    View.ShowError($"Failed to load purchase history: {ex.Message}")
                End Try
            Else
                View.PurchaseHistory = Nothing
            End If
        End Function

        Private Sub UpdateViewFromEditor(title As String)
            View.EditorTitle = title
            View.EditorName = _editorPresenter.Name
            View.EditorPhone = _editorPresenter.Phone
            View.EditorEmail = _editorPresenter.Email
            View.EditorContactPerson = _editorPresenter.ContactPerson
            View.EditorAddress = _editorPresenter.Address
            View.EditorTaxId = _editorPresenter.TaxId
            View.EditorNotes = _editorPresenter.Notes
            View.EditorLeadTimeDays = _editorPresenter.LeadTimeDays
        End Sub

        Private Sub UpdateEditorFromView()
            _editorPresenter.Name = View.EditorName
            _editorPresenter.Phone = View.EditorPhone
            _editorPresenter.Email = View.EditorEmail
            _editorPresenter.ContactPerson = View.EditorContactPerson
            _editorPresenter.Address = View.EditorAddress
            _editorPresenter.TaxId = View.EditorTaxId
            _editorPresenter.Notes = View.EditorNotes
            _editorPresenter.LeadTimeDays = View.EditorLeadTimeDays
        End Sub

        Private Function IsManager() As Boolean
            Dim role = _sessionService.CurrentUserRole
            Return role = UserRole.Manager OrElse role = UserRole.Owner OrElse role = UserRole.Developer
        End Function
    End Class

    Public Class POSummaryRow
        Public Property PONumber As String
        Public Property CreatedAt As DateTime
        Public Property TotalAmount As Decimal
        Public Property Status As String
    End Class
End Namespace
INNER

cat << 'INNER' > src/MerchSys.Purchasing/Views/VendorDirectoryView.Designer.vb
Namespace Views
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class VendorDirectoryView
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.SearchTextBox = New System.Windows.Forms.ToolStripTextBox()
        Me.RefreshButton = New System.Windows.Forms.ToolStripButton()
        Me.AddButton = New System.Windows.Forms.ToolStripButton()
        Me.EditButton = New System.Windows.Forms.ToolStripButton()
        Me.DeleteButton = New System.Windows.Forms.ToolStripButton()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.VendorGrid = New System.Windows.Forms.DataGridView()
        Me.DetailPanel = New System.Windows.Forms.Panel()
        Me.POGrid = New System.Windows.Forms.DataGridView()
        Me.StatsLabel = New System.Windows.Forms.Label()
        Me.DetailTitleLabel = New System.Windows.Forms.Label()
        Me.EditorPanel = New System.Windows.Forms.Panel()
        Me.CancelEditButton = New System.Windows.Forms.Button()
        Me.SaveEditButton = New System.Windows.Forms.Button()
        Me.LeadTimeTextBox = New System.Windows.Forms.TextBox()
        Me.LeadTimeLabel = New System.Windows.Forms.Label()
        Me.PhoneTextBox = New System.Windows.Forms.TextBox()
        Me.PhoneLabel = New System.Windows.Forms.Label()
        Me.EmailTextBox = New System.Windows.Forms.TextBox()
        Me.EmailLabel = New System.Windows.Forms.Label()
        Me.ContactTextBox = New System.Windows.Forms.TextBox()
        Me.ContactLabel = New System.Windows.Forms.Label()
        Me.AddressTextBox = New System.Windows.Forms.TextBox()
        Me.AddressLabel = New System.Windows.Forms.Label()
        Me.TaxIdTextBox = New System.Windows.Forms.TextBox()
        Me.TaxIdLabel = New System.Windows.Forms.Label()
        Me.NotesTextBox = New System.Windows.Forms.TextBox()
        Me.NotesLabel = New System.Windows.Forms.Label()
        Me.NameTextBox = New System.Windows.Forms.TextBox()
        Me.NameLabel = New System.Windows.Forms.Label()
        Me.EditorTitleLabel = New System.Windows.Forms.Label()
        Me.ToolStrip1.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.VendorGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.DetailPanel.SuspendLayout()
        CType(Me.POGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.EditorPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SearchTextBox, Me.RefreshButton, Me.AddButton, Me.EditButton, Me.DeleteButton})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(800, 25)
        Me.ToolStrip1.TabIndex = 0
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'SearchTextBox
        '
        Me.SearchTextBox.Name = "SearchTextBox"
        Me.SearchTextBox.Size = New System.Drawing.Size(200, 25)
        Me.SearchTextBox.ToolTipText = "Search vendors... (Press Escape to clear)"
        '
        'RefreshButton
        '
        Me.RefreshButton.Name = "RefreshButton"
        Me.RefreshButton.Size = New System.Drawing.Size(50, 22)
        Me.RefreshButton.Text = "Refresh"
        '
        'AddButton
        '
        Me.AddButton.Name = "AddButton"
        Me.AddButton.Size = New System.Drawing.Size(33, 22)
        Me.AddButton.Text = "Add"
        '
        'EditButton
        '
        Me.EditButton.Name = "EditButton"
        Me.EditButton.Size = New System.Drawing.Size(31, 22)
        Me.EditButton.Text = "Edit"
        '
        'DeleteButton
        '
        Me.DeleteButton.Name = "DeleteButton"
        Me.DeleteButton.Size = New System.Drawing.Size(44, 22)
        Me.DeleteButton.Text = "Delete"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 25)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.VendorGrid)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.DetailPanel)
        Me.SplitContainer1.Size = New System.Drawing.Size(800, 425)
        Me.SplitContainer1.SplitterDistance = 450
        Me.SplitContainer1.TabIndex = 1
        '
        'VendorGrid
        '
        Me.VendorGrid.AllowUserToAddRows = False
        Me.VendorGrid.AllowUserToDeleteRows = False
        Me.VendorGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.VendorGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.VendorGrid.Location = New System.Drawing.Point(0, 0)
        Me.VendorGrid.MultiSelect = False
        Me.VendorGrid.Name = "VendorGrid"
        Me.VendorGrid.ReadOnly = True
        Me.VendorGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.VendorGrid.Size = New System.Drawing.Size(450, 425)
        Me.VendorGrid.TabIndex = 0
        '
        'DetailPanel
        '
        Me.DetailPanel.Controls.Add(Me.POGrid)
        Me.DetailPanel.Controls.Add(Me.StatsLabel)
        Me.DetailPanel.Controls.Add(Me.DetailTitleLabel)
        Me.DetailPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DetailPanel.Location = New System.Drawing.Point(0, 0)
        Me.DetailPanel.Name = "DetailPanel"
        Me.DetailPanel.Padding = New System.Windows.Forms.Padding(10)
        Me.DetailPanel.Size = New System.Drawing.Size(346, 425)
        Me.DetailPanel.TabIndex = 0
        '
        'POGrid
        '
        Me.POGrid.AllowUserToAddRows = False
        Me.POGrid.AllowUserToDeleteRows = False
        Me.POGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.POGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.POGrid.Location = New System.Drawing.Point(10, 80)
        Me.POGrid.Name = "POGrid"
        Me.POGrid.ReadOnly = True
        Me.POGrid.Size = New System.Drawing.Size(326, 335)
        Me.POGrid.TabIndex = 2
        '
        'StatsLabel
        '
        Me.StatsLabel.Dock = System.Windows.Forms.DockStyle.Top
        Me.StatsLabel.Location = New System.Drawing.Point(10, 30)
        Me.StatsLabel.Name = "StatsLabel"
        Me.StatsLabel.Size = New System.Drawing.Size(326, 50)
        Me.StatsLabel.TabIndex = 1
        Me.StatsLabel.Text = "Stats..."
        '
        'DetailTitleLabel
        '
        Me.DetailTitleLabel.Dock = System.Windows.Forms.DockStyle.Top
        Me.DetailTitleLabel.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me.DetailTitleLabel.Location = New System.Drawing.Point(10, 10)
        Me.DetailTitleLabel.Name = "DetailTitleLabel"
        Me.DetailTitleLabel.Size = New System.Drawing.Size(326, 20)
        Me.DetailTitleLabel.TabIndex = 0
        Me.DetailTitleLabel.Text = "Vendor Details"
        '
        'EditorPanel
        '
        Me.EditorPanel.Controls.Add(Me.CancelEditButton)
        Me.EditorPanel.Controls.Add(Me.SaveEditButton)
        Me.EditorPanel.Controls.Add(Me.NameLabel)
        Me.EditorPanel.Controls.Add(Me.NameTextBox)
        Me.EditorPanel.Controls.Add(Me.PhoneLabel)
        Me.EditorPanel.Controls.Add(Me.PhoneTextBox)
        Me.EditorPanel.Controls.Add(Me.EmailLabel)
        Me.EditorPanel.Controls.Add(Me.EmailTextBox)
        Me.EditorPanel.Controls.Add(Me.ContactLabel)
        Me.EditorPanel.Controls.Add(Me.ContactTextBox)
        Me.EditorPanel.Controls.Add(Me.AddressLabel)
        Me.EditorPanel.Controls.Add(Me.AddressTextBox)
        Me.EditorPanel.Controls.Add(Me.TaxIdLabel)
        Me.EditorPanel.Controls.Add(Me.TaxIdTextBox)
        Me.EditorPanel.Controls.Add(Me.LeadTimeLabel)
        Me.EditorPanel.Controls.Add(Me.LeadTimeTextBox)
        Me.EditorPanel.Controls.Add(Me.NotesLabel)
        Me.EditorPanel.Controls.Add(Me.NotesTextBox)
        Me.EditorPanel.Controls.Add(Me.EditorTitleLabel)
        Me.EditorPanel.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.EditorPanel.Location = New System.Drawing.Point(0, 450)
        Me.EditorPanel.Name = "EditorPanel"
        Me.EditorPanel.Size = New System.Drawing.Size(800, 200)
        Me.EditorPanel.TabIndex = 2
        Me.EditorPanel.Visible = False
        '
        'EditorTitleLabel
        '
        Me.EditorTitleLabel.AutoSize = True
        Me.EditorTitleLabel.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me.EditorTitleLabel.Location = New System.Drawing.Point(14, 14)
        Me.EditorTitleLabel.Name = "EditorTitleLabel"
        Me.EditorTitleLabel.Size = New System.Drawing.Size(42, 15)
        Me.EditorTitleLabel.TabIndex = 0
        Me.EditorTitleLabel.Text = "Editor"
        '
        'NameLabel
        '
        Me.NameLabel.AutoSize = True
        Me.NameLabel.Location = New System.Drawing.Point(14, 41)
        Me.NameLabel.Name = "NameLabel"
        Me.NameLabel.Size = New System.Drawing.Size(42, 15)
        Me.NameLabel.TabIndex = 1
        Me.NameLabel.Text = "Name:"
        '
        'NameTextBox
        '
        Me.NameTextBox.Location = New System.Drawing.Point(100, 38)
        Me.NameTextBox.Name = "NameTextBox"
        Me.NameTextBox.Size = New System.Drawing.Size(200, 23)
        Me.NameTextBox.TabIndex = 2
        '
        'PhoneLabel
        '
        Me.PhoneLabel.AutoSize = True
        Me.PhoneLabel.Location = New System.Drawing.Point(14, 70)
        Me.PhoneLabel.Name = "PhoneLabel"
        Me.PhoneLabel.Size = New System.Drawing.Size(44, 15)
        Me.PhoneLabel.TabIndex = 3
        Me.PhoneLabel.Text = "Phone:"
        '
        'PhoneTextBox
        '
        Me.PhoneTextBox.Location = New System.Drawing.Point(100, 67)
        Me.PhoneTextBox.Name = "PhoneTextBox"
        Me.PhoneTextBox.Size = New System.Drawing.Size(200, 23)
        Me.PhoneTextBox.TabIndex = 4
        '
        'EmailLabel
        '
        Me.EmailLabel.AutoSize = True
        Me.EmailLabel.Location = New System.Drawing.Point(14, 99)
        Me.EmailLabel.Name = "EmailLabel"
        Me.EmailLabel.Size = New System.Drawing.Size(39, 15)
        Me.EmailLabel.TabIndex = 5
        Me.EmailLabel.Text = "Email:"
        '
        'EmailTextBox
        '
        Me.EmailTextBox.Location = New System.Drawing.Point(100, 96)
        Me.EmailTextBox.Name = "EmailTextBox"
        Me.EmailTextBox.Size = New System.Drawing.Size(200, 23)
        Me.EmailTextBox.TabIndex = 6
        '
        'ContactLabel
        '
        Me.ContactLabel.AutoSize = True
        Me.ContactLabel.Location = New System.Drawing.Point(14, 128)
        Me.ContactLabel.Name = "ContactLabel"
        Me.ContactLabel.Size = New System.Drawing.Size(51, 15)
        Me.ContactLabel.TabIndex = 7
        Me.ContactLabel.Text = "Contact:"
        '
        'ContactTextBox
        '
        Me.ContactTextBox.Location = New System.Drawing.Point(100, 125)
        Me.ContactTextBox.Name = "ContactTextBox"
        Me.ContactTextBox.Size = New System.Drawing.Size(200, 23)
        Me.ContactTextBox.TabIndex = 8
        '
        'AddressLabel
        '
        Me.AddressLabel.AutoSize = True
        Me.AddressLabel.Location = New System.Drawing.Point(320, 41)
        Me.AddressLabel.Name = "AddressLabel"
        Me.AddressLabel.Size = New System.Drawing.Size(52, 15)
        Me.AddressLabel.TabIndex = 9
        Me.AddressLabel.Text = "Address:"
        '
        'AddressTextBox
        '
        Me.AddressTextBox.Location = New System.Drawing.Point(380, 38)
        Me.AddressTextBox.Name = "AddressTextBox"
        Me.AddressTextBox.Size = New System.Drawing.Size(200, 23)
        Me.AddressTextBox.TabIndex = 10
        '
        'TaxIdLabel
        '
        Me.TaxIdLabel.AutoSize = True
        Me.TaxIdLabel.Location = New System.Drawing.Point(320, 70)
        Me.TaxIdLabel.Name = "TaxIdLabel"
        Me.TaxIdLabel.Size = New System.Drawing.Size(39, 15)
        Me.TaxIdLabel.TabIndex = 11
        Me.TaxIdLabel.Text = "TaxId:"
        '
        'TaxIdTextBox
        '
        Me.TaxIdTextBox.Location = New System.Drawing.Point(380, 67)
        Me.TaxIdTextBox.Name = "TaxIdTextBox"
        Me.TaxIdTextBox.Size = New System.Drawing.Size(200, 23)
        Me.TaxIdTextBox.TabIndex = 12
        '
        'LeadTimeLabel
        '
        Me.LeadTimeLabel.AutoSize = True
        Me.LeadTimeLabel.Location = New System.Drawing.Point(320, 99)
        Me.LeadTimeLabel.Name = "LeadTimeLabel"
        Me.LeadTimeLabel.Size = New System.Drawing.Size(63, 15)
        Me.LeadTimeLabel.TabIndex = 13
        Me.LeadTimeLabel.Text = "Lead Time:"
        '
        'LeadTimeTextBox
        '
        Me.LeadTimeTextBox.Location = New System.Drawing.Point(380, 96)
        Me.LeadTimeTextBox.Name = "LeadTimeTextBox"
        Me.LeadTimeTextBox.Size = New System.Drawing.Size(100, 23)
        Me.LeadTimeTextBox.TabIndex = 14
        '
        'NotesLabel
        '
        Me.NotesLabel.AutoSize = True
        Me.NotesLabel.Location = New System.Drawing.Point(320, 128)
        Me.NotesLabel.Name = "NotesLabel"
        Me.NotesLabel.Size = New System.Drawing.Size(41, 15)
        Me.NotesLabel.TabIndex = 15
        Me.NotesLabel.Text = "Notes:"
        '
        'NotesTextBox
        '
        Me.NotesTextBox.Location = New System.Drawing.Point(380, 125)
        Me.NotesTextBox.Name = "NotesTextBox"
        Me.NotesTextBox.Size = New System.Drawing.Size(200, 23)
        Me.NotesTextBox.TabIndex = 16
        '
        'SaveEditButton
        '
        Me.SaveEditButton.Location = New System.Drawing.Point(14, 160)
        Me.SaveEditButton.Name = "SaveEditButton"
        Me.SaveEditButton.Size = New System.Drawing.Size(75, 23)
        Me.SaveEditButton.TabIndex = 17
        Me.SaveEditButton.Text = "Save"
        Me.SaveEditButton.UseVisualStyleBackColor = True
        '
        'CancelEditButton
        '
        Me.CancelEditButton.Location = New System.Drawing.Point(95, 160)
        Me.CancelEditButton.Name = "CancelEditButton"
        Me.CancelEditButton.Size = New System.Drawing.Size(75, 23)
        Me.CancelEditButton.TabIndex = 18
        Me.CancelEditButton.Text = "Cancel"
        Me.CancelEditButton.UseVisualStyleBackColor = True
        '
        'VendorDirectoryView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.EditorPanel)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Name = "VendorDirectoryView"
        Me.Size = New System.Drawing.Size(800, 600)
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.VendorGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.DetailPanel.ResumeLayout(False)
        CType(Me.POGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.EditorPanel.ResumeLayout(False)
        Me.EditorPanel.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Friend WithEvents SearchTextBox As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents RefreshButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents AddButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents EditButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents DeleteButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents VendorGrid As System.Windows.Forms.DataGridView
    Friend WithEvents DetailPanel As System.Windows.Forms.Panel
    Friend WithEvents POGrid As System.Windows.Forms.DataGridView
    Friend WithEvents StatsLabel As System.Windows.Forms.Label
    Friend WithEvents DetailTitleLabel As System.Windows.Forms.Label
    Friend WithEvents EditorPanel As System.Windows.Forms.Panel
    Friend WithEvents CancelEditButton As System.Windows.Forms.Button
    Friend WithEvents SaveEditButton As System.Windows.Forms.Button
    Friend WithEvents LeadTimeTextBox As System.Windows.Forms.TextBox
    Friend WithEvents LeadTimeLabel As System.Windows.Forms.Label
    Friend WithEvents PhoneTextBox As System.Windows.Forms.TextBox
    Friend WithEvents PhoneLabel As System.Windows.Forms.Label
    Friend WithEvents EmailTextBox As System.Windows.Forms.TextBox
    Friend WithEvents EmailLabel As System.Windows.Forms.Label
    Friend WithEvents ContactTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ContactLabel As System.Windows.Forms.Label
    Friend WithEvents AddressTextBox As System.Windows.Forms.TextBox
    Friend WithEvents AddressLabel As System.Windows.Forms.Label
    Friend WithEvents TaxIdTextBox As System.Windows.Forms.TextBox
    Friend WithEvents TaxIdLabel As System.Windows.Forms.Label
    Friend WithEvents NotesTextBox As System.Windows.Forms.TextBox
    Friend WithEvents NotesLabel As System.Windows.Forms.Label
    Friend WithEvents NameTextBox As System.Windows.Forms.TextBox
    Friend WithEvents NameLabel As System.Windows.Forms.Label
    Friend WithEvents EditorTitleLabel As System.Windows.Forms.Label

End Class
End Namespace
INNER

cat << 'INNER' > src/MerchSys.Purchasing/Views/VendorDirectoryView.vb
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Collections.Generic
Imports MerchSys.Purchasing.Services
Imports MerchSys.Purchasing.Presenters

Namespace Views
    Public Class VendorDirectoryView
        Inherits UserControl
        Implements IVendorDirectoryView

        Public Event SearchTextChanged As EventHandler Implements IVendorDirectoryView.SearchTextChanged
        Public Event RefreshClicked As EventHandler Implements IVendorDirectoryView.RefreshClicked
        Public Event AddClicked As EventHandler Implements IVendorDirectoryView.AddClicked
        Public Event EditClicked As EventHandler Implements IVendorDirectoryView.EditClicked
        Public Event DeleteClicked As EventHandler Implements IVendorDirectoryView.DeleteClicked
        Public Event SaveClicked As EventHandler Implements IVendorDirectoryView.SaveClicked
        Public Event CancelClicked As EventHandler Implements IVendorDirectoryView.CancelClicked
        Public Event SelectionChanged As EventHandler Implements IVendorDirectoryView.SelectionChanged

        Private _presenter As VendorListPresenter

        Public Sub New()
            InitializeComponent()
        End Sub

        Public Sub New(presenter As VendorListPresenter)
            InitializeComponent()
            _presenter = presenter
            _presenter.View = Me

            WireUpEvents()

            VendorGrid.AutoGenerateColumns = True
            POGrid.AutoGenerateColumns = True

            If Not DesignMode Then
                Me.BeginInvoke(New Action(Async Sub() Await _presenter.InitializeAsync()))
            End If
        End Sub

        Private Sub WireUpEvents()
            AddHandler SearchTextBox.TextChanged, Sub(s, e) RaiseEvent SearchTextChanged(Me, EventArgs.Empty)
            AddHandler SearchTextBox.KeyDown, AddressOf SearchTextBox_KeyDown
            AddHandler RefreshButton.Click, Sub(s, e) RaiseEvent RefreshClicked(Me, EventArgs.Empty)
            AddHandler AddButton.Click, Sub(s, e) RaiseEvent AddClicked(Me, EventArgs.Empty)
            AddHandler EditButton.Click, Sub(s, e) RaiseEvent EditClicked(Me, EventArgs.Empty)
            AddHandler DeleteButton.Click, Sub(s, e) RaiseEvent DeleteClicked(Me, EventArgs.Empty)
            AddHandler SaveEditButton.Click, Sub(s, e) RaiseEvent SaveClicked(Me, EventArgs.Empty)
            AddHandler CancelEditButton.Click, Sub(s, e) RaiseEvent CancelClicked(Me, EventArgs.Empty)
            AddHandler VendorGrid.SelectionChanged, Sub(s, e) RaiseEvent SelectionChanged(Me, EventArgs.Empty)
            AddHandler VendorGrid.CellDoubleClick, Sub(s, e) RaiseEvent EditClicked(Me, EventArgs.Empty)
        End Sub

        Private Sub SearchTextBox_KeyDown(sender As Object, e As KeyEventArgs)
            If e.KeyCode = Keys.Escape Then
                SearchTextBox.Text = String.Empty
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End Sub

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property Vendors As List(Of VendorDetailDto) Implements IVendorDirectoryView.Vendors
            Get
                Return TryCast(VendorGrid.DataSource, List(Of VendorDetailDto))
            End Get
            Set(value As List(Of VendorDetailDto))
                VendorGrid.DataSource = Nothing
                VendorGrid.DataSource = value
                If VendorGrid.Columns.Contains("IsDeleted") Then
                    VendorGrid.Columns("IsDeleted").Visible = False
                End If
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SelectedVendor As VendorDetailDto Implements IVendorDirectoryView.SelectedVendor
            Get
                If VendorGrid.SelectedRows.Count > 0 Then
                    Return TryCast(VendorGrid.SelectedRows(0).DataBoundItem, VendorDetailDto)
                End If
                Return Nothing
            End Get
            Set(value As VendorDetailDto)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property PurchaseHistory As VendorPurchaseHistoryDto Implements IVendorDirectoryView.PurchaseHistory
            Get
                Return Nothing
            End Get
            Set(value As VendorPurchaseHistoryDto)
                If value IsNot Nothing Then
                    StatsLabel.Text = $"Total Orders: {value.TotalOrders}" & vbCrLf &
                                      $"Total Amount: {value.TotalAmount:C2}" & vbCrLf &
                                      $"Last Order: {If(value.LastOrderDate.HasValue, value.LastOrderDate.Value.ToString("g"), "N/A")}"
                Else
                    StatsLabel.Text = "Select a vendor to view stats."
                End If
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property IsEditorOpen As Boolean Implements IVendorDirectoryView.IsEditorOpen
            Get
                Return EditorPanel.Visible
            End Get
            Set(value As Boolean)
                EditorPanel.Visible = value
                If value Then
                    NameTextBox.Focus()
                End If
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property EditorTitle As String Implements IVendorDirectoryView.EditorTitle
            Get
                Return EditorTitleLabel.Text
            End Get
            Set(value As String)
                EditorTitleLabel.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property EditorName As String Implements IVendorDirectoryView.EditorName
            Get
                Return NameTextBox.Text
            End Get
            Set(value As String)
                NameTextBox.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property EditorPhone As String Implements IVendorDirectoryView.EditorPhone
            Get
                Return PhoneTextBox.Text
            End Get
            Set(value As String)
                PhoneTextBox.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property EditorEmail As String Implements IVendorDirectoryView.EditorEmail
            Get
                Return EmailTextBox.Text
            End Get
            Set(value As String)
                EmailTextBox.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property EditorContactPerson As String Implements IVendorDirectoryView.EditorContactPerson
            Get
                Return ContactTextBox.Text
            End Get
            Set(value As String)
                ContactTextBox.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property EditorAddress As String Implements IVendorDirectoryView.EditorAddress
            Get
                Return AddressTextBox.Text
            End Get
            Set(value As String)
                AddressTextBox.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property EditorTaxId As String Implements IVendorDirectoryView.EditorTaxId
            Get
                Return TaxIdTextBox.Text
            End Get
            Set(value As String)
                TaxIdTextBox.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property EditorNotes As String Implements IVendorDirectoryView.EditorNotes
            Get
                Return NotesTextBox.Text
            End Get
            Set(value As String)
                NotesTextBox.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property EditorLeadTimeDays As Integer Implements IVendorDirectoryView.EditorLeadTimeDays
            Get
                Dim result As Integer
                If Integer.TryParse(LeadTimeTextBox.Text, result) Then
                    Return result
                End If
                Return 0
            End Get
            Set(value As Integer)
                LeadTimeTextBox.Text = value.ToString()
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SearchTerm As String Implements IVendorDirectoryView.SearchTerm
            Get
                Return SearchTextBox.Text
            End Get
            Set(value As String)
                SearchTextBox.Text = value
            End Set
        End Property

        Public Sub ShowError(message As String) Implements IVendorDirectoryView.ShowError
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Sub

        Public Sub ShowMessage(message As String) Implements IVendorDirectoryView.ShowMessage
            MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Sub

        Public Function ConfirmDelete(vendorName As String) As Boolean Implements IVendorDirectoryView.ConfirmDelete
            Dim result = MessageBox.Show($"Are you sure you want to delete vendor '{vendorName}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
            Return result = DialogResult.Yes
        End Function
    End Class
End Namespace
INNER

cat << 'INNER' > src/MerchSys.Purchasing/Extensions/PurchasingServiceCollectionExtensions.vb
Imports Microsoft.Extensions.DependencyInjection
Imports System.Runtime.CompilerServices
Imports MerchSys.Purchasing.Services
Imports MerchSys.Purchasing.Views
Imports MerchSys.Purchasing.Views.Dialogs
Imports MerchSys.Purchasing.Presenters

Namespace Extensions
    Public Module PurchasingServiceCollectionExtensions
        <Extension()>
        Public Sub AddPurchasingServices(services As IServiceCollection)
            services.AddScoped(Of IPurchaseOrderService, PurchaseOrderService)()
            services.AddScoped(Of IPriceChangeService, PriceChangeService)()
            services.AddScoped(Of IGoodsReceivingService, GoodsReceivingService)()
            services.AddScoped(Of IVendorService, VendorService)()
            services.AddScoped(Of IAccountsPayableService, AccountsPayableService)()
            services.AddScoped(Of IReorderService, ReorderService)()

            ' Views
            services.AddTransient(Of IPurchaseOrderListView, PurchaseOrderListView)()
            services.AddTransient(Of IPurchaseOrderEditorView, PurchaseOrderEditorDialog)()
            services.AddTransient(Of IGoodsReceivingView, GoodsReceivingView)()
            services.AddTransient(Of IVendorDirectoryView, VendorDirectoryView)()

            ' Presenters
            services.AddTransient(Of PurchaseOrderListPresenter)()
            services.AddTransient(Of PurchaseOrderEditorPresenter)()
            services.AddTransient(Of GoodsReceivingPresenter)()
            services.AddTransient(Of VendorEditorPresenter)()
            services.AddTransient(Of VendorListPresenter)()
        End Sub
    End Module
End Namespace
INNER
