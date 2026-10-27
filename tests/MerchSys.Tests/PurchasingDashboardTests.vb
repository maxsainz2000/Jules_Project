Imports MerchSys.Purchasing.Data
Imports MerchSys.Purchasing.Dtos
Imports MerchSys.Purchasing.Entities
Imports MerchSys.Purchasing.Presenters
Imports MerchSys.Purchasing.Views
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Interfaces
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Diagnostics
Imports Moq
Imports Xunit
Imports System.Windows.Forms
Imports MerchSys.Purchasing.Views.Purchasing
Imports MerchSys.App.Presenters
Imports MerchSys.App.Views
Imports MerchSys.App.Services
Imports Microsoft.Extensions.DependencyInjection

Namespace Tests
    Public Class PurchasingDashboardTests
        Implements IDisposable

        Private ReadOnly _dbContext As PurchasingDbContext
        Private ReadOnly _mockView As Mock(Of IPurchasingDashboardView)
        Private ReadOnly _mockNotification As Mock(Of INotificationService)
        Private ReadOnly _presenter As PurchasingDashboardPresenter

        Public Sub New()
            Dim options = New DbContextOptionsBuilder(Of PurchasingDbContext)().
                UseInMemoryDatabase(databaseName:=Guid.NewGuid().ToString()).
                ConfigureWarnings(Function(w) w.Ignore(InMemoryEventId.TransactionIgnoredWarning)).
                Options

            _dbContext = New PurchasingDbContext(options)
            _mockView = New Mock(Of IPurchasingDashboardView)()
            _mockView.SetupAllProperties()
            _mockNotification = New Mock(Of INotificationService)()

            _presenter = New PurchasingDashboardPresenter(_dbContext, _mockNotification.Object)
            _presenter.View = _mockView.Object
        End Sub

        <Fact>
        Public Async Function Presenter_AggregatesKpisAccurately() As Task
            ' Arrange
            _dbContext.Vendors.Add(New Vendor With {.Id = 1, .Name = "Vendor A", .LeadTimeDays = 5})
            _dbContext.Vendors.Add(New Vendor With {.Id = 2, .Name = "Vendor B", .LeadTimeDays = 15, .IsDeleted = True}) ' Inactive
            _dbContext.Vendors.Add(New Vendor With {.Id = 3, .Name = "Vendor C", .LeadTimeDays = 10})

            _dbContext.AccountsPayable.Add(New AccountsPayableEntry With {.Id = 1, .VendorId = 1, .TotalAmount = 1000, .AmountPaid = 200, .IsPaid = False, .DueDate = DateTime.Today.AddDays(-1)}) ' Overdue, 800 outstanding
            _dbContext.AccountsPayable.Add(New AccountsPayableEntry With {.Id = 2, .VendorId = 1, .TotalAmount = 500, .AmountPaid = 500, .IsPaid = True, .DueDate = DateTime.Today.AddDays(-1)}) ' Paid
            _dbContext.AccountsPayable.Add(New AccountsPayableEntry With {.Id = 3, .VendorId = 3, .TotalAmount = 2000, .AmountPaid = 0, .IsPaid = False, .DueDate = DateTime.Today.AddDays(5)}) ' Not overdue, 2000 outstanding

            _dbContext.PurchaseOrders.Add(New PurchaseOrder With {.Id = 1, .VendorId = 1, .Status = PurchaseOrderStatus.Submitted, .TotalAmount = 1500, .OrderNumber = "PO-01"})
            _dbContext.PurchaseOrders.Add(New PurchaseOrder With {.Id = 2, .VendorId = 3, .Status = PurchaseOrderStatus.PartialDelivery, .TotalAmount = 2000, .OrderNumber = "PO-02"})
            _dbContext.PurchaseOrders.Add(New PurchaseOrder With {.Id = 3, .VendorId = 1, .Status = PurchaseOrderStatus.Received, .TotalAmount = 500, .OrderNumber = "PO-03"})

            _dbContext.ReorderSuggestions.Add(New ReorderSuggestion With {.Id = 1, .ProductId = 1, .SuggestedQuantity = 10})
            _dbContext.ReorderSuggestions.Add(New ReorderSuggestion With {.Id = 2, .ProductId = 2, .SuggestedQuantity = 20})

            Await _dbContext.SaveChangesAsync()

            ' Act
            _mockView.Raise(Sub(m) AddHandler m.ViewLoaded, Nothing, EventArgs.Empty)
            Await Task.Yield() ' Allow async event handler to complete

            ' Assert
            Assert.Equal(2800D, _mockView.Object.OutstandingAp) ' 800 + 2000
            Assert.Equal(1, _mockView.Object.OverdueApCount)
            Assert.Equal(2, _mockView.Object.PendingDeliveriesCount)
            Assert.Equal(2, _mockView.Object.ReorderSuggestionsCount)
            Assert.Equal(2, _mockView.Object.ActiveVendorsCount)
            Assert.Equal(7.5D, _mockView.Object.AverageLeadTimeDays) ' (5 + 10) / 2
        End Function

        <Fact>
        Public Async Function Presenter_SetsGridDataAccurately() As Task
            ' Arrange
            _dbContext.Vendors.Add(New Vendor With {.Id = 1, .Name = "Vendor A"})
            _dbContext.Vendors.Add(New Vendor With {.Id = 2, .Name = "Vendor B"})

            _dbContext.AccountsPayable.Add(New AccountsPayableEntry With {.Id = 1, .VendorId = 1, .TotalAmount = 1500})
            _dbContext.AccountsPayable.Add(New AccountsPayableEntry With {.Id = 2, .VendorId = 1, .TotalAmount = 500})
            _dbContext.AccountsPayable.Add(New AccountsPayableEntry With {.Id = 3, .VendorId = 2, .TotalAmount = 3000})

            _dbContext.PurchaseOrders.Add(New PurchaseOrder With {.Id = 1, .VendorId = 1, .Status = PurchaseOrderStatus.Submitted, .TotalAmount = 1500, .OrderNumber = "PO-01"})

            Await _dbContext.SaveChangesAsync()

            Dim pendingDeliveriesCapture As IEnumerable(Of PendingDeliveryRowDto) = Nothing
            _mockView.Setup(Sub(m) m.SetPendingDeliveries(It.IsAny(Of IEnumerable(Of PendingDeliveryRowDto))())).Callback(Sub(items As IEnumerable(Of PendingDeliveryRowDto)) pendingDeliveriesCapture = items)

            Dim topVendorsCapture As IEnumerable(Of TopVendorSpendDto) = Nothing
            _mockView.Setup(Sub(m) m.SetTopVendorsBySpend(It.IsAny(Of IEnumerable(Of TopVendorSpendDto))())).Callback(Sub(items As IEnumerable(Of TopVendorSpendDto)) topVendorsCapture = items)

            ' Act
            _mockView.Raise(Sub(m) AddHandler m.ViewLoaded, Nothing, EventArgs.Empty)
            Await Task.Yield()

            ' Assert
            Assert.NotNull(pendingDeliveriesCapture)
            Dim pendingList = pendingDeliveriesCapture.ToList()
            Assert.Single(pendingList)
            Assert.Equal("PO-01", pendingList(0).OrderNumber)
            Assert.Equal("Vendor A", pendingList(0).VendorName)

            Assert.NotNull(topVendorsCapture)
            Dim topList = topVendorsCapture.ToList()
            Assert.Equal(2, topList.Count)
            Assert.Equal("Vendor B", topList(0).VendorName)
            Assert.Equal(3000D, topList(0).TotalSpend)
            Assert.Equal("Vendor A", topList(1).VendorName)
            Assert.Equal(2000D, topList(1).TotalSpend)
        End Function

        <Fact>
        Public Sub View_Constructor_InitializesCleanly()
            Dim view As New PurchasingDashboardView()
            Assert.NotNull(view)
            Assert.True(view.AutoScroll)
            view.Dispose()
        End Sub

        <Fact>
        Public Sub View_UI_HasCorrectTags()
            Dim view As New PurchasingDashboardView()

            Dim tOutstanding = view.GetType().GetProperty("pnlOutstandingAp", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance).GetValue(view)
            Assert.Equal("PrimaryMetricCard", DirectCast(tOutstanding, Control).Tag)

            Dim tOverdue = view.GetType().GetProperty("pnlOverdueAp", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance).GetValue(view)
            Assert.Equal("SecondaryMetricCard", DirectCast(tOverdue, Control).Tag)

            Dim tToolbar = view.GetType().GetProperty("pnlToolbar", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance).GetValue(view)
            Assert.Equal("FilterBar", DirectCast(tToolbar, Control).Tag)

            view.Dispose()
        End Sub

        <Fact>
        Public Sub View_ApplyTheme_DoesNotThrow()
            Dim view As New PurchasingDashboardView()
            Dim styler As New MerchSys.App.Services.Theming.ThemeService()

            Dim ex = Record.Exception(Sub() styler.ApplyTheme(view))
            Assert.Null(ex)

            view.Dispose()
        End Sub

        <Fact>
        Public Sub Navigation_RebuildModuleCollections_IncludesPurchasingDashboard()
            ' Arrange
            Dim mockView = New Mock(Of IMainWindowView)()
            Dim mockSession = New LoginSessionService()
            mockSession.SetUser(New MerchSys.SharedKernel.Entities.UserAccount With {.Username = "TestUser", .Role = UserRole.Manager})

            Dim services = New ServiceCollection()
            Dim sp = services.BuildServiceProvider()

            Dim presenter As New MainWindowPresenter(mockView.Object, mockSession, sp, Nothing)

            ' Act
            presenter.RebuildModuleCollections()

            ' Assert
            Dim purchasingGroup = presenter.NavigationGroups.FirstOrDefault(Function(g) g.GroupName = "Purchasing")
            Assert.NotNull(purchasingGroup)
            Assert.True(purchasingGroup.Items.Count > 0)
            Assert.Equal("Purchasing Dashboard", purchasingGroup.Items(0).Name)
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            _presenter.Dispose()
            _dbContext.Dispose()
        End Sub
    End Class
End Namespace