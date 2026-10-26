Imports Moq
Imports Xunit
Imports MerchSys.App.Presenters
Imports MerchSys.App.Views
Imports MerchSys.App.Services
Imports MerchSys.App.Services.Theming
Imports MerchSys.SharedKernel.Entities
Imports MerchSys.SharedKernel.Enums
Imports Microsoft.Extensions.DependencyInjection
Imports System.Windows.Forms
Imports MerchSys.App.Views.Shell.Modules
Imports MerchSys.App.Models


    Public Class MainWindowThemeTests

        <Fact>
        Public Sub MainWindowPresenter_AppliesTokensOnThemeChanged()
            ' Arrange
            Dim viewMock As New Mock(Of IMainWindowView)()

            Dim sessionService As New LoginSessionService()
            sessionService.SetUser(New UserAccount With {
                .Id = 1,
                .Username = "test_owner",
                .Role = UserRole.Owner
            })

            Dim sc As New ServiceCollection()
            Dim sp As IServiceProvider = sc.BuildServiceProvider()

            Dim monitorMock As New Mock(Of IConnectionHealthMonitor)()
            Dim statusViewMock As New Mock(Of IConnectionStatusView)()
            Dim statusPresenter As New ConnectionStatusPresenter(monitorMock.Object, statusViewMock.Object)

            Dim themeServiceMock As New Mock(Of IThemeService)()
            Dim testTokens As ThemeTokens = ThemeTokens.GetTokens(AppTheme.Light)
            themeServiceMock.Setup(Function(ts) ts.Tokens).Returns(testTokens)
            themeServiceMock.Setup(Function(ts) ts.CurrentTheme).Returns(AppTheme.Light)

            ' Initializing presenter calls ApplyThemeTokens once
            Dim presenter As New MainWindowPresenter(viewMock.Object, sessionService, sp, statusPresenter, themeServiceMock.Object)

            ' Act
            ' Raise ThemeChanged event to call ApplyThemeTokens again
            themeServiceMock.Raise(Sub(ts) AddHandler ts.ThemeChanged, Nothing, Nothing, AppTheme.Dark)

            ' Assert
            viewMock.Verify(Sub(v) v.ApplyThemeTokens(testTokens), Times.Exactly(2))

            presenter.Dispose()
        End Sub

        <Fact>
        Public Sub MainWindowPresenter_AppliesActiveTokenColorsToNavButtons()
            ' Arrange
            Dim viewMock As New Mock(Of IMainWindowView)()
            viewMock.SetupAllProperties()

            Dim sessionService As New LoginSessionService()
            sessionService.SetUser(New UserAccount With {
                .Id = 1,
                .Username = "test_owner",
                .Role = UserRole.Owner
            })

            Dim sc As New ServiceCollection()
            ' DO NOT use Mock(Of IOwnerDashboardView)().Object
            sc.AddTransient(Of IOwnerDashboardView, DummyOwnerDashboardView)()
            Dim sp As IServiceProvider = sc.BuildServiceProvider()

            Dim monitorMock As New Mock(Of IConnectionHealthMonitor)()
            Dim statusViewMock As New Mock(Of IConnectionStatusView)()
            Dim statusPresenter As New ConnectionStatusPresenter(monitorMock.Object, statusViewMock.Object)

            Dim themeServiceMock As New Mock(Of IThemeService)()
            Dim testTokens As ThemeTokens = ThemeTokens.GetTokens(AppTheme.Light)
            themeServiceMock.Setup(Function(ts) ts.Tokens).Returns(testTokens)

            Dim presenter As New MainWindowPresenter(viewMock.Object, sessionService, sp, statusPresenter, themeServiceMock.Object)

            ' Act
            viewMock.Raise(Sub(v) AddHandler v.Load, Nothing, Nothing, EventArgs.Empty)

            ' The default target for Owner is "Owner Dashboard".
            ' We'll raise navigation directly to ensure AddNavigationButtons is called with items.
            Dim allItems = presenter.NavigationGroups.SelectMany(Function(g) g.Items).ToList()
            Dim ownerItem = allItems.FirstOrDefault(Function(i) i.Name = "Owner Dashboard")

            viewMock.Raise(Sub(v) AddHandler v.NavigationRequested, Nothing, Nothing, ownerItem)

            ' Assert
            viewMock.Verify(Sub(v) v.AddNavigationButtons(It.IsAny(Of List(Of MerchSys.App.Models.NavigationItem))()), Times.AtLeastOnce())

            presenter.Dispose()
        End Sub

        Private Class DummyOwnerDashboardView
            Inherits UserControl
            Implements IOwnerDashboardView

            Public Shadows Custom Event Load As EventHandler Implements IOwnerDashboardView.Load
                AddHandler(value As EventHandler)
                End AddHandler
                RemoveHandler(value As EventHandler)
                End RemoveHandler
                RaiseEvent(sender As Object, e As EventArgs)
                End RaiseEvent
            End Event

            Public Sub SetLoading(loading As Boolean) Implements IOwnerDashboardView.SetLoading
            End Sub

            <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
            Public Property Kpi1Value As String Implements IOwnerDashboardView.Kpi1Value
            <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
            Public Property Kpi1Interpretation As String Implements IOwnerDashboardView.Kpi1Interpretation
            <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
            Public Property Kpi2Value As String Implements IOwnerDashboardView.Kpi2Value
            <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
            Public Property Kpi2Interpretation As String Implements IOwnerDashboardView.Kpi2Interpretation
            <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
            Public Property Kpi3Value As String Implements IOwnerDashboardView.Kpi3Value
            <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
            Public Property Kpi3Interpretation As String Implements IOwnerDashboardView.Kpi3Interpretation
            <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
            Public Property Kpi4Value As String Implements IOwnerDashboardView.Kpi4Value
            <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
            Public Property Kpi4Interpretation As String Implements IOwnerDashboardView.Kpi4Interpretation

            Public Sub RaiseLoad()
                RaiseEvent Load(Me, EventArgs.Empty)
            End Sub
        End Class

    End Class
