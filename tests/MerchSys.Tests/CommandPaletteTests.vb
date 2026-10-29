Imports Moq
Imports Xunit
Imports MerchSys.App.Presenters
Imports MerchSys.App.Views
Imports MerchSys.App.Models
Imports MerchSys.SharedKernel.Queries
Imports MediatR
Imports Microsoft.Extensions.DependencyInjection
Imports System.Threading.Tasks
Imports System.Threading

Namespace Tests
    Public Class CommandPaletteTests
        Private ReadOnly _serviceProvider As IServiceProvider

        Public Sub New()
            Dim services As New ServiceCollection()
            ' Register view for navigation tests
            services.AddTransient(Of IOwnerDashboardView, OwnerDashboardView)()

            services.AddTransient(Of CommandPalettePresenter)()
            services.AddTransient(Of ICommandPaletteView, MerchSys.App.Controls.CommandPalette)()

            _serviceProvider = services.BuildServiceProvider()
        End Sub

        <Fact>
        Public Sub MainWindowPresenter_AllNavigableItems_ReturnsAllRoleVisibleItems()
            ' Arrange
            Dim mockView = New Mock(Of IMainWindowView)()
            Dim mockSessionService = New MerchSys.App.Services.LoginSessionService()
            Dim presenter = New MainWindowPresenter(mockView.Object, mockSessionService, _serviceProvider, Nothing, Nothing)

            Dim group1 = New NavigationGroup With { .GroupName = "Group 1" }
            group1.Items.Add(New NavigationItem With { .Name = "Item 1" })
            group1.Items.Add(New NavigationItem With { .Name = "Item 2" })

            Dim group2 = New NavigationGroup With { .GroupName = "Group 2" }
            group2.Items.Add(New NavigationItem With { .Name = "Item 3" })

            presenter.NavigationGroups.Add(group1)
            presenter.NavigationGroups.Add(group2)

            ' Act
            Dim allItems = presenter.AllNavigableItems

            ' Assert
            Assert.NotNull(allItems)
            Assert.Equal(3, allItems.Count)
            Assert.Equal("Item 1", allItems(0).Name)
            Assert.Equal("Item 2", allItems(1).Name)
            Assert.Equal("Item 3", allItems(2).Name)
        End Sub

        <Fact>
        Public Async Function CommandPalettePresenter_SyncFilter_ReturnsMatchingScreens() As Task
            ' Arrange
            Dim mockView = New Mock(Of IMainWindowView)()
            Dim mockSessionService = New MerchSys.App.Services.LoginSessionService()
            Dim mainWindowPresenter = New MainWindowPresenter(mockView.Object, mockSessionService, _serviceProvider, Nothing, Nothing)

            Dim group1 = New NavigationGroup With { .GroupName = "Group 1" }
            group1.Items.Add(New NavigationItem With { .Name = "Sales Dashboard" })
            group1.Items.Add(New NavigationItem With { .Name = "Inventory Management" })
            mainWindowPresenter.NavigationGroups.Add(group1)

            Dim mockMediator = New Mock(Of IMediator)()
            mockMediator.Setup(Function(m) m.Send(It.IsAny(Of GetProductsForCatalogQuery)(), It.IsAny(Of CancellationToken)())).
                ReturnsAsync(New List(Of CatalogProductDto)())

            Dim mockPaletteView = New Mock(Of ICommandPaletteView)()
            Dim presenter = New CommandPalettePresenter(mockPaletteView.Object, mainWindowPresenter, mockMediator.Object)

            Dim tcs As New TaskCompletionSource(Of Boolean)()
            Dim capturedResults As List(Of CommandPaletteItem) = Nothing

            Dim callCount = 0
            mockPaletteView.Setup(Sub(v) v.SetResults(It.IsAny(Of IEnumerable(Of CommandPaletteItem))())).
                Callback(Sub(items As IEnumerable(Of CommandPaletteItem))
                             callCount += 1
                             If callCount = 2 Then ' 1st is initial load, 2nd is search result
                                 capturedResults = items.ToList()
                                 tcs.TrySetResult(True)
                             End If
                         End Sub)

            ' Act
            mockPaletteView.Raise(Sub(m) AddHandler m.SearchQueryChanged, Nothing, Nothing, "sale")

            Dim finishedTask = Await Task.WhenAny(tcs.Task, Task.Delay(3000))

            ' Assert
            Assert.Same(tcs.Task, finishedTask)
            Assert.NotNull(capturedResults)
            Assert.Single(capturedResults)
            Assert.Equal("Sales Dashboard", capturedResults(0).Title)
            Assert.Equal(CommandPaletteItemKind.Screen, capturedResults(0).Kind)
        End Function

        <Fact>
        Public Async Function CommandPalettePresenter_AsyncSearch_ReturnsProductsAndCapsAt5() As Task
            ' Arrange
            Dim mockView = New Mock(Of IMainWindowView)()
            Dim mockSessionService = New MerchSys.App.Services.LoginSessionService()
            Dim mainWindowPresenter = New MainWindowPresenter(mockView.Object, mockSessionService, _serviceProvider, Nothing, Nothing)

            Dim mockMediator = New Mock(Of IMediator)()

            Dim products = New List(Of CatalogProductDto)()
            For i = 1 To 10
                products.Add(New CatalogProductDto With { .Id = i, .Name = $"Product {i}", .RetailPrice = i * 10D })
            Next

            mockMediator.Setup(Function(m) m.Send(It.IsAny(Of GetProductsForCatalogQuery)(), It.IsAny(Of CancellationToken)())).
                ReturnsAsync(products)

            Dim mockPaletteView = New Mock(Of ICommandPaletteView)()
            Dim presenter = New CommandPalettePresenter(mockPaletteView.Object, mainWindowPresenter, mockMediator.Object)

            Dim tcs As New TaskCompletionSource(Of Boolean)()
            Dim capturedResults As List(Of CommandPaletteItem) = Nothing

            Dim callCount = 0
            mockPaletteView.Setup(Sub(v) v.SetResults(It.IsAny(Of IEnumerable(Of CommandPaletteItem))())).
                Callback(Sub(items As IEnumerable(Of CommandPaletteItem))
                             callCount += 1
                             If callCount = 2 Then ' 1st is initial load, 2nd is search result
                                 capturedResults = items.ToList()
                                 tcs.TrySetResult(True)
                             End If
                         End Sub)

            ' Act
            mockPaletteView.Raise(Sub(m) AddHandler m.SearchQueryChanged, Nothing, Nothing, "Product")

            Dim finishedTask = Await Task.WhenAny(tcs.Task, Task.Delay(3000))

            ' Assert
            Assert.Same(tcs.Task, finishedTask)
            Assert.NotNull(capturedResults)
            Assert.Equal(5, capturedResults.Count) ' capped at 5
            Assert.All(capturedResults, Sub(r) Assert.Equal(CommandPaletteItemKind.Product, r.Kind))
        End Function

        <Fact>
        Public Sub CommandPalettePresenter_OnItemSelected_Screen_NavigatesAndHides()
            ' Arrange
            Dim mockView = New Mock(Of IMainWindowView)()
            Dim mockSessionService = New MerchSys.App.Services.LoginSessionService()
            Dim mainWindowPresenter = New MainWindowPresenter(mockView.Object, mockSessionService, _serviceProvider, Nothing, Nothing)

            Dim mockMediator = New Mock(Of IMediator)()
            Dim mockPaletteView = New Mock(Of ICommandPaletteView)()
            Dim presenter = New CommandPalettePresenter(mockPaletteView.Object, mainWindowPresenter, mockMediator.Object)

            Dim navItem = New NavigationItem With { .Name = "Dashboard", .ViewFactory = Function(sp) DirectCast(sp.GetRequiredService(Of IOwnerDashboardView)(), System.Windows.Forms.UserControl) }
            Dim item = New CommandPaletteItem With { .Kind = CommandPaletteItemKind.Screen, .NavigationItem = navItem }

            ' Act
            mockPaletteView.Raise(Sub(m) AddHandler m.ItemSelected, Nothing, Nothing, item)

            ' Assert
            ' Navigation sets View properties and hides palette
            mockPaletteView.Verify(Sub(v) v.HidePalette(), Times.Once())
        End Sub

        <Fact>
        Public Sub CommandPalettePresenter_OnDismissRequested_HidesPalette()
            ' Arrange
            Dim mockView = New Mock(Of IMainWindowView)()
            Dim mockSessionService = New MerchSys.App.Services.LoginSessionService()
            Dim mainWindowPresenter = New MainWindowPresenter(mockView.Object, mockSessionService, _serviceProvider, Nothing, Nothing)

            Dim mockMediator = New Mock(Of IMediator)()
            Dim mockPaletteView = New Mock(Of ICommandPaletteView)()
            Dim presenter = New CommandPalettePresenter(mockPaletteView.Object, mainWindowPresenter, mockMediator.Object)

            ' Act
            mockPaletteView.Raise(Sub(m) AddHandler m.DismissRequested, Nothing, Nothing, EventArgs.Empty)

            ' Assert
            mockPaletteView.Verify(Sub(v) v.HidePalette(), Times.Once())
        End Sub
    End Class
End Namespace
