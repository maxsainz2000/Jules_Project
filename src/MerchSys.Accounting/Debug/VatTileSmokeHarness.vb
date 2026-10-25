#If DEBUG Then
Imports System.IO
Imports System.Threading
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities
Imports MerchSys.Accounting.Enums
Imports MerchSys.Accounting.Services
Imports MerchSys.Accounting.Presenters
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Interfaces
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Diagnostics
Imports Microsoft.Extensions.Logging
Imports MediatR
Imports MerchSys.Accounting.Views
Imports System.Reflection

Namespace Debug
    Public Class VatTileSmokeHarness
        Public Async Function RunAndReportAsync() As Task(Of String)
            Dim dbName = "SmokeTestDb_" & Guid.NewGuid().ToString()
            Dim options = New DbContextOptionsBuilder(Of AccountingDbContext)().
                UseInMemoryDatabase(dbName).
                ConfigureWarnings(Sub(w) w.Ignore(InMemoryEventId.TransactionIgnoredWarning)).
                Options

            Dim report = New Text.StringBuilder()
            report.AppendLine("# VAT Tile Integration Smoke Report")
            report.AppendLine($"Run Time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC")

            Using db As New AccountingDbContext(options)
                db.Database.EnsureCreated()

                ' Seed records
                db.RevenueRecords.Add(New RevenueRecord With {
                    .RecordDate = DateTime.UtcNow.AddMonths(-1),
                    .ProductId = 1,
                    .ProductName = "Test Product",
                    .QuantitySold = 1,
                    .UnitPrice = 112000,
                    .TotalRevenue = 112000,
                    .VatableAmount = 100000,
                    .OutputVat = 12000,
                    .GrossProfit = 50000
                })

                db.ExpenseRecords.Add(New ExpenseRecord With {
                    .ExpenseDate = DateTime.UtcNow.AddMonths(-1),
                    .Category = "Purchases",
                    .Description = "Test Expense",
                    .Amount = 28000,
                    .VatableAmount = 25000,
                    .InputVat = 3000
                })

                Await db.SaveChangesAsync()
            End Using

            Using db As New AccountingDbContext(options)
                ' Setup dependencies using mock-like proxies without Moq
                Dim fakeMediator = New FakeMediator()
                Dim fakeLogger = New FakeLogger(Of VatReportingService)()
                Dim vatService = New VatReportingService(db, fakeMediator, fakeLogger)
                Dim kpiProvider = New VatPayableKpiProvider(vatService)

                Dim fakeOverviewService = New FakeFinancialOverviewService()

                Dim enrichedService = New VatEnrichedFinancialOverviewService(fakeOverviewService, {kpiProvider})

                Dim overview = Await enrichedService.GetOverviewAsync()

                If overview.VatPayable = 9000D Then
                    report.AppendLine("## VAT Calculation: PASS")
                    report.AppendLine("- Expected: 9,000.00")
                    report.AppendLine($"- Actual: {overview.VatPayable:N2}")
                Else
                    report.AppendLine("## VAT Calculation: FAIL")
                    report.AppendLine("- Expected: 9,000.00")
                    report.AppendLine($"- Actual: {overview.VatPayable:N2}")
                End If

                ' Test Navigation Permissions
                Dim fakeWhatThisMeans = New FakeWhatThisMeansService()
                Dim fakeNotification = New FakeNotificationService()

                ' Test Manager Role
                Dim managerSession = New FakeSessionService()
                managerSession.SetRole(UserRole.Manager)

                Dim managerPresenter = New FinancialOverviewPresenter(enrichedService, fakeWhatThisMeans, fakeNotification, fakeMediator, managerSession)
                Dim managerView = New FakeFinancialOverviewView()
                managerPresenter.View = managerView

                Dim managerNavigated = False
                AddHandler managerPresenter.NavigateToVatReturnRequested, Sub(s, e) managerNavigated = True
                managerView.SimulateVatTileClicked()

                report.AppendLine(If(managerNavigated, "## Manager Navigation: PASS", "## Manager Navigation: FAIL"))

                ' Test Owner Role
                Dim ownerSession = New FakeSessionService()
                ownerSession.SetRole(UserRole.Owner)

                Dim ownerPresenter = New FinancialOverviewPresenter(enrichedService, fakeWhatThisMeans, fakeNotification, fakeMediator, ownerSession)
                Dim ownerView = New FakeFinancialOverviewView()
                ownerPresenter.View = ownerView

                Dim ownerNavigated = False
                AddHandler ownerPresenter.NavigateToVatReturnRequested, Sub(s, e) ownerNavigated = True
                ownerView.SimulateVatTileClicked()

                report.AppendLine(If(Not ownerNavigated, "## Owner Navigation: PASS", "## Owner Navigation: FAIL"))
            End Using

            Dim path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"vat-tile-smoke-report-{DateTime.UtcNow:yyyyMMddHHmmss}.md")
            File.WriteAllText(path, report.ToString())
            Return path
        End Function
    End Class

    Public Class FakeMediator
        Implements IMediator
        Public Function Send(Of TResponse)(request As IRequest(Of TResponse), Optional cancellationToken As CancellationToken = Nothing) As Task(Of TResponse) Implements IMediator.Send
            Return Task.FromResult(CType(Nothing, TResponse))
        End Function
        Public Function Send(request As Object, Optional cancellationToken As CancellationToken = Nothing) As Task(Of Object) Implements IMediator.Send
            Return Task.FromResult(CType(Nothing, Object))
        End Function
        Public Function Send(Of TRequest As IRequest)(request As TRequest, Optional cancellationToken As CancellationToken = Nothing) As Task Implements ISender.Send
            Return Task.CompletedTask
        End Function
        Public Function Publish(notification As Object, Optional cancellationToken As CancellationToken = Nothing) As Task Implements IPublisher.Publish
            Return Task.CompletedTask
        End Function
        Public Function Publish(Of TNotification As INotification)(notification As TNotification, Optional cancellationToken As CancellationToken = Nothing) As Task Implements IPublisher.Publish
            Return Task.CompletedTask
        End Function
        Public Function CreateStream(Of TResponse)(request As IStreamRequest(Of TResponse), Optional cancellationToken As CancellationToken = Nothing) As IAsyncEnumerable(Of TResponse) Implements IMediator.CreateStream
            Return Nothing
        End Function
        Public Function CreateStream(request As Object, Optional cancellationToken As CancellationToken = Nothing) As IAsyncEnumerable(Of Object) Implements IMediator.CreateStream
            Return Nothing
        End Function
    End Class

    Public Class FakeLogger(Of T)
        Implements ILogger(Of T)
        Public Sub Log(Of TState)(logLevel As LogLevel, eventId As EventId, state As TState, exception As Exception, formatter As Func(Of TState, Exception, String)) Implements ILogger.Log
        End Sub
        Public Function IsEnabled(logLevel As LogLevel) As Boolean Implements ILogger.IsEnabled
            Return False
        End Function
        Public Function BeginScope(Of TState)(state As TState) As IDisposable Implements ILogger.BeginScope
            Return Nothing
        End Function
    End Class

    Public Class FakeFinancialOverviewService
        Implements IFinancialOverviewService
        Public Function GetOverviewAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of FinancialOverviewDto) Implements IFinancialOverviewService.GetOverviewAsync
            Return Task.FromResult(New FinancialOverviewDto())
        End Function
        Public Function RefreshSnapshotAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of FinancialSnapshot) Implements IFinancialOverviewService.RefreshSnapshotAsync
            Return Task.FromResult(New FinancialSnapshot())
        End Function
    End Class

    Public Class FakeWhatThisMeansService
        Implements IWhatThisMeansService
        Public Function GenerateOverviewInterpretation(overview As FinancialOverviewDto) As String Implements IWhatThisMeansService.GenerateOverviewInterpretation
            Return "Fake Interpretation"
        End Function
        Public Function GenerateIncomeStatementInterpretation(statement As IncomeStatementDto, Optional previousMargin As Decimal? = Nothing) As String Implements IWhatThisMeansService.GenerateIncomeStatementInterpretation
            Return "Fake Interpretation"
        End Function
        Public Function GenerateSalesSummaryInterpretation(summary As AccountingSalesSummaryDto) As String Implements IWhatThisMeansService.GenerateSalesSummaryInterpretation
            Return "Fake Interpretation"
        End Function
        Public Function GenerateMarginAlert(currentMargin As Decimal, previousMargin As Decimal, Optional thresholdPercent As Decimal = 3.0D) As String Implements IWhatThisMeansService.GenerateMarginAlert
            Return "Fake Alert"
        End Function
    End Class

    Public Class FakeNotificationService
        Implements INotificationService
        Public Sub ShowSuccess(message As String) Implements INotificationService.ShowSuccess
        End Sub
        Public Sub ShowError(message As String) Implements INotificationService.ShowError
        End Sub
        Public Sub ShowWarning(message As String) Implements INotificationService.ShowWarning
        End Sub
        Public Sub ShowInfo(message As String) Implements INotificationService.ShowInfo
        End Sub
    End Class

    Public Class FakeSessionService
        Implements ISessionService
        Private _role As UserRole?

        Public Sub New()
            _role = Nothing
        End Sub

        Public Sub SetRole(role As UserRole?)
            _role = role
        End Sub

        Public ReadOnly Property CurrentUserRole As UserRole? Implements ISessionService.CurrentUserRole
            Get
                Return _role
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

    Public Class FakeFinancialOverviewView
        Implements IFinancialOverviewView

        Public Property TodayRevenueDisplay As String Implements IFinancialOverviewView.TodayRevenueDisplay
        Public Property MtdRevenueDisplay As String Implements IFinancialOverviewView.MtdRevenueDisplay
        Public Property YtdRevenueDisplay As String Implements IFinancialOverviewView.YtdRevenueDisplay
        Public Property TotalARDisplay As String Implements IFinancialOverviewView.TotalARDisplay
        Public Property TotalAPDisplay As String Implements IFinancialOverviewView.TotalAPDisplay
        Public Property InventoryValueDisplay As String Implements IFinancialOverviewView.InventoryValueDisplay
        Public Property GrossMarginTodayDisplay As String Implements IFinancialOverviewView.GrossMarginTodayDisplay
        Public Property WhatThisMeansText As String Implements IFinancialOverviewView.WhatThisMeansText
        Public Property OverdueARCount As Integer Implements IFinancialOverviewView.OverdueARCount
        Public Property OverdueAPCount As Integer Implements IFinancialOverviewView.OverdueAPCount
        Public Property LowStockAlertCount As Integer Implements IFinancialOverviewView.LowStockAlertCount
        Public Property IsLoading As Boolean Implements IFinancialOverviewView.IsLoading
        Public Property ErrorMessage As String Implements IFinancialOverviewView.ErrorMessage
        Public Property LastRefreshedText As String Implements IFinancialOverviewView.LastRefreshedText
        Public Property VatPayableDisplay As String Implements IFinancialOverviewView.VatPayableDisplay
        Public Property VatDueDateDisplay As String Implements IFinancialOverviewView.VatDueDateDisplay
        Public Property VatSeverity As KpiSeverity Implements IFinancialOverviewView.VatSeverity

        Public Sub SetTopSellingProducts(products As IEnumerable(Of TopProductDto)) Implements IFinancialOverviewView.SetTopSellingProducts
        End Sub

        Public Sub SetMonthlyTrends(trends As IEnumerable(Of MonthlyTrendDto)) Implements IFinancialOverviewView.SetMonthlyTrends
        End Sub

        Public Event RefreshRequested As EventHandler Implements IFinancialOverviewView.RefreshRequested
        Public Event ViewLoaded As EventHandler Implements IFinancialOverviewView.ViewLoaded
        Public Event VatTileClicked As EventHandler Implements IFinancialOverviewView.VatTileClicked

        Public Sub SimulateVatTileClicked()
            RaiseEvent VatTileClicked(Me, EventArgs.Empty)
        End Sub
    End Class
End Namespace
#End If
