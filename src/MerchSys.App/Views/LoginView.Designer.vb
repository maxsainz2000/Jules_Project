Namespace Views
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class LoginView
        Inherits System.Windows.Forms.Form

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
            Me.lblTitle = New System.Windows.Forms.Label()
            Me.txtUsername = New System.Windows.Forms.TextBox()
            Me.txtPassword = New System.Windows.Forms.TextBox()
            Me.btnLogin = New System.Windows.Forms.Button()
            Me.lblError = New System.Windows.Forms.Label()
            Me.btnTogglePassword = New System.Windows.Forms.Button()
            Me.pnlFirstLogin = New System.Windows.Forms.Panel()
            Me.lblNewPassword = New System.Windows.Forms.Label()
            Me.txtNewPassword = New System.Windows.Forms.TextBox()
            Me.lblConfirmPassword = New System.Windows.Forms.Label()
            Me.txtConfirmPassword = New System.Windows.Forms.TextBox()
            Me.btnChangePassword = New System.Windows.Forms.Button()
            Me.pnlFirstLogin.SuspendLayout()
            Me.SuspendLayout()
            '
            'lblTitle
            '
            Me.lblTitle.AutoSize = True
            Me.lblTitle.Font = New System.Drawing.Font("Segoe UI", 24.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblTitle.ForeColor = System.Drawing.Color.White
            Me.lblTitle.Location = New System.Drawing.Point(50, 30)
            Me.lblTitle.Name = "lblTitle"
            Me.lblTitle.Size = New System.Drawing.Size(300, 45)
            Me.lblTitle.TabIndex = 0
            Me.lblTitle.Text = "MERCHSYS LOGIN"
            '
            'txtUsername
            '
            Me.txtUsername.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(48, Byte), Integer))
            Me.txtUsername.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.txtUsername.Font = New System.Drawing.Font("Segoe UI", 12.0!)
            Me.txtUsername.ForeColor = System.Drawing.Color.White
            Me.txtUsername.Location = New System.Drawing.Point(58, 100)
            Me.txtUsername.Name = "txtUsername"
            Me.txtUsername.PlaceholderText = "Username"
            Me.txtUsername.Size = New System.Drawing.Size(284, 29)
            Me.txtUsername.TabIndex = 1
            '
            'txtPassword
            '
            Me.txtPassword.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(48, Byte), Integer))
            Me.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.txtPassword.Font = New System.Drawing.Font("Segoe UI", 12.0!)
            Me.txtPassword.ForeColor = System.Drawing.Color.White
            Me.txtPassword.Location = New System.Drawing.Point(58, 150)
            Me.txtPassword.Name = "txtPassword"
            Me.txtPassword.PlaceholderText = "Password"
            Me.txtPassword.Size = New System.Drawing.Size(248, 29)
            Me.txtPassword.TabIndex = 2
            Me.txtPassword.UseSystemPasswordChar = True
            '
            'btnTogglePassword
            '
            Me.btnTogglePassword.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(48, Byte), Integer))
            Me.btnTogglePassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnTogglePassword.ForeColor = System.Drawing.Color.White
            Me.btnTogglePassword.Location = New System.Drawing.Point(312, 150)
            Me.btnTogglePassword.Name = "btnTogglePassword"
            Me.btnTogglePassword.Size = New System.Drawing.Size(30, 29)
            Me.btnTogglePassword.TabIndex = 3
            Me.btnTogglePassword.Text = "👁"
            Me.btnTogglePassword.UseVisualStyleBackColor = False
            '
            'btnLogin
            '
            Me.btnLogin.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(122, Byte), Integer), CType(CType(204, Byte), Integer))
            Me.btnLogin.FlatAppearance.BorderSize = 0
            Me.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnLogin.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
            Me.btnLogin.ForeColor = System.Drawing.Color.White
            Me.btnLogin.Location = New System.Drawing.Point(58, 200)
            Me.btnLogin.Name = "btnLogin"
            Me.btnLogin.Size = New System.Drawing.Size(284, 40)
            Me.btnLogin.TabIndex = 4
            Me.btnLogin.Text = "LOG IN"
            Me.btnLogin.UseVisualStyleBackColor = False
            '
            'lblError
            '
            Me.lblError.ForeColor = System.Drawing.Color.IndianRed
            Me.lblError.Location = New System.Drawing.Point(58, 250)
            Me.lblError.Name = "lblError"
            Me.lblError.Size = New System.Drawing.Size(284, 40)
            Me.lblError.TabIndex = 5
            Me.lblError.Visible = False
            '
            'pnlFirstLogin
            '
            Me.pnlFirstLogin.Controls.Add(Me.lblNewPassword)
            Me.pnlFirstLogin.Controls.Add(Me.txtNewPassword)
            Me.pnlFirstLogin.Controls.Add(Me.lblConfirmPassword)
            Me.pnlFirstLogin.Controls.Add(Me.txtConfirmPassword)
            Me.pnlFirstLogin.Controls.Add(Me.btnChangePassword)
            Me.pnlFirstLogin.Location = New System.Drawing.Point(58, 300)
            Me.pnlFirstLogin.Name = "pnlFirstLogin"
            Me.pnlFirstLogin.Size = New System.Drawing.Size(284, 180)
            Me.pnlFirstLogin.TabIndex = 6
            Me.pnlFirstLogin.Visible = False
            '
            'lblNewPassword
            '
            Me.lblNewPassword.AutoSize = True
            Me.lblNewPassword.ForeColor = System.Drawing.Color.White
            Me.lblNewPassword.Location = New System.Drawing.Point(0, 5)
            Me.lblNewPassword.Name = "lblNewPassword"
            Me.lblNewPassword.Size = New System.Drawing.Size(87, 15)
            Me.lblNewPassword.TabIndex = 0
            Me.lblNewPassword.Text = "New Password:"
            '
            'txtNewPassword
            '
            Me.txtNewPassword.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(48, Byte), Integer))
            Me.txtNewPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.txtNewPassword.Font = New System.Drawing.Font("Segoe UI", 12.0!)
            Me.txtNewPassword.ForeColor = System.Drawing.Color.White
            Me.txtNewPassword.Location = New System.Drawing.Point(0, 25)
            Me.txtNewPassword.Name = "txtNewPassword"
            Me.txtNewPassword.Size = New System.Drawing.Size(284, 29)
            Me.txtNewPassword.TabIndex = 1
            Me.txtNewPassword.UseSystemPasswordChar = True
            '
            'lblConfirmPassword
            '
            Me.lblConfirmPassword.AutoSize = True
            Me.lblConfirmPassword.ForeColor = System.Drawing.Color.White
            Me.lblConfirmPassword.Location = New System.Drawing.Point(0, 65)
            Me.lblConfirmPassword.Name = "lblConfirmPassword"
            Me.lblConfirmPassword.Size = New System.Drawing.Size(107, 15)
            Me.lblConfirmPassword.TabIndex = 2
            Me.lblConfirmPassword.Text = "Confirm Password:"
            '
            'txtConfirmPassword
            '
            Me.txtConfirmPassword.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(48, Byte), Integer))
            Me.txtConfirmPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.txtConfirmPassword.Font = New System.Drawing.Font("Segoe UI", 12.0!)
            Me.txtConfirmPassword.ForeColor = System.Drawing.Color.White
            Me.txtConfirmPassword.Location = New System.Drawing.Point(0, 85)
            Me.txtConfirmPassword.Name = "txtConfirmPassword"
            Me.txtConfirmPassword.Size = New System.Drawing.Size(284, 29)
            Me.txtConfirmPassword.TabIndex = 3
            Me.txtConfirmPassword.UseSystemPasswordChar = True
            '
            'btnChangePassword
            '
            Me.btnChangePassword.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(122, Byte), Integer), CType(CType(204, Byte), Integer))
            Me.btnChangePassword.FlatAppearance.BorderSize = 0
            Me.btnChangePassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnChangePassword.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
            Me.btnChangePassword.ForeColor = System.Drawing.Color.White
            Me.btnChangePassword.Location = New System.Drawing.Point(0, 130)
            Me.btnChangePassword.Name = "btnChangePassword"
            Me.btnChangePassword.Size = New System.Drawing.Size(284, 40)
            Me.btnChangePassword.TabIndex = 4
            Me.btnChangePassword.Text = "CHANGE PASSWORD"
            Me.btnChangePassword.UseVisualStyleBackColor = False
            '
            'LoginView
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
            Me.ClientSize = New System.Drawing.Size(400, 520)
            Me.Controls.Add(Me.pnlFirstLogin)
            Me.Controls.Add(Me.lblError)
            Me.Controls.Add(Me.btnLogin)
            Me.Controls.Add(Me.btnTogglePassword)
            Me.Controls.Add(Me.txtPassword)
            Me.Controls.Add(Me.txtUsername)
            Me.Controls.Add(Me.lblTitle)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "LoginView"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "Login"
            Me.pnlFirstLogin.ResumeLayout(False)
            Me.pnlFirstLogin.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents lblTitle As System.Windows.Forms.Label
        Friend WithEvents txtUsername As System.Windows.Forms.TextBox
        Friend WithEvents txtPassword As System.Windows.Forms.TextBox
        Friend WithEvents btnTogglePassword As System.Windows.Forms.Button
        Friend WithEvents btnLogin As System.Windows.Forms.Button
        Friend WithEvents lblError As System.Windows.Forms.Label
        Friend WithEvents pnlFirstLogin As System.Windows.Forms.Panel
        Friend WithEvents lblNewPassword As System.Windows.Forms.Label
        Friend WithEvents txtNewPassword As System.Windows.Forms.TextBox
        Friend WithEvents lblConfirmPassword As System.Windows.Forms.Label
        Friend WithEvents txtConfirmPassword As System.Windows.Forms.TextBox
        Friend WithEvents btnChangePassword As System.Windows.Forms.Button
    End Class
End Namespace
