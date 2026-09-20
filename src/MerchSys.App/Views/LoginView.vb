Imports System.Windows.Forms
Imports MerchSys.App.Presenters

Public Class LoginView
    Inherits Form

    Private _presenter As LoginPresenter

    Public Sub SetPresenter(presenter As LoginPresenter)
        _presenter = presenter
        AddHandler _presenter.LoginSucceeded, AddressOf OnLoginSucceeded
        AddHandler _presenter.LoginFailed, AddressOf OnLoginFailed
        AddHandler _presenter.ShowPasswordChangeRequested, AddressOf OnShowPasswordChangeRequested
        AddHandler _presenter.PasswordChangeSucceeded, AddressOf OnPasswordChangeSucceeded
        AddHandler _presenter.PasswordChangeFailed, AddressOf OnPasswordChangeFailed
    End Sub

    Private Async Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        lblError.Text = ""
        btnLogin.Enabled = False
        Await _presenter.HandleLoginAsync(txtUsername.Text, txtPassword.Text)
        btnLogin.Enabled = True
    End Sub

    Private Async Sub btnChangePassword_Click(sender As Object, e As EventArgs) Handles btnChangePassword.Click
        If txtNewPassword.Text <> txtConfirmPassword.Text Then
            lblChangePasswordError.Text = "Passwords do not match."
            Return
        End If

        lblChangePasswordError.Text = ""
        btnChangePassword.Enabled = False
        Await _presenter.HandlePasswordChangeAsync(txtNewPassword.Text)
        btnChangePassword.Enabled = True
    End Sub

    Private Sub OnLoginSucceeded(sender As Object, e As EventArgs)
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub OnLoginFailed(sender As Object, errorMessage As String)
        lblError.Text = errorMessage
    End Sub

    Private Sub OnShowPasswordChangeRequested(sender As Object, e As EventArgs)
        pnlLogin.Visible = False
        pnlChangePassword.Visible = True
    End Sub

    Private Sub OnPasswordChangeSucceeded(sender As Object, e As EventArgs)
        ' LoginSucceeded will also be raised by the presenter
    End Sub

    Private Sub OnPasswordChangeFailed(sender As Object, errorMessage As String)
        lblChangePasswordError.Text = errorMessage
    End Sub

    Private Sub btnTogglePassword_Click(sender As Object, e As EventArgs) Handles btnTogglePassword.Click
        If txtPassword.UseSystemPasswordChar Then
            txtPassword.UseSystemPasswordChar = False
            btnTogglePassword.Text = "Hide"
        Else
            txtPassword.UseSystemPasswordChar = True
            btnTogglePassword.Text = "Show"
        End If
    End Sub
End Class
