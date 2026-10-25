Imports System.IO
Imports Xunit
Imports MediatR
Imports MerchSys.Accounting.Presenters
Imports MerchSys.Accounting.Services
Imports MerchSys.Accounting.Views
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.Accounting.Debug

Namespace Tests
    Public Class VatTileNavigationTests
        <Fact>
        Public Sub VatTileClicked_ManagerRole_RaisesNavigateToVatReturnRequested()
            ' Arrange
            Dim fakeOverview = New FakeFinancialOverviewService()
            Dim fakeWhatThisMeans = New FakeWhatThisMeansService()
            Dim fakeNotification = New FakeNotificationService()
            Dim fakeMediator = New FakeMediator()

            Dim fakeSession = New FakeSessionServiceForTests() With { .Role = UserRole.Manager }

            Dim presenter = New FinancialOverviewPresenter(fakeOverview, fakeWhatThisMeans, fakeNotification, fakeMediator, fakeSession)
            Dim fakeView = New FakeFinancialOverviewView()
            presenter.View = fakeView

            Dim eventRaised = False
            AddHandler presenter.NavigateToVatReturnRequested, Sub(sender, e) eventRaised = True

            ' Act
            fakeView.SimulateVatTileClicked()

            ' Assert
            Assert.True(eventRaised)
        End Sub

        <Fact>
        Public Sub VatTileClicked_OwnerRole_SuppressesNavigation()
            ' Arrange
            Dim fakeOverview = New FakeFinancialOverviewService()
            Dim fakeWhatThisMeans = New FakeWhatThisMeansService()
            Dim fakeNotification = New FakeNotificationService()
            Dim fakeMediator = New FakeMediator()

            Dim fakeSession = New FakeSessionServiceForTests() With { .Role = UserRole.Owner }

            Dim presenter = New FinancialOverviewPresenter(fakeOverview, fakeWhatThisMeans, fakeNotification, fakeMediator, fakeSession)
            Dim fakeView = New FakeFinancialOverviewView()
            presenter.View = fakeView

            Dim eventRaised = False
            AddHandler presenter.NavigateToVatReturnRequested, Sub(sender, e) eventRaised = True

            ' Act
            fakeView.SimulateVatTileClicked()

            ' Assert
            Assert.False(eventRaised)
        End Sub

        <Fact>
        Public Async Function VatTileSmokeHarness_GeneratesReport_WithPass() As Task
#If DEBUG Then
            ' Arrange
            Dim harness = New VatTileSmokeHarness()

            ' Act
            Dim reportPath = Await harness.RunAndReportAsync()

            ' Assert
            Assert.True(File.Exists(reportPath))
            Dim reportContent = File.ReadAllText(reportPath)
            Assert.Contains("## VAT Calculation: PASS", reportContent)
            Assert.Contains("## Manager Navigation: PASS", reportContent)
            Assert.Contains("## Owner Navigation: PASS", reportContent)

            File.Delete(reportPath)
#Else
            Assert.True(True, "Skipping VatTileSmokeHarness in Release build")
#End If
        End Function
    End Class

    Public Class FakeSessionServiceForTests
        Implements ISessionService

        Public Property Role As UserRole?

        Public ReadOnly Property CurrentUserRole As UserRole? Implements ISessionService.CurrentUserRole
            Get
                Return Role
            End Get
        End Property
        Public ReadOnly Property CurrentUsername As String Implements ISessionService.CurrentUsername
            Get
                Return "Fake User"
            End Get
        End Property
        Public ReadOnly Property IsAuthenticated As Boolean Implements ISessionService.IsAuthenticated
            Get
                Return True
            End Get
        End Property
        Public Sub ClearUser() Implements ISessionService.ClearUser
        End Sub
    End Class
End Namespace
