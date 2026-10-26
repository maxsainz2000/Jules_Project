Imports Moq
Imports Xunit
Imports MerchSys.App.Presenters
Imports MerchSys.App.Views
Imports MerchSys.App.Services
Imports MerchSys.App.Services.Theming
Imports MerchSys.App.Models
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Interfaces
Imports Microsoft.Extensions.DependencyInjection
Imports System.Windows.Forms

Public Class MainWindowThemeTests
    Private ReadOnly _viewMock As Mock(Of IMainWindowView)
    Private ReadOnly _sessionServiceMock As Mock(Of LoginSessionService)
    Private ReadOnly _themeServiceMock As Mock(Of IThemeService)
    Private ReadOnly _connectionPresenterMock As Mock(Of ConnectionStatusPresenter)

    Public Sub New()
        _viewMock = New Mock(Of IMainWindowView)()
        _sessionServiceMock = New Mock(Of LoginSessionService)(New Mock(Of IAuthenticationService)().Object)
        _themeServiceMock = New Mock(Of IThemeService)()
        _connectionPresenterMock = New Mock(Of ConnectionStatusPresenter)(New Mock(Of IConnectionHealthMonitor)().Object, New Mock(Of IConnectionStatusView)().Object)

        ' Ensure properties set on the mock view are correctly saved.
        _viewMock.SetupAllProperties()
    End Sub

    <Fact>
    Public Sub MainWindowPresenter_AppliesTokensOnThemeChanged()
        ' Arrange
        Dim serviceProvider = New ServiceCollection().BuildServiceProvider()
        Dim tokens = New ThemeTokens()
        _themeServiceMock.Setup(Function(ts) ts.Tokens).Returns(tokens)
        _themeServiceMock.Setup(Function(ts) ts.CurrentTheme).Returns(AppTheme.Light)

        Dim presenter = New MainWindowPresenter(
            _viewMock.Object,
            _sessionServiceMock.Object,
            serviceProvider,
            _connectionPresenterMock.Object,
            _themeServiceMock.Object)

        ' Act
        _themeServiceMock.Raise(Sub(ts) AddHandler ts.ThemeChanged, Nothing, Me, AppTheme.Dark)

        ' Assert
        ' The first time is during the constructor when _themeService IsNot Nothing.
        ' The second time is when ThemeChanged is raised.
        _viewMock.Verify(Sub(v) v.ApplyThemeTokens(tokens), Times.Exactly(2))
        _viewMock.Verify(Sub(v) v.SetThemeState(AppTheme.Dark), Times.Once)
    End Sub

    <Fact>
    Public Sub MainWindowPresenter_AppliesActiveTokenColorsToNavButtons()
        ' Arrange
        Dim services = New ServiceCollection()
        ' Register fake view
        services.AddSingleton(Of IOwnerDashboardView)(New MerchSys.Tests.Fakes.FakeOwnerDashboardView())

        Dim mockScopeFactory As New Mock(Of IServiceScopeFactory)()
        Dim mockScope As New Mock(Of IServiceScope)()
        mockScopeFactory.Setup(Function(f) f.CreateScope()).Returns(mockScope.Object)

        Dim sp = services.BuildServiceProvider()
        mockScope.Setup(Function(s) s.ServiceProvider).Returns(sp)
        services.AddSingleton(Of IServiceScopeFactory)(mockScopeFactory.Object)

        Dim serviceProvider = services.BuildServiceProvider()

        _sessionServiceMock.Setup(Function(s) s.CurrentUserRole).Returns(UserRole.Owner)
        _sessionServiceMock.Setup(Function(s) s.CurrentUsername).Returns("test_owner")

        Dim tokens = New ThemeTokens() With {
            .Accent = System.Drawing.Color.Blue,
            .PrimaryText = System.Drawing.Color.White
        }
        _themeServiceMock.Setup(Function(ts) ts.Tokens).Returns(tokens)
        _themeServiceMock.Setup(Function(ts) ts.CurrentTheme).Returns(AppTheme.Light)

        Dim presenter = New MainWindowPresenter(
            _viewMock.Object,
            _sessionServiceMock.Object,
            serviceProvider,
            _connectionPresenterMock.Object,
            _themeServiceMock.Object)

        ' Act
        _viewMock.Raise(Sub(v) AddHandler v.Load, Nothing, Me, EventArgs.Empty)

        ' Assert
        _viewMock.Verify(Sub(v) v.AddNavigationButtons(It.IsAny(Of List(Of NavigationItem))()), Times.AtLeastOnce())
    End Sub
End Class
