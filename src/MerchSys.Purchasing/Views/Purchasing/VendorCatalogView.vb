Imports System.Windows.Forms
Imports System.Collections
Imports System.Threading.Tasks
Imports System.ComponentModel

Namespace Views.Purchasing
    Public Class VendorCatalogView
        Inherits UserControl
        Implements IVendorCatalogView

        Public Sub New()
            InitializeComponent()

            pnlEditor.Visible = False
        End Sub

        Public Sub New(presenter As MerchSys.Purchasing.Presenters.VendorCatalogPresenter)
            Me.New()
            presenter.View = Me

            ' Fire and forget load vendors
            Dim dummy = presenter.LoadVendorsAsync()

            AddHandler dgvVendors.SelectionChanged, AddressOf OnDgvVendorsSelectionChanged
            AddHandler btnAdd.Click, Async Sub(s, e) Await HandleAddAsync()
            AddHandler btnEdit.Click, Async Sub(s, e) Await HandleEditAsync()
            AddHandler btnSave.Click, Async Sub(s, e) Await HandleSaveAsync()
            AddHandler btnDelete.Click, Async Sub(s, e) Await HandleDeleteAsync()
        End Sub

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SelectedVendorId As Integer? Implements IVendorCatalogView.SelectedVendorId
            Get
                If dgvVendors.SelectedRows.Count > 0 Then
                    Return CInt(dgvVendors.SelectedRows(0).Cells("Id").Value)
                End If
                Return Nothing
            End Get
            Set(value As Integer?)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SelectedVendorProductId As Integer? Implements IVendorCatalogView.SelectedVendorProductId
            Get
                If dgvProducts.SelectedRows.Count > 0 Then
                    Return CInt(dgvProducts.SelectedRows(0).Cells("Id").Value)
                End If
                Return Nothing
            End Get
            Set(value As Integer?)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ProductIdInput As Integer Implements IVendorCatalogView.ProductIdInput
            Get
                Return CInt(numProductId.Value)
            End Get
            Set(value As Integer)
                numProductId.Value = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property UnitCostInput As Decimal Implements IVendorCatalogView.UnitCostInput
            Get
                Return numUnitCost.Value
            End Get
            Set(value As Decimal)
                numUnitCost.Value = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property NotesInput As String Implements IVendorCatalogView.NotesInput
            Get
                Return txtNotes.Text
            End Get
            Set(value As String)
                txtNotes.Text = value
            End Set
        End Property

        Public Sub BindVendors(items As IEnumerable) Implements IVendorCatalogView.BindVendors
            If dgvVendors.Columns.Count = 0 Then
                dgvVendors.Columns.Add(New DataGridViewTextBoxColumn With { .Name = "Id", .DataPropertyName = "Id", .HeaderText = "ID", .Width = 50 })
                dgvVendors.Columns.Add(New DataGridViewTextBoxColumn With { .Name = "Name", .DataPropertyName = "Name", .HeaderText = "Name", .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill })
            End If

            ' Unhook to prevent premature events
            RemoveHandler dgvVendors.SelectionChanged, AddressOf OnDgvVendorsSelectionChanged

            dgvVendors.DataSource = items

            AddHandler dgvVendors.SelectionChanged, AddressOf OnDgvVendorsSelectionChanged

            ' Trigger manually for first item
            If dgvVendors.Rows.Count > 0 Then
                OnDgvVendorsSelectionChanged(Nothing, Nothing)
            End If
        End Sub

        Public Sub BindVendorProducts(items As IEnumerable) Implements IVendorCatalogView.BindVendorProducts
            If dgvProducts.Columns.Count = 0 Then
                dgvProducts.Columns.Add(New DataGridViewTextBoxColumn With { .Name = "Id", .DataPropertyName = "Id", .HeaderText = "ID", .Visible = False })
                dgvProducts.Columns.Add(New DataGridViewTextBoxColumn With { .Name = "ProductId", .DataPropertyName = "ProductId", .HeaderText = "Product ID", .Width = 80 })
                dgvProducts.Columns.Add(New DataGridViewTextBoxColumn With { .Name = "UnitCost", .DataPropertyName = "UnitCost", .HeaderText = "Unit Cost", .DefaultCellStyle = New DataGridViewCellStyle With { .Format = "₱{0:N2}" } })
                dgvProducts.Columns.Add(New DataGridViewTextBoxColumn With { .Name = "Notes", .DataPropertyName = "Notes", .HeaderText = "Notes", .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill })
            End If
            dgvProducts.DataSource = items
            pnlEditor.Visible = False
        End Sub

        Public Sub ShowError(message As String) Implements IVendorCatalogView.ShowError
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Sub

        Public Function ConfirmAction(message As String, title As String) As Boolean Implements IVendorCatalogView.ConfirmAction
            Return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes
        End Function

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property OnAdd As Func(Of Task) Implements IVendorCatalogView.OnAdd

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property OnEdit As Func(Of Task) Implements IVendorCatalogView.OnEdit

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property OnSave As Func(Of Task) Implements IVendorCatalogView.OnSave

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property OnDelete As Func(Of Task) Implements IVendorCatalogView.OnDelete

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property OnVendorSelected As Func(Of Task) Implements IVendorCatalogView.OnVendorSelected

        Private Async Sub OnDgvVendorsSelectionChanged(sender As Object, e As EventArgs)
            If OnVendorSelected IsNot Nothing Then
                Await OnVendorSelected.Invoke()
            End If
        End Sub

        Private Async Function HandleAddAsync() As Task
            numProductId.Value = 0
            numProductId.Enabled = True
            numUnitCost.Value = 0
            txtNotes.Text = ""
            pnlEditor.Visible = True
            If OnAdd IsNot Nothing Then
                Await OnAdd.Invoke()
            End If
        End Function

        Private Async Function HandleEditAsync() As Task
            If dgvProducts.SelectedRows.Count = 0 Then
                ShowError("Please select a vendor product to edit.")
                Return
            End If

            Dim row = dgvProducts.SelectedRows(0)
            numProductId.Value = CDec(row.Cells("ProductId").Value)
            numProductId.Enabled = False ' Do not allow changing ProductId on edit
            numUnitCost.Value = CDec(row.Cells("UnitCost").Value)
            txtNotes.Text = If(row.Cells("Notes").Value?.ToString(), "")

            pnlEditor.Visible = True

            If OnEdit IsNot Nothing Then
                Await OnEdit.Invoke()
            End If
        End Function

        Private Async Function HandleSaveAsync() As Task
            If OnSave IsNot Nothing Then
                Await OnSave.Invoke()
            End If
        End Function

        Private Async Function HandleDeleteAsync() As Task
            If OnDelete IsNot Nothing Then
                Await OnDelete.Invoke()
            End If
        End Function
    End Class
End Namespace
