Imports System.Threading
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Logging
Imports MerchSys.Accounting.Data
Imports System.Globalization

Namespace Services

    Public Class VatReliefReportService
        Implements IVatReliefReportService

        Private ReadOnly _dbContext As AccountingDbContext
        Private ReadOnly _logger As ILogger(Of VatReliefReportService)

        Public Sub New(dbContext As AccountingDbContext, logger As ILogger(Of VatReliefReportService))
            _dbContext = dbContext
            _logger = logger
        End Sub

        Public Async Function GetMonthlySummaryAsync(year As Integer, month As Integer, Optional cancellationToken As CancellationToken = Nothing) As Task(Of VatReliefSummary) Implements IVatReliefReportService.GetMonthlySummaryAsync
            Dim startDate As New Date(year, month, 1)
            Dim endDate As Date = startDate.AddMonths(1).AddDays(-1)

            Dim summary As New VatReliefSummary With {
                .Year = year,
                .Month = month,
                .PeriodLabel = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month) & " " & year
            }

            ' Fetch revenue records
            Dim revenueData = Await _dbContext.RevenueRecords.
                Where(Function(r) r.RecordDate >= startDate AndAlso r.RecordDate <= endDate).
                GroupBy(Function(r) 1).
                Select(Function(g) New With {
                    .VatableSales = g.Sum(Function(r) r.VatableAmount),
                    .VatExemptSales = g.Sum(Function(r) r.VatExemptAmount),
                    .ZeroRatedSales = g.Sum(Function(r) r.ZeroRatedAmount),
                    .OutputVat = g.Sum(Function(r) r.OutputVat)
                }).
                FirstOrDefaultAsync(cancellationToken)

            If revenueData IsNot Nothing Then
                summary.VatableSales = revenueData.VatableSales
                summary.VatExemptSales = revenueData.VatExemptSales
                summary.ZeroRatedSales = revenueData.ZeroRatedSales
                summary.OutputVat = revenueData.OutputVat
            End If
            summary.TotalSales = summary.VatableSales + summary.VatExemptSales + summary.ZeroRatedSales + summary.OutputVat

            ' Fetch expense records
            Dim expenseData = Await _dbContext.ExpenseRecords.
                Where(Function(e) e.ExpenseDate >= startDate AndAlso e.ExpenseDate <= endDate).
                GroupBy(Function(e) 1).
                Select(Function(g) New With {
                    .VatablePurchases = g.Sum(Function(e) e.VatableAmount),
                    .VatExemptPurchases = g.Sum(Function(e) e.VatExemptAmount),
                    .ZeroRatedPurchases = g.Sum(Function(e) e.ZeroRatedAmount),
                    .InputVat = g.Sum(Function(e) e.InputVat)
                }).
                FirstOrDefaultAsync(cancellationToken)

            If expenseData IsNot Nothing Then
                summary.VatablePurchases = expenseData.VatablePurchases
                summary.VatExemptPurchases = expenseData.VatExemptPurchases
                summary.ZeroRatedPurchases = expenseData.ZeroRatedPurchases
                summary.InputVat = expenseData.InputVat
            End If
            summary.TotalPurchases = summary.VatablePurchases + summary.VatExemptPurchases + summary.ZeroRatedPurchases + summary.InputVat

            summary.NetVatPayable = summary.OutputVat - summary.InputVat

            Return summary
        End Function

        Public Async Function GetTrailingMonthsAsync(endYear As Integer, endMonth As Integer, Optional count As Integer = 12, Optional cancellationToken As CancellationToken = Nothing) As Task(Of List(Of VatReliefSummary)) Implements IVatReliefReportService.GetTrailingMonthsAsync
            Dim results As New List(Of VatReliefSummary)()

            Dim currentYear = endYear
            Dim currentMonth = endMonth

            For i As Integer = 0 To count - 1
                Dim summary = Await GetMonthlySummaryAsync(currentYear, currentMonth, cancellationToken)
                results.Add(summary)

                currentMonth -= 1
                If currentMonth = 0 Then
                    currentMonth = 12
                    currentYear -= 1
                End If
            Next

            results.Reverse() ' Order chronologically (oldest first)
            Return results
        End Function
    End Class

End Namespace
