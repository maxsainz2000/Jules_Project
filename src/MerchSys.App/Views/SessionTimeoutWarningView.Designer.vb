Imports System.Drawing
Imports System.Windows.Forms

Namespace Views
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class SessionTimeoutWarningView
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
            Me.lblMessage = New System.Windows.Forms.Label()
            Me.lblCountdown = New System.Windows.Forms.Label()
            Me.btnStaySignedIn = New System.Windows.Forms.Button()
            Me.btnSignOut = New System.Windows.Forms.Button()
            Me.SuspendLayout()
            '
            'lblMessage
            '
            Me.lblMessage.AutoSize = True
            Me.lblMessage.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me.lblMessage.Location = New System.Drawing.Point(50, 30)
            Me.lblMessage.Name = "lblMessage"
            Me.lblMessage.Size = New System.Drawing.Size(300, 21)
            Me.lblMessage.TabIndex = 0
            Me.lblMessage.Text = "Your session is about to expire due to inactivity."
            Me.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'lblCountdown
            '
            Me.lblCountdown.AutoSize = True
            Me.lblCountdown.Font = New System.Drawing.Font("Segoe UI", 36.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblCountdown.Location = New System.Drawing.Point(140, 70)
            Me.lblCountdown.Name = "lblCountdown"
            Me.lblCountdown.Size = New System.Drawing.Size(120, 65)
            Me.lblCountdown.TabIndex = 1
            Me.lblCountdown.Text = "1:00"
            Me.lblCountdown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'btnStaySignedIn
            '
            Me.btnStaySignedIn.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(122, Byte), Integer), CType(CType(204, Byte), Integer))
            Me.btnStaySignedIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnStaySignedIn.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.btnStaySignedIn.Location = New System.Drawing.Point(50, 160)
            Me.btnStaySignedIn.Name = "btnStaySignedIn"
            Me.btnStaySignedIn.Size = New System.Drawing.Size(140, 40)
            Me.btnStaySignedIn.TabIndex = 2
            Me.btnStaySignedIn.Text = "Stay Signed In"
            Me.btnStaySignedIn.UseVisualStyleBackColor = False
            '
            'btnSignOut
            '
            Me.btnSignOut.BackColor = System.Drawing.Color.FromArgb(CType(CType(62, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(66, Byte), Integer))
            Me.btnSignOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnSignOut.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me.btnSignOut.Location = New System.Drawing.Point(210, 160)
            Me.btnSignOut.Name = "btnSignOut"
            Me.btnSignOut.Size = New System.Drawing.Size(140, 40)
            Me.btnSignOut.TabIndex = 3
            Me.btnSignOut.Text = "Sign Out Now"
            Me.btnSignOut.UseVisualStyleBackColor = False
            '
            'SessionTimeoutWarningView
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(400, 230)
            Me.Controls.Add(Me.btnSignOut)
            Me.Controls.Add(Me.btnStaySignedIn)
            Me.Controls.Add(Me.lblCountdown)
            Me.Controls.Add(Me.lblMessage)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "SessionTimeoutWarningView"
            Me.Text = "Session Timeout Warning"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents lblMessage As System.Windows.Forms.Label
        Friend WithEvents lblCountdown As System.Windows.Forms.Label
        Friend WithEvents btnStaySignedIn As System.Windows.Forms.Button
        Friend WithEvents btnSignOut As System.Windows.Forms.Button
    End Class
End Namespace
