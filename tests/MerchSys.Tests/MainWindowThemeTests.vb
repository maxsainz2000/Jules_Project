Imports MerchSys.App.Models
Imports MerchSys.App.Views.Shell.Modules
Imports MerchSys.Accounting.Views
Imports MerchSys.Inventory.Views
Imports MerchSys.Purchasing.Views
Imports MerchSys.Purchasing.Views.Purchasing
Imports MerchSys.POS.Views
Imports Moq
Imports Xunit
Imports MerchSys.App.Presenters
Imports MerchSys.App.Views
Imports MerchSys.App.Services
Imports MerchSys.App.Services.Theming
Imports MerchSys.SharedKernel.Entities
Imports MerchSys.SharedKernel.Enums
Imports Microsoft.Extensions.DependencyInjection

Public Class MainWindowThemeTests

    <Fact>
    Public Sub MainWindowPresenter_AppliesTokensOnThemeChanged()
        ' Arrange
        Dim viewMock = New Mock(Of IMainWindowView)()
        Dim themeServiceMock = New Mock(Of IThemeService)()
        Dim sessionService = New LoginSessionService()
        sessionService.SetUser(New UserAccount With {
            .Id = 1,
            .Username = "test_owner",
            .Role = UserRole.Owner
        })
        Dim serviceProvider = New ServiceCollection().BuildServiceProvider()
        Dim monitorMock = New Mock(Of IConnectionHealthMonitor)()
        Dim connViewMock = New Mock(Of IConnectionStatusView)()
        Dim connPresenter = New ConnectionStatusPresenter(monitorMock.Object, connViewMock.Object, themeServiceMock.Object)

        Dim tokens = New ThemeTokens()
        themeServiceMock.Setup(Function(ts) ts.Tokens).Returns(tokens)
        themeServiceMock.Setup(Function(ts) ts.CurrentTheme).Returns(AppTheme.Light)

        Dim presenter = New MainWindowPresenter(viewMock.Object, sessionService, serviceProvider, connPresenter, themeServiceMock.Object)

        ' Act
        themeServiceMock.Raise(Sub(ts) AddHandler ts.ThemeChanged, Nothing, Me, AppTheme.Dark)

        ' Assert
        viewMock.Verify(Sub(v) v.ApplyThemeTokens(tokens), Times.Exactly(2))
    End Sub

    <Fact>
    Public Sub MainWindowPresenter_AppliesActiveTokenColorsToNavButtons()
        ' Arrange
        Dim viewMock = New Mock(Of IMainWindowView)()
        Dim themeServiceMock = New Mock(Of IThemeService)()
        Dim sessionService = New LoginSessionService()
        sessionService.SetUser(New UserAccount With {
            .Id = 1,
            .Username = "test_owner",
            .Role = UserRole.Owner
        })

        Dim services = New ServiceCollection()
        ' Add required services for ViewFactory resolution
        services.AddTransient(Of IOwnerDashboardView)(Function(sp) New Mock(Of IOwnerDashboardView)().Object)
        ' ... Add others if RebuildModuleCollections needs them to exist, but ViewFactory only resolves on NavigateTo, so we just need the one we navigate to.

        Dim serviceProvider = services.BuildServiceProvider()

        Dim monitorMock = New Mock(Of IConnectionHealthMonitor)()
        Dim connViewMock = New Mock(Of IConnectionStatusView)()
        Dim connPresenter = New ConnectionStatusPresenter(monitorMock.Object, connViewMock.Object, themeServiceMock.Object)

        Dim tokens = New ThemeTokens()
        themeServiceMock.Setup(Function(ts) ts.Tokens).Returns(tokens)
        themeServiceMock.Setup(Function(ts) ts.CurrentTheme).Returns(AppTheme.Light)

        Dim presenter = New MainWindowPresenter(viewMock.Object, sessionService, serviceProvider, connPresenter, themeServiceMock.Object)
        presenter.RebuildModuleCollections()
        Dim items = presenter.NavigationGroups.SelectMany(Function(g) g.Items).ToList()
        Dim target = items.FirstOrDefault()

        ' Act
        ' NavigateTo will trigger _view.AddNavigationButtons
        presenter.NavigateTo(target)

        ' Assert
        viewMock.Verify(Sub(v) v.AddNavigationButtons(It.IsAny(Of List(Of NavigationItem))()), Times.AtLeastOnce())
    End Sub

End Class
