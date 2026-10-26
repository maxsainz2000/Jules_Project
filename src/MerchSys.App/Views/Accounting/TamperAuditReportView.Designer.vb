Imports System.Windows.Forms
Imports System.Drawing

Namespace Views.Accounting

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Public Class TamperAuditReportView
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
            Me.pnlHeader = New System.Windows.Forms.Panel()
            Me.lblTitle = New System.Windows.Forms.Label()
            Me.pnlFilter = New System.Windows.Forms.Panel()
            Me.lblTo = New System.Windows.Forms.Label()
            Me.dtpTo = New System.Windows.Forms.DateTimePicker()
            Me.lblFrom = New System.Windows.Forms.Label()
            Me.dtpFrom = New System.Windows.Forms.DateTimePicker()
            Me.btnRefresh = New System.Windows.Forms.Button()
            Me.pnlContent = New System.Windows.Forms.Panel()
            Me.lblEmptyState = New System.Windows.Forms.Label()
            Me.dgvIncidents = New System.Windows.Forms.DataGridView()
            Me.colDetectedAt = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.colReceiptNumber = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.colExpectedHash = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.colActualHash = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.colTamperKind = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.colDetails = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.btnExportPdf = New System.Windows.Forms.Button()
            Me.btnExportCsv = New System.Windows.Forms.Button()
            Me.pnlHeader.SuspendLayout()
            Me.pnlFilter.SuspendLayout()
            Me.pnlContent.SuspendLayout()
            CType(Me.dgvIncidents, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'pnlHeader
            '
            Me.pnlHeader.BackColor = System.Drawing.Color.White
            Me.pnlHeader.Controls.Add(Me.lblTitle)
            Me.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top
            Me.pnlHeader.Location = New System.Drawing.Point(0, 0)
            Me.pnlHeader.Name = "pnlHeader"
            Me.pnlHeader.Size = New System.Drawing.Size(900, 60)
            Me.pnlHeader.TabIndex = 0
            '
            'lblTitle
            '
            Me.lblTitle.AutoSize = True
            Me.lblTitle.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me.lblTitle.Location = New System.Drawing.Point(20, 15)
            Me.lblTitle.Name = "lblTitle"
            Me.lblTitle.Size = New System.Drawing.Size(222, 30)
            Me.lblTitle.TabIndex = 0
            Me.lblTitle.Text = "Tamper Audit Report"
            '
            'pnlFilter
            '
            Me.pnlFilter.BackColor = System.Drawing.Color.WhiteSmoke
            Me.pnlFilter.Controls.Add(Me.lblTo)
            Me.pnlFilter.Controls.Add(Me.dtpTo)
            Me.pnlFilter.Controls.Add(Me.lblFrom)
            Me.pnlFilter.Controls.Add(Me.dtpFrom)
            Me.pnlFilter.Controls.Add(Me.btnExportPdf)
            Me.pnlFilter.Controls.Add(Me.btnExportCsv)
            Me.pnlFilter.Controls.Add(Me.btnRefresh)
            Me.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top
            Me.pnlFilter.Location = New System.Drawing.Point(0, 60)
            Me.pnlFilter.Name = "pnlFilter"
            Me.pnlFilter.Size = New System.Drawing.Size(900, 50)
            Me.pnlFilter.TabIndex = 1
            '
            'lblTo
            '
            Me.lblTo.AutoSize = True
            Me.lblTo.Location = New System.Drawing.Point(220, 17)
            Me.lblTo.Name = "lblTo"
            Me.lblTo.Size = New System.Drawing.Size(22, 15)
            Me.lblTo.TabIndex = 4
            Me.lblTo.Text = "To:"
            '
            'dtpTo
            '
            Me.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short
            Me.dtpTo.Location = New System.Drawing.Point(250, 13)
            Me.dtpTo.Name = "dtpTo"
            Me.dtpTo.Size = New System.Drawing.Size(120, 23)
            Me.dtpTo.TabIndex = 3
            '
            'lblFrom
            '
            Me.lblFrom.AutoSize = True
            Me.lblFrom.Location = New System.Drawing.Point(20, 17)
            Me.lblFrom.Name = "lblFrom"
            Me.lblFrom.Size = New System.Drawing.Size(38, 15)
            Me.lblFrom.TabIndex = 2
            Me.lblFrom.Text = "From:"
            '
            'dtpFrom
            '
            Me.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short
            Me.dtpFrom.Location = New System.Drawing.Point(70, 13)
            Me.dtpFrom.Name = "dtpFrom"
            Me.dtpFrom.Size = New System.Drawing.Size(120, 23)
            Me.dtpFrom.TabIndex = 1
            '
            'btnRefresh
            '
            Me.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnRefresh.Location = New System.Drawing.Point(400, 12)
            Me.btnRefresh.Name = "btnRefresh"
            Me.btnRefresh.Size = New System.Drawing.Size(90, 26)
            Me.btnRefresh.TabIndex = 0
            Me.btnRefresh.Text = "Refresh"
            Me.btnRefresh.UseVisualStyleBackColor = True
            '
            'btnExportPdf'
            '
            Me.btnExportPdf.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnExportPdf.Location = New System.Drawing.Point(596, 12)
            Me.btnExportPdf.Name = "btnExportPdf"
            Me.btnExportPdf.Size = New System.Drawing.Size(90, 26)
            Me.btnExportPdf.TabIndex = 6
            Me.btnExportPdf.Text = "Export PDF"
            Me.btnExportPdf.UseVisualStyleBackColor = True
            '
            'btnExportCsv'
            '
            Me.btnExportCsv.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnExportCsv.Location = New System.Drawing.Point(500, 12)
            Me.btnExportCsv.Name = "btnExportCsv"
            Me.btnExportCsv.Size = New System.Drawing.Size(90, 26)
            Me.btnExportCsv.TabIndex = 5
            Me.btnExportCsv.Text = "Export CSV"
            Me.btnExportCsv.UseVisualStyleBackColor = True
            '
            'pnlContent
            '
            Me.pnlContent.Controls.Add(Me.lblEmptyState)
            Me.pnlContent.Controls.Add(Me.dgvIncidents)
            Me.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pnlContent.Location = New System.Drawing.Point(0, 110)
            Me.pnlContent.Name = "pnlContent"
            Me.pnlContent.Padding = New System.Windows.Forms.Padding(20)
            Me.pnlContent.Size = New System.Drawing.Size(900, 490)
            Me.pnlContent.TabIndex = 2
            '
            'lblEmptyState
            '
            Me.lblEmptyState.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblEmptyState.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me.lblEmptyState.ForeColor = System.Drawing.SystemColors.ControlDarkDark
            Me.lblEmptyState.Location = New System.Drawing.Point(20, 20)
            Me.lblEmptyState.Name = "lblEmptyState"
            Me.lblEmptyState.Size = New System.Drawing.Size(860, 450)
            Me.lblEmptyState.TabIndex = 1
            Me.lblEmptyState.Text = "No tamper incidents found for the selected date range."
            Me.lblEmptyState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            Me.lblEmptyState.Visible = False
            '
            'dgvIncidents
            '
            Me.dgvIncidents.AllowUserToAddRows = False
            Me.dgvIncidents.AllowUserToDeleteRows = False
            Me.dgvIncidents.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
            Me.dgvIncidents.BackgroundColor = System.Drawing.Color.White
            Me.dgvIncidents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvIncidents.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colDetectedAt, Me.colReceiptNumber, Me.colExpectedHash, Me.colActualHash, Me.colTamperKind, Me.colDetails})
            Me.dgvIncidents.Dock = System.Windows.Forms.DockStyle.Fill
            Me.dgvIncidents.Location = New System.Drawing.Point(20, 20)
            Me.dgvIncidents.Name = "dgvIncidents"
            Me.dgvIncidents.ReadOnly = True
            Me.dgvIncidents.RowHeadersVisible = False
            Me.dgvIncidents.RowTemplate.Height = 25
            Me.dgvIncidents.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
            Me.dgvIncidents.Size = New System.Drawing.Size(860, 450)
            Me.dgvIncidents.TabIndex = 0
            Me.dgvIncidents.AutoGenerateColumns = False
            '
            'colDetectedAt
            '
            Me.colDetectedAt.DataPropertyName = "DetectedAt"
            Me.colDetectedAt.HeaderText = "Date/Time"
            Me.colDetectedAt.Name = "colDetectedAt"
            Me.colDetectedAt.ReadOnly = True
            '
            'colReceiptNumber
            '
            Me.colReceiptNumber.DataPropertyName = "ReceiptNumber"
            Me.colReceiptNumber.HeaderText = "Receipt Number"
            Me.colReceiptNumber.Name = "colReceiptNumber"
            Me.colReceiptNumber.ReadOnly = True
            '
            'colExpectedHash
            '
            Me.colExpectedHash.DataPropertyName = "ExpectedHashShort"
            Me.colExpectedHash.HeaderText = "Expected Hash"
            Me.colExpectedHash.Name = "colExpectedHash"
            Me.colExpectedHash.ReadOnly = True
            '
            'colActualHash
            '
            Me.colActualHash.DataPropertyName = "ActualHashShort"
            Me.colActualHash.HeaderText = "Actual Hash"
            Me.colActualHash.Name = "colActualHash"
            Me.colActualHash.ReadOnly = True
            '
            'colTamperKind
            '
            Me.colTamperKind.DataPropertyName = "TamperKind"
            Me.colTamperKind.HeaderText = "Tamper Kind"
            Me.colTamperKind.Name = "colTamperKind"
            Me.colTamperKind.ReadOnly = True
            '
            'colDetails
            '
            Me.colDetails.DataPropertyName = "Details"
            Me.colDetails.HeaderText = "Details"
            Me.colDetails.Name = "colDetails"
            Me.colDetails.ReadOnly = True
            '
            'TamperAuditReportView
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.pnlContent)
            Me.Controls.Add(Me.pnlFilter)
            Me.Controls.Add(Me.pnlHeader)
            Me.Name = "TamperAuditReportView"
            Me.Size = New System.Drawing.Size(900, 600)
            Me.pnlHeader.ResumeLayout(False)
            Me.pnlHeader.PerformLayout()
            Me.pnlFilter.ResumeLayout(False)
            Me.pnlFilter.PerformLayout()
            Me.pnlContent.ResumeLayout(False)
            CType(Me.dgvIncidents, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents pnlHeader As Panel
        Friend WithEvents lblTitle As Label
        Friend WithEvents pnlFilter As Panel
        Friend WithEvents pnlContent As Panel
        Friend WithEvents lblTo As Label
        Friend WithEvents dtpTo As DateTimePicker
        Friend WithEvents lblFrom As Label
        Friend WithEvents dtpFrom As DateTimePicker
        Friend WithEvents btnRefresh As Button
        Friend WithEvents dgvIncidents As DataGridView
        Friend WithEvents lblEmptyState As Label
        Friend WithEvents colDetectedAt As DataGridViewTextBoxColumn
        Friend WithEvents colReceiptNumber As DataGridViewTextBoxColumn
        Friend WithEvents colExpectedHash As DataGridViewTextBoxColumn
        Friend WithEvents colActualHash As DataGridViewTextBoxColumn
        Friend WithEvents colTamperKind As DataGridViewTextBoxColumn
        Friend WithEvents colDetails As DataGridViewTextBoxColumn
        Friend WithEvents btnExportCsv As Button
        Friend WithEvents btnExportPdf As Button
    End Class

End Namespace
