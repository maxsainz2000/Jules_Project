Imports System.Windows.Forms

Namespace Views.Dialogs

    Partial Class ProductEditDialog
        ''' <summary>
        ''' Required designer variable.
        ''' </summary>
        Private components As System.ComponentModel.IContainer = Nothing

        ''' <summary>
        ''' Clean up any resources being used.
        ''' </summary>
        ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso (components IsNot Nothing) Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
            Me.lblName = New System.Windows.Forms.Label()
            Me.txtName = New System.Windows.Forms.TextBox()
            Me.lblCategory = New System.Windows.Forms.Label()
            Me.cboCategory = New System.Windows.Forms.ComboBox()
            Me.lblSKU = New System.Windows.Forms.Label()
            Me.txtSKU = New System.Windows.Forms.TextBox()
            Me.lblRetailPrice = New System.Windows.Forms.Label()
            Me.nudRetailPrice = New System.Windows.Forms.NumericUpDown()
            Me.lblUnit = New System.Windows.Forms.Label()
            Me.txtUnit = New System.Windows.Forms.TextBox()
            Me.chkHasExpiry = New System.Windows.Forms.CheckBox()
            Me.lblMinThreshold = New System.Windows.Forms.Label()
            Me.nudMinThreshold = New System.Windows.Forms.NumericUpDown()
            Me.btnSave = New System.Windows.Forms.Button()
            Me.btnCancel = New System.Windows.Forms.Button()
            CType(Me.nudRetailPrice, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.nudMinThreshold, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'lblName
            '
            Me.lblName.AutoSize = True
            Me.lblName.Location = New System.Drawing.Point(20, 20)
            Me.lblName.Name = "lblName"
            Me.lblName.Size = New System.Drawing.Size(42, 15)
            Me.lblName.TabIndex = 0
            Me.lblName.Text = "Name:"
            '
            'txtName
            '
            Me.txtName.Location = New System.Drawing.Point(120, 17)
            Me.txtName.Name = "txtName"
            Me.txtName.Size = New System.Drawing.Size(200, 23)
            Me.txtName.TabIndex = 1
            '
            'lblCategory
            '
            Me.lblCategory.AutoSize = True
            Me.lblCategory.Location = New System.Drawing.Point(20, 50)
            Me.lblCategory.Name = "lblCategory"
            Me.lblCategory.Size = New System.Drawing.Size(58, 15)
            Me.lblCategory.TabIndex = 2
            Me.lblCategory.Text = "Category:"
            '
            'cboCategory
            '
            Me.cboCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cboCategory.FormattingEnabled = True
            Me.cboCategory.Location = New System.Drawing.Point(120, 47)
            Me.cboCategory.Name = "cboCategory"
            Me.cboCategory.Size = New System.Drawing.Size(200, 23)
            Me.cboCategory.TabIndex = 3
            '
            'lblSKU
            '
            Me.lblSKU.AutoSize = True
            Me.lblSKU.Location = New System.Drawing.Point(20, 80)
            Me.lblSKU.Name = "lblSKU"
            Me.lblSKU.Size = New System.Drawing.Size(31, 15)
            Me.lblSKU.TabIndex = 4
            Me.lblSKU.Text = "SKU:"
            '
            'txtSKU
            '
            Me.txtSKU.Location = New System.Drawing.Point(120, 77)
            Me.txtSKU.Name = "txtSKU"
            Me.txtSKU.Size = New System.Drawing.Size(200, 23)
            Me.txtSKU.TabIndex = 5
            '
            'lblRetailPrice
            '
            Me.lblRetailPrice.AutoSize = True
            Me.lblRetailPrice.Location = New System.Drawing.Point(20, 110)
            Me.lblRetailPrice.Name = "lblRetailPrice"
            Me.lblRetailPrice.Size = New System.Drawing.Size(68, 15)
            Me.lblRetailPrice.TabIndex = 6
            Me.lblRetailPrice.Text = "Retail Price:"
            '
            'nudRetailPrice
            '
            Me.nudRetailPrice.DecimalPlaces = 2
            Me.nudRetailPrice.Location = New System.Drawing.Point(120, 107)
            Me.nudRetailPrice.Maximum = New Decimal(New Integer() {1000000, 0, 0, 0})
            Me.nudRetailPrice.Name = "nudRetailPrice"
            Me.nudRetailPrice.Size = New System.Drawing.Size(200, 23)
            Me.nudRetailPrice.TabIndex = 7
            '
            'lblUnit
            '
            Me.lblUnit.AutoSize = True
            Me.lblUnit.Location = New System.Drawing.Point(20, 140)
            Me.lblUnit.Name = "lblUnit"
            Me.lblUnit.Size = New System.Drawing.Size(32, 15)
            Me.lblUnit.TabIndex = 8
            Me.lblUnit.Text = "Unit:"
            '
            'txtUnit
            '
            Me.txtUnit.Location = New System.Drawing.Point(120, 137)
            Me.txtUnit.Name = "txtUnit"
            Me.txtUnit.Size = New System.Drawing.Size(200, 23)
            Me.txtUnit.TabIndex = 9
            '
            'chkHasExpiry
            '
            Me.chkHasExpiry.AutoSize = True
            Me.chkHasExpiry.Location = New System.Drawing.Point(120, 170)
            Me.chkHasExpiry.Name = "chkHasExpiry"
            Me.chkHasExpiry.Size = New System.Drawing.Size(82, 19)
            Me.chkHasExpiry.TabIndex = 10
            Me.chkHasExpiry.Text = "Has Expiry"
            Me.chkHasExpiry.UseVisualStyleBackColor = True
            '
            'lblMinThreshold
            '
            Me.lblMinThreshold.AutoSize = True
            Me.lblMinThreshold.Location = New System.Drawing.Point(20, 200)
            Me.lblMinThreshold.Name = "lblMinThreshold"
            Me.lblMinThreshold.Size = New System.Drawing.Size(89, 15)
            Me.lblMinThreshold.TabIndex = 11
            Me.lblMinThreshold.Text = "Min Threshold:"
            '
            'nudMinThreshold
            '
            Me.nudMinThreshold.Location = New System.Drawing.Point(120, 197)
            Me.nudMinThreshold.Maximum = New Decimal(New Integer() {100000, 0, 0, 0})
            Me.nudMinThreshold.Name = "nudMinThreshold"
            Me.nudMinThreshold.Size = New System.Drawing.Size(200, 23)
            Me.nudMinThreshold.TabIndex = 12
            '
            'btnSave
            '
            Me.btnSave.Location = New System.Drawing.Point(164, 240)
            Me.btnSave.Name = "btnSave"
            Me.btnSave.Size = New System.Drawing.Size(75, 23)
            Me.btnSave.TabIndex = 13
            Me.btnSave.Text = "Save"
            Me.btnSave.UseVisualStyleBackColor = True
            '
            'btnCancel
            '
            Me.btnCancel.Location = New System.Drawing.Point(245, 240)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(75, 23)
            Me.btnCancel.TabIndex = 14
            Me.btnCancel.Text = "Cancel"
            Me.btnCancel.UseVisualStyleBackColor = True
            '
            'ProductEditDialog
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(350, 280)
            Me.Controls.Add(Me.btnCancel)
            Me.Controls.Add(Me.btnSave)
            Me.Controls.Add(Me.nudMinThreshold)
            Me.Controls.Add(Me.lblMinThreshold)
            Me.Controls.Add(Me.chkHasExpiry)
            Me.Controls.Add(Me.txtUnit)
            Me.Controls.Add(Me.lblUnit)
            Me.Controls.Add(Me.nudRetailPrice)
            Me.Controls.Add(Me.lblRetailPrice)
            Me.Controls.Add(Me.txtSKU)
            Me.Controls.Add(Me.lblSKU)
            Me.Controls.Add(Me.cboCategory)
            Me.Controls.Add(Me.lblCategory)
            Me.Controls.Add(Me.txtName)
            Me.Controls.Add(Me.lblName)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "ProductEditDialog"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Product"
            CType(Me.nudRetailPrice, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.nudMinThreshold, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()
        End Sub

        Friend WithEvents lblName As System.Windows.Forms.Label
        Friend WithEvents txtName As System.Windows.Forms.TextBox
        Friend WithEvents lblCategory As System.Windows.Forms.Label
        Friend WithEvents cboCategory As System.Windows.Forms.ComboBox
        Friend WithEvents lblSKU As System.Windows.Forms.Label
        Friend WithEvents txtSKU As System.Windows.Forms.TextBox
        Friend WithEvents lblRetailPrice As System.Windows.Forms.Label
        Friend WithEvents nudRetailPrice As System.Windows.Forms.NumericUpDown
        Friend WithEvents lblUnit As System.Windows.Forms.Label
        Friend WithEvents txtUnit As System.Windows.Forms.TextBox
        Friend WithEvents chkHasExpiry As System.Windows.Forms.CheckBox
        Friend WithEvents lblMinThreshold As System.Windows.Forms.Label
        Friend WithEvents nudMinThreshold As System.Windows.Forms.NumericUpDown
        Friend WithEvents btnSave As System.Windows.Forms.Button
        Friend WithEvents btnCancel As System.Windows.Forms.Button

    End Class

End Namespace
