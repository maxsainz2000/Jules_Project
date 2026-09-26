Imports System.Windows.Forms
Imports System.Collections.ObjectModel
Imports MerchSys.App.Services
Imports MerchSys.SharedKernel.Enums
Imports Microsoft.Extensions.DependencyInjection
Imports MerchSys.App.Views
Imports MerchSys.App.Models
Imports MerchSys.App.Views.Shell.Modules

Namespace Presenters
    Public Class MainWindowPresenter
        Private ReadOnly _view As IMainWindowView
        Private ReadOnly _sessionService As LoginSessionService
        Private ReadOnly _serviceProvider As IServiceProvider

        Public Property NavigationGroups As New List(Of NavigationGroup)

        Private _currentViewScope As IServiceScope

        Public Sub New(view As IMainWindowView, sessionService As LoginSessionService, serviceProvider As IServiceProvider)
            _view = view
            _sessionService = sessionService
            _serviceProvider = serviceProvider

            AddHandler _view.Load, AddressOf OnViewLoad
            AddHandler _view.LogoutRequested, AddressOf OnLogoutRequested
            AddHandler _view.NavigationRequested, AddressOf OnNavigationRequested
        End Sub

        Private Sub OnViewLoad(sender As Object, e As EventArgs)
            RefreshNavigation()
        End Sub
        
        Private Sub RefreshNavigation()
            If _sessionService.CurrentUserRole.HasValue Then
                Dim mainView = TryCast(_view, MainWindow)
                If mainView IsNot Nothing Then
                    mainView.Text = $"MerchSys - {_sessionService.CurrentUsername} ({_sessionService.CurrentUserRole.Value.ToString()})"
                End If
                
                RebuildModuleCollections()

                Dim allItems = NavigationGroups.SelectMany(Function(g) g.Items).ToList()
                _view.AddNavigationButtons(allItems)

                NavigateToDefault()
            End If
        End Sub

        Public Sub RebuildModuleCollections()
            NavigationGroups.Clear()

            Dim purchasingGroup As New NavigationGroup With { .GroupName = "Purchasing" }
            Dim inventoryGroup As New NavigationGroup With { .GroupName = "Inventory" }
            Dim posGroup As New NavigationGroup With { .GroupName = "POS" }
            Dim accountingGroup As New NavigationGroup With { .GroupName = "Accounting" }

            ' Add mock nav items
            purchasingGroup.Items.Add(New NavigationItem With { .Name = "Purchasing Panel", .ViewType = GetType(PurchasingPanel), .NavigationGroup = "Purchasing" })
            inventoryGroup.Items.Add(New NavigationItem With { .Name = "Inventory Panel", .ViewType = GetType(InventoryPanel), .NavigationGroup = "Inventory" })
            posGroup.Items.Add(New NavigationItem With { .Name = "POS Panel", .ViewType = GetType(PosPanel), .NavigationGroup = "POS" })
            accountingGroup.Items.Add(New NavigationItem With { .Name = "Accounting Panel", .ViewType = GetType(AccountingPanel), .NavigationGroup = "Accounting" })

            NavigationGroups.Add(purchasingGroup)
            NavigationGroups.Add(inventoryGroup)
            NavigationGroups.Add(posGroup)
            NavigationGroups.Add(accountingGroup)

#If DEBUG Then
            Dim devGroup As New NavigationGroup With { .GroupName = "Developer Tools" }
            devGroup.Items.Add(New NavigationItem With { .Name = "Developer Tools", .ViewType = GetType(DeveloperToolsPanel), .NavigationGroup = "Developer Tools" })
            NavigationGroups.Add(devGroup)
#End If
        End Sub

        Private Sub OnNavigationRequested(sender As Object, viewType As Type)
            NavigateTo(viewType)
        End Sub

        Public Sub NavigateToDefault()
            Dim firstGroup = NavigationGroups.FirstOrDefault()
            If firstGroup IsNot Nothing AndAlso firstGroup.Items.Any() Then
                NavigateTo(firstGroup.Items.First().ViewType)
            End If
        End Sub

        Public Sub NavigateTo(viewType As Type)
            If _currentViewScope IsNot Nothing Then
                _currentViewScope.Dispose()
            End If

            _currentViewScope = _serviceProvider.CreateScope()
            Dim viewInstance = DirectCast(_currentViewScope.ServiceProvider.GetRequiredService(viewType), UserControl)

            ' Update active state
            Dim allItems = NavigationGroups.SelectMany(Function(g) g.Items).ToList()
            For Each item In allItems
                item.IsActive = (item.ViewType = viewType)
            Next
            _view.AddNavigationButtons(allItems)

            _view.RenderView(viewInstance)
        End Sub

        Private Sub OnLogoutRequested(sender As Object, e As EventArgs)
            _sessionService.ClearUser()
            Application.Restart()
        End Sub
    End Class
End Namespace
