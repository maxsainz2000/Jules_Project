Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports MerchSys.App.Services
Imports MerchSys.App.Views

Namespace Presenters
    Public Class ConnectionStatusPresenter
        Private ReadOnly _monitor As IConnectionHealthMonitor
        Private ReadOnly _view As IConnectionStatusView

        Public Sub New(monitor As IConnectionHealthMonitor, view As IConnectionStatusView)
            _monitor = monitor
            _view = view

            AddHandler _monitor.StateChanged, AddressOf OnStateChanged
            AddHandler _view.RetryClicked, AddressOf OnRetryClicked

            ' Initialize View State
            UpdateViewState(_monitor.CurrentState)
        End Sub

        ''' <summary>The hostable control for the shell to place in the sidebar.</summary>
        Public ReadOnly Property ViewControl As Control
            Get
                Return _view.Control
            End Get
        End Property

        Private Sub OnStateChanged(sender As Object, e As ConnectionStateChangedEventArgs)
            UpdateViewState(e.State)
        End Sub

        Private Sub UpdateViewState(state As ConnectionState)
            Dim statusText As String = "Offline"
            Dim indicatorColor As Color = Color.Red
            Dim isRetryVisible As Boolean = True

            Select Case state
                Case ConnectionState.Online
                    statusText = "Online"
                    indicatorColor = Color.Green
                    isRetryVisible = False
                Case ConnectionState.Reconnecting
                    statusText = "Reconnecting..."
                    indicatorColor = Color.Orange
                    isRetryVisible = False
                Case ConnectionState.Offline
                    statusText = "Offline"
                    indicatorColor = Color.Red
                    isRetryVisible = True
            End Select

            _view.SetState(statusText, indicatorColor, isRetryVisible)
        End Sub

        Private Async Sub OnRetryClicked(sender As Object, e As EventArgs)
            Await _monitor.RetryNowAsync()
        End Sub
    End Class
End Namespace
