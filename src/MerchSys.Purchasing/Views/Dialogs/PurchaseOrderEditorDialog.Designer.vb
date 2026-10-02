Namespace Views.Dialogs
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class PurchaseOrderEditorDialog
        Inherits System.Windows.Forms.Form

        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        Private components As System.ComponentModel.IContainer
        Friend WithEvents cmbVendor As System.Windows.Forms.ComboBox
        Friend WithEvents txtNotes As System.Windows.Forms.TextBox
        Friend WithEvents dtpDeliveryDate As System.Windows.Forms.DateTimePicker
        Friend WithEvents dgvLines As System.Windows.Forms.DataGridView
        Friend WithEvents btnSaveDraft As System.Windows.Forms.Button
        Friend WithEvents btnSubmit As System.Windows.Forms.Button
        Friend WithEvents btnCancel As System.Windows.Forms.Button
        Friend WithEvents btnAddLine As System.Windows.Forms.Button
        Friend WithEvents btnRemoveLine As System.Windows.Forms.Button
        Friend WithEvents lblTotal As System.Windows.Forms.Label
        Friend WithEvents lblStatus As System.Windows.Forms.Label
        
        Friend WithEvents cmbProduct As System.Windows.Forms.ComboBox
        Friend WithEvents txtQuantity As System.Windows.Forms.TextBox
        Friend WithEvents txtUnitCost As System.Windows.Forms.TextBox
        Friend WithEvents txtLineTotal As System.Windows.Forms.TextBox

        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.cmbVendor = New System.Windows.Forms.ComboBox()
            Me.txtNotes = New System.Windows.Forms.TextBox()
            Me.dtpDeliveryDate = New System.Windows.Forms.DateTimePicker()
            Me.dgvLines = New System.Windows.Forms.DataGridView()
            Me.btnSaveDraft = New System.Windows.Forms.Button()
            Me.btnSubmit = New System.Windows.Forms.Button()
            Me.btnCancel = New System.Windows.Forms.Button()
            Me.btnAddLine = New System.Windows.Forms.Button()
            Me.btnRemoveLine = New System.Windows.Forms.Button()
            Me.lblTotal = New System.Windows.Forms.Label()
            Me.lblStatus = New System.Windows.Forms.Label()
            
            Me.cmbProduct = New System.Windows.Forms.ComboBox()
            Me.txtQuantity = New System.Windows.Forms.TextBox()
            Me.txtUnitCost = New System.Windows.Forms.TextBox()
            Me.txtLineTotal = New System.Windows.Forms.TextBox()
            
            CType(Me.dgvLines, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'cmbVendor
            '
            Me.cmbVendor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cmbVendor.FormattingEnabled = True
            Me.cmbVendor.Location = New System.Drawing.Point(12, 25)
            Me.cmbVendor.Name = "cmbVendor"
            Me.cmbVendor.Size = New System.Drawing.Size(200, 21)
            Me.cmbVendor.TabIndex = 0
            '
            'dtpDeliveryDate
            '
            Me.dtpDeliveryDate.Format = System.Windows.Forms.DateTimePickerFormat.Short
            Me.dtpDeliveryDate.Location = New System.Drawing.Point(230, 26)
            Me.dtpDeliveryDate.Name = "dtpDeliveryDate"
            Me.dtpDeliveryDate.Size = New System.Drawing.Size(120, 20)
            Me.dtpDeliveryDate.TabIndex = 1
            Me.dtpDeliveryDate.ShowCheckBox = True
            '
            'lblStatus
            '
            Me.lblStatus.AutoSize = True
            Me.lblStatus.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.lblStatus.Location = New System.Drawing.Point(370, 29)
            Me.lblStatus.Name = "lblStatus"
            Me.lblStatus.Size = New System.Drawing.Size(43, 13)
            Me.lblStatus.TabIndex = 2
            Me.lblStatus.Text = "Status"
            '
            'txtNotes
            '
            Me.txtNotes.Location = New System.Drawing.Point(12, 60)
            Me.txtNotes.Multiline = True
            Me.txtNotes.Name = "txtNotes"
            Me.txtNotes.Size = New System.Drawing.Size(760, 60)
            Me.txtNotes.TabIndex = 3
            Me.txtNotes.PlaceholderText = "Notes..."
            '
            'dgvLines
            '
            Me.dgvLines.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) Or System.Windows.Forms.AnchorStyles.Left) Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.dgvLines.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvLines.Location = New System.Drawing.Point(12, 160)
            Me.dgvLines.Name = "dgvLines"
            Me.dgvLines.Size = New System.Drawing.Size(760, 200)
            Me.dgvLines.TabIndex = 4
            Me.dgvLines.AllowUserToAddRows = False
            Me.dgvLines.AllowUserToDeleteRows = False
            Me.dgvLines.ReadOnly = True
            Me.dgvLines.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
            '
            'cmbProduct
            '
            Me.cmbProduct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cmbProduct.Location = New System.Drawing.Point(12, 130)
            Me.cmbProduct.Name = "cmbProduct"
            Me.cmbProduct.Size = New System.Drawing.Size(150, 21)
            Me.cmbProduct.TabIndex = 5
            '
            'txtQuantity
            '
            Me.txtQuantity.Location = New System.Drawing.Point(170, 130)
            Me.txtQuantity.Name = "txtQuantity"
            Me.txtQuantity.Size = New System.Drawing.Size(50, 20)
            Me.txtQuantity.TabIndex = 60
            Me.txtQuantity.PlaceholderText = "Qty"
            '
            'txtUnitCost
            '
            Me.txtUnitCost.Location = New System.Drawing.Point(230, 130)
            Me.txtUnitCost.Name = "txtUnitCost"
            Me.txtUnitCost.Size = New System.Drawing.Size(80, 20)
            Me.txtUnitCost.TabIndex = 6
            Me.txtUnitCost.PlaceholderText = "Unit Cost"
            '
            'txtLineTotal
            '
            Me.txtLineTotal.Location = New System.Drawing.Point(320, 130)
            Me.txtLineTotal.Name = "txtLineTotal"
            Me.txtLineTotal.Size = New System.Drawing.Size(80, 20)
            Me.txtLineTotal.TabIndex = 7
            Me.txtLineTotal.PlaceholderText = "Total"
            '
            'btnAddLine
            '
            Me.btnAddLine.Location = New System.Drawing.Point(410, 128)
            Me.btnAddLine.Name = "btnAddLine"
            Me.btnAddLine.Size = New System.Drawing.Size(75, 23)
            Me.btnAddLine.TabIndex = 8
            Me.btnAddLine.Text = "Add Line"
            Me.btnAddLine.UseVisualStyleBackColor = True
            '
            'btnRemoveLine
            '
            Me.btnRemoveLine.Location = New System.Drawing.Point(491, 128)
            Me.btnRemoveLine.Name = "btnRemoveLine"
            Me.btnRemoveLine.Size = New System.Drawing.Size(90, 23)
            Me.btnRemoveLine.TabIndex = 9
            Me.btnRemoveLine.Text = "Remove Line"
            Me.btnRemoveLine.UseVisualStyleBackColor = True
            '
            'lblTotal
            '
            Me.lblTotal.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblTotal.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.lblTotal.Location = New System.Drawing.Point(522, 363)
            Me.lblTotal.Name = "lblTotal"
            Me.lblTotal.Size = New System.Drawing.Size(250, 23)
            Me.lblTotal.TabIndex = 10
            Me.lblTotal.Text = "Total: ₱0.00"
            Me.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'btnSaveDraft
            '
            Me.btnSaveDraft.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnSaveDraft.Location = New System.Drawing.Point(535, 415)
            Me.btnSaveDraft.Name = "btnSaveDraft"
            Me.btnSaveDraft.Size = New System.Drawing.Size(75, 23)
            Me.btnSaveDraft.TabIndex = 11
            Me.btnSaveDraft.Text = "Save Draft"
            Me.btnSaveDraft.UseVisualStyleBackColor = True
            '
            'btnSubmit
            '
            Me.btnSubmit.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnSubmit.Location = New System.Drawing.Point(616, 415)
            Me.btnSubmit.Name = "btnSubmit"
            Me.btnSubmit.Size = New System.Drawing.Size(75, 23)
            Me.btnSubmit.TabIndex = 12
            Me.btnSubmit.Text = "Submit"
            Me.btnSubmit.UseVisualStyleBackColor = True
            '
            'btnCancel
            '
            Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.btnCancel.Location = New System.Drawing.Point(697, 415)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(75, 23)
            Me.btnCancel.TabIndex = 13
            Me.btnCancel.Text = "Cancel"
            Me.btnCancel.UseVisualStyleBackColor = True
            '
            'PurchaseOrderEditorDialog
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(800, 450)
            Me.Controls.Add(Me.btnCancel)
            Me.Controls.Add(Me.btnSubmit)
            Me.Controls.Add(Me.btnSaveDraft)
            Me.Controls.Add(Me.lblTotal)
            Me.Controls.Add(Me.btnRemoveLine)
            Me.Controls.Add(Me.btnAddLine)
            Me.Controls.Add(Me.txtLineTotal)
            Me.Controls.Add(Me.txtUnitCost)
            Me.Controls.Add(Me.txtQuantity)
            Me.Controls.Add(Me.cmbProduct)
            Me.Controls.Add(Me.dgvLines)
            Me.Controls.Add(Me.txtNotes)
            Me.Controls.Add(Me.lblStatus)
            Me.Controls.Add(Me.dtpDeliveryDate)
            Me.Controls.Add(Me.cmbVendor)
            Me.Name = "PurchaseOrderEditorDialog"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Purchase Order"
            CType(Me.dgvLines, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()
        End Sub
    End Class
End Namespace
