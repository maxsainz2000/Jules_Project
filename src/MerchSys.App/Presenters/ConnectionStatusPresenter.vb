Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports MerchSys.App.Services
Imports MerchSys.App.Views

Namespace Presenters
    Public Class ConnectionStatusPresenter
        Implements IDisposable
        Private ReadOnly _monitor As IConnectionHealthMonitor
        Private ReadOnly _view As IConnectionStatusView
        Private ReadOnly _themeService As MerchSys.App.Services.Theming.IThemeService
        Private _disposed As Boolean

        Public Sub New(monitor As IConnectionHealthMonitor, view As IConnectionStatusView)
            Me.New(monitor, view, Nothing)
        End Sub

        Public Sub New(monitor As IConnectionHealthMonitor, view As IConnectionStatusView, themeService As MerchSys.App.Services.Theming.IThemeService)
            _monitor = monitor
            _view = view
            _themeService = themeService

            AddHandler _monitor.StateChanged, AddressOf OnStateChanged
            AddHandler _view.RetryClicked, AddressOf OnRetryClicked

            If _themeService IsNot Nothing Then
                AddHandler _themeService.ThemeChanged, AddressOf OnThemeChanged
            End If

            ' Initialize View State
            UpdateViewState(_monitor.CurrentState)
        End Sub

        ''' <summary>The hostable control for the shell to place in the sidebar.</summary>
        Public ReadOnly Property ViewControl As Control
            Get
                Return _view.Control
            End Get
        End Property

        Private Sub OnThemeChanged(sender As Object, theme As MerchSys.App.Services.Theming.AppTheme)
            UpdateViewState(_monitor.CurrentState)
        End Sub

        Private Sub OnStateChanged(sender As Object, e As ConnectionStateChangedEventArgs)
            UpdateViewState(e.State)
        End Sub

        Private Sub UpdateViewState(state As ConnectionState)
            Dim statusText As String = "Offline"
            Dim indicatorColor As Color = Color.Red
            Dim isRetryVisible As Boolean = True
            Dim foreColor As Color = Color.White

            If _themeService IsNot Nothing AndAlso _themeService.Tokens IsNot Nothing Then
                foreColor = _themeService.Tokens.PrimaryText
                indicatorColor = _themeService.Tokens.Danger

                Select Case state
                    Case ConnectionState.Online
                        statusText = "Online"
                        indicatorColor = _themeService.Tokens.Success
                        isRetryVisible = False
                    Case ConnectionState.Reconnecting
                        statusText = "Reconnecting..."
                        indicatorColor = _themeService.Tokens.Warning
                        isRetryVisible = False
                    Case ConnectionState.Offline
                        statusText = "Offline"
                        indicatorColor = _themeService.Tokens.Danger
                        isRetryVisible = True
                End Select
            Else
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
            End If

            _view.SetState(statusText, indicatorColor, isRetryVisible, foreColor)
        End Sub

        Private Async Sub OnRetryClicked(sender As Object, e As EventArgs)
            Await _monitor.RetryNowAsync()
        End Sub
        Public Sub Dispose() Implements IDisposable.Dispose
            If Not _disposed Then
                _disposed = True
                RemoveHandler _monitor.StateChanged, AddressOf OnStateChanged
                RemoveHandler _view.RetryClicked, AddressOf OnRetryClicked
                If _themeService IsNot Nothing Then
                    RemoveHandler _themeService.ThemeChanged, AddressOf OnThemeChanged
                End If
            End If
        End Sub
    End Class
End Namespace
