Namespace Views

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class PurchaseOrderListView
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.PanelTop = New System.Windows.Forms.Panel()
        Me.btnCancelPO = New System.Windows.Forms.Button()
        Me.btnSubmitPO = New System.Windows.Forms.Button()
        Me.btnEdit = New System.Windows.Forms.Button()
        Me.btnNew = New System.Windows.Forms.Button()
        Me.txtSearch = New System.Windows.Forms.TextBox()
        Me.lblSearch = New System.Windows.Forms.Label()
        Me.cmbStatusFilter = New System.Windows.Forms.ComboBox()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.dgvOrders = New System.Windows.Forms.DataGridView()
        Me.PanelEditor = New System.Windows.Forms.Panel()
        Me.btnCancelEditor = New System.Windows.Forms.Button()
        Me.btnSaveDraft = New System.Windows.Forms.Button()
        Me.lblTotal = New System.Windows.Forms.Label()
        Me.dgvLines = New System.Windows.Forms.DataGridView()
        Me.txtNotes = New System.Windows.Forms.TextBox()
        Me.lblNotes = New System.Windows.Forms.Label()
        Me.dtpExpectedDelivery = New System.Windows.Forms.DateTimePicker()
        Me.lblExpectedDelivery = New System.Windows.Forms.Label()
        Me.cmbVendor = New System.Windows.Forms.ComboBox()
        Me.lblVendor = New System.Windows.Forms.Label()
        Me.btnAddLine = New System.Windows.Forms.Button()
        Me.txtProductName = New System.Windows.Forms.TextBox()
        Me.numUnitCost = New System.Windows.Forms.NumericUpDown()
        Me.numQuantity = New System.Windows.Forms.NumericUpDown()
        Me.PanelTop.SuspendLayout()
        CType(Me.dgvOrders, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelEditor.SuspendLayout()
        CType(Me.dgvLines, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numUnitCost, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numQuantity, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PanelTop
        '
        Me.PanelTop.Controls.Add(Me.btnCancelPO)
        Me.PanelTop.Controls.Add(Me.btnSubmitPO)
        Me.PanelTop.Controls.Add(Me.btnEdit)
        Me.PanelTop.Controls.Add(Me.btnNew)
        Me.PanelTop.Controls.Add(Me.txtSearch)
        Me.PanelTop.Controls.Add(Me.lblSearch)
        Me.PanelTop.Controls.Add(Me.cmbStatusFilter)
        Me.PanelTop.Controls.Add(Me.lblStatus)
        Me.PanelTop.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelTop.Location = New System.Drawing.Point(0, 0)
        Me.PanelTop.Name = "PanelTop"
        Me.PanelTop.Size = New System.Drawing.Size(900, 50)
        Me.PanelTop.TabIndex = 0
        '
        'btnCancelPO
        '
        Me.btnCancelPO.Location = New System.Drawing.Point(800, 12)
        Me.btnCancelPO.Name = "btnCancelPO"
        Me.btnCancelPO.Size = New System.Drawing.Size(75, 25)
        Me.btnCancelPO.TabIndex = 7
        Me.btnCancelPO.Text = "Cancel PO"
        Me.btnCancelPO.UseVisualStyleBackColor = True
        '
        'btnSubmitPO
        '
        Me.btnSubmitPO.Location = New System.Drawing.Point(719, 12)
        Me.btnSubmitPO.Name = "btnSubmitPO"
        Me.btnSubmitPO.Size = New System.Drawing.Size(75, 25)
        Me.btnSubmitPO.TabIndex = 6
        Me.btnSubmitPO.Text = "Submit PO"
        Me.btnSubmitPO.UseVisualStyleBackColor = True
        '
        'btnEdit
        '
        Me.btnEdit.Location = New System.Drawing.Point(638, 12)
        Me.btnEdit.Name = "btnEdit"
        Me.btnEdit.Size = New System.Drawing.Size(75, 25)
        Me.btnEdit.TabIndex = 5
        Me.btnEdit.Text = "Edit"
        Me.btnEdit.UseVisualStyleBackColor = True
        '
        'btnNew
        '
        Me.btnNew.Location = New System.Drawing.Point(557, 12)
        Me.btnNew.Name = "btnNew"
        Me.btnNew.Size = New System.Drawing.Size(75, 25)
        Me.btnNew.TabIndex = 4
        Me.btnNew.Text = "New PO"
        Me.btnNew.UseVisualStyleBackColor = True
        '
        'txtSearch
        '
        Me.txtSearch.Location = New System.Drawing.Point(232, 14)
        Me.txtSearch.Name = "txtSearch"
        Me.txtSearch.Size = New System.Drawing.Size(150, 23)
        Me.txtSearch.TabIndex = 3
        '
        'lblSearch
        '
        Me.lblSearch.AutoSize = True
        Me.lblSearch.Location = New System.Drawing.Point(179, 17)
        Me.lblSearch.Name = "lblSearch"
        Me.lblSearch.Size = New System.Drawing.Size(45, 15)
        Me.lblSearch.TabIndex = 2
        Me.lblSearch.Text = "Search:"
        '
        'cmbStatusFilter
        '
        Me.cmbStatusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbStatusFilter.FormattingEnabled = True
        Me.cmbStatusFilter.Location = New System.Drawing.Point(57, 14)
        Me.cmbStatusFilter.Name = "cmbStatusFilter"
        Me.cmbStatusFilter.Size = New System.Drawing.Size(110, 23)
        Me.cmbStatusFilter.TabIndex = 1
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Location = New System.Drawing.Point(12, 17)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(42, 15)
        Me.lblStatus.TabIndex = 0
        Me.lblStatus.Text = "Status:"
        '
        'dgvOrders
        '
        Me.dgvOrders.AllowUserToAddRows = False
        Me.dgvOrders.AllowUserToDeleteRows = False
        Me.dgvOrders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvOrders.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvOrders.Location = New System.Drawing.Point(0, 50)
        Me.dgvOrders.Name = "dgvOrders"
        Me.dgvOrders.ReadOnly = True
        Me.dgvOrders.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvOrders.Size = New System.Drawing.Size(900, 300)
        Me.dgvOrders.TabIndex = 1
        '
        'PanelEditor
        '
        Me.PanelEditor.Controls.Add(Me.numQuantity)
        Me.PanelEditor.Controls.Add(Me.numUnitCost)
        Me.PanelEditor.Controls.Add(Me.txtProductName)
        Me.PanelEditor.Controls.Add(Me.btnAddLine)
        Me.PanelEditor.Controls.Add(Me.btnCancelEditor)
        Me.PanelEditor.Controls.Add(Me.btnSaveDraft)
        Me.PanelEditor.Controls.Add(Me.lblTotal)
        Me.PanelEditor.Controls.Add(Me.dgvLines)
        Me.PanelEditor.Controls.Add(Me.txtNotes)
        Me.PanelEditor.Controls.Add(Me.lblNotes)
        Me.PanelEditor.Controls.Add(Me.dtpExpectedDelivery)
        Me.PanelEditor.Controls.Add(Me.lblExpectedDelivery)
        Me.PanelEditor.Controls.Add(Me.cmbVendor)
        Me.PanelEditor.Controls.Add(Me.lblVendor)
        Me.PanelEditor.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelEditor.Location = New System.Drawing.Point(0, 350)
        Me.PanelEditor.Name = "PanelEditor"
        Me.PanelEditor.Size = New System.Drawing.Size(900, 250)
        Me.PanelEditor.TabIndex = 2
        Me.PanelEditor.Visible = False
        '
        'btnCancelEditor
        '
        Me.btnCancelEditor.Location = New System.Drawing.Point(96, 215)
        Me.btnCancelEditor.Name = "btnCancelEditor"
        Me.btnCancelEditor.Size = New System.Drawing.Size(75, 25)
        Me.btnCancelEditor.TabIndex = 10
        Me.btnCancelEditor.Text = "Cancel"
        Me.btnCancelEditor.UseVisualStyleBackColor = True
        '
        'btnSaveDraft
        '
        Me.btnSaveDraft.Location = New System.Drawing.Point(15, 215)
        Me.btnSaveDraft.Name = "btnSaveDraft"
        Me.btnSaveDraft.Size = New System.Drawing.Size(75, 25)
        Me.btnSaveDraft.TabIndex = 9
        Me.btnSaveDraft.Text = "Save Draft"
        Me.btnSaveDraft.UseVisualStyleBackColor = True
        '
        'lblTotal
        '
        Me.lblTotal.AutoSize = True
        Me.lblTotal.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me.lblTotal.Location = New System.Drawing.Point(650, 215)
        Me.lblTotal.Name = "lblTotal"
        Me.lblTotal.Size = New System.Drawing.Size(126, 21)
        Me.lblTotal.TabIndex = 8
        Me.lblTotal.Text = "Total: ₱0.00"
        '
        'dgvLines
        '
        Me.dgvLines.AllowUserToAddRows = False
        Me.dgvLines.AllowUserToDeleteRows = False
        Me.dgvLines.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvLines.Location = New System.Drawing.Point(350, 45)
        Me.dgvLines.Name = "dgvLines"
        Me.dgvLines.ReadOnly = True
        Me.dgvLines.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvLines.Size = New System.Drawing.Size(525, 150)
        Me.dgvLines.TabIndex = 7
        '
        'txtNotes
        '
        Me.txtNotes.Location = New System.Drawing.Point(15, 120)
        Me.txtNotes.Multiline = True
        Me.txtNotes.Name = "txtNotes"
        Me.txtNotes.Size = New System.Drawing.Size(300, 75)
        Me.txtNotes.TabIndex = 5
        '
        'lblNotes
        '
        Me.lblNotes.AutoSize = True
        Me.lblNotes.Location = New System.Drawing.Point(12, 100)
        Me.lblNotes.Name = "lblNotes"
        Me.lblNotes.Size = New System.Drawing.Size(41, 15)
        Me.lblNotes.TabIndex = 4
        Me.lblNotes.Text = "Notes:"
        '
        'dtpExpectedDelivery
        '
        Me.dtpExpectedDelivery.Location = New System.Drawing.Point(115, 60)
        Me.dtpExpectedDelivery.Name = "dtpExpectedDelivery"
        Me.dtpExpectedDelivery.Size = New System.Drawing.Size(200, 23)
        Me.dtpExpectedDelivery.TabIndex = 3
        '
        'lblExpectedDelivery
        '
        Me.lblExpectedDelivery.AutoSize = True
        Me.lblExpectedDelivery.Location = New System.Drawing.Point(12, 65)
        Me.lblExpectedDelivery.Name = "lblExpectedDelivery"
        Me.lblExpectedDelivery.Size = New System.Drawing.Size(102, 15)
        Me.lblExpectedDelivery.TabIndex = 2
        Me.lblExpectedDelivery.Text = "Expected Delivery:"
        '
        'cmbVendor
        '
        Me.cmbVendor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbVendor.FormattingEnabled = True
        Me.cmbVendor.Location = New System.Drawing.Point(115, 20)
        Me.cmbVendor.Name = "cmbVendor"
        Me.cmbVendor.Size = New System.Drawing.Size(200, 23)
        Me.cmbVendor.TabIndex = 1
        '
        'lblVendor
        '
        Me.lblVendor.AutoSize = True
        Me.lblVendor.Location = New System.Drawing.Point(12, 23)
        Me.lblVendor.Name = "lblVendor"
        Me.lblVendor.Size = New System.Drawing.Size(47, 15)
        Me.lblVendor.TabIndex = 0
        Me.lblVendor.Text = "Vendor:"
        '
        'btnAddLine
        '
        Me.btnAddLine.Location = New System.Drawing.Point(800, 15)
        Me.btnAddLine.Name = "btnAddLine"
        Me.btnAddLine.Size = New System.Drawing.Size(75, 25)
        Me.btnAddLine.TabIndex = 11
        Me.btnAddLine.Text = "Add Line"
        Me.btnAddLine.UseVisualStyleBackColor = True
        '
        'txtProductName
        '
        Me.txtProductName.Location = New System.Drawing.Point(350, 16)
        Me.txtProductName.Name = "txtProductName"
        Me.txtProductName.PlaceholderText = "Product Name"
        Me.txtProductName.Size = New System.Drawing.Size(200, 23)
        Me.txtProductName.TabIndex = 12
        '
        'numUnitCost
        '
        Me.numUnitCost.DecimalPlaces = 2
        Me.numUnitCost.Location = New System.Drawing.Point(560, 16)
        Me.numUnitCost.Maximum = New Decimal(New Integer() {1000000, 0, 0, 0})
        Me.numUnitCost.Name = "numUnitCost"
        Me.numUnitCost.Size = New System.Drawing.Size(100, 23)
        Me.numUnitCost.TabIndex = 13
        '
        'numQuantity
        '
        Me.numQuantity.Location = New System.Drawing.Point(670, 16)
        Me.numQuantity.Maximum = New Decimal(New Integer() {100000, 0, 0, 0})
        Me.numQuantity.Name = "numQuantity"
        Me.numQuantity.Size = New System.Drawing.Size(80, 23)
        Me.numQuantity.TabIndex = 14
        '
        'PurchaseOrderListView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.dgvOrders)
        Me.Controls.Add(Me.PanelEditor)
        Me.Controls.Add(Me.PanelTop)
        Me.Name = "PurchaseOrderListView"
        Me.Size = New System.Drawing.Size(900, 600)
        Me.PanelTop.ResumeLayout(False)
        Me.PanelTop.PerformLayout()
        CType(Me.dgvOrders, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelEditor.ResumeLayout(False)
        Me.PanelEditor.PerformLayout()
        CType(Me.dgvLines, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numUnitCost, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numQuantity, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PanelTop As System.Windows.Forms.Panel
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents cmbStatusFilter As System.Windows.Forms.ComboBox
    Friend WithEvents lblSearch As System.Windows.Forms.Label
    Friend WithEvents txtSearch As System.Windows.Forms.TextBox
    Friend WithEvents btnNew As System.Windows.Forms.Button
    Friend WithEvents btnEdit As System.Windows.Forms.Button
    Friend WithEvents btnSubmitPO As System.Windows.Forms.Button
    Friend WithEvents btnCancelPO As System.Windows.Forms.Button
    Friend WithEvents dgvOrders As System.Windows.Forms.DataGridView
    Friend WithEvents PanelEditor As System.Windows.Forms.Panel
    Friend WithEvents lblVendor As System.Windows.Forms.Label
    Friend WithEvents cmbVendor As System.Windows.Forms.ComboBox
    Friend WithEvents lblExpectedDelivery As System.Windows.Forms.Label
    Friend WithEvents dtpExpectedDelivery As System.Windows.Forms.DateTimePicker
    Friend WithEvents lblNotes As System.Windows.Forms.Label
    Friend WithEvents txtNotes As System.Windows.Forms.TextBox
    Friend WithEvents dgvLines As System.Windows.Forms.DataGridView
    Friend WithEvents lblTotal As System.Windows.Forms.Label
    Friend WithEvents btnSaveDraft As System.Windows.Forms.Button
    Friend WithEvents btnCancelEditor As System.Windows.Forms.Button
    Friend WithEvents btnAddLine As System.Windows.Forms.Button
    Friend WithEvents txtProductName As System.Windows.Forms.TextBox
    Friend WithEvents numUnitCost As System.Windows.Forms.NumericUpDown
    Friend WithEvents numQuantity As System.Windows.Forms.NumericUpDown

End Class
End Namespace
