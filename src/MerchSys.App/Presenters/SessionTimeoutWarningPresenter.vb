Imports System
Imports MerchSys.App.Views

Namespace Presenters
    Public Class SessionTimeoutWarningPresenter
        Private ReadOnly _view As ISessionTimeoutWarningView

        Public Sub New(view As ISessionTimeoutWarningView)
            _view = view
            AddHandler _view.StaySignedInRequested, AddressOf OnStaySignedInRequested
            AddHandler _view.SignOutRequested, AddressOf OnSignOutRequested
        End Sub

        Public Sub Tick(remainingSeconds As Integer)
            Dim ts As TimeSpan = TimeSpan.FromSeconds(remainingSeconds)
            _view.SetCountdown(String.Format("{0}:{1:D2}", ts.Minutes, ts.Seconds))
        End Sub

        Private Sub OnStaySignedInRequested(sender As Object, e As EventArgs)
            ' Handled by the shell/coordinator
        End Sub

        Private Sub OnSignOutRequested(sender As Object, e As EventArgs)
            ' Handled by the shell/coordinator
        End Sub

    End Class
End Namespace
