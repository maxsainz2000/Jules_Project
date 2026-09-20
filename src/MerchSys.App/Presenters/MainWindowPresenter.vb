Imports System.Windows.Forms
Imports MerchSys.App.Services

Namespace Presenters
    Public Class MainWindowPresenter
        Private ReadOnly _view As MainWindow
        Private ReadOnly _sessionService As LoginSessionService

        Public Sub New(view As MainWindow, sessionService As LoginSessionService)
            _view = view
            _sessionService = sessionService

            AddHandler _view.Load, AddressOf OnViewLoad
            AddHandler _view.LogoutRequested, AddressOf OnLogoutRequested
        End Sub

        Private Sub OnViewLoad(sender As Object, e As EventArgs)
            RefreshNavigation()
        End Sub

        Private Sub RefreshNavigation()
            If _sessionService.CurrentUserRole.HasValue Then
                _view.Text = $"MerchSys - {_sessionService.CurrentUsername} ({_sessionService.CurrentUserRole.Value.ToString()})"
                ' We can show/hide navigation buttons based on role here in the future
            End If
        End Sub

        Private Sub OnLogoutRequested(sender As Object, e As EventArgs)
            _sessionService.ClearUser()
            Application.Restart()
        End Sub
    End Class
End Namespace
