Imports System.Windows.Forms
Imports MerchSys.Purchasing.Entities

Namespace Views.Shell.Modules.Purchasing
    Public Class PurchaseOrderListView
        Inherits System.Windows.Forms.UserControl
        Implements IPurchaseOrderListView

        Private _presenter As PurchaseOrderListPresenter

        Private dgvOrders As System.Windows.Forms.DataGridView
        Private btnNew As System.Windows.Forms.Button
        Private btnEdit As System.Windows.Forms.Button
        Private btnSubmit As System.Windows.Forms.Button
        Private btnDelete As System.Windows.Forms.Button
        Private txtSearch As System.Windows.Forms.TextBox
        Private cboStatusFilter As System.Windows.Forms.ComboBox
        Private btnRefresh As System.Windows.Forms.Button
        Private lblSearch As System.Windows.Forms.Label
        Private lblStatus As System.Windows.Forms.Label
        Private colOrderNumber As System.Windows.Forms.DataGridViewTextBoxColumn
        Private colVendor As System.Windows.Forms.DataGridViewTextBoxColumn
        Private colStatus As System.Windows.Forms.DataGridViewTextBoxColumn
        Private colOrderDate As System.Windows.Forms.DataGridViewTextBoxColumn
        Private colTotalAmount As System.Windows.Forms.DataGridViewTextBoxColumn

        Public Sub New()
            InitializeComponentLocal()
        End Sub

        Private Sub InitializeComponentLocal()
            Me.dgvOrders = New System.Windows.Forms.DataGridView()
            Me.btnNew = New System.Windows.Forms.Button()
            Me.btnEdit = New System.Windows.Forms.Button()
            Me.btnSubmit = New System.Windows.Forms.Button()
            Me.btnDelete = New System.Windows.Forms.Button()
            Me.txtSearch = New System.Windows.Forms.TextBox()
            Me.cboStatusFilter = New System.Windows.Forms.ComboBox()
            Me.btnRefresh = New System.Windows.Forms.Button()
            Me.lblSearch = New System.Windows.Forms.Label()
            Me.lblStatus = New System.Windows.Forms.Label()

            Me.colOrderNumber = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.colVendor = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.colStatus = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.colOrderDate = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.colTotalAmount = New System.Windows.Forms.DataGridViewTextBoxColumn()

            CType(Me.dgvOrders, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()

            Me.dgvOrders.AllowUserToAddRows = False
            Me.dgvOrders.AllowUserToDeleteRows = False
            Me.dgvOrders.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.dgvOrders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvOrders.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colOrderNumber, Me.colVendor, Me.colStatus, Me.colOrderDate, Me.colTotalAmount})
            Me.dgvOrders.Location = New System.Drawing.Point(14, 53)
            Me.dgvOrders.Name = "dgvOrders"
            Me.dgvOrders.ReadOnly = True
            Me.dgvOrders.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
            Me.dgvOrders.Size = New System.Drawing.Size(773, 335)
            Me.dgvOrders.TabIndex = 0

            Me.colOrderNumber.DataPropertyName = "OrderNumber"
            Me.colOrderNumber.HeaderText = "Order Number"
            Me.colOrderNumber.Name = "colOrderNumber"
            Me.colOrderNumber.ReadOnly = True

            Me.colVendor.DataPropertyName = "VendorId"
            Me.colVendor.HeaderText = "Vendor ID"
            Me.colVendor.Name = "colVendor"
            Me.colVendor.ReadOnly = True

            Me.colStatus.DataPropertyName = "Status"
            Me.colStatus.HeaderText = "Status"
            Me.colStatus.Name = "colStatus"
            Me.colStatus.ReadOnly = True

            Me.colOrderDate.DataPropertyName = "CreatedAt"
            Me.colOrderDate.HeaderText = "Date"
            Me.colOrderDate.Name = "colOrderDate"
            Me.colOrderDate.ReadOnly = True

            Me.colTotalAmount.DataPropertyName = "TotalAmount"
            Me.colTotalAmount.HeaderText = "Total"
            Me.colTotalAmount.Name = "colTotalAmount"
            Me.colTotalAmount.ReadOnly = True

            Me.btnNew.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.btnNew.Location = New System.Drawing.Point(14, 404)
            Me.btnNew.Name = "btnNew"
            Me.btnNew.Size = New System.Drawing.Size(75, 23)
            Me.btnNew.TabIndex = 1
            Me.btnNew.Text = "New PO"
            Me.btnNew.UseVisualStyleBackColor = True

            Me.btnEdit.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.btnEdit.Location = New System.Drawing.Point(95, 404)
            Me.btnEdit.Name = "btnEdit"
            Me.btnEdit.Size = New System.Drawing.Size(75, 23)
            Me.btnEdit.TabIndex = 2
            Me.btnEdit.Text = "Edit"
            Me.btnEdit.UseVisualStyleBackColor = True

            Me.btnSubmit.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.btnSubmit.Location = New System.Drawing.Point(176, 404)
            Me.btnSubmit.Name = "btnSubmit"
            Me.btnSubmit.Size = New System.Drawing.Size(75, 23)
            Me.btnSubmit.TabIndex = 3
            Me.btnSubmit.Text = "Submit"
            Me.btnSubmit.UseVisualStyleBackColor = True

            Me.btnDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.btnDelete.Location = New System.Drawing.Point(257, 404)
            Me.btnDelete.Name = "btnDelete"
            Me.btnDelete.Size = New System.Drawing.Size(75, 23)
            Me.btnDelete.TabIndex = 4
            Me.btnDelete.Text = "Delete"
            Me.btnDelete.UseVisualStyleBackColor = True

            Me.txtSearch.Location = New System.Drawing.Point(63, 14)
            Me.txtSearch.Name = "txtSearch"
            Me.txtSearch.Size = New System.Drawing.Size(200, 23)
            Me.txtSearch.TabIndex = 5

            Me.cboStatusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cboStatusFilter.FormattingEnabled = True
            Me.cboStatusFilter.Location = New System.Drawing.Point(324, 14)
            Me.cboStatusFilter.Name = "cboStatusFilter"
            Me.cboStatusFilter.Size = New System.Drawing.Size(121, 23)
            Me.cboStatusFilter.TabIndex = 6

            Me.btnRefresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnRefresh.Location = New System.Drawing.Point(712, 13)
            Me.btnRefresh.Name = "btnRefresh"
            Me.btnRefresh.Size = New System.Drawing.Size(75, 23)
            Me.btnRefresh.TabIndex = 7
            Me.btnRefresh.Text = "Refresh"
            Me.btnRefresh.UseVisualStyleBackColor = True

            Me.lblSearch.AutoSize = True
            Me.lblSearch.Location = New System.Drawing.Point(14, 17)
            Me.lblSearch.Name = "lblSearch"
            Me.lblSearch.Size = New System.Drawing.Size(45, 15)
            Me.lblSearch.TabIndex = 8
            Me.lblSearch.Text = "Search:"

            Me.lblStatus.AutoSize = True
            Me.lblStatus.Location = New System.Drawing.Point(279, 17)
            Me.lblStatus.Name = "lblStatus"
            Me.lblStatus.Size = New System.Drawing.Size(42, 15)
            Me.lblStatus.TabIndex = 9
            Me.lblStatus.Text = "Status:"

            Me.Controls.Add(Me.lblStatus)
            Me.Controls.Add(Me.lblSearch)
            Me.Controls.Add(Me.btnRefresh)
            Me.Controls.Add(Me.cboStatusFilter)
            Me.Controls.Add(Me.txtSearch)
            Me.Controls.Add(Me.btnDelete)
            Me.Controls.Add(Me.btnSubmit)
            Me.Controls.Add(Me.btnEdit)
            Me.Controls.Add(Me.btnNew)
            Me.Controls.Add(Me.dgvOrders)
            Me.Name = "PurchaseOrderListView"
            Me.Size = New System.Drawing.Size(800, 442)
            CType(Me.dgvOrders, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

            AddHandler btnNew.Click, Sub(sender, e) RaiseEvent CreateRequested(sender, e)
            AddHandler btnEdit.Click, Sub(sender, e) RaiseEvent EditRequested(sender, e)
            AddHandler btnSubmit.Click, Sub(sender, e) RaiseEvent SubmitRequested(sender, e)
            AddHandler btnDelete.Click, Sub(sender, e) RaiseEvent DeleteRequested(sender, e)
            AddHandler btnRefresh.Click, Sub(sender, e) RaiseEvent RefreshRequested(sender, e)
            AddHandler dgvOrders.DoubleClick, Sub(sender, e) RaiseEvent EditRequested(sender, e)
        End Sub

        Public Event RefreshRequested As EventHandler Implements IPurchaseOrderListView.RefreshRequested
        Public Event CreateRequested As EventHandler Implements IPurchaseOrderListView.CreateRequested
        Public Event EditRequested As EventHandler Implements IPurchaseOrderListView.EditRequested
        Public Event SubmitRequested As EventHandler Implements IPurchaseOrderListView.SubmitRequested
        Public Event DeleteRequested As EventHandler Implements IPurchaseOrderListView.DeleteRequested

        Public Sub AttachPresenter(presenter As PurchaseOrderListPresenter) Implements IPurchaseOrderListView.AttachPresenter
            _presenter = presenter
        End Sub

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property SearchText As String Implements IPurchaseOrderListView.SearchText
            Get
                Return txtSearch.Text
            End Get
            Set(value As String)
                txtSearch.Text = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property StatusFilter As String Implements IPurchaseOrderListView.StatusFilter
            Get
                Return If(cboStatusFilter.SelectedItem?.ToString(), "All")
            End Get
            Set(value As String)
                cboStatusFilter.SelectedItem = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property SelectedOrderId As Integer? Implements IPurchaseOrderListView.SelectedOrderId
            Get
                If dgvOrders.SelectedRows.Count > 0 Then
                    Dim order = TryCast(dgvOrders.SelectedRows(0).DataBoundItem, PurchaseOrder)
                    If order IsNot Nothing Then
                        Return order.Id
                    End If
                End If
                Return Nothing
            End Get
            Set(value As Integer?)
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property Orders As Object Implements IPurchaseOrderListView.Orders
            Get
                Return dgvOrders.DataSource
            End Get
            Set(value As Object)
                dgvOrders.DataSource = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property CanAdd As Boolean Implements IPurchaseOrderListView.CanAdd
            Get
                Return btnNew.Enabled
            End Get
            Set(value As Boolean)
                btnNew.Enabled = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property CanEdit As Boolean Implements IPurchaseOrderListView.CanEdit
            Get
                Return btnEdit.Enabled
            End Get
            Set(value As Boolean)
                btnEdit.Enabled = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property CanSubmit As Boolean Implements IPurchaseOrderListView.CanSubmit
            Get
                Return btnSubmit.Enabled
            End Get
            Set(value As Boolean)
                btnSubmit.Enabled = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property CanDelete As Boolean Implements IPurchaseOrderListView.CanDelete
            Get
                Return btnDelete.Enabled
            End Get
            Set(value As Boolean)
                btnDelete.Enabled = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property FilterStatus As Integer Implements IPurchaseOrderListView.FilterStatus
            Get
                Return cboStatusFilter.SelectedIndex - 1 ' 0 is All, 1 is Draft (0), 2 is Submitted (1) etc
            End Get
            Set(value As Integer)
                cboStatusFilter.SelectedIndex = value + 1
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property SearchTerm As String Implements IPurchaseOrderListView.SearchTerm
            Get
                Return txtSearch.Text
            End Get
            Set(value As String)
                txtSearch.Text = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property DataSource As Object Implements IPurchaseOrderListView.DataSource
            Get
                Return dgvOrders.DataSource
            End Get
            Set(value As Object)
                dgvOrders.DataSource = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property SelectedPO As PurchaseOrder Implements IPurchaseOrderListView.SelectedPO
            Get
                If dgvOrders.SelectedRows.Count > 0 Then
                    Return TryCast(dgvOrders.SelectedRows(0).DataBoundItem, PurchaseOrder)
                End If
                Return Nothing
            End Get
            Set(value As PurchaseOrder)
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property CanCreate As Boolean Implements IPurchaseOrderListView.CanCreate
            Get
                Return btnNew.Enabled
            End Get
            Set(value As Boolean)
                btnNew.Enabled = value
            End Set
        End Property

        Public Sub ShowError(message As String) Implements IPurchaseOrderListView.ShowError
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Sub

        Public Sub ShowMessage(message As String) Implements IPurchaseOrderListView.ShowMessage
            MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Sub

        Public Function Confirm(message As String, title As String) As Boolean Implements IPurchaseOrderListView.Confirm
            Return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes
        End Function

        Public Sub InitializeStatusFilter(statuses As System.Collections.Generic.List(Of String)) Implements IPurchaseOrderListView.InitializeStatusFilter
            cboStatusFilter.Items.Clear()
            cboStatusFilter.Items.Add("All")
            For Each s In statuses
                cboStatusFilter.Items.Add(s)
            Next
            cboStatusFilter.SelectedIndex = 0
        End Sub
    End Class
End Namespace
