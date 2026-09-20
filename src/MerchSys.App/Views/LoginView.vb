Imports System.Windows.Forms
Imports System.ComponentModel

Namespace Views
    Public Partial Class LoginView
        Inherits Form
        Implements ILoginView

        Public Event LoginClicked As EventHandler Implements ILoginView.LoginClicked
        Public Event ChangePasswordClicked As EventHandler Implements ILoginView.ChangePasswordClicked

        Public Sub New()
            InitializeComponent()
        End Sub

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property Username As String Implements ILoginView.Username
            Get
                Return txtUsername.Text
            End Get
            Set(value As String)
                txtUsername.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property Password As String Implements ILoginView.Password
            Get
                Return txtPassword.Text
            End Get
            Set(value As String)
                txtPassword.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property NewPassword As String Implements ILoginView.NewPassword
            Get
                Return txtNewPassword.Text
            End Get
            Set(value As String)
                txtNewPassword.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ConfirmNewPassword As String Implements ILoginView.ConfirmNewPassword
            Get
                Return txtConfirmPassword.Text
            End Get
            Set(value As String)
                txtConfirmPassword.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ErrorMessage As String Implements ILoginView.ErrorMessage
            Get
                Return lblError.Text
            End Get
            Set(value As String)
                lblError.Text = value
            End Set
        End Property

        Public Sub ShowError(message As String) Implements ILoginView.ShowError
            lblError.Text = message
            lblError.Visible = True
        End Sub

        Public Sub ClearError() Implements ILoginView.ClearError
            lblError.Text = String.Empty
            lblError.Visible = False
        End Sub

        Public Sub ShowFirstLoginPanel() Implements ILoginView.ShowFirstLoginPanel
            pnlFirstLogin.Visible = True
            btnLogin.Enabled = False
            txtUsername.Enabled = False
            txtPassword.Enabled = False
            btnTogglePassword.Enabled = False
        End Sub

        Public Sub EnableControls(enabled As Boolean) Implements ILoginView.EnableControls
            txtUsername.Enabled = enabled
            txtPassword.Enabled = enabled
            btnLogin.Enabled = enabled
            btnTogglePassword.Enabled = enabled
            txtNewPassword.Enabled = enabled
            txtConfirmPassword.Enabled = enabled
            btnChangePassword.Enabled = enabled
        End Sub

        Public Sub CloseWithSuccess() Implements ILoginView.CloseWithSuccess
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Sub

        Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
            RaiseEvent LoginClicked(Me, EventArgs.Empty)
        End Sub

        Private Sub btnChangePassword_Click(sender As Object, e As EventArgs) Handles btnChangePassword.Click
            RaiseEvent ChangePasswordClicked(Me, EventArgs.Empty)
        End Sub

        Private Sub btnTogglePassword_Click(sender As Object, e As EventArgs) Handles btnTogglePassword.Click
            txtPassword.UseSystemPasswordChar = Not txtPassword.UseSystemPasswordChar
        End Sub

        ' In order for Enter key to work
        Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
            If keyData = Keys.Enter Then
                If pnlFirstLogin.Visible Then
                    btnChangePassword.PerformClick()
                Else
                    btnLogin.PerformClick()
                End If
                Return True
            End If
            Return MyBase.ProcessCmdKey(msg, keyData)
        End Function
    End Class
End Namespace
