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
        Implements IDisposable

        Private ReadOnly _view As IMainWindowView
        Private ReadOnly _sessionService As LoginSessionService
        Private ReadOnly _serviceProvider As IServiceProvider
        Private ReadOnly _connectionStatusPresenter As ConnectionStatusPresenter

        Public Property NavigationGroups As New List(Of NavigationGroup)

        Private _currentViewScope As IServiceScope
        Private _disposed As Boolean

        Public Sub New(view As IMainWindowView,
                       sessionService As LoginSessionService,
                       serviceProvider As IServiceProvider,
                       connectionStatusPresenter As ConnectionStatusPresenter)
            _view = view
            _sessionService = sessionService
            _serviceProvider = serviceProvider
            _connectionStatusPresenter = connectionStatusPresenter

            Dim mw = TryCast(_view, MainWindow)
            If mw IsNot Nothing Then
                mw.Presenter = Me
            End If

            AddHandler _view.Load, AddressOf OnViewLoad
            AddHandler _view.LogoutRequested, AddressOf OnLogoutRequested
            AddHandler _view.NavigationRequested, AddressOf OnNavigationRequested
        End Sub

        Private Sub OnViewLoad(sender As Object, e As EventArgs)
            RefreshNavigation()
        End Sub

        Private Sub RefreshNavigation()
            If Not _sessionService.CurrentUserRole.HasValue Then Return

            _view.UserIdentityText = $"{_sessionService.CurrentUsername} ({_sessionService.CurrentUserRole.Value})"

            RebuildModuleCollections()

            Dim allItems = NavigationGroups.SelectMany(Function(g) g.Items).ToList()
            _view.AddNavigationButtons(allItems)

            ' Host the connection status indicator in the shell sidebar (H3).
            If _connectionStatusPresenter IsNot Nothing Then
                _view.AddStatusIndicator(_connectionStatusPresenter.ViewControl)
            End If

            NavigateToDefault()
        End Sub

        ''' <summary>
        ''' Builds the sidebar, applying role-based visibility: Manager/Developer see every
        ''' operational view; Owner sees only the read-only subset required by INFRA-16
        ''' (verification Tests 12 and 18).
        ''' </summary>
        Public Sub RebuildModuleCollections()
            NavigationGroups.Clear()

            Dim role = _sessionService.CurrentUserRole
            Dim isManager = (role = UserRole.Manager OrElse role = UserRole.Developer)

            Dim overviewGroup As New NavigationGroup With {.GroupName = "Overview"}
            overviewGroup.Items.Add(Item("Owner Dashboard", "Overview", AddressOf ViewFactory(Of IOwnerDashboardView)))

            Dim purchasingGroup As New NavigationGroup With {.GroupName = "Purchasing"}
            purchasingGroup.Items.Add(Item("Purchase Orders", "Purchasing", AddressOf ViewFactory(Of MerchSys.Purchasing.Views.IPurchaseOrderListView)))
            purchasingGroup.Items.Add(Item("Accounts Payable", "Purchasing", AddressOf ViewFactory(Of MerchSys.Purchasing.Views.IAPLedgerView)))
            If isManager Then
                purchasingGroup.Items.Add(Item("Goods Receiving", "Purchasing", AddressOf ViewFactory(Of MerchSys.Purchasing.Views.IGoodsReceivingView)))
                purchasingGroup.Items.Add(Item("Vendor Directory", "Purchasing", AddressOf ViewFactory(Of MerchSys.Purchasing.Views.IVendorDirectoryView)))
                purchasingGroup.Items.Add(Item("Vendor Product Catalog", "Purchasing", AddressOf ViewFactory(Of MerchSys.Purchasing.Views.Purchasing.IVendorCatalogView)))
                purchasingGroup.Items.Add(Item("Reorder Suggestions", "Purchasing", AddressOf ViewFactory(Of MerchSys.Purchasing.Views.Purchasing.IReorderSuggestionsView)))
            End If

            Dim inventoryGroup As New NavigationGroup With {.GroupName = "Inventory"}
            inventoryGroup.Items.Add(Item("Stock Dashboard", "Inventory", AddressOf ViewFactory(Of MerchSys.Inventory.Views.IStockDashboardView)))
            If isManager Then
                inventoryGroup.Items.Add(Item("Product Management", "Inventory", AddressOf ViewFactory(Of MerchSys.Inventory.Views.IProductManagementView)))
                inventoryGroup.Items.Add(Item("Expiry Monitor", "Inventory", AddressOf ViewFactory(Of MerchSys.Inventory.Views.IExpiryMonitorView)))
                inventoryGroup.Items.Add(Item("Shrinkage", "Inventory", AddressOf ViewFactory(Of MerchSys.Inventory.Views.IShrinkageView)))
            End If

            Dim accountingGroup As New NavigationGroup With {.GroupName = "Accounting"}
            accountingGroup.Items.Add(Item("Financial Overview", "Accounting", AddressOf ViewFactory(Of MerchSys.Accounting.Views.IFinancialOverviewView)))
            accountingGroup.Items.Add(Item("Income Statement", "Accounting", AddressOf ViewFactory(Of MerchSys.Accounting.Views.IIncomeStatementView)))
            accountingGroup.Items.Add(Item("Sales Summary", "Accounting", AddressOf ViewFactory(Of MerchSys.Accounting.Views.ISalesSummaryView)))
            accountingGroup.Items.Add(Item("Tamper Audit Report", "Accounting", AddressOf ViewFactory(Of MerchSys.Accounting.Views.ITamperAuditReportView)))
            If isManager Then
                accountingGroup.Items.Add(Item("VAT Relief Report", "Accounting", AddressOf ViewFactory(Of MerchSys.Accounting.Views.IVatReliefReportView)))
                accountingGroup.Items.Add(Item("VAT Returns", "Accounting", AddressOf ViewFactory(Of MerchSys.Accounting.Views.IVatReturnView)))
                accountingGroup.Items.Add(Item("VAT Settings", "Accounting", AddressOf ViewFactory(Of MerchSys.POS.Views.IVatSettingsView)))
            End If

            NavigationGroups.Add(overviewGroup)
            NavigationGroups.Add(purchasingGroup)
            NavigationGroups.Add(inventoryGroup)
            NavigationGroups.Add(accountingGroup)

#If DEBUG Then
            If isManager Then
                Dim devGroup As New NavigationGroup With {.GroupName = "Developer Tools"}
                devGroup.Items.Add(Item("Developer Tools", "Developer Tools", AddressOf ViewFactory(Of DeveloperToolsPanel)))
                NavigationGroups.Add(devGroup)
            End If
#End If
        End Sub

        Private Function Item(name As String, group As String, factory As Func(Of IServiceProvider, UserControl)) As NavigationItem
            Return New NavigationItem With {.Name = name, .NavigationGroup = group, .ViewFactory = factory}
        End Function

        ''' <summary>Resolves a registered view through its interface and returns it as a control.</summary>
        Private Shared Function ViewFactory(Of TView As Class)(sp As IServiceProvider) As UserControl
            Dim resolved As Object = sp.GetRequiredService(Of TView)()
            Return DirectCast(resolved, UserControl)
        End Function

        Private Sub OnNavigationRequested(sender As Object, item As NavigationItem)
            NavigateTo(item)
        End Sub

        Public Sub NavigateToDefault()
            Dim allItems = NavigationGroups.SelectMany(Function(g) g.Items).ToList()
            Dim target As NavigationItem = Nothing

            If _sessionService.CurrentUserRole = UserRole.Owner Then
                target = allItems.FirstOrDefault(Function(i) i.Name = "Owner Dashboard")
            Else
                target = allItems.FirstOrDefault(Function(i) i.Name = "Stock Dashboard")
            End If

            If target Is Nothing Then target = allItems.FirstOrDefault()
            If target IsNot Nothing Then NavigateTo(target)
        End Sub

        Public Sub NavigateTo(item As NavigationItem)
            If item Is Nothing Then Return

            DisposeCurrentViewScope()
            _currentViewScope = _serviceProvider.CreateScope()
            Dim viewInstance = item.ViewFactory(_currentViewScope.ServiceProvider)

            Dim allItems = NavigationGroups.SelectMany(Function(g) g.Items).ToList()
            For Each navItem In allItems
                navItem.IsActive = Object.ReferenceEquals(navItem, item)
            Next
            _view.AddNavigationButtons(allItems)

            _view.RenderView(viewInstance)
        End Sub

        Private Sub OnLogoutRequested(sender As Object, e As EventArgs)
            _sessionService.ClearUser()
            DisposeCurrentViewScope()
            ' Close the shell so the login flow can run again. Replacing Application.Restart()
            ' avoids a full process restart and the disposed-singleton re-login crash (H7).
            _view.CloseWindow()
        End Sub

        Private Sub DisposeCurrentViewScope()
            If _currentViewScope IsNot Nothing Then
                _currentViewScope.Dispose()
                _currentViewScope = Nothing
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            If _disposed Then Return
            _disposed = True

            RemoveHandler _view.Load, AddressOf OnViewLoad
            RemoveHandler _view.LogoutRequested, AddressOf OnLogoutRequested
            RemoveHandler _view.NavigationRequested, AddressOf OnNavigationRequested

            ' Previously the current view scope leaked until the next navigation (L9).
            DisposeCurrentViewScope()
        End Sub
    End Class
End Namespace
