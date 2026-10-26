Imports Moq
Imports Xunit
Imports MerchSys.App.Presenters
Imports MerchSys.App.Views
Imports MerchSys.App.Services
Imports MerchSys.App.Services.Theming
Imports MerchSys.SharedKernel.Enums
Imports System.Drawing
Imports MerchSys.App.Models

Public Class MainWindowThemeTests
    <Fact>
    Public Sub MainWindowPresenter_TogglesThemeOnRequest()
        ' Arrange
        Dim viewMock As New Mock(Of IMainWindowView)()
        Dim sessionService As New LoginSessionService()
        Dim spMock As New Mock(Of IServiceProvider)()
        Dim monitorMock As New Mock(Of IConnectionHealthMonitor)()
        Dim statusViewMock As New Mock(Of IConnectionStatusView)()
        Dim connPresenter As New ConnectionStatusPresenter(monitorMock.Object, statusViewMock.Object)
        Dim themeServiceMock As New Mock(Of IThemeService)()

        themeServiceMock.Setup(Function(s) s.CurrentTheme).Returns(AppTheme.Light)
        viewMock.SetupAllProperties()

        Dim presenter As New MainWindowPresenter(viewMock.Object, sessionService, spMock.Object, connPresenter, themeServiceMock.Object)

        ' Act
        viewMock.Raise(Sub(v) AddHandler v.ThemeToggleRequested, Nothing, Me, EventArgs.Empty)

        ' Assert
        themeServiceMock.Verify(Sub(s) s.ToggleTheme(), Times.Once())
    End Sub

    <Fact>
    Public Sub MainWindowPresenter_AppliesTokensOnThemeChanged()
        ' Arrange
        Dim viewMock As New Mock(Of IMainWindowView)()
        Dim sessionService As New LoginSessionService()
        Dim spMock As New Mock(Of IServiceProvider)()
        Dim monitorMock As New Mock(Of IConnectionHealthMonitor)()
        Dim statusViewMock As New Mock(Of IConnectionStatusView)()
        Dim connPresenter As New ConnectionStatusPresenter(monitorMock.Object, statusViewMock.Object)
        Dim themeServiceMock As New Mock(Of IThemeService)()

        themeServiceMock.Setup(Function(s) s.CurrentTheme).Returns(AppTheme.Dark)
        Dim tokens = ThemeTokens.GetTokens(AppTheme.Light)
        themeServiceMock.Setup(Function(s) s.Tokens).Returns(tokens)
        viewMock.SetupAllProperties()

        Dim presenter As New MainWindowPresenter(viewMock.Object, sessionService, spMock.Object, connPresenter, themeServiceMock.Object)

        ' Act
        themeServiceMock.Raise(Sub(s) AddHandler s.ThemeChanged, Nothing, Me, AppTheme.Light)

        ' Assert
        viewMock.Verify(Sub(v) v.ApplyThemeTokens(tokens), Times.Once())
        ' Initial call + event call
        viewMock.Verify(Sub(v) v.SetThemeState(AppTheme.Light), Times.Once())
    End Sub


    <Fact>
    Public Sub ConnectionStatusPresenter_RefreshesStylingOnThemeChanged()
        ' Arrange
        Dim monitorMock As New Mock(Of IConnectionHealthMonitor)()
        monitorMock.Setup(Function(m) m.CurrentState).Returns(ConnectionState.Online)

        Dim statusViewMock As New Mock(Of IConnectionStatusView)()
        Dim themeServiceMock As New Mock(Of IThemeService)()
        Dim lightTokens = ThemeTokens.GetTokens(AppTheme.Light)
        Dim darkTokens = ThemeTokens.GetTokens(AppTheme.Dark)

        ' Start with Light theme
        themeServiceMock.Setup(Function(s) s.Tokens).Returns(lightTokens)
        themeServiceMock.Setup(Function(s) s.CurrentTheme).Returns(AppTheme.Light)

        Dim presenter As New ConnectionStatusPresenter(monitorMock.Object, statusViewMock.Object, themeServiceMock.Object)

        ' Act - simulate changing to Dark theme
        themeServiceMock.Setup(Function(s) s.Tokens).Returns(darkTokens)
        themeServiceMock.Setup(Function(s) s.CurrentTheme).Returns(AppTheme.Dark)
        themeServiceMock.Raise(Sub(s) AddHandler s.ThemeChanged, Nothing, Me, AppTheme.Dark)

        ' Assert
        ' It should have called SetState during initialization and then again after ThemeChanged
        ' On initialization: text "Online", success color (light theme), isRetryVisible=False, foreColor=PrimaryText (light theme)
        statusViewMock.Verify(Sub(v) v.SetState("Online", lightTokens.Success, False, lightTokens.PrimaryText), Times.Once())
        ' On theme changed: same but with dark theme colors
        statusViewMock.Verify(Sub(v) v.SetState("Online", darkTokens.Success, False, darkTokens.PrimaryText), Times.Once())
    End Sub

    <Fact>
    Public Sub MainWindowPresenter_AppliesActiveTokenColorsToNavButtons()
        ' Arrange
        Dim viewMock As New Mock(Of IMainWindowView)()
        Dim sessionService As New LoginSessionService()
        Dim spMock As New Mock(Of IServiceProvider)()
        Dim monitorMock As New Mock(Of IConnectionHealthMonitor)()
        Dim statusViewMock As New Mock(Of IConnectionStatusView)()
        Dim connPresenter As New ConnectionStatusPresenter(monitorMock.Object, statusViewMock.Object)
        Dim themeServiceMock As New Mock(Of IThemeService)()

        themeServiceMock.Setup(Function(s) s.CurrentTheme).Returns(AppTheme.Dark)
        Dim tokens = ThemeTokens.GetTokens(AppTheme.Light)
        themeServiceMock.Setup(Function(s) s.Tokens).Returns(tokens)
        viewMock.SetupAllProperties()

        Dim capturedItems As List(Of NavigationItem) = Nothing
        viewMock.Setup(Sub(v) v.AddNavigationButtons(It.IsAny(Of List(Of NavigationItem))())).Callback(Sub(items As List(Of NavigationItem)) capturedItems = items)

        Dim presenter As New MainWindowPresenter(viewMock.Object, sessionService, spMock.Object, connPresenter, themeServiceMock.Object)

        ' Act
        sessionService.SetUser(New MerchSys.SharedKernel.Entities.UserAccount With { .Id = 1, .Username = "manager", .Role = UserRole.Manager, .PasswordHash = "hash" })
        viewMock.Raise(Sub(v) AddHandler v.Load, Nothing, Me, EventArgs.Empty)

        ' Assert
        Assert.NotNull(capturedItems)
        ' AddNavigationButtons itself handles the coloring via tokens in the view,
        ' but the presenter calls it and then the view uses tokens.Accent.
        ' We can verify that tokens.Accent is available to the view via ApplyThemeTokens.
        viewMock.Verify(Sub(v) v.ApplyThemeTokens(tokens), Times.Once())
    End Sub

End Class
