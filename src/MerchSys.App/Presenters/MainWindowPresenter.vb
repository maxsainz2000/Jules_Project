Imports System.Windows.Forms
Imports MerchSys.App.Services
Imports MerchSys.SharedKernel.Enums
Imports Microsoft.Extensions.DependencyInjection
Imports MerchSys.App.Views

Namespace Presenters
    Public Class MainWindowPresenter
        Private ReadOnly _view As MainWindow
        Private ReadOnly _sessionService As LoginSessionService
        Private ReadOnly _serviceScopeFactory As IServiceScopeFactory

        Public Sub New(view As MainWindow, sessionService As LoginSessionService, serviceScopeFactory As IServiceScopeFactory)
            _view = view
            _sessionService = sessionService
            _serviceScopeFactory = serviceScopeFactory

            AddHandler _view.Load, AddressOf OnViewLoad
            AddHandler _view.LogoutRequested, AddressOf OnLogoutRequested
            AddHandler _view.DashboardRequested, AddressOf OnDashboardRequested
        End Sub

        Private Sub OnViewLoad(sender As Object, e As EventArgs)
            RefreshNavigation()
        End Sub

        Private Sub RefreshNavigation()
            If _sessionService.CurrentUserRole.HasValue Then
                _view.Text = $"MerchSys - {_sessionService.CurrentUsername} ({_sessionService.CurrentUserRole.Value.ToString()})"
                _view.UserIdentityText = $"{_sessionService.CurrentUsername} ({_sessionService.CurrentUserRole.Value.ToString()})"
                _view.DashboardButtonVisible = (_sessionService.CurrentUserRole.Value = UserRole.Owner)
            End If
        End Sub

        Private _currentViewScope As IServiceScope

        Private Sub OnDashboardRequested(sender As Object, e As EventArgs)
            If _currentViewScope IsNot Nothing Then
                _currentViewScope.Dispose()
            End If

            _currentViewScope = _serviceScopeFactory.CreateScope()
            Dim dashboardView = _currentViewScope.ServiceProvider.GetRequiredService(Of IOwnerDashboardView)()
            Dim dashboardPresenter = _currentViewScope.ServiceProvider.GetRequiredService(Of OwnerDashboardPresenter)()

            _view.ShowView(DirectCast(dashboardView, UserControl))
        End Sub

        Private Sub OnLogoutRequested(sender As Object, e As EventArgs)
            _sessionService.ClearUser()
            Application.Restart()
        End Sub
    End Class
End Namespace
