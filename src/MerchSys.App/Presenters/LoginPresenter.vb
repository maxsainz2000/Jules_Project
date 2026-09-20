Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.App.Views
Imports MerchSys.App.Services

Namespace Presenters
    Public Class LoginPresenter
        Private ReadOnly _view As ILoginView
        Private ReadOnly _authService As IAuthenticationService
        Private ReadOnly _sessionService As LoginSessionService

        Public Event LoginSucceeded As EventHandler

        Public Sub New(view As ILoginView, authService As IAuthenticationService, sessionService As LoginSessionService)
            _view = view
            _authService = authService
            _sessionService = sessionService

            AddHandler _view.LoginClicked, AddressOf OnLoginClicked
            AddHandler _view.ChangePasswordClicked, AddressOf OnChangePasswordClicked
        End Sub

        Private Async Sub OnLoginClicked(sender As Object, e As EventArgs)
            _view.ClearError()
            _view.EnableControls(False)

            Try
                Dim result = Await _authService.AuthenticateAsync(_view.Username, _view.Password)

                If result.Success Then
                    If result.RequiresPasswordChange Then
                        _view.ShowFirstLoginPanel()
                    Else
                        _sessionService.SetUser(result.User)
                        RaiseEvent LoginSucceeded(Me, EventArgs.Empty)
                        _view.CloseWithSuccess()
                    End If
                Else
                    _view.ShowError(result.ErrorMessage)
                End If
            Catch ex As Exception
                _view.ShowError("An unexpected error occurred during login.")
            Finally
                _view.EnableControls(True)
            End Try
        End Sub

        Private Async Sub OnChangePasswordClicked(sender As Object, e As EventArgs)
            _view.ClearError()

            If String.IsNullOrWhiteSpace(_view.NewPassword) Then
                _view.ShowError("New password is required.")
                Return
            End If

            If _view.NewPassword <> _view.ConfirmNewPassword Then
                _view.ShowError("Passwords do not match.")
                Return
            End If

            _view.EnableControls(False)

            Try
                Dim result = Await _authService.ChangePasswordForFirstLoginAsync(_view.Username, _view.Password, _view.NewPassword)

                If result.Success Then
                    ' Re-authenticate to get the updated user
                    Dim authResult = Await _authService.AuthenticateAsync(_view.Username, _view.NewPassword)
                    If authResult.Success Then
                        _sessionService.SetUser(authResult.User)
                        RaiseEvent LoginSucceeded(Me, EventArgs.Empty)
                        _view.CloseWithSuccess()
                    Else
                        _view.ShowError("Password changed, but login failed. Please restart.")
                    End If
                Else
                    _view.ShowError(result.ErrorMessage)
                End If
            Catch ex As Exception
                _view.ShowError("An unexpected error occurred while changing password.")
            Finally
                _view.EnableControls(True)
            End Try
        End Sub
    End Class
End Namespace
