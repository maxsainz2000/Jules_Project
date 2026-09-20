Imports MerchSys.App.Services
Imports MerchSys.SharedKernel.Interfaces

Namespace Presenters
    Public Class LoginPresenter
        Private ReadOnly _authService As IAuthenticationService
        Private ReadOnly _sessionService As ISessionService

        Public Event LoginSucceeded As EventHandler
        Public Event ShowPasswordChangeRequested As EventHandler
        Public Event LoginFailed As EventHandler(Of String)
        Public Event PasswordChangeFailed As EventHandler(Of String)
        Public Event PasswordChangeSucceeded As EventHandler

        Public Sub New(authService As IAuthenticationService, sessionService As ISessionService)
            _authService = authService
            _sessionService = sessionService
        End Sub

        Public Async Function HandleLoginAsync(username As String, password As String) As Task
            Try
                Dim result = Await _authService.AuthenticateAsync(username, password)

                If result.Success Then
                    _sessionService.CurrentUser = result.User

                    If result.RequiresPasswordChange Then
                        RaiseEvent ShowPasswordChangeRequested(Me, EventArgs.Empty)
                    Else
                        RaiseEvent LoginSucceeded(Me, EventArgs.Empty)
                    End If
                Else
                    RaiseEvent LoginFailed(Me, result.ErrorMessage)
                End If
            Catch ex As Exception
                RaiseEvent LoginFailed(Me, "An error occurred during login.")
            End Try
        End Function

        Public Async Function HandlePasswordChangeAsync(newPassword As String) As Task
            Try
                Dim user = DirectCast(_sessionService.CurrentUser, SharedKernel.Entities.UserAccount)
                If user Is Nothing Then
                    RaiseEvent PasswordChangeFailed(Me, "User not authenticated.")
                    Return
                End If

                Dim result = Await _authService.ChangePasswordAsync(user.Id, newPassword)

                If result.Success Then
                    RaiseEvent PasswordChangeSucceeded(Me, EventArgs.Empty)
                    RaiseEvent LoginSucceeded(Me, EventArgs.Empty)
                Else
                    RaiseEvent PasswordChangeFailed(Me, result.ErrorMessage)
                End If
            Catch ex As Exception
                RaiseEvent PasswordChangeFailed(Me, "An error occurred during password change.")
            End Try
        End Function
    End Class
End Namespace
