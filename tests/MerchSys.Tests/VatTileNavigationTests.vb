Imports System.IO
Imports System.Threading.Tasks
Imports Xunit
Imports Moq
Imports MerchSys.Accounting.Presenters
Imports MerchSys.Accounting.Views
Imports MerchSys.Accounting.Services
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.SharedKernel.Enums
Imports MediatR
Imports MerchSys.Tests.Fakes

Namespace Tests

    Public Class VatTileNavigationTests

        <Fact>
        Public Sub ManagerRole_ClickingVatTile_RaisesNavigateToVatReturnRequested()
            ' Arrange
            Dim mockView = New Mock(Of IFinancialOverviewView)()
            Dim mockOverviewService = New Mock(Of IFinancialOverviewService)()
            Dim mockWhatThisMeansService = New Mock(Of IWhatThisMeansService)()
            Dim mockNotificationService = New Mock(Of INotificationService)()
            Dim mockMediator = New Mock(Of IMediator)()
            Dim mockSessionService = New Mock(Of ISessionService)()

            mockSessionService.Setup(Function(s) s.CurrentUserRole).Returns(UserRole.Manager)

            Dim presenter As New FinancialOverviewPresenter(
                mockOverviewService.Object,
                mockWhatThisMeansService.Object,
                mockNotificationService.Object,
                mockMediator.Object,
                mockSessionService.Object)

            presenter.View = mockView.Object

            Dim eventRaised = False
            AddHandler presenter.NavigateToVatReturnRequested, Sub(sender, e) eventRaised = True

            ' Act
            mockView.Raise(Sub(v) AddHandler v.VatTileClicked, Nothing, EventArgs.Empty)

            ' Assert
            Assert.True(eventRaised, "Event should be raised for Manager role.")
        End Sub

        <Fact>
        Public Sub OwnerRole_ClickingVatTile_DoesNotRaiseNavigateToVatReturnRequested()
            ' Arrange
            Dim mockView = New Mock(Of IFinancialOverviewView)()
            Dim mockOverviewService = New Mock(Of IFinancialOverviewService)()
            Dim mockWhatThisMeansService = New Mock(Of IWhatThisMeansService)()
            Dim mockNotificationService = New Mock(Of INotificationService)()
            Dim mockMediator = New Mock(Of IMediator)()
            Dim mockSessionService = New Mock(Of ISessionService)()

            mockSessionService.Setup(Function(s) s.CurrentUserRole).Returns(UserRole.Owner)

            Dim presenter As New FinancialOverviewPresenter(
                mockOverviewService.Object,
                mockWhatThisMeansService.Object,
                mockNotificationService.Object,
                mockMediator.Object,
                mockSessionService.Object)

            presenter.View = mockView.Object

            Dim eventRaised = False
            AddHandler presenter.NavigateToVatReturnRequested, Sub(sender, e) eventRaised = True

            ' Act
            mockView.Raise(Sub(v) AddHandler v.VatTileClicked, Nothing, EventArgs.Empty)

            ' Assert
            Assert.False(eventRaised, "Event should NOT be raised for Owner role.")
        End Sub

        <Fact>
        Public Async Function VatTileSmokeHarness_GeneratesExpectedMarkdownReport() As Task
            ' Arrange
            Await Task.CompletedTask
#If DEBUG Then
            Dim harness As New MerchSys.Accounting.Debug.VatTileSmokeHarness()
            Dim fakeOverviewService As New FakeFinancialOverviewService()
            Dim fakeKpiProvider As New FakeKpiProvider()

            ' Act
            Dim reportPath = Await harness.RunAndReportAsync(fakeOverviewService, New IKpiProvider() {fakeKpiProvider})

            ' Assert
            Assert.True(File.Exists(reportPath))
            Dim content = Await File.ReadAllTextAsync(reportPath)

            Assert.Contains("## VAT Calculation: PASS", content)
            Assert.Contains("## Role-Based Navigation: PASS", content)
#End If
        End Function

    End Class

End Namespace
