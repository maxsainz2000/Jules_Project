Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Windows.Forms
Imports MerchSys.Purchasing.Entities
Imports MerchSys.Purchasing.Services

Namespace Views.Dialogs
    Public Class PurchaseOrderEditorView
        Implements IPurchaseOrderEditorView

        Private _lines As BindingList(Of PurchaseOrderLine)
        Private _presenter As Presenters.PurchaseOrders.PurchaseOrderEditorPresenter

        Public Sub New()
            InitializeComponent()
            _lines = New BindingList(Of PurchaseOrderLine)()

            dgvLines.AutoGenerateColumns = False
            dgvLines.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "ProductName", .HeaderText = "Product Name", .Width = 200})
            dgvLines.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "Quantity", .HeaderText = "Qty", .Width = 80})
            dgvLines.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "UnitCost", .HeaderText = "Unit Cost", .Width = 100, .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "₱0.00"}})
            dgvLines.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "LineTotal", .HeaderText = "Total", .Width = 100, .ReadOnly = True, .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "₱0.00"}})

            dgvLines.DataSource = _lines
        End Sub

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property Vendors As List(Of VendorDetailDto) Implements IPurchaseOrderEditorView.Vendors
            Get
                Return CType(cmbVendors.DataSource, List(Of VendorDetailDto))
            End Get
            Set(value As List(Of VendorDetailDto))
                cmbVendors.DataSource = value
                cmbVendors.DisplayMember = "Name"
                cmbVendors.ValueMember = "Id"
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SelectedVendorId As Integer? Implements IPurchaseOrderEditorView.SelectedVendorId
            Get
                If cmbVendors.SelectedValue IsNot Nothing Then
                    Return CInt(cmbVendors.SelectedValue)
                End If
                Return Nothing
            End Get
            Set(value As Integer?)
                cmbVendors.SelectedValue = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property Notes As String Implements IPurchaseOrderEditorView.Notes
            Get
                Return txtNotes.Text
            End Get
            Set(value As String)
                txtNotes.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ExpectedDeliveryDate As DateTime? Implements IPurchaseOrderEditorView.ExpectedDeliveryDate
            Get
                If dtpExpectedDate.Checked Then
                    Return dtpExpectedDate.Value
                End If
                Return Nothing
            End Get
            Set(value As DateTime?)
                If value.HasValue Then
                    dtpExpectedDate.Value = value.Value
                    dtpExpectedDate.Checked = True
                Else
                    dtpExpectedDate.Checked = False
                End If
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property Lines As List(Of PurchaseOrderLine) Implements IPurchaseOrderEditorView.Lines
            Get
                Return New List(Of PurchaseOrderLine)(_lines)
            End Get
            Set(value As List(Of PurchaseOrderLine))
                _lines.Clear()
                If value IsNot Nothing Then
                    For Each item In value
                        _lines.Add(item)
                    Next
                End If
                UpdateRunningTotal()
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property RunningTotal As Decimal Implements IPurchaseOrderEditorView.RunningTotal
            Get
                Dim total As Decimal = 0
                For Each line In _lines
                    total += line.LineTotal
                Next
                Return total
            End Get
            Set(value As Decimal)
                lblRunningTotal.Text = String.Format("₱{0:N2}", value)
            End Set
        End Property

        Public Sub SetPresenter(presenter As Presenters.PurchaseOrders.PurchaseOrderEditorPresenter) Implements IPurchaseOrderEditorView.SetPresenter
            _presenter = presenter
        End Sub

        Public Shadows Function ShowDialog(owner As IWin32Window) As DialogResult Implements IPurchaseOrderEditorView.ShowDialog
            Return MyBase.ShowDialog(owner)
        End Function

        Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
        End Sub

        Private Async Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
            If _presenter IsNot Nothing Then
                btnSave.Enabled = False
                Await _presenter.SaveAsync()
                Me.DialogResult = DialogResult.OK
                Me.Close()
            End If
        End Sub

        Private Sub btnAddLine_Click(sender As Object, e As EventArgs) Handles btnAddLine.Click
            Dim newLine As New PurchaseOrderLine With {
                .ProductName = "New Item",
                .Quantity = 1,
                .UnitCost = 0
            }
            _lines.Add(newLine)
            UpdateRunningTotal()
        End Sub

        Private Sub btnRemoveLine_Click(sender As Object, e As EventArgs) Handles btnRemoveLine.Click
            If dgvLines.SelectedRows.Count > 0 Then
                Dim selectedLine = CType(dgvLines.SelectedRows(0).DataBoundItem, PurchaseOrderLine)
                _lines.Remove(selectedLine)
                UpdateRunningTotal()
            End If
        End Sub

        Private Sub dgvLines_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvLines.CellValueChanged
            If e.RowIndex >= 0 Then
                Dim line = _lines(e.RowIndex)
                line.LineTotal = line.Quantity * line.UnitCost
                dgvLines.InvalidateRow(e.RowIndex)
                UpdateRunningTotal()
            End If
        End Sub

        Private Sub UpdateRunningTotal()
            Dim total As Decimal = 0
            For Each line In _lines
                total += line.LineTotal
            Next
            RunningTotal = total
        End Sub
    End Class
End Namespace