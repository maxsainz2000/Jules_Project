Namespace Views.Dialogs
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ReorderConfigEditorDialog
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
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
            Me.lblReorderThreshold = New System.Windows.Forms.Label()
            Me.numReorderThreshold = New System.Windows.Forms.NumericUpDown()
            Me.lblSafetyStock = New System.Windows.Forms.Label()
            Me.numSafetyStock = New System.Windows.Forms.NumericUpDown()
            Me.lblLeadTimeDays = New System.Windows.Forms.Label()
            Me.numLeadTimeDays = New System.Windows.Forms.NumericUpDown()
            Me.lblSeasonalMultiplier = New System.Windows.Forms.Label()
            Me.numSeasonalMultiplier = New System.Windows.Forms.NumericUpDown()
            Me.btnSave = New System.Windows.Forms.Button()
            Me.btnCancel = New System.Windows.Forms.Button()
            CType(Me.numReorderThreshold, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.numSafetyStock, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.numLeadTimeDays, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.numSeasonalMultiplier, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'lblReorderThreshold
            '
            Me.lblReorderThreshold.AutoSize = True
            Me.lblReorderThreshold.Location = New System.Drawing.Point(20, 20)
            Me.lblReorderThreshold.Name = "lblReorderThreshold"
            Me.lblReorderThreshold.Size = New System.Drawing.Size(107, 15)
            Me.lblReorderThreshold.TabIndex = 0
            Me.lblReorderThreshold.Text = "Reorder Threshold:"
            '
            'numReorderThreshold
            '
            Me.numReorderThreshold.Location = New System.Drawing.Point(140, 18)
            Me.numReorderThreshold.Maximum = New Decimal(New Integer() {100000, 0, 0, 0})
            Me.numReorderThreshold.Name = "numReorderThreshold"
            Me.numReorderThreshold.Size = New System.Drawing.Size(120, 23)
            Me.numReorderThreshold.TabIndex = 1
            '
            'lblSafetyStock
            '
            Me.lblSafetyStock.AutoSize = True
            Me.lblSafetyStock.Location = New System.Drawing.Point(20, 50)
            Me.lblSafetyStock.Name = "lblSafetyStock"
            Me.lblSafetyStock.Size = New System.Drawing.Size(73, 15)
            Me.lblSafetyStock.TabIndex = 2
            Me.lblSafetyStock.Text = "Safety Stock:"
            '
            'numSafetyStock
            '
            Me.numSafetyStock.Location = New System.Drawing.Point(140, 48)
            Me.numSafetyStock.Maximum = New Decimal(New Integer() {100000, 0, 0, 0})
            Me.numSafetyStock.Name = "numSafetyStock"
            Me.numSafetyStock.Size = New System.Drawing.Size(120, 23)
            Me.numSafetyStock.TabIndex = 3
            '
            'lblLeadTimeDays
            '
            Me.lblLeadTimeDays.AutoSize = True
            Me.lblLeadTimeDays.Location = New System.Drawing.Point(20, 80)
            Me.lblLeadTimeDays.Name = "lblLeadTimeDays"
            Me.lblLeadTimeDays.Size = New System.Drawing.Size(95, 15)
            Me.lblLeadTimeDays.TabIndex = 4
            Me.lblLeadTimeDays.Text = "Lead Time Days:"
            '
            'numLeadTimeDays
            '
            Me.numLeadTimeDays.Location = New System.Drawing.Point(140, 78)
            Me.numLeadTimeDays.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
            Me.numLeadTimeDays.Name = "numLeadTimeDays"
            Me.numLeadTimeDays.Size = New System.Drawing.Size(120, 23)
            Me.numLeadTimeDays.TabIndex = 5
            '
            'lblSeasonalMultiplier
            '
            Me.lblSeasonalMultiplier.AutoSize = True
            Me.lblSeasonalMultiplier.Location = New System.Drawing.Point(20, 110)
            Me.lblSeasonalMultiplier.Name = "lblSeasonalMultiplier"
            Me.lblSeasonalMultiplier.Size = New System.Drawing.Size(111, 15)
            Me.lblSeasonalMultiplier.TabIndex = 6
            Me.lblSeasonalMultiplier.Text = "Seasonal Multiplier:"
            '
            'numSeasonalMultiplier
            '
            Me.numSeasonalMultiplier.DecimalPlaces = 2
            Me.numSeasonalMultiplier.Location = New System.Drawing.Point(140, 108)
            Me.numSeasonalMultiplier.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
            Me.numSeasonalMultiplier.Name = "numSeasonalMultiplier"
            Me.numSeasonalMultiplier.Size = New System.Drawing.Size(120, 23)
            Me.numSeasonalMultiplier.TabIndex = 7
            '
            'btnSave
            '
            Me.btnSave.Location = New System.Drawing.Point(104, 150)
            Me.btnSave.Name = "btnSave"
            Me.btnSave.Size = New System.Drawing.Size(75, 23)
            Me.btnSave.TabIndex = 8
            Me.btnSave.Text = "Save"
            Me.btnSave.UseVisualStyleBackColor = True
            '
            'btnCancel
            '
            Me.btnCancel.Location = New System.Drawing.Point(185, 150)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(75, 23)
            Me.btnCancel.TabIndex = 9
            Me.btnCancel.Text = "Cancel"
            Me.btnCancel.UseVisualStyleBackColor = True
            '
            'ReorderConfigEditorDialog
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(284, 191)
            Me.Controls.Add(Me.btnCancel)
            Me.Controls.Add(Me.btnSave)
            Me.Controls.Add(Me.numSeasonalMultiplier)
            Me.Controls.Add(Me.lblSeasonalMultiplier)
            Me.Controls.Add(Me.numLeadTimeDays)
            Me.Controls.Add(Me.lblLeadTimeDays)
            Me.Controls.Add(Me.numSafetyStock)
            Me.Controls.Add(Me.lblSafetyStock)
            Me.Controls.Add(Me.numReorderThreshold)
            Me.Controls.Add(Me.lblReorderThreshold)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "ReorderConfigEditorDialog"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Edit Reorder Config"
            CType(Me.numReorderThreshold, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.numSafetyStock, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.numLeadTimeDays, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.numSeasonalMultiplier, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()
        End Sub

        Friend WithEvents lblReorderThreshold As System.Windows.Forms.Label
        Friend WithEvents numReorderThreshold As System.Windows.Forms.NumericUpDown
        Friend WithEvents lblSafetyStock As System.Windows.Forms.Label
        Friend WithEvents numSafetyStock As System.Windows.Forms.NumericUpDown
        Friend WithEvents lblLeadTimeDays As System.Windows.Forms.Label
        Friend WithEvents numLeadTimeDays As System.Windows.Forms.NumericUpDown
        Friend WithEvents lblSeasonalMultiplier As System.Windows.Forms.Label
        Friend WithEvents numSeasonalMultiplier As System.Windows.Forms.NumericUpDown
        Friend WithEvents btnSave As System.Windows.Forms.Button
        Friend WithEvents btnCancel As System.Windows.Forms.Button
    End Class
End Namespace
