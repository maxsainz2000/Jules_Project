Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq
Imports System.Windows.Forms
Imports MerchSys.Purchasing.Entities
Imports MerchSys.Purchasing.Services
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.Purchasing.Presenters

Namespace Views
    Public Partial Class PurchaseOrderListView
        Implements IPurchaseOrderListView, IPurchaseOrderEditorView

        Private _listPresenter As PurchaseOrderListPresenter
        Private _editorPresenter As PurchaseOrderEditorPresenter

        Public Sub New()
            InitializeComponent()
            SetupDataGridViews()
            PopulateStatusFilter()
        End Sub

        Public Sub SetPresenters(listPresenter As PurchaseOrderListPresenter, editorPresenter As PurchaseOrderEditorPresenter)
            _listPresenter = listPresenter
            _editorPresenter = editorPresenter
        End Sub

        Private Sub SetupDataGridViews()
            dgvOrders.AutoGenerateColumns = False
            dgvOrders.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "Id", .HeaderText = "ID", .Visible = False})
            dgvOrders.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "OrderNumber", .HeaderText = "Order Number", .Width = 120})
            dgvOrders.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "ExpectedDeliveryDate", .HeaderText = "Expected Delivery", .Width = 150})
            dgvOrders.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "Status", .HeaderText = "Status", .Width = 100})
            dgvOrders.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "TotalAmount", .HeaderText = "Total Amount", .Width = 120, .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "₱0.00"}})

            dgvLines.AutoGenerateColumns = False
            dgvLines.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "Id", .HeaderText = "ID", .Visible = False})
            dgvLines.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "ProductName", .HeaderText = "Product Name", .Width = 150})
            dgvLines.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "Quantity", .HeaderText = "Quantity", .Width = 80})
            dgvLines.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "UnitCost", .HeaderText = "Unit Cost", .Width = 100, .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "₱0.00"}})
            dgvLines.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "LineTotal", .HeaderText = "Total", .Width = 100, .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "₱0.00"}})

            Dim btnRemove As New DataGridViewButtonColumn()
            btnRemove.HeaderText = "Action"
            btnRemove.Text = "Remove"
            btnRemove.UseColumnTextForButtonValue = True
            btnRemove.Width = 80
            dgvLines.Columns.Add(btnRemove)
        End Sub

        Private Sub PopulateStatusFilter()
            Dim statuses = [Enum].GetValues(GetType(PurchaseOrderStatus)).Cast(Of PurchaseOrderStatus)().Select(Function(s) New With {.Text = s.ToString(), .Value = s}).ToList()
            statuses.Insert(0, New With {.Text = "All", .Value = CType(-1, PurchaseOrderStatus)})

            cmbStatusFilter.DisplayMember = "Text"
            cmbStatusFilter.ValueMember = "Value"
            cmbStatusFilter.DataSource = statuses
        End Sub

        ' --- IPurchaseOrderListView Properties ---
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property StatusFilter As PurchaseOrderStatus? Implements IPurchaseOrderListView.StatusFilter
            Get
                If cmbStatusFilter.SelectedIndex > 0 Then
                    Return CType(cmbStatusFilter.SelectedValue, PurchaseOrderStatus)
                End If
                Return Nothing
            End Get
            Set(value As PurchaseOrderStatus?)
                If value.HasValue Then
                    cmbStatusFilter.SelectedValue = value.Value
                Else
                    cmbStatusFilter.SelectedIndex = 0
                End If
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property SearchTerm As String Implements IPurchaseOrderListView.SearchTerm
            Get
                Return txtSearch.Text.Trim()
            End Get
            Set(value As String)
                txtSearch.Text = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property SelectedPurchaseOrderId As Integer? Implements IPurchaseOrderListView.SelectedPurchaseOrderId
            Get
                If dgvOrders.SelectedRows.Count > 0 Then
                    Return Convert.ToInt32(dgvOrders.SelectedRows(0).Cells(0).Value)
                End If
                Return Nothing
            End Get
            Set(value As Integer?)
                ' Not implemented for UI setting yet
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Private _isManager As Boolean
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property IsManager As Boolean Implements IPurchaseOrderListView.IsManager
            Get
                Return _isManager
            End Get
            Set(value As Boolean)
                _isManager = value
                btnNew.Visible = value
                btnSubmitPO.Visible = value
                btnCancelPO.Visible = value
                btnEdit.Visible = value
            End Set
        End Property

        ' --- IPurchaseOrderEditorView Properties ---
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property SelectedVendorId As Integer? Implements IPurchaseOrderEditorView.SelectedVendorId
            Get
                If cmbVendor.SelectedValue IsNot Nothing Then
                    Return Convert.ToInt32(cmbVendor.SelectedValue)
                End If
                Return Nothing
            End Get
            Set(value As Integer?)
                If value.HasValue Then
                    cmbVendor.SelectedValue = value.Value
                Else
                    cmbVendor.SelectedIndex = -1
                End If
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property ExpectedDeliveryDate As DateTime? Implements IPurchaseOrderEditorView.ExpectedDeliveryDate
            Get
                Return If(dtpExpectedDelivery.Checked, dtpExpectedDelivery.Value, CType(Nothing, DateTime?))
            End Get
            Set(value As DateTime?)
                If value.HasValue Then
                    dtpExpectedDelivery.Value = value.Value
                    dtpExpectedDelivery.Checked = True
                Else
                    dtpExpectedDelivery.Checked = False
                End If
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property Notes As String Implements IPurchaseOrderEditorView.Notes
            Get
                Return txtNotes.Text
            End Get
            Set(value As String)
                txtNotes.Text = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property TotalAmountText As String Implements IPurchaseOrderEditorView.TotalAmountText
            Get
                Return lblTotal.Text
            End Get
            Set(value As String)
                lblTotal.Text = value
            End Set
        End Property

        ' --- View Methods ---
        Public Sub BindPurchaseOrders(orders As List(Of PurchaseOrder)) Implements IPurchaseOrderListView.BindPurchaseOrders
            dgvOrders.DataSource = orders
            ApplyRowColors()
        End Sub

        Private Sub ApplyRowColors()
            For Each row As DataGridViewRow In dgvOrders.Rows
                Dim status = CType(row.Cells(3).Value, PurchaseOrderStatus)
                Select Case status
                    Case PurchaseOrderStatus.Draft
                        row.DefaultCellStyle.BackColor = Color.LightGray
                    Case PurchaseOrderStatus.Pending
                        row.DefaultCellStyle.BackColor = Color.LightYellow
                    Case PurchaseOrderStatus.Approved
                        row.DefaultCellStyle.BackColor = Color.LightGreen
                    Case PurchaseOrderStatus.Closed
                        row.DefaultCellStyle.BackColor = Color.LightBlue
                End Select
            Next
        End Sub

        Public Sub BindVendors(vendors As List(Of VendorDetailDto)) Implements IPurchaseOrderEditorView.BindVendors
            cmbVendor.DisplayMember = "Name"
            cmbVendor.ValueMember = "Id"
            cmbVendor.DataSource = vendors
        End Sub

        Public Sub BindLineItems(lines As List(Of PurchaseOrderLine)) Implements IPurchaseOrderEditorView.BindLineItems
            dgvLines.DataSource = Nothing
            dgvLines.DataSource = lines
        End Sub

        Public Sub SetEditorVisible(visible As Boolean) Implements IPurchaseOrderEditorView.SetEditorVisible
            PanelEditor.Visible = visible
            PanelTop.Enabled = Not visible
            dgvOrders.Enabled = Not visible
        End Sub

        Public Sub SetEditorEnabled(enabled As Boolean) Implements IPurchaseOrderEditorView.SetEditorEnabled
            cmbVendor.Enabled = enabled
            dtpExpectedDelivery.Enabled = enabled
            txtNotes.Enabled = enabled
            btnAddLine.Enabled = enabled
            txtProductName.Enabled = enabled
            numUnitCost.Enabled = enabled
            numQuantity.Enabled = enabled
            btnSaveDraft.Enabled = enabled
            dgvLines.Columns(5).Visible = enabled ' Action column
        End Sub

        Public Sub ShowMessage(message As String) Implements IPurchaseOrderListView.ShowMessage, IPurchaseOrderEditorView.ShowMessage
            MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Sub

        Public Sub ShowError(errorMessage As String) Implements IPurchaseOrderListView.ShowError, IPurchaseOrderEditorView.ShowError
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Sub

        Public Function ConfirmAction(message As String) As Boolean Implements IPurchaseOrderListView.ConfirmAction, IPurchaseOrderEditorView.ConfirmAction
            Return MessageBox.Show(message, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes
        End Function

        ' --- UI Event Handlers ---
        Private Sub cmbStatusFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbStatusFilter.SelectedIndexChanged
            If _listPresenter IsNot Nothing Then _listPresenter.ApplyFilters()
        End Sub

        Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
            If _listPresenter IsNot Nothing Then _listPresenter.ApplyFilters()
        End Sub

        Private Sub btnNew_Click(sender As Object, e As EventArgs) Handles btnNew.Click
            If _listPresenter IsNot Nothing Then _listPresenter.PrepareNewPO()
        End Sub

        Private Async Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
            If _listPresenter IsNot Nothing Then Await _listPresenter.EditSelectedPOAsync()
        End Sub

        Private Async Sub dgvOrders_DoubleClick(sender As Object, e As EventArgs) Handles dgvOrders.DoubleClick
            If _listPresenter IsNot Nothing Then Await _listPresenter.EditSelectedPOAsync()
        End Sub

        Private Async Sub btnSubmitPO_Click(sender As Object, e As EventArgs) Handles btnSubmitPO.Click
            If _listPresenter IsNot Nothing Then Await _listPresenter.SubmitPOAsync()
        End Sub

        Private Async Sub btnCancelPO_Click(sender As Object, e As EventArgs) Handles btnCancelPO.Click
            If _listPresenter IsNot Nothing Then Await _listPresenter.CancelPOAsync()
        End Sub

        Private Async Sub btnSaveDraft_Click(sender As Object, e As EventArgs) Handles btnSaveDraft.Click
            If _editorPresenter IsNot Nothing Then
                Dim success = Await _editorPresenter.SaveDraftAsync()
                If success AndAlso _listPresenter IsNot Nothing Then
                    Await _listPresenter.LoadPurchaseOrdersAsync()
                    _editorPresenter.ClearEditor()
                End If
            End If
        End Sub

        Private Sub btnCancelEditor_Click(sender As Object, e As EventArgs) Handles btnCancelEditor.Click
            If _editorPresenter IsNot Nothing Then _editorPresenter.ClearEditor()
        End Sub

        Private Async Sub btnAddLine_Click(sender As Object, e As EventArgs) Handles btnAddLine.Click
            If _editorPresenter IsNot Nothing Then
                Dim pName = txtProductName.Text.Trim()
                Dim uCost = numUnitCost.Value
                Dim qty = Convert.ToInt32(numQuantity.Value)

                If String.IsNullOrEmpty(pName) OrElse qty <= 0 Then
                    ShowError("Product name and valid quantity are required.")
                    Return
                End If

                Await _editorPresenter.AddLineAsync(pName, uCost, qty)
                txtProductName.Clear()
                numUnitCost.Value = 0
                numQuantity.Value = 0
            End If
        End Sub

        Private Async Sub dgvLines_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvLines.CellClick
            If e.RowIndex >= 0 AndAlso e.ColumnIndex = 5 Then ' Remove Button
                If _editorPresenter IsNot Nothing Then
                    Dim lineId = Convert.ToInt32(dgvLines.Rows(e.RowIndex).Cells(0).Value)
                    Await _editorPresenter.RemoveLineAsync(lineId)
                End If
            End If
        End Sub
    End Class
End Namespace
