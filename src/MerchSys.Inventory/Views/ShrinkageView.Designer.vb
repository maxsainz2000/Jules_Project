Imports System.Windows.Forms

Namespace Views

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class ShrinkageView
        Inherits UserControl

        Private components As System.ComponentModel.IContainer

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
            Me.pnlSummary = New Panel()
            Me.lblTotalItems = New Label()
            Me.lblTotalValue = New Label()
            Me.btnRecord = New Button()
            Me.pnlFilter = New Panel()
            Me.chkDateFilter = New CheckBox()
            Me.dtpStart = New DateTimePicker()
            Me.lblTo = New Label()
            Me.dtpEnd = New DateTimePicker()
            Me.lblReason = New Label()
            Me.cmbReason = New ComboBox()
            Me.btnFilter = New Button()
            Me.dgvHistory = New DataGridView()

            Me.pnlSummary.SuspendLayout()
            Me.pnlFilter.SuspendLayout()
            CType(Me.dgvHistory, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()

            'pnlSummary
            Me.pnlSummary.Controls.Add(Me.lblTotalItems)
            Me.pnlSummary.Controls.Add(Me.lblTotalValue)
            Me.pnlSummary.Controls.Add(Me.btnRecord)
            Me.pnlSummary.Dock = DockStyle.Top
            Me.pnlSummary.Location = New System.Drawing.Point(0, 0)
            Me.pnlSummary.Name = "pnlSummary"
            Me.pnlSummary.Size = New System.Drawing.Size(800, 60)

            'lblTotalItems
            Me.lblTotalItems.AutoSize = True
            Me.lblTotalItems.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
            Me.lblTotalItems.Location = New System.Drawing.Point(20, 20)
            Me.lblTotalItems.Name = "lblTotalItems"
            Me.lblTotalItems.Size = New System.Drawing.Size(155, 21)
            Me.lblTotalItems.Text = "Total Items Lost: 0"

            'lblTotalValue
            Me.lblTotalValue.AutoSize = True
            Me.lblTotalValue.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
            Me.lblTotalValue.Location = New System.Drawing.Point(220, 20)
            Me.lblTotalValue.Name = "lblTotalValue"
            Me.lblTotalValue.Size = New System.Drawing.Size(175, 21)
            Me.lblTotalValue.Text = "Total Value Lost: $0.00"

            'btnRecord
            Me.btnRecord.Anchor = AnchorStyles.Top Or AnchorStyles.Right
            Me.btnRecord.Location = New System.Drawing.Point(650, 15)
            Me.btnRecord.Name = "btnRecord"
            Me.btnRecord.Size = New System.Drawing.Size(130, 30)
            Me.btnRecord.Text = "Record Shrinkage"

            'pnlFilter
            Me.pnlFilter.Controls.Add(Me.chkDateFilter)
            Me.pnlFilter.Controls.Add(Me.dtpStart)
            Me.pnlFilter.Controls.Add(Me.lblTo)
            Me.pnlFilter.Controls.Add(Me.dtpEnd)
            Me.pnlFilter.Controls.Add(Me.lblReason)
            Me.pnlFilter.Controls.Add(Me.cmbReason)
            Me.pnlFilter.Controls.Add(Me.btnFilter)
            Me.pnlFilter.Dock = DockStyle.Top
            Me.pnlFilter.Location = New System.Drawing.Point(0, 60)
            Me.pnlFilter.Name = "pnlFilter"
            Me.pnlFilter.Size = New System.Drawing.Size(800, 50)

            'chkDateFilter
            Me.chkDateFilter.AutoSize = True
            Me.chkDateFilter.Location = New System.Drawing.Point(20, 15)
            Me.chkDateFilter.Name = "chkDateFilter"
            Me.chkDateFilter.Size = New System.Drawing.Size(82, 19)
            Me.chkDateFilter.Text = "Filter Date:"

            'dtpStart
            Me.dtpStart.Format = DateTimePickerFormat.Short
            Me.dtpStart.Location = New System.Drawing.Point(110, 13)
            Me.dtpStart.Name = "dtpStart"
            Me.dtpStart.Size = New System.Drawing.Size(100, 23)

            'lblTo
            Me.lblTo.AutoSize = True
            Me.lblTo.Location = New System.Drawing.Point(220, 17)
            Me.lblTo.Name = "lblTo"
            Me.lblTo.Size = New System.Drawing.Size(18, 15)
            Me.lblTo.Text = "to"

            'dtpEnd
            Me.dtpEnd.Format = DateTimePickerFormat.Short
            Me.dtpEnd.Location = New System.Drawing.Point(250, 13)
            Me.dtpEnd.Name = "dtpEnd"
            Me.dtpEnd.Size = New System.Drawing.Size(100, 23)

            'lblReason
            Me.lblReason.AutoSize = True
            Me.lblReason.Location = New System.Drawing.Point(370, 17)
            Me.lblReason.Name = "lblReason"
            Me.lblReason.Size = New System.Drawing.Size(48, 15)
            Me.lblReason.Text = "Reason:"

            'cmbReason
            Me.cmbReason.DropDownStyle = ComboBoxStyle.DropDownList
            Me.cmbReason.Location = New System.Drawing.Point(425, 13)
            Me.cmbReason.Name = "cmbReason"
            Me.cmbReason.Size = New System.Drawing.Size(120, 23)

            'btnFilter
            Me.btnFilter.Location = New System.Drawing.Point(560, 12)
            Me.btnFilter.Name = "btnFilter"
            Me.btnFilter.Size = New System.Drawing.Size(75, 25)
            Me.btnFilter.Text = "Apply"

            'dgvHistory
            Me.dgvHistory.AllowUserToAddRows = False
            Me.dgvHistory.AllowUserToDeleteRows = False
            Me.dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            Me.dgvHistory.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvHistory.Dock = DockStyle.Fill
            Me.dgvHistory.Location = New System.Drawing.Point(0, 110)
            Me.dgvHistory.Name = "dgvHistory"
            Me.dgvHistory.ReadOnly = True
            Me.dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            Me.dgvHistory.Size = New System.Drawing.Size(800, 490)

            'ShrinkageView
            Me.Controls.Add(Me.dgvHistory)
            Me.Controls.Add(Me.pnlFilter)
            Me.Controls.Add(Me.pnlSummary)
            Me.Name = "ShrinkageView"
            Me.Size = New System.Drawing.Size(800, 600)

            Me.pnlSummary.ResumeLayout(False)
            Me.pnlSummary.PerformLayout()
            Me.pnlFilter.ResumeLayout(False)
            Me.pnlFilter.PerformLayout()
            CType(Me.dgvHistory, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
        End Sub

        Friend WithEvents pnlSummary As Panel
        Friend WithEvents lblTotalItems As Label
        Friend WithEvents lblTotalValue As Label
        Friend WithEvents btnRecord As Button
        Friend WithEvents pnlFilter As Panel
        Friend WithEvents chkDateFilter As CheckBox
        Friend WithEvents dtpStart As DateTimePicker
        Friend WithEvents lblTo As Label
        Friend WithEvents dtpEnd As DateTimePicker
        Friend WithEvents lblReason As Label
        Friend WithEvents cmbReason As ComboBox
        Friend WithEvents btnFilter As Button
        Friend WithEvents dgvHistory As DataGridView

    End Class
End Namespace
