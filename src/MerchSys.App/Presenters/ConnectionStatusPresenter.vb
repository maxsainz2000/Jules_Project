Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports MerchSys.App.Services
Imports MerchSys.App.Views
Imports MerchSys.App.Services.Theming

Namespace Presenters
    Public Class ConnectionStatusPresenter
        Private ReadOnly _monitor As IConnectionHealthMonitor
        Private ReadOnly _view As IConnectionStatusView
        Private ReadOnly _themeService As IThemeService

        Public Sub New(monitor As IConnectionHealthMonitor, view As IConnectionStatusView)
            Me.New(monitor, view, Nothing)
        End Sub

        Public Sub New(monitor As IConnectionHealthMonitor, view As IConnectionStatusView, themeService As IThemeService)
            _monitor = monitor
            _view = view
            _themeService = themeService

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
            Dim indicatorColor As Color = If(_themeService IsNot Nothing AndAlso _themeService.Tokens IsNot Nothing, _themeService.Tokens.Danger, Color.Red)
            Dim textColor As Color = If(_themeService IsNot Nothing AndAlso _themeService.Tokens IsNot Nothing, _themeService.Tokens.PrimaryText, Color.White)
            Dim isRetryVisible As Boolean = True

            Select Case state
                Case ConnectionState.Online
                    statusText = "Online"
                    indicatorColor = If(_themeService IsNot Nothing AndAlso _themeService.Tokens IsNot Nothing, _themeService.Tokens.Success, Color.Green)
                    isRetryVisible = False
                Case ConnectionState.Reconnecting
                    statusText = "Reconnecting..."
                    indicatorColor = If(_themeService IsNot Nothing AndAlso _themeService.Tokens IsNot Nothing, _themeService.Tokens.Warning, Color.Orange)
                    isRetryVisible = False
                Case ConnectionState.Offline
                    statusText = "Offline"
                    indicatorColor = If(_themeService IsNot Nothing AndAlso _themeService.Tokens IsNot Nothing, _themeService.Tokens.Danger, Color.Red)
                    isRetryVisible = True
            End Select

            _view.SetState(statusText, indicatorColor, textColor, isRetryVisible)
        End Sub

        Private Async Sub OnRetryClicked(sender As Object, e As EventArgs)
            Await _monitor.RetryNowAsync()
        End Sub
    End Class
End Namespace
