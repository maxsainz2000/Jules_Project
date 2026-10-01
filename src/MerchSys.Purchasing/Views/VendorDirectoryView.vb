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
