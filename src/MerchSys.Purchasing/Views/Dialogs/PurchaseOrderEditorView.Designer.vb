Imports System.Windows.Forms

Namespace Views.Dialogs
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class PurchaseOrderEditorView
        Inherits Form

        'Form overrides dispose to clean up the component list.
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
            Me.lblVendor = New System.Windows.Forms.Label()
            Me.cmbVendors = New System.Windows.Forms.ComboBox()
            Me.lblExpectedDate = New System.Windows.Forms.Label()
            Me.dtpExpectedDate = New System.Windows.Forms.DateTimePicker()
            Me.lblNotes = New System.Windows.Forms.Label()
            Me.txtNotes = New System.Windows.Forms.TextBox()
            Me.dgvLines = New System.Windows.Forms.DataGridView()
            Me.btnAddLine = New System.Windows.Forms.Button()
            Me.btnRemoveLine = New System.Windows.Forms.Button()
            Me.lblTotal = New System.Windows.Forms.Label()
            Me.lblRunningTotal = New System.Windows.Forms.Label()
            Me.btnSave = New System.Windows.Forms.Button()
            Me.btnCancel = New System.Windows.Forms.Button()
            CType(Me.dgvLines, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'lblVendor
            '
            Me.lblVendor.AutoSize = True
            Me.lblVendor.Location = New System.Drawing.Point(13, 13)
            Me.lblVendor.Name = "lblVendor"
            Me.lblVendor.Size = New System.Drawing.Size(47, 15)
            Me.lblVendor.TabIndex = 0
            Me.lblVendor.Text = "Vendor:"
            '
            'cmbVendors
            '
            Me.cmbVendors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cmbVendors.FormattingEnabled = True
            Me.cmbVendors.Location = New System.Drawing.Point(66, 10)
            Me.cmbVendors.Name = "cmbVendors"
            Me.cmbVendors.Size = New System.Drawing.Size(200, 23)
            Me.cmbVendors.TabIndex = 1
            '
            'lblExpectedDate
            '
            Me.lblExpectedDate.AutoSize = True
            Me.lblExpectedDate.Location = New System.Drawing.Point(286, 13)
            Me.lblExpectedDate.Name = "lblExpectedDate"
            Me.lblExpectedDate.Size = New System.Drawing.Size(86, 15)
            Me.lblExpectedDate.TabIndex = 2
            Me.lblExpectedDate.Text = "Expected Date:"
            '
            'dtpExpectedDate
            '
            Me.dtpExpectedDate.Format = System.Windows.Forms.DateTimePickerFormat.Short
            Me.dtpExpectedDate.Location = New System.Drawing.Point(378, 10)
            Me.dtpExpectedDate.Name = "dtpExpectedDate"
            Me.dtpExpectedDate.Size = New System.Drawing.Size(120, 23)
            Me.dtpExpectedDate.TabIndex = 3
            '
            'lblNotes
            '
            Me.lblNotes.AutoSize = True
            Me.lblNotes.Location = New System.Drawing.Point(13, 42)
            Me.lblNotes.Name = "lblNotes"
            Me.lblNotes.Size = New System.Drawing.Size(41, 15)
            Me.lblNotes.TabIndex = 4
            Me.lblNotes.Text = "Notes:"
            '
            'txtNotes
            '
            Me.txtNotes.Location = New System.Drawing.Point(66, 39)
            Me.txtNotes.Multiline = True
            Me.txtNotes.Name = "txtNotes"
            Me.txtNotes.Size = New System.Drawing.Size(432, 50)
            Me.txtNotes.TabIndex = 5
            '
            'dgvLines
            '
            Me.dgvLines.AllowUserToAddRows = False
            Me.dgvLines.AllowUserToDeleteRows = False
            Me.dgvLines.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvLines.Location = New System.Drawing.Point(16, 124)
            Me.dgvLines.MultiSelect = False
            Me.dgvLines.Name = "dgvLines"
            Me.dgvLines.RowHeadersVisible = False
            Me.dgvLines.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
            Me.dgvLines.Size = New System.Drawing.Size(580, 200)
            Me.dgvLines.TabIndex = 6
            '
            'btnAddLine
            '
            Me.btnAddLine.Location = New System.Drawing.Point(16, 95)
            Me.btnAddLine.Name = "btnAddLine"
            Me.btnAddLine.Size = New System.Drawing.Size(75, 23)
            Me.btnAddLine.TabIndex = 7
            Me.btnAddLine.Text = "Add Line"
            Me.btnAddLine.UseVisualStyleBackColor = True
            '
            'btnRemoveLine
            '
            Me.btnRemoveLine.Location = New System.Drawing.Point(97, 95)
            Me.btnRemoveLine.Name = "btnRemoveLine"
            Me.btnRemoveLine.Size = New System.Drawing.Size(89, 23)
            Me.btnRemoveLine.TabIndex = 8
            Me.btnRemoveLine.Text = "Remove Line"
            Me.btnRemoveLine.UseVisualStyleBackColor = True
            '
            'lblTotal
            '
            Me.lblTotal.AutoSize = True
            Me.lblTotal.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblTotal.Location = New System.Drawing.Point(420, 331)
            Me.lblTotal.Name = "lblTotal"
            Me.lblTotal.Size = New System.Drawing.Size(37, 15)
            Me.lblTotal.TabIndex = 9
            Me.lblTotal.Text = "Total:"
            '
            'lblRunningTotal
            '
            Me.lblRunningTotal.AutoSize = True
            Me.lblRunningTotal.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblRunningTotal.Location = New System.Drawing.Point(463, 331)
            Me.lblRunningTotal.Name = "lblRunningTotal"
            Me.lblRunningTotal.Size = New System.Drawing.Size(38, 15)
            Me.lblRunningTotal.TabIndex = 10
            Me.lblRunningTotal.Text = "₱0.00"
            '
            'btnSave
            '
            Me.btnSave.Location = New System.Drawing.Point(440, 360)
            Me.btnSave.Name = "btnSave"
            Me.btnSave.Size = New System.Drawing.Size(75, 23)
            Me.btnSave.TabIndex = 11
            Me.btnSave.Text = "Save"
            Me.btnSave.UseVisualStyleBackColor = True
            '
            'btnCancel
            '
            Me.btnCancel.Location = New System.Drawing.Point(521, 360)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(75, 23)
            Me.btnCancel.TabIndex = 12
            Me.btnCancel.Text = "Cancel"
            Me.btnCancel.UseVisualStyleBackColor = True
            '
            'PurchaseOrderEditorView
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(610, 395)
            Me.Controls.Add(Me.btnCancel)
            Me.Controls.Add(Me.btnSave)
            Me.Controls.Add(Me.lblRunningTotal)
            Me.Controls.Add(Me.lblTotal)
            Me.Controls.Add(Me.btnRemoveLine)
            Me.Controls.Add(Me.btnAddLine)
            Me.Controls.Add(Me.dgvLines)
            Me.Controls.Add(Me.txtNotes)
            Me.Controls.Add(Me.lblNotes)
            Me.Controls.Add(Me.dtpExpectedDate)
            Me.Controls.Add(Me.lblExpectedDate)
            Me.Controls.Add(Me.cmbVendors)
            Me.Controls.Add(Me.lblVendor)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "PurchaseOrderEditorView"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Purchase Order Editor"
            CType(Me.dgvLines, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents lblVendor As Label
        Friend WithEvents cmbVendors As ComboBox
        Friend WithEvents lblExpectedDate As Label
        Friend WithEvents dtpExpectedDate As DateTimePicker
        Friend WithEvents lblNotes As Label
        Friend WithEvents txtNotes As TextBox
        Friend WithEvents dgvLines As DataGridView
        Friend WithEvents btnAddLine As Button
        Friend WithEvents btnRemoveLine As Button
        Friend WithEvents lblTotal As Label
        Friend WithEvents lblRunningTotal As Label
        Friend WithEvents btnSave As Button
        Friend WithEvents btnCancel As Button
    End Class
End Namespace