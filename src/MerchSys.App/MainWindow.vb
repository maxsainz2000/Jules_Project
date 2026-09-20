Imports MerchSys.App.Presenters

Public Class MainWindow
    Private _presenter As MainWindowPresenter

    Public Sub SetPresenter(presenter As MainWindowPresenter)
        _presenter = presenter
        AddHandler _presenter.LogoutRequested, AddressOf OnLogoutRequested

        ' Example of role-based navigation refresh
        If _presenter.CurrentRole.HasValue Then
            ' Configure UI based on role
        End If
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs) Handles btnLogout.Click
        _presenter.HandleLogout()
    End Sub

    Private Sub OnLogoutRequested(sender As Object, e As EventArgs)
        Me.Close()
        Application.Restart() ' Simplest way to restart app and show login again
    End Sub
End Class
