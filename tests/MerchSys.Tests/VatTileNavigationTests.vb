Imports Xunit
Imports Moq
Imports MediatR
Imports MerchSys.Accounting.Presenters
Imports MerchSys.Accounting.Views
Imports MerchSys.Accounting.Services
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.SharedKernel.Enums
Imports System.Threading.Tasks
Imports Microsoft.Extensions.DependencyInjection

Public Class VatTileNavigationTests

    <Fact>
    Public Sub OnVatTileClicked_ManagerRole_RaisesNavigateEvent()
        ' Arrange
        Dim mockSession = New Mock(Of ISessionService)()
        mockSession.Setup(Function(s) s.CurrentUserRole).Returns(UserRole.Manager)

        Dim presenter = New FinancialOverviewPresenter(
            Mock.Of(Of IFinancialOverviewService)(),
            Mock.Of(Of IWhatThisMeansService)(),
            Mock.Of(Of INotificationService)(),
            Mock.Of(Of IMediator)(),
            mockSession.Object)

        Dim mockView = New Mock(Of IFinancialOverviewView)()
        presenter.View = mockView.Object

        Dim eventRaised As Boolean = False
        AddHandler presenter.NavigateToVatReturnRequested, Sub(sender, e) eventRaised = True

        ' Act
        mockView.Raise(Sub(v) AddHandler v.VatTileClicked, Nothing, EventArgs.Empty)

        ' Assert
        Assert.True(eventRaised, "Event should be raised for Manager role.")
    End Sub

    <Fact>
    Public Sub OnVatTileClicked_OwnerRole_DoesNotRaiseNavigateEvent()
        ' Arrange
        Dim mockSession = New Mock(Of ISessionService)()
        mockSession.Setup(Function(s) s.CurrentUserRole).Returns(UserRole.Owner)

        Dim presenter = New FinancialOverviewPresenter(
            Mock.Of(Of IFinancialOverviewService)(),
            Mock.Of(Of IWhatThisMeansService)(),
            Mock.Of(Of INotificationService)(),
            Mock.Of(Of IMediator)(),
            mockSession.Object)

        Dim mockView = New Mock(Of IFinancialOverviewView)()
        presenter.View = mockView.Object

        Dim eventRaised As Boolean = False
        AddHandler presenter.NavigateToVatReturnRequested, Sub(sender, e) eventRaised = True

        ' Act
        mockView.Raise(Sub(v) AddHandler v.VatTileClicked, Nothing, EventArgs.Empty)

        ' Assert
        Assert.False(eventRaised, "Event should not be raised for Owner role.")
    End Sub

#If DEBUG Then
    <Fact>
    Public Async Function VatTileSmokeHarness_RunsSuccessfully() As Task
        ' Arrange
        Dim harness As New MerchSys.Accounting.Debug.VatTileSmokeHarness()

        Dim services As New ServiceCollection()
        services.AddSingleton(Mock.Of(Of IMediator)())
        services.AddSingleton(Mock.Of(Of Microsoft.Extensions.Logging.ILogger(Of FinancialOverviewService))())
        services.AddSingleton(Mock.Of(Of Microsoft.Extensions.Logging.ILogger(Of VatReportingService))())
        Dim serviceProvider = services.BuildServiceProvider()

        ' Act
        Dim reportPath = Await harness.RunAndReportAsync(serviceProvider)

        ' Assert
        Assert.True(System.IO.File.Exists(reportPath), "Report file should be generated.")

        Dim content = System.IO.File.ReadAllText(reportPath)
        Assert.Contains("## VAT Calculation: PASS", content)
        Assert.Contains("## Role Navigation (Manager): PASS", content)
        Assert.Contains("## Role Navigation (Developer): PASS", content)
        Assert.Contains("## Role Navigation (Owner): PASS", content)

        ' Cleanup
        If System.IO.File.Exists(reportPath) Then
            System.IO.File.Delete(reportPath)
        End If
    End Function
#End If
End Class
