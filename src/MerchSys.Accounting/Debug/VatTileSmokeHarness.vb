#If DEBUG Then
Imports System.IO
Imports System.Threading.Tasks
Imports System.Threading
Imports MerchSys.Accounting.Services
Imports MerchSys.Accounting.Presenters
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Interfaces

Namespace Debug
    Public Class VatTileSmokeHarness
        Public Async Function RunAndReportAsync(
                fakeFinancialOverviewService As IFinancialOverviewService,
                fakeKpiProvider As IKpiProvider,
                fakeSessionServiceManager As ISessionService,
                fakeSessionServiceOwner As ISessionService,
                fakeWhatThisMeans As IWhatThisMeansService,
                fakeNotification As INotificationService,
                fakeMediator As MediatR.IMediator) As Task(Of String)

            Dim calcPass = False
            Dim navPass = False

            ' 1. Test VAT Calculation (Uses provided fake KPI provider configured with Output=12000, Input=3000 -> amount 9000)
            Dim svc = New VatEnrichedFinancialOverviewService(fakeFinancialOverviewService, New List(Of IKpiProvider) From {fakeKpiProvider})
            Dim dto = Await svc.GetOverviewAsync()

            If dto.VatPayable = 9000D Then
                calcPass = True
            End If

            ' 2. Test Role Navigation
            Dim presenterManager = New FinancialOverviewPresenter(
                fakeFinancialOverviewService, fakeWhatThisMeans, fakeNotification, fakeMediator, fakeSessionServiceManager)

            Dim eventRaisedForManager = False
            AddHandler presenterManager.NavigateToVatReturnRequested, Sub(s, e) eventRaisedForManager = True

            ' Use reflection to trigger the private click handler
            Dim methodInfo = GetType(FinancialOverviewPresenter).GetMethod("OnVatTileClicked", System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Instance)
            methodInfo.Invoke(presenterManager, New Object() {Nothing, EventArgs.Empty})

            Dim presenterOwner = New FinancialOverviewPresenter(
                fakeFinancialOverviewService, fakeWhatThisMeans, fakeNotification, fakeMediator, fakeSessionServiceOwner)

            Dim eventRaisedForOwner = False
            AddHandler presenterOwner.NavigateToVatReturnRequested, Sub(s, e) eventRaisedForOwner = True

            methodInfo.Invoke(presenterOwner, New Object() {Nothing, EventArgs.Empty})

            If eventRaisedForManager AndAlso Not eventRaisedForOwner Then
                navPass = True
            End If

            ' 3. Generate Report
            Dim reportPath = Path.Combine(Path.GetTempPath(), $"vat-tile-smoke-report-{DateTime.UtcNow:yyyyMMddHHmmss}.md")

            Dim sb As New System.Text.StringBuilder()
            sb.AppendLine("# VAT Tile Integration Smoke Report")
            sb.AppendLine(If(calcPass, "## VAT Calculation: PASS", "## VAT Calculation: FAIL"))
            sb.AppendLine(If(navPass, "## Role-Based Navigation: PASS", "## Role-Based Navigation: FAIL"))

            Await File.WriteAllTextAsync(reportPath, sb.ToString())

            Return reportPath
        End Function
    End Class
End Namespace
#End If
