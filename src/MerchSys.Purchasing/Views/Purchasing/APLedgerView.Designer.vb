Imports System.Drawing
Imports System.Windows.Forms

Namespace Views.Purchasing
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class APLedgerView
        Inherits UserControl

        'UserControl overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.pnlHeader = New System.Windows.Forms.Panel()
            Me.lblTitle = New System.Windows.Forms.Label()
            Me.lblTotalOutstanding = New System.Windows.Forms.Label()

            Me.pnlToolbar = New System.Windows.Forms.Panel()
            Me.btnRecordPayment = New System.Windows.Forms.Button()
            Me.cboVendors = New System.Windows.Forms.ComboBox()
            Me.lblVendor = New System.Windows.Forms.Label()
            Me.btnFilterPaid = New System.Windows.Forms.RadioButton()
            Me.btnFilterOverdue = New System.Windows.Forms.RadioButton()
            Me.btnFilterOutstanding = New System.Windows.Forms.RadioButton()
            Me.btnFilterAll = New System.Windows.Forms.RadioButton()

            Me.dgvLedger = New System.Windows.Forms.DataGridView()

            Me.pnlHeader.SuspendLayout()
            Me.pnlToolbar.SuspendLayout()
            CType(Me.dgvLedger, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'pnlHeader
            '
            Me.pnlHeader.BackColor = System.Drawing.Color.White
            Me.pnlHeader.Controls.Add(Me.lblTotalOutstanding)
            Me.pnlHeader.Controls.Add(Me.lblTitle)
            Me.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top
            Me.pnlHeader.Location = New System.Drawing.Point(0, 0)
            Me.pnlHeader.Name = "pnlHeader"
            Me.pnlHeader.Size = New System.Drawing.Size(1000, 60)
            Me.pnlHeader.TabIndex = 0
            '
            'lblTitle
            '
            Me.lblTitle.AutoSize = True
            Me.lblTitle.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me.lblTitle.Location = New System.Drawing.Point(15, 15)
            Me.lblTitle.Name = "lblTitle"
            Me.lblTitle.Size = New System.Drawing.Size(193, 30)
            Me.lblTitle.TabIndex = 0
            Me.lblTitle.Text = "Accounts Payable"
            '
            'lblTotalOutstanding
            '
            Me.lblTotalOutstanding.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblTotalOutstanding.Font = New System.Drawing.Font("Segoe UI Semibold", 14.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblTotalOutstanding.ForeColor = System.Drawing.Color.Firebrick
            Me.lblTotalOutstanding.Location = New System.Drawing.Point(680, 15)
            Me.lblTotalOutstanding.Name = "lblTotalOutstanding"
            Me.lblTotalOutstanding.Size = New System.Drawing.Size(300, 30)
            Me.lblTotalOutstanding.TabIndex = 1
            Me.lblTotalOutstanding.Text = "Outstanding: ₱0.00"
            Me.lblTotalOutstanding.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'pnlToolbar
            '
            Me.pnlToolbar.BackColor = System.Drawing.Color.WhiteSmoke
            Me.pnlToolbar.Controls.Add(Me.btnRecordPayment)
            Me.pnlToolbar.Controls.Add(Me.cboVendors)
            Me.pnlToolbar.Controls.Add(Me.lblVendor)
            Me.pnlToolbar.Controls.Add(Me.btnFilterPaid)
            Me.pnlToolbar.Controls.Add(Me.btnFilterOverdue)
            Me.pnlToolbar.Controls.Add(Me.btnFilterOutstanding)
            Me.pnlToolbar.Controls.Add(Me.btnFilterAll)
            Me.pnlToolbar.Dock = System.Windows.Forms.DockStyle.Top
            Me.pnlToolbar.Location = New System.Drawing.Point(0, 60)
            Me.pnlToolbar.Name = "pnlToolbar"
            Me.pnlToolbar.Size = New System.Drawing.Size(1000, 50)
            Me.pnlToolbar.TabIndex = 1
            '
            'btnFilterAll
            '
            Me.btnFilterAll.Appearance = System.Windows.Forms.Appearance.Button
            Me.btnFilterAll.Checked = True
            Me.btnFilterAll.Location = New System.Drawing.Point(15, 10)
            Me.btnFilterAll.Name = "btnFilterAll"
            Me.btnFilterAll.Size = New System.Drawing.Size(80, 30)
            Me.btnFilterAll.TabIndex = 0
            Me.btnFilterAll.TabStop = True
            Me.btnFilterAll.Text = "All"
            Me.btnFilterAll.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            Me.btnFilterAll.UseVisualStyleBackColor = True
            '
            'btnFilterOutstanding
            '
            Me.btnFilterOutstanding.Appearance = System.Windows.Forms.Appearance.Button
            Me.btnFilterOutstanding.Location = New System.Drawing.Point(100, 10)
            Me.btnFilterOutstanding.Name = "btnFilterOutstanding"
            Me.btnFilterOutstanding.Size = New System.Drawing.Size(90, 30)
            Me.btnFilterOutstanding.TabIndex = 1
            Me.btnFilterOutstanding.Text = "Outstanding"
            Me.btnFilterOutstanding.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            Me.btnFilterOutstanding.UseVisualStyleBackColor = True
            '
            'btnFilterOverdue
            '
            Me.btnFilterOverdue.Appearance = System.Windows.Forms.Appearance.Button
            Me.btnFilterOverdue.Location = New System.Drawing.Point(195, 10)
            Me.btnFilterOverdue.Name = "btnFilterOverdue"
            Me.btnFilterOverdue.Size = New System.Drawing.Size(80, 30)
            Me.btnFilterOverdue.TabIndex = 2
            Me.btnFilterOverdue.Text = "Overdue"
            Me.btnFilterOverdue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            Me.btnFilterOverdue.UseVisualStyleBackColor = True
            '
            'btnFilterPaid
            '
            Me.btnFilterPaid.Appearance = System.Windows.Forms.Appearance.Button
            Me.btnFilterPaid.Location = New System.Drawing.Point(280, 10)
            Me.btnFilterPaid.Name = "btnFilterPaid"
            Me.btnFilterPaid.Size = New System.Drawing.Size(80, 30)
            Me.btnFilterPaid.TabIndex = 3
            Me.btnFilterPaid.Text = "Paid"
            Me.btnFilterPaid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            Me.btnFilterPaid.UseVisualStyleBackColor = True
            '
            'lblVendor
            '
            Me.lblVendor.AutoSize = True
            Me.lblVendor.Location = New System.Drawing.Point(380, 17)
            Me.lblVendor.Name = "lblVendor"
            Me.lblVendor.Size = New System.Drawing.Size(47, 15)
            Me.lblVendor.TabIndex = 4
            Me.lblVendor.Text = "Vendor:"
            '
            'cboVendors
            '
            Me.cboVendors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cboVendors.FormattingEnabled = True
            Me.cboVendors.Location = New System.Drawing.Point(433, 14)
            Me.cboVendors.Name = "cboVendors"
            Me.cboVendors.Size = New System.Drawing.Size(200, 23)
            Me.cboVendors.TabIndex = 5
            '
            'btnRecordPayment
            '
            Me.btnRecordPayment.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnRecordPayment.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(120, Byte), Integer), CType(CType(215, Byte), Integer))
            Me.btnRecordPayment.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnRecordPayment.ForeColor = System.Drawing.Color.White
            Me.btnRecordPayment.Location = New System.Drawing.Point(850, 10)
            Me.btnRecordPayment.Name = "btnRecordPayment"
            Me.btnRecordPayment.Size = New System.Drawing.Size(130, 30)
            Me.btnRecordPayment.TabIndex = 6
            Me.btnRecordPayment.Text = "Record Payment"
            Me.btnRecordPayment.UseVisualStyleBackColor = False
            '
            'dgvLedger
            '
            Me.dgvLedger.AllowUserToAddRows = False
            Me.dgvLedger.AllowUserToDeleteRows = False
            Me.dgvLedger.BackgroundColor = System.Drawing.Color.White
            Me.dgvLedger.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvLedger.Dock = System.Windows.Forms.DockStyle.Fill
            Me.dgvLedger.Location = New System.Drawing.Point(0, 110)
            Me.dgvLedger.Name = "dgvLedger"
            Me.dgvLedger.ReadOnly = True
            Me.dgvLedger.RowHeadersVisible = False
            Me.dgvLedger.RowTemplate.Height = 25
            Me.dgvLedger.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
            Me.dgvLedger.Size = New System.Drawing.Size(1000, 590)
            Me.dgvLedger.TabIndex = 2
            '
            'APLedgerView
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.dgvLedger)
            Me.Controls.Add(Me.pnlToolbar)
            Me.Controls.Add(Me.pnlHeader)
            Me.Name = "APLedgerView"
            Me.Size = New System.Drawing.Size(1000, 700)
            Me.pnlHeader.ResumeLayout(False)
            Me.pnlHeader.PerformLayout()
            Me.pnlToolbar.ResumeLayout(False)
            Me.pnlToolbar.PerformLayout()
            CType(Me.dgvLedger, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents pnlHeader As Panel
        Friend WithEvents lblTitle As Label
        Friend WithEvents lblTotalOutstanding As Label
        Friend WithEvents pnlToolbar As Panel
        Friend WithEvents btnFilterAll As RadioButton
        Friend WithEvents btnFilterOutstanding As RadioButton
        Friend WithEvents btnFilterOverdue As RadioButton
        Friend WithEvents btnFilterPaid As RadioButton
        Friend WithEvents lblVendor As Label
        Friend WithEvents cboVendors As ComboBox
        Friend WithEvents btnRecordPayment As Button
        Friend WithEvents dgvLedger As DataGridView

    End Class
End Namespace
