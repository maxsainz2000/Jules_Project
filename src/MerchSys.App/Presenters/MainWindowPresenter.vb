Imports System.Windows.Forms
Imports System.Collections.ObjectModel
Imports MerchSys.App.Services
Imports MerchSys.SharedKernel.Enums
Imports Microsoft.Extensions.DependencyInjection
Imports MerchSys.App.Views
Imports MerchSys.App.Models

Namespace Presenters
    Public Class MainWindowPresenter
        Private ReadOnly _view As MainWindow
        Private ReadOnly _sessionService As LoginSessionService
        Private ReadOnly _serviceScopeFactory As IServiceScopeFactory

        Public Property ActiveModule As AppModule
        Public Property ActiveModuleName As String

        Public Property NavigationGroups As New Dictionary(Of String, List(Of NavigationItem))
        Private ReadOnly _serviceProvider As IServiceProvider

        Public Sub New(view As IMainWindowView, sessionService As LoginSessionService, serviceScopeFactory As IServiceScopeFactory, serviceProvider As IServiceProvider)
            ' HACK: We receive IMainWindowView but might need to cast to Form to listen to events
            _view = DirectCast(view, MainWindow)
            _sessionService = sessionService
            _serviceScopeFactory = serviceScopeFactory
            _serviceProvider = serviceProvider

            AddHandler _view.Load, AddressOf OnViewLoad
            AddHandler _view.LogoutRequested, AddressOf OnLogoutRequested
            AddHandler _view.DashboardRequested, AddressOf OnDashboardRequested
            AddHandler _view.ModuleSelected, AddressOf OnModuleSelectedFromView
            AddHandler view.NavigationRequested, AddressOf OnNavigationRequested
        End Sub

        Private Sub OnNavigationRequested(sender As Object, item As NavigationItem)
            If item.ViewType IsNot Nothing Then
                NavigateTo(item.ViewType)
            End If
        End Sub

        Public Sub NavigateTo(viewType As Type)
            Dim viewInstance = DirectCast(_serviceProvider.GetRequiredService(viewType), UserControl)
            _view.ShowView(viewInstance)
        End Sub

        Public Sub NavigateToDefault()
            NavigateTo(GetType(StockDashboardView))
        End Sub

        Private Sub OnViewLoad(sender As Object, e As EventArgs)
            RefreshNavigation()
        End Sub
        
        Private Sub OnModuleSelectedFromView(sender As Object, moduleId As AppModule)
            SelectModule(moduleId)
        End Sub

        Private Sub RefreshNavigation()
            If _sessionService.CurrentUserRole.HasValue Then
                _view.Text = $"MerchSys - {_sessionService.CurrentUsername} ({_sessionService.CurrentUserRole.Value.ToString()})"
                _view.UserIdentityText = $"{_sessionService.CurrentUsername} ({_sessionService.CurrentUserRole.Value.ToString()})"
                
                RebuildModuleCollections()
                SelectModule(AppModule.Purchasing) ' Default module
            End If
        End Sub

        Public Sub SelectModule(moduleId As AppModule)
            ActiveModule = moduleId
            Select Case moduleId
                Case AppModule.Purchasing
                    ActiveModuleName = "Purchasing"
                Case AppModule.Inventory
                    ActiveModuleName = "Inventory"
                Case AppModule.POS
                    ActiveModuleName = "Point of Sale"
                Case AppModule.Accounting
                    ActiveModuleName = "Accounting"
                Case AppModule.DeveloperTools
                    ActiveModuleName = "Developer Tools"
            End Select
            _view.SyncActiveModuleVisuals(moduleId)

            ' To simplify, we might want to tell the view to render the navigation based on selected module.
            ' But the requirement is that it renders all nav buttons, grouped by NavigationGroup.
            ' So let's re-render just to be safe.
            _view.RenderNavigation(NavigationGroups)
        End Sub

        Public Sub RebuildModuleCollections()
            NavigationGroups.Clear()
            Dim inventoryItems As New List(Of NavigationItem)
            Dim purchasingItems As New List(Of NavigationItem)
            Dim posItems As New List(Of NavigationItem)
            Dim accountingItems As New List(Of NavigationItem)

            ' We would build these lists based on the role.
            Dim role = _sessionService.CurrentUserRole.Value

            inventoryItems.Add(New NavigationItem With { .Name = "Stock Dashboard", .ViewType = GetType(StockDashboardView), .NavigationGroup = "Inventory" })

            If role = UserRole.Manager OrElse role = UserRole.Owner Then
                NavigationGroups("Inventory") = inventoryItems
            End If

            _view.RenderNavigation(NavigationGroups)
            NavigateToDefault()
        End Sub

        Private _currentViewScope As IServiceScope

        Private Sub OnDashboardRequested(sender As Object, e As EventArgs)
            If _currentViewScope IsNot Nothing Then
                _currentViewScope.Dispose()
            End If

            _currentViewScope = _serviceScopeFactory.CreateScope()
            Dim dashboardView = _currentViewScope.ServiceProvider.GetRequiredService(Of IOwnerDashboardView)()

            _view.ShowView(DirectCast(dashboardView, UserControl))
        End Sub

        Private Sub OnLogoutRequested(sender As Object, e As EventArgs)
            _sessionService.ClearUser()
            Application.Restart()
        End Sub
    End Class
End Namespace
