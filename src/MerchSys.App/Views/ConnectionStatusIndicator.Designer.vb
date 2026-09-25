Imports System
Imports System.Drawing
Imports System.Windows.Forms

Namespace Views
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ConnectionStatusIndicator
    Inherits System.Windows.Forms.UserControl

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

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.pnlIndicator = New System.Windows.Forms.Panel()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.btnRetry = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'pnlIndicator
        '
        Me.pnlIndicator.Location = New System.Drawing.Point(5, 7)
        Me.pnlIndicator.Name = "pnlIndicator"
        Me.pnlIndicator.Size = New System.Drawing.Size(10, 10)
        Me.pnlIndicator.TabIndex = 0
        Me.pnlIndicator.BackColor = System.Drawing.Color.Red
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.ForeColor = System.Drawing.Color.White
        Me.lblStatus.Location = New System.Drawing.Point(20, 5)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(43, 15)
        Me.lblStatus.TabIndex = 1
        Me.lblStatus.Text = "Offline"
        '
        'btnRetry
        '
        Me.btnRetry.BackColor = System.Drawing.Color.FromArgb(CType(CType(62, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(66, Byte), Integer))
        Me.btnRetry.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnRetry.ForeColor = System.Drawing.Color.White
        Me.btnRetry.Location = New System.Drawing.Point(130, 2)
        Me.btnRetry.Name = "btnRetry"
        Me.btnRetry.Size = New System.Drawing.Size(50, 22)
        Me.btnRetry.TabIndex = 2
        Me.btnRetry.Text = "Retry"
        Me.btnRetry.UseVisualStyleBackColor = False
        Me.btnRetry.Visible = False
        Me.btnRetry.FlatAppearance.BorderSize = 0
        '
        'ConnectionStatusIndicator
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Transparent
        Me.Controls.Add(Me.btnRetry)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.pnlIndicator)
        Me.Name = "ConnectionStatusIndicator"
        Me.Size = New System.Drawing.Size(200, 24)
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents pnlIndicator As System.Windows.Forms.Panel
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents btnRetry As System.Windows.Forms.Button

End Class
End Namespace
