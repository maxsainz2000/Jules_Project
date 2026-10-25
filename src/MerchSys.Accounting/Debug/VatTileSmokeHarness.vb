#If DEBUG Then
Imports System.IO
Imports System.Threading.Tasks
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities
Imports MerchSys.Accounting.Services
Imports MerchSys.Accounting.Presenters
Imports MerchSys.Accounting.Views
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.SharedKernel.Enums
Imports MediatR
Imports System.Collections.Generic

Namespace Debug

    Public Class VatTileSmokeHarness

        Private Class DummySessionService
            Implements ISessionService

            Public Property CurrentUserRole As UserRole? Implements ISessionService.CurrentUserRole
            Public Property CurrentUsername As String Implements ISessionService.CurrentUsername
            Public Property IsAuthenticated As Boolean Implements ISessionService.IsAuthenticated

            Public Sub ClearUser() Implements ISessionService.ClearUser
            End Sub
        End Class

        Private Class DummyNotificationService
            Implements INotificationService
            Public Sub ShowError(message As String) Implements INotificationService.ShowError
            End Sub
            Public Sub ShowInfo(message As String) Implements INotificationService.ShowInfo
            End Sub
            Public Sub ShowSuccess(message As String) Implements INotificationService.ShowSuccess
            End Sub
            Public Sub ShowWarning(message As String) Implements INotificationService.ShowWarning
            End Sub
        End Class

        Private Class DummyWhatThisMeansService
            Implements IWhatThisMeansService
            Public Function GenerateOverviewInterpretation(overview As FinancialOverviewDto) As String Implements IWhatThisMeansService.GenerateOverviewInterpretation
                Return ""
            End Function
            Public Function GenerateIncomeStatementInterpretation(statement As IncomeStatementDto, Optional previousMargin As Decimal? = Nothing) As String Implements IWhatThisMeansService.GenerateIncomeStatementInterpretation
                Return ""
            End Function
            Public Function GenerateSalesSummaryInterpretation(summary As AccountingSalesSummaryDto) As String Implements IWhatThisMeansService.GenerateSalesSummaryInterpretation
                Return ""
            End Function
            Public Function GenerateMarginAlert(currentMargin As Decimal, previousMargin As Decimal, Optional thresholdPercent As Decimal = 3.0D) As String Implements IWhatThisMeansService.GenerateMarginAlert
                Return ""
            End Function
        End Class

        Private Class DummyView
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

            Public Sub SetMonthlyTrends(trends As IEnumerable(Of MonthlyTrendDto)) Implements IFinancialOverviewView.SetMonthlyTrends
            End Sub

            Public Sub SetTopSellingProducts(products As IEnumerable(Of TopProductDto)) Implements IFinancialOverviewView.SetTopSellingProducts
            End Sub

            Public Event RefreshRequested As EventHandler Implements IFinancialOverviewView.RefreshRequested
            Public Event VatTileClicked As EventHandler Implements IFinancialOverviewView.VatTileClicked
            Public Event ViewLoaded As EventHandler Implements IFinancialOverviewView.ViewLoaded

            Public Sub TriggerVatTileClick()
                RaiseEvent VatTileClicked(Me, EventArgs.Empty)
            End Sub
        End Class

        Public Async Function RunAndReportAsync(serviceProvider As IServiceProvider) As Task(Of String)
            Dim reportPath = Path.Combine(Path.GetTempPath(), $"vat-tile-smoke-report-{DateTime.UtcNow:yyyyMMddHHmmss}.md")
            Dim reportLines As New List(Of String)()
            reportLines.Add("# VAT Tile Integration Smoke Report")
            reportLines.Add($"Run Date: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC")
            reportLines.Add("")

            Try
                ' Setup DB
                Dim options = New DbContextOptionsBuilder(Of AccountingDbContext)().UseInMemoryDatabase("SmokeTestDb_" & Guid.NewGuid().ToString()).Options
                Using db = New AccountingDbContext(options)
                    Dim activeDate = DateTime.UtcNow.Date.AddMonths(-1)
                    Dim transactionDate = New DateTime(activeDate.Year, activeDate.Month, 15, 12, 0, 0, DateTimeKind.Utc)

                    db.RevenueRecords.Add(New RevenueRecord With {
                        .RecordDate = transactionDate,
                        .TotalRevenue = 112000,
                        .VatableAmount = 100000,
                        .OutputVat = 12000
                    })
                    db.ExpenseRecords.Add(New ExpenseRecord With {
                        .ExpenseDate = transactionDate,
                        .Amount = 28000,
                        .VatableAmount = 25000,
                        .InputVat = 3000
                    })
                    Await db.SaveChangesAsync()
                End Using

                ' 1. Verify VAT calculation
                Dim computedVatPayable As Decimal = 0
                Using db = New AccountingDbContext(options)
                    Dim logger1 = serviceProvider.GetRequiredService(Of Microsoft.Extensions.Logging.ILogger(Of FinancialOverviewService))()
                    Dim logger2 = serviceProvider.GetRequiredService(Of Microsoft.Extensions.Logging.ILogger(Of VatReportingService))()
                    Dim mediator = serviceProvider.GetRequiredService(Of IMediator)()

                    Dim overviewService As IFinancialOverviewService = New FinancialOverviewService(db, mediator, logger1)
                    Dim vatReportingService As IVatReportingService = New VatReportingService(db, mediator, logger2)
                    Dim provider As IKpiProvider = New VatPayableKpiProvider(vatReportingService)

                    Dim kpiProviders As New List(Of IKpiProvider) From { provider }
                    Dim enrichedService As IFinancialOverviewService = New VatEnrichedFinancialOverviewService(overviewService, kpiProviders)

                    Dim overview = Await enrichedService.GetOverviewAsync()
                    computedVatPayable = overview.VatPayable

                    If computedVatPayable = 9000D Then
                        reportLines.Add("## VAT Calculation: PASS")
                        reportLines.Add($"- Expected: 9000, Actual: {computedVatPayable}")
                    Else
                        reportLines.Add("## VAT Calculation: FAIL")
                        reportLines.Add($"- Expected: 9000, Actual: {computedVatPayable}")
                    End If
                End Using
                ' 2. Verify Role Navigation
                Dim checkRole = Sub(role As UserRole?, expectedNavCount As Integer, roleName As String)
                    Dim sessionService = New DummySessionService() With { .CurrentUserRole = role }
                    Dim mediator = serviceProvider.GetRequiredService(Of IMediator)() ' Or dummy
                    Dim presenter As New FinancialOverviewPresenter(Nothing, New DummyWhatThisMeansService(), New DummyNotificationService(), mediator, sessionService)
                    Dim dummyView = New DummyView()
                    presenter.View = dummyView

                    Dim navCount = 0
                    AddHandler presenter.NavigateToVatReturnRequested, Sub(s, e) navCount += 1

                    dummyView.TriggerVatTileClick()

                    If navCount = expectedNavCount Then
                        reportLines.Add($"## Role Navigation ({roleName}): PASS")
                    Else
                        reportLines.Add($"## Role Navigation ({roleName}): FAIL")
                    End If
                End Sub

                checkRole(UserRole.Manager, 1, "Manager")
                checkRole(UserRole.Developer, 1, "Developer")
                checkRole(UserRole.Owner, 0, "Owner")

            Catch ex As Exception
                reportLines.Add($"## Error: {ex.Message}")
                reportLines.Add($"Stack Trace: {ex.StackTrace}")
            End Try

            File.WriteAllLines(reportPath, reportLines)
            Return reportPath
        End Function
    End Class
End Namespace
#End If
