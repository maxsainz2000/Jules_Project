Namespace Views
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class GoodsReceivingView
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
        Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
        Me.CboPurchaseOrders = New System.Windows.Forms.ToolStripComboBox()
        Me.BtnConfirmReceipt = New System.Windows.Forms.ToolStripButton()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.LblStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.PanelPlaceholder = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.DgvLines = New System.Windows.Forms.DataGridView()
        Me.ColWarning = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColProductName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColQtyOrdered = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColQtyReceived = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColUnitCost = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColExpiryDate = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColDiscrepancyNotes = New System.Windows.Forms.DataGridViewTextBoxColumn()

        Me.ToolStrip1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.PanelPlaceholder.SuspendLayout()
        CType(Me.DgvLines, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLabel1, Me.CboPurchaseOrders, Me.BtnConfirmReceipt})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(800, 25)
        Me.ToolStrip1.TabIndex = 0
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripLabel1
        '
        Me.ToolStripLabel1.Name = "ToolStripLabel1"
        Me.ToolStripLabel1.Size = New System.Drawing.Size(89, 22)
        Me.ToolStripLabel1.Text = "Select PO:"
        '
        'CboPurchaseOrders
        '
        Me.CboPurchaseOrders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CboPurchaseOrders.Name = "CboPurchaseOrders"
        Me.CboPurchaseOrders.Size = New System.Drawing.Size(250, 25)
        '
        'BtnConfirmReceipt
        '
        Me.BtnConfirmReceipt.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.BtnConfirmReceipt.Name = "BtnConfirmReceipt"
        Me.BtnConfirmReceipt.Size = New System.Drawing.Size(100, 22)
        Me.BtnConfirmReceipt.Text = "Confirm Receipt"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LblStatus})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 428)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(800, 22)
        Me.StatusStrip1.TabIndex = 1
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'LblStatus
        '
        Me.LblStatus.Name = "LblStatus"
        Me.LblStatus.Size = New System.Drawing.Size(39, 17)
        Me.LblStatus.Text = "Ready"
        '
        'PanelPlaceholder
        '
        Me.PanelPlaceholder.Controls.Add(Me.Label1)
        Me.PanelPlaceholder.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelPlaceholder.Location = New System.Drawing.Point(0, 25)
        Me.PanelPlaceholder.Name = "PanelPlaceholder"
        Me.PanelPlaceholder.Size = New System.Drawing.Size(800, 403)
        Me.PanelPlaceholder.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Font = New System.Drawing.Font("Segoe UI", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.Label1.ForeColor = System.Drawing.SystemColors.ControlDark
        Me.Label1.Location = New System.Drawing.Point(0, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(800, 403)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Select a Purchase Order to begin receiving goods."
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'DgvLines
        '
        Me.DgvLines.AllowUserToAddRows = False
        Me.DgvLines.AllowUserToDeleteRows = False
        Me.DgvLines.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DgvLines.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvLines.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ColWarning, Me.ColProductName, Me.ColQtyOrdered, Me.ColQtyReceived, Me.ColUnitCost, Me.ColExpiryDate, Me.ColDiscrepancyNotes})
        Me.DgvLines.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DgvLines.Location = New System.Drawing.Point(0, 25)
        Me.DgvLines.Name = "DgvLines"
        Me.DgvLines.Size = New System.Drawing.Size(800, 403)
        Me.DgvLines.TabIndex = 3
        Me.DgvLines.Visible = False
        '
        'ColWarning
        '
        Me.ColWarning.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.ColWarning.HeaderText = "⚠"
        Me.ColWarning.Name = "ColWarning"
        Me.ColWarning.ReadOnly = True
        Me.ColWarning.Width = 30
        '
        'ColProductName
        '
        Me.ColProductName.DataPropertyName = "ProductName"
        Me.ColProductName.HeaderText = "Product"
        Me.ColProductName.Name = "ColProductName"
        Me.ColProductName.ReadOnly = True
        '
        'ColQtyOrdered
        '
        Me.ColQtyOrdered.DataPropertyName = "QuantityOrdered"
        Me.ColQtyOrdered.HeaderText = "Qty Ordered"
        Me.ColQtyOrdered.Name = "ColQtyOrdered"
        Me.ColQtyOrdered.ReadOnly = True
        '
        'ColQtyReceived
        '
        Me.ColQtyReceived.DataPropertyName = "QuantityReceived"
        Me.ColQtyReceived.HeaderText = "Qty Received"
        Me.ColQtyReceived.Name = "ColQtyReceived"
        '
        'ColUnitCost
        '
        Me.ColUnitCost.DataPropertyName = "UnitCost"
        Me.ColUnitCost.HeaderText = "Unit Cost"
        Me.ColUnitCost.Name = "ColUnitCost"
        '
        'ColExpiryDate
        '
        Me.ColExpiryDate.DataPropertyName = "ExpiryDate"
        Me.ColExpiryDate.HeaderText = "Expiry Date"
        Me.ColExpiryDate.Name = "ColExpiryDate"
        '
        'ColDiscrepancyNotes
        '
        Me.ColDiscrepancyNotes.DataPropertyName = "DiscrepancyNotes"
        Me.ColDiscrepancyNotes.HeaderText = "Discrepancy Notes"
        Me.ColDiscrepancyNotes.Name = "ColDiscrepancyNotes"
        '
        'GoodsReceivingView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.DgvLines)
        Me.Controls.Add(Me.PanelPlaceholder)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Name = "GoodsReceivingView"
        Me.Size = New System.Drawing.Size(800, 450)
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.PanelPlaceholder.ResumeLayout(False)
        CType(Me.DgvLines, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Friend WithEvents ToolStripLabel1 As System.Windows.Forms.ToolStripLabel
    Friend WithEvents CboPurchaseOrders As System.Windows.Forms.ToolStripComboBox
    Friend WithEvents BtnConfirmReceipt As System.Windows.Forms.ToolStripButton
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents LblStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents PanelPlaceholder As System.Windows.Forms.Panel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents DgvLines As System.Windows.Forms.DataGridView
    Friend WithEvents ColWarning As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ColProductName As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ColQtyOrdered As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ColQtyReceived As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ColUnitCost As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ColExpiryDate As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ColDiscrepancyNotes As System.Windows.Forms.DataGridViewTextBoxColumn

    End Class
End Namespace
