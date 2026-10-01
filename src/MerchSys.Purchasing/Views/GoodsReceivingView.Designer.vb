Namespace Views
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class GoodsReceivingView
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
            Me.components = New System.ComponentModel.Container()
            Me.pnlTop = New System.Windows.Forms.Panel()
            Me.lblSelectPO = New System.Windows.Forms.Label()
            Me.cmbPurchaseOrders = New System.Windows.Forms.ComboBox()
            Me.btnConfirmReceipt = New System.Windows.Forms.Button()
            Me.dgvLines = New System.Windows.Forms.DataGridView()
            Me.pnlPlaceholder = New System.Windows.Forms.Panel()
            Me.lblPlaceholder = New System.Windows.Forms.Label()
            Me.statusStrip = New System.Windows.Forms.StatusStrip()
            Me.lblStatus = New System.Windows.Forms.ToolStripStatusLabel()
            Me.bindingSourceLines = New System.Windows.Forms.BindingSource(Me.components)
            Me.pnlTop.SuspendLayout()
            CType(Me.dgvLines, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.pnlPlaceholder.SuspendLayout()
            Me.statusStrip.SuspendLayout()
            CType(Me.bindingSourceLines, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'pnlTop
            '
            Me.pnlTop.Controls.Add(Me.btnConfirmReceipt)
            Me.pnlTop.Controls.Add(Me.cmbPurchaseOrders)
            Me.pnlTop.Controls.Add(Me.lblSelectPO)
            Me.pnlTop.Dock = System.Windows.Forms.DockStyle.Top
            Me.pnlTop.Location = New System.Drawing.Point(0, 0)
            Me.pnlTop.Name = "pnlTop"
            Me.pnlTop.Size = New System.Drawing.Size(800, 50)
            Me.pnlTop.TabIndex = 0
            '
            'lblSelectPO
            '
            Me.lblSelectPO.AutoSize = True
            Me.lblSelectPO.Location = New System.Drawing.Point(12, 18)
            Me.lblSelectPO.Name = "lblSelectPO"
            Me.lblSelectPO.Size = New System.Drawing.Size(125, 15)
            Me.lblSelectPO.TabIndex = 0
            Me.lblSelectPO.Text = "Select Submitted PO:"
            '
            'cmbPurchaseOrders
            '
            Me.cmbPurchaseOrders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cmbPurchaseOrders.FormattingEnabled = True
            Me.cmbPurchaseOrders.Location = New System.Drawing.Point(143, 15)
            Me.cmbPurchaseOrders.Name = "cmbPurchaseOrders"
            Me.cmbPurchaseOrders.Size = New System.Drawing.Size(300, 23)
            Me.cmbPurchaseOrders.TabIndex = 1
            '
            'btnConfirmReceipt
            '
            Me.btnConfirmReceipt.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnConfirmReceipt.Enabled = False
            Me.btnConfirmReceipt.Location = New System.Drawing.Point(673, 14)
            Me.btnConfirmReceipt.Name = "btnConfirmReceipt"
            Me.btnConfirmReceipt.Size = New System.Drawing.Size(115, 25)
            Me.btnConfirmReceipt.TabIndex = 2
            Me.btnConfirmReceipt.Text = "Confirm Receipt"
            Me.btnConfirmReceipt.UseVisualStyleBackColor = True
            '
            'dgvLines
            '
            Me.dgvLines.AllowUserToAddRows = False
            Me.dgvLines.AllowUserToDeleteRows = False
            Me.dgvLines.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvLines.Dock = System.Windows.Forms.DockStyle.Fill
            Me.dgvLines.Location = New System.Drawing.Point(0, 50)
            Me.dgvLines.Name = "dgvLines"
            Me.dgvLines.RowTemplate.Height = 25
            Me.dgvLines.Size = New System.Drawing.Size(800, 328)
            Me.dgvLines.TabIndex = 1
            Me.dgvLines.Visible = False
            '
            'pnlPlaceholder
            '
            Me.pnlPlaceholder.Controls.Add(Me.lblPlaceholder)
            Me.pnlPlaceholder.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pnlPlaceholder.Location = New System.Drawing.Point(0, 50)
            Me.pnlPlaceholder.Name = "pnlPlaceholder"
            Me.pnlPlaceholder.Size = New System.Drawing.Size(800, 328)
            Me.pnlPlaceholder.TabIndex = 2
            '
            'lblPlaceholder
            '
            Me.lblPlaceholder.Dock = System.Windows.Forms.DockStyle.Fill
            Me.lblPlaceholder.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me.lblPlaceholder.ForeColor = System.Drawing.SystemColors.ControlDark
            Me.lblPlaceholder.Location = New System.Drawing.Point(0, 0)
            Me.lblPlaceholder.Name = "lblPlaceholder"
            Me.lblPlaceholder.Size = New System.Drawing.Size(800, 328)
            Me.lblPlaceholder.TabIndex = 0
            Me.lblPlaceholder.Text = "Please select a Purchase Order to begin receiving."
            Me.lblPlaceholder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'statusStrip
            '
            Me.statusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblStatus})
            Me.statusStrip.Location = New System.Drawing.Point(0, 378)
            Me.statusStrip.Name = "statusStrip"
            Me.statusStrip.Size = New System.Drawing.Size(800, 22)
            Me.statusStrip.TabIndex = 3
            Me.statusStrip.Text = "statusStrip"
            '
            'lblStatus
            '
            Me.lblStatus.Name = "lblStatus"
            Me.lblStatus.Size = New System.Drawing.Size(39, 17)
            Me.lblStatus.Text = "Ready"
            '
            'GoodsReceivingView
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.pnlPlaceholder)
            Me.Controls.Add(Me.dgvLines)
            Me.Controls.Add(Me.statusStrip)
            Me.Controls.Add(Me.pnlTop)
            Me.Name = "GoodsReceivingView"
            Me.Size = New System.Drawing.Size(800, 400)
            Me.pnlTop.ResumeLayout(False)
            Me.pnlTop.PerformLayout()
            CType(Me.dgvLines, System.ComponentModel.ISupportInitialize).EndInit()
            Me.pnlPlaceholder.ResumeLayout(False)
            Me.statusStrip.ResumeLayout(False)
            Me.statusStrip.PerformLayout()
            CType(Me.bindingSourceLines, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Protected Friend WithEvents pnlTop As System.Windows.Forms.Panel
        Protected Friend WithEvents btnConfirmReceipt As System.Windows.Forms.Button
        Protected Friend WithEvents cmbPurchaseOrders As System.Windows.Forms.ComboBox
        Protected Friend WithEvents lblSelectPO As System.Windows.Forms.Label
        Protected Friend WithEvents dgvLines As System.Windows.Forms.DataGridView
        Protected Friend WithEvents pnlPlaceholder As System.Windows.Forms.Panel
        Protected Friend WithEvents lblPlaceholder As System.Windows.Forms.Label
        Protected Friend WithEvents statusStrip As System.Windows.Forms.StatusStrip
        Protected Friend WithEvents lblStatus As System.Windows.Forms.ToolStripStatusLabel
        Protected Friend WithEvents bindingSourceLines As System.Windows.Forms.BindingSource

    End Class
End Namespace
