Imports System.Windows.Forms

Namespace Views.Dialogs

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class RecordShrinkageDialog
        Inherits Form

        Private components As System.ComponentModel.IContainer

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
            Me.lblProduct = New Label()
            Me.cmbProduct = New ComboBox()
            Me.lblBatch = New Label()
            Me.cmbBatch = New ComboBox()
            Me.lblQuantity = New Label()
            Me.numQuantity = New NumericUpDown()
            Me.lblReason = New Label()
            Me.txtReason = New TextBox()
            Me.btnOk = New Button()
            Me.btnCancel = New Button()

            CType(Me.numQuantity, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()

            'lblProduct
            Me.lblProduct.AutoSize = True
            Me.lblProduct.Location = New System.Drawing.Point(20, 20)
            Me.lblProduct.Name = "lblProduct"
            Me.lblProduct.Size = New System.Drawing.Size(47, 15)
            Me.lblProduct.Text = "Product"

            'cmbProduct
            Me.cmbProduct.DropDownStyle = ComboBoxStyle.DropDownList
            Me.cmbProduct.Location = New System.Drawing.Point(100, 17)
            Me.cmbProduct.Name = "cmbProduct"
            Me.cmbProduct.Size = New System.Drawing.Size(250, 23)

            'lblBatch
            Me.lblBatch.AutoSize = True
            Me.lblBatch.Location = New System.Drawing.Point(20, 60)
            Me.lblBatch.Name = "lblBatch"
            Me.lblBatch.Size = New System.Drawing.Size(80, 15)
            Me.lblBatch.Text = "Batch (Optional)"

            'cmbBatch
            Me.cmbBatch.DropDownStyle = ComboBoxStyle.DropDownList
            Me.cmbBatch.Location = New System.Drawing.Point(100, 57)
            Me.cmbBatch.Name = "cmbBatch"
            Me.cmbBatch.Size = New System.Drawing.Size(250, 23)

            'lblQuantity
            Me.lblQuantity.AutoSize = True
            Me.lblQuantity.Location = New System.Drawing.Point(20, 100)
            Me.lblQuantity.Name = "lblQuantity"
            Me.lblQuantity.Size = New System.Drawing.Size(53, 15)
            Me.lblQuantity.Text = "Quantity"

            'numQuantity
            Me.numQuantity.Location = New System.Drawing.Point(100, 98)
            Me.numQuantity.Maximum = New Decimal(New Integer() {100000, 0, 0, 0})
            Me.numQuantity.Name = "numQuantity"
            Me.numQuantity.Size = New System.Drawing.Size(100, 23)

            'lblReason
            Me.lblReason.AutoSize = True
            Me.lblReason.Location = New System.Drawing.Point(20, 140)
            Me.lblReason.Name = "lblReason"
            Me.lblReason.Size = New System.Drawing.Size(45, 15)
            Me.lblReason.Text = "Reason"

            'txtReason
            Me.txtReason.Location = New System.Drawing.Point(100, 137)
            Me.txtReason.Name = "txtReason"
            Me.txtReason.Size = New System.Drawing.Size(250, 23)

            'btnOk
            Me.btnOk.Location = New System.Drawing.Point(194, 180)
            Me.btnOk.Name = "btnOk"
            Me.btnOk.Size = New System.Drawing.Size(75, 25)
            Me.btnOk.Text = "OK"

            'btnCancel
            Me.btnCancel.Location = New System.Drawing.Point(275, 180)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(75, 25)
            Me.btnCancel.Text = "Cancel"

            'RecordShrinkageDialog
            Me.ClientSize = New System.Drawing.Size(370, 220)
            Me.Controls.Add(Me.lblProduct)
            Me.Controls.Add(Me.cmbProduct)
            Me.Controls.Add(Me.lblBatch)
            Me.Controls.Add(Me.cmbBatch)
            Me.Controls.Add(Me.lblQuantity)
            Me.Controls.Add(Me.numQuantity)
            Me.Controls.Add(Me.lblReason)
            Me.Controls.Add(Me.txtReason)
            Me.Controls.Add(Me.btnOk)
            Me.Controls.Add(Me.btnCancel)
            Me.FormBorderStyle = FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "RecordShrinkageDialog"
            Me.StartPosition = FormStartPosition.CenterParent
            Me.Text = "Record Shrinkage"

            CType(Me.numQuantity, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()
        End Sub

        Friend WithEvents lblProduct As Label
        Friend WithEvents cmbProduct As ComboBox
        Friend WithEvents lblBatch As Label
        Friend WithEvents cmbBatch As ComboBox
        Friend WithEvents lblQuantity As Label
        Friend WithEvents numQuantity As NumericUpDown
        Friend WithEvents lblReason As Label
        Friend WithEvents txtReason As TextBox
        Friend WithEvents btnOk As Button
        Friend WithEvents btnCancel As Button

    End Class
End Namespace
