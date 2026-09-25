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

        Public Property PurchasingItems As New List(Of NavigationItem)
        Public Property InventoryItems As New List(Of NavigationItem)
        Public Property PosItems As New List(Of NavigationItem)
        Public Property AccountingItems As New List(Of NavigationItem)
        Public Property DeveloperToolsItems As New List(Of NavigationItem)

        Public Event ActiveModuleChanged As EventHandler(Of AppModule)

        Public Sub New(view As MainWindow, sessionService As LoginSessionService, serviceScopeFactory As IServiceScopeFactory)
            _view = view
            _sessionService = sessionService
            _serviceScopeFactory = serviceScopeFactory

            AddHandler _view.Load, AddressOf OnViewLoad
            AddHandler _view.LogoutRequested, AddressOf OnLogoutRequested
            AddHandler _view.DashboardRequested, AddressOf OnDashboardRequested
            AddHandler _view.ModuleSelected, AddressOf OnModuleSelectedFromView
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
            RaiseEvent ActiveModuleChanged(Me, moduleId)
        End Sub

        Public Sub RebuildModuleCollections()
            PurchasingItems.Clear()
            InventoryItems.Clear()
            PosItems.Clear()
            AccountingItems.Clear()
            DeveloperToolsItems.Clear()

            ' We would build these lists based on the role.
            Dim role = _sessionService.CurrentUserRole.Value
            If role = UserRole.Manager Then
                ' Add mock Manager nav items for demonstration or matching previous structure
                PurchasingItems.Add(New NavigationItem With { .DisplayName = "Purchase Orders" })
                PurchasingItems.Add(New NavigationItem With { .DisplayName = "Goods Receiving" })
                PurchasingItems.Add(New NavigationItem With { .DisplayName = "Vendor Directory" })
                PurchasingItems.Add(New NavigationItem With { .DisplayName = "Accounts Payable" })
                
                InventoryItems.Add(New NavigationItem With { .DisplayName = "Reorder Suggestions" })
                InventoryItems.Add(New NavigationItem With { .DisplayName = "Stock Dashboard" })
                InventoryItems.Add(New NavigationItem With { .DisplayName = "Product Management" })
                InventoryItems.Add(New NavigationItem With { .DisplayName = "Expiry Monitor" })
                InventoryItems.Add(New NavigationItem With { .DisplayName = "Shrinkage" })
                
                PosItems.Add(New NavigationItem With { .DisplayName = "Sales Cart" })
                PosItems.Add(New NavigationItem With { .DisplayName = "Credit Management" })
                PosItems.Add(New NavigationItem With { .DisplayName = "Transaction History" })
                PosItems.Add(New NavigationItem With { .DisplayName = "Daily Summary" })
                PosItems.Add(New NavigationItem With { .DisplayName = "VAT Settings" })
                PosItems.Add(New NavigationItem With { .DisplayName = "Tamper Audit Report" })
                
                AccountingItems.Add(New NavigationItem With { .DisplayName = "Financial Overview" })
                AccountingItems.Add(New NavigationItem With { .DisplayName = "Income Statement" })
                AccountingItems.Add(New NavigationItem With { .DisplayName = "Sales Summary" })
                AccountingItems.Add(New NavigationItem With { .DisplayName = "VAT Return (BIR)" })
                
#If DEBUG Then
                DeveloperToolsItems.Add(New NavigationItem With { .DisplayName = "Developer Tools" })
#End If
            ElseIf role = UserRole.Owner Then
                PurchasingItems.Add(New NavigationItem With { .DisplayName = "Purchase Orders" })
                PurchasingItems.Add(New NavigationItem With { .DisplayName = "Accounts Payable" })
                
                InventoryItems.Add(New NavigationItem With { .DisplayName = "Stock Dashboard" })
                
                PosItems.Add(New NavigationItem With { .DisplayName = "Transaction History" })
                
                AccountingItems.Add(New NavigationItem With { .DisplayName = "Financial Overview" })
                AccountingItems.Add(New NavigationItem With { .DisplayName = "Income Statement" })
                AccountingItems.Add(New NavigationItem With { .DisplayName = "Sales Summary" })
            End If
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
