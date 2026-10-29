Imports MediatR
Imports Moq
Imports Xunit
Imports MerchSys.App.Presenters
Imports MerchSys.App.Views
Imports MerchSys.App.Models
Imports MerchSys.SharedKernel.Queries
Imports MerchSys.App.Services
Imports Microsoft.Extensions.DependencyInjection
Imports System.Threading

Public Class CommandPaletteTests
    Private Function GetLoginSessionService() As LoginSessionService
        Dim svc = New LoginSessionService()
        svc.SetUser(New MerchSys.SharedKernel.Entities.UserAccount With {.Username = "TestUser", .Role = MerchSys.SharedKernel.Enums.UserRole.Manager})
        Return svc
    End Function

    <Fact>
    Public Sub MainWindowPresenter_AllNavigableItems_ReturnsAllItems()
        Dim mockView = New Mock(Of IMainWindowView)()
        Dim sessionService = GetLoginSessionService()
        Dim sp = New ServiceCollection().BuildServiceProvider()

        Dim presenter = New MainWindowPresenter(mockView.Object, sessionService, sp, Nothing)
        presenter.RebuildModuleCollections()

        Dim items = presenter.AllNavigableItems

        Assert.NotNull(items)
        Assert.True(items.Count > 0)

        ' As Manager, should see VAT Returns and other manager-level items
        Assert.Contains(items, Function(i) i.Name = "VAT Returns")
        Assert.Contains(items, Function(i) i.Name = "Product Management")
    End Sub

    <Fact>
    Public Sub CommandPalettePresenter_SyncFiltering_FiltersScreensCaseInsensitive()
        Dim mockView = New Mock(Of ICommandPaletteView)()
        Dim mockMediator = New Mock(Of IMediator)()

        Dim sessionService = GetLoginSessionService()
        Dim sp = New ServiceCollection().BuildServiceProvider()
        Dim mockMainWindowView = New Mock(Of IMainWindowView)()
        Dim mainWindowPresenter = New MainWindowPresenter(mockMainWindowView.Object, sessionService, sp, Nothing)
        mainWindowPresenter.RebuildModuleCollections()

        Dim presenter = New CommandPalettePresenter(mockView.Object, mainWindowPresenter, mockMediator.Object)

        ' Set up MediatR to return empty list to not affect sync test
        mockMediator.Setup(Function(m) m.Send(It.IsAny(Of GetProductsForCatalogQuery)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(New List(Of CatalogProductDto)())

        Dim capturedResults As List(Of CommandPaletteItem) = Nothing
        mockView.Setup(Sub(v) v.SetResults(It.IsAny(Of IEnumerable(Of CommandPaletteItem))())).Callback(Sub(res As IEnumerable(Of CommandPaletteItem)) capturedResults = res.ToList())

        ' Act - search for "vat" (case insensitive)
        mockView.Raise(Sub(v) AddHandler v.SearchQueryChanged, Nothing, Nothing, "vat")

        ' Need to wait for debounce Task.Delay(250) + processing
        Thread.Sleep(500)

        Assert.NotNull(capturedResults)
        Assert.True(capturedResults.Count > 0)
        Assert.All(capturedResults, Sub(item) Assert.Contains("vat", item.Title.ToLower()))
        Assert.All(capturedResults, Sub(item) Assert.Equal(CommandPaletteItemKind.Screen, item.Kind))
    End Sub

    <Fact>
    Public Sub CommandPalettePresenter_AsyncSearch_ReturnsProductsAndCapsAt5()
        Dim mockView = New Mock(Of ICommandPaletteView)()
        Dim mockMediator = New Mock(Of IMediator)()

        Dim sessionService = GetLoginSessionService()
        Dim sp = New ServiceCollection().BuildServiceProvider()
        Dim mockMainWindowView = New Mock(Of IMainWindowView)()
        Dim mainWindowPresenter = New MainWindowPresenter(mockMainWindowView.Object, sessionService, sp, Nothing)
        mainWindowPresenter.RebuildModuleCollections()

        Dim presenter = New CommandPalettePresenter(mockView.Object, mainWindowPresenter, mockMediator.Object)

        Dim mockProducts = New List(Of CatalogProductDto)()
        For i = 1 To 10
            mockProducts.Add(New CatalogProductDto With {.Id = i, .Name = $"TestProduct {i}", .RetailPrice = 10D + i})
        Next

        mockMediator.Setup(Function(m) m.Send(It.IsAny(Of GetProductsForCatalogQuery)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(mockProducts)

        Dim capturedResults As List(Of CommandPaletteItem) = Nothing
        mockView.Setup(Sub(v) v.SetResults(It.IsAny(Of IEnumerable(Of CommandPaletteItem))())).Callback(Sub(res As IEnumerable(Of CommandPaletteItem)) capturedResults = res.ToList())

        ' Act - search for "TestProduct" (which should match 10 products, but cap at 5)
        mockView.Raise(Sub(v) AddHandler v.SearchQueryChanged, Nothing, Nothing, "TestProduct")

        Thread.Sleep(500)

        Assert.NotNull(capturedResults)
        Dim productResults = capturedResults.Where(Function(r) r.Kind = CommandPaletteItemKind.Product).ToList()

        Assert.Equal(5, productResults.Count)
        Assert.All(productResults, Sub(item) Assert.Contains("TestProduct", item.Title))
    End Sub

    <Fact>
    Public Sub SearchQueryChanged_SynchronousScreenFilter_UpdatesResults()
        Dim mockView = New Mock(Of ICommandPaletteView)()
        Dim mockMediator = New Mock(Of IMediator)()

        Dim sessionService = GetLoginSessionService()
        Dim sp = New ServiceCollection().BuildServiceProvider()
        Dim mockMainWindowView = New Mock(Of IMainWindowView)()
        Dim mainWindowPresenter = New MainWindowPresenter(mockMainWindowView.Object, sessionService, sp, Nothing)
        mainWindowPresenter.RebuildModuleCollections()

        Dim presenter = New CommandPalettePresenter(mockView.Object, mainWindowPresenter, mockMediator.Object)

        Dim mockProducts = New List(Of CatalogProductDto)()

        mockMediator.Setup(Function(m) m.Send(It.IsAny(Of GetProductsForCatalogQuery)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(mockProducts)

        Dim capturedResults As List(Of CommandPaletteItem) = Nothing
        mockView.Setup(Sub(v) v.SetResults(It.IsAny(Of IEnumerable(Of CommandPaletteItem))())).Callback(Sub(res As IEnumerable(Of CommandPaletteItem)) capturedResults = res.ToList())

        ' Act - search for "vat" (case insensitive)
        mockView.Raise(Sub(v) AddHandler v.SearchQueryChanged, Nothing, Nothing, "vat")

        Assert.NotNull(capturedResults)
        Assert.True(capturedResults.Count > 0)
        Assert.All(capturedResults, Sub(item) Assert.Contains("vat", item.Title.ToLower()))
        Assert.All(capturedResults, Sub(item) Assert.Equal(CommandPaletteItemKind.Screen, item.Kind))
    End Sub

    <Fact>
    Public Sub CommandPalettePresenter_OnItemSelected_Screen_NavigatesAndHides()
        Dim mockView = New Mock(Of ICommandPaletteView)()
        Dim mockMediator = New Mock(Of IMediator)()

        Dim sessionService = GetLoginSessionService()
        Dim sp = New ServiceCollection().BuildServiceProvider()
        Dim mockMainWindowView = New Mock(Of IMainWindowView)()
        Dim mainWindowPresenter = New MainWindowPresenter(mockMainWindowView.Object, sessionService, sp, Nothing)

        Dim presenter = New CommandPalettePresenter(mockView.Object, mainWindowPresenter, mockMediator.Object)

        Dim dummyNavItem = New NavigationItem With {
            .Name = "Owner Dashboard",
            .NavigationGroup = "Overview",
            .ViewFactory = Function(services) New System.Windows.Forms.UserControl()
        }

        Dim itemToSelect = New CommandPaletteItem With {
            .Kind = CommandPaletteItemKind.Screen,
            .NavigationItem = dummyNavItem
        }

        mockView.Raise(Sub(v) AddHandler v.ItemSelected, Nothing, Nothing, itemToSelect)

        mockView.Verify(Sub(v) v.HidePalette(), Times.Once())
        mockMainWindowView.Verify(Sub(v) v.RenderView(It.IsAny(Of System.Windows.Forms.UserControl)()), Times.Once())
    End Sub
End Class
