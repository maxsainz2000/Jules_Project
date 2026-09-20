Imports MerchSys.SharedKernel.Interfaces

Namespace Presenters
    Public Class MainWindowPresenter
        Private ReadOnly _sessionService As ISessionService

        Public Event LogoutRequested As EventHandler

        Public Sub New(sessionService As ISessionService)
            _sessionService = sessionService
        End Sub

        Public ReadOnly Property CurrentRole As SharedKernel.Enums.UserRole?
            Get
                Return _sessionService.CurrentRole
            End Get
        End Property

        Public Sub HandleLogout()
            _sessionService.ClearUser()
            RaiseEvent LogoutRequested(Me, EventArgs.Empty)
        End Sub
    End Class
End Namespace
