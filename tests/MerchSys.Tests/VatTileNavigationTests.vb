Imports System.IO
Imports System.Threading.Tasks
Imports Xunit
Imports Moq
Imports MediatR
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.Accounting.Presenters
Imports MerchSys.Accounting.Services
#If DEBUG Then
Imports MerchSys.Accounting.Debug
#End If

Public Class VatTileNavigationTests

    <Fact>
    Public Sub OnVatTileClicked_ManagerRole_RaisesNavigateToVatReturnRequested()
        ' Arrange
        Dim mockSession = New Mock(Of ISessionService)()
        mockSession.Setup(Function(s) s.CurrentUserRole).Returns(UserRole.Manager)

        Dim mockInner = New Mock(Of IFinancialOverviewService)()
        Dim mockWhat = New Mock(Of IWhatThisMeansService)()
        Dim mockNotif = New Mock(Of INotificationService)()
        Dim mockMed = New Mock(Of IMediator)()

        Dim presenter = New FinancialOverviewPresenter(mockInner.Object, mockWhat.Object, mockNotif.Object, mockMed.Object, mockSession.Object)
        Dim eventRaised = False
        AddHandler presenter.NavigateToVatReturnRequested, Sub(s, e) eventRaised = True

        ' Act
        Dim methodInfo = GetType(FinancialOverviewPresenter).GetMethod("OnVatTileClicked", System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Instance)
        methodInfo.Invoke(presenter, New Object() {Nothing, EventArgs.Empty})

        ' Assert
        Assert.True(eventRaised)
    End Sub

    <Fact>
    Public Sub OnVatTileClicked_DeveloperRole_RaisesNavigateToVatReturnRequested()
        ' Arrange
        Dim mockSession = New Mock(Of ISessionService)()
        mockSession.Setup(Function(s) s.CurrentUserRole).Returns(UserRole.Developer)

        Dim mockInner = New Mock(Of IFinancialOverviewService)()
        Dim mockWhat = New Mock(Of IWhatThisMeansService)()
        Dim mockNotif = New Mock(Of INotificationService)()
        Dim mockMed = New Mock(Of IMediator)()

        Dim presenter = New FinancialOverviewPresenter(mockInner.Object, mockWhat.Object, mockNotif.Object, mockMed.Object, mockSession.Object)
        Dim eventRaised = False
        AddHandler presenter.NavigateToVatReturnRequested, Sub(s, e) eventRaised = True

        ' Act
        Dim methodInfo = GetType(FinancialOverviewPresenter).GetMethod("OnVatTileClicked", System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Instance)
        methodInfo.Invoke(presenter, New Object() {Nothing, EventArgs.Empty})

        ' Assert
        Assert.True(eventRaised)
    End Sub

    <Fact>
    Public Sub OnVatTileClicked_OwnerRole_DoesNotRaiseNavigateToVatReturnRequested()
        ' Arrange
        Dim mockSession = New Mock(Of ISessionService)()
        mockSession.Setup(Function(s) s.CurrentUserRole).Returns(UserRole.Owner)

        Dim mockInner = New Mock(Of IFinancialOverviewService)()
        Dim mockWhat = New Mock(Of IWhatThisMeansService)()
        Dim mockNotif = New Mock(Of INotificationService)()
        Dim mockMed = New Mock(Of IMediator)()

        Dim presenter = New FinancialOverviewPresenter(mockInner.Object, mockWhat.Object, mockNotif.Object, mockMed.Object, mockSession.Object)
        Dim eventRaised = False
        AddHandler presenter.NavigateToVatReturnRequested, Sub(s, e) eventRaised = True

        ' Act
        Dim methodInfo = GetType(FinancialOverviewPresenter).GetMethod("OnVatTileClicked", System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Instance)
        methodInfo.Invoke(presenter, New Object() {Nothing, EventArgs.Empty})

        ' Assert
        Assert.False(eventRaised)
    End Sub

    <Fact>
    Public Async Function VatTileSmokeHarness_GeneratesExpectedMarkdownReport() As Task
#If DEBUG Then
        ' Arrange
        Dim harness = New VatTileSmokeHarness()

        Dim fakeOverview = New FakeFinancialOverviewService()
        Dim fakeKpi = New FakeKpiProvider() With {.OutputVat = 12000D, .InputVat = 3000D}
        Dim fakeSessionManager = New FakeSessionService() With {.Role = UserRole.Manager}
        Dim fakeSessionOwner = New FakeSessionService() With {.Role = UserRole.Owner}
        Dim mockWhat = New Mock(Of IWhatThisMeansService)()
        Dim mockNotif = New Mock(Of INotificationService)()
        Dim mockMed = New Mock(Of IMediator)()

        ' Act
        Dim reportPath = Await harness.RunAndReportAsync(
            fakeOverview, fakeKpi, fakeSessionManager, fakeSessionOwner, mockWhat.Object, mockNotif.Object, mockMed.Object)

        ' Assert
        Assert.True(File.Exists(reportPath))

        Dim contents = Await File.ReadAllTextAsync(reportPath)
        Assert.Contains("## VAT Calculation: PASS", contents)
        Assert.Contains("## Role-Based Navigation: PASS", contents)

        ' Clean up
        File.Delete(reportPath)
#Else
        ' Smoke harness is not available in Release mode
        Await Task.CompletedTask
#End If
    End Function

End Class
