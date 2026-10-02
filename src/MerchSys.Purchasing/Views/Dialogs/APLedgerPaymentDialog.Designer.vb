Imports System.Drawing
Imports System.Windows.Forms

Namespace Views.Dialogs
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class APLedgerPaymentDialog
        Inherits Form

        'Form overrides dispose to clean up the component list.
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
            Me.lblInvoiceDetails = New System.Windows.Forms.Label()
            Me.lblBalance = New System.Windows.Forms.Label()
            Me.lblPaymentAmount = New System.Windows.Forms.Label()
            Me.txtPaymentAmount = New System.Windows.Forms.TextBox()
            Me.btnOK = New System.Windows.Forms.Button()
            Me.btnCancel = New System.Windows.Forms.Button()
            Me.SuspendLayout()
            '
            'lblInvoiceDetails
            '
            Me.lblInvoiceDetails.AutoSize = True
            Me.lblInvoiceDetails.Location = New System.Drawing.Point(20, 20)
            Me.lblInvoiceDetails.Name = "lblInvoiceDetails"
            Me.lblInvoiceDetails.Size = New System.Drawing.Size(84, 15)
            Me.lblInvoiceDetails.TabIndex = 0
            Me.lblInvoiceDetails.Text = "Invoice Details"
            '
            'lblBalance
            '
            Me.lblBalance.AutoSize = True
            Me.lblBalance.Location = New System.Drawing.Point(20, 50)
            Me.lblBalance.Name = "lblBalance"
            Me.lblBalance.Size = New System.Drawing.Size(82, 15)
            Me.lblBalance.TabIndex = 1
            Me.lblBalance.Text = "Balance: ₱0.00"
            '
            'lblPaymentAmount
            '
            Me.lblPaymentAmount.AutoSize = True
            Me.lblPaymentAmount.Location = New System.Drawing.Point(20, 85)
            Me.lblPaymentAmount.Name = "lblPaymentAmount"
            Me.lblPaymentAmount.Size = New System.Drawing.Size(102, 15)
            Me.lblPaymentAmount.TabIndex = 2
            Me.lblPaymentAmount.Text = "Payment Amount:"
            '
            'txtPaymentAmount
            '
            Me.txtPaymentAmount.Location = New System.Drawing.Point(140, 82)
            Me.txtPaymentAmount.Name = "txtPaymentAmount"
            Me.txtPaymentAmount.Size = New System.Drawing.Size(120, 23)
            Me.txtPaymentAmount.TabIndex = 3
            Me.txtPaymentAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
            '
            'btnOK
            '
            Me.btnOK.Location = New System.Drawing.Point(100, 130)
            Me.btnOK.Name = "btnOK"
            Me.btnOK.Size = New System.Drawing.Size(75, 25)
            Me.btnOK.TabIndex = 4
            Me.btnOK.Text = "Confirm"
            Me.btnOK.UseVisualStyleBackColor = True
            '
            'btnCancel
            '
            Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.btnCancel.Location = New System.Drawing.Point(185, 130)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(75, 25)
            Me.btnCancel.TabIndex = 5
            Me.btnCancel.Text = "Cancel"
            Me.btnCancel.UseVisualStyleBackColor = True
            '
            'APLedgerPaymentDialog
            '
            Me.AcceptButton = Me.btnOK
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.btnCancel
            Me.ClientSize = New System.Drawing.Size(294, 171)
            Me.Controls.Add(Me.btnCancel)
            Me.Controls.Add(Me.btnOK)
            Me.Controls.Add(Me.txtPaymentAmount)
            Me.Controls.Add(Me.lblPaymentAmount)
            Me.Controls.Add(Me.lblBalance)
            Me.Controls.Add(Me.lblInvoiceDetails)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "APLedgerPaymentDialog"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Record Payment"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents lblInvoiceDetails As Label
        Friend WithEvents lblBalance As Label
        Friend WithEvents lblPaymentAmount As Label
        Friend WithEvents txtPaymentAmount As TextBox
        Friend WithEvents btnOK As Button
        Friend WithEvents btnCancel As Button
    End Class
End Namespace
