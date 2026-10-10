Imports System.Threading
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities
Imports MerchSys.SharedKernel.Enums

Namespace Services

    Public Class SalesSummaryService
        Implements ISalesSummaryService

        Private ReadOnly _dbContext As AccountingDbContext

        Public Sub New(dbContext As AccountingDbContext)
            _dbContext = dbContext
        End Sub

        Private Async Function BuildSalesSummaryAsync(startDate As DateTime, endDate As DateTime, periodLabel As String, cancellationToken As CancellationToken) As Task(Of AccountingSalesSummaryDto)
            Dim revRecords = Await _dbContext.RevenueRecords.
                Where(Function(r) r.RecordDate >= startDate AndAlso r.RecordDate <= endDate).
                ToListAsync(cancellationToken)

            Dim totalGrossSales = revRecords.Sum(Function(r) r.TotalRevenue)
            Dim transactionCount = revRecords.Select(Function(r) r.SourceTransactionId).Distinct().Count()
            Dim totalReturns = 0D
            Dim totalNetSales = totalGrossSales - totalReturns
            Dim avgTransactionValue = If(transactionCount > 0, totalNetSales / transactionCount, 0D)

            Dim breakdowns As New List(Of PaymentBreakdownDto)()
            Dim paymentGroups = revRecords.GroupBy(Function(r) r.PaymentMethod)
            For Each pg In paymentGroups
                Dim methodGross = pg.Sum(Function(r) r.TotalRevenue)
                Dim methodCount = pg.Select(Function(r) r.SourceTransactionId).Distinct().Count()
                Dim methodPct = If(totalGrossSales > 0D, (methodGross / totalGrossSales) * 100D, 0D)
                breakdowns.Add(New PaymentBreakdownDto With {
                    .PaymentMethod = pg.Key,
                    .PaymentMethodName = pg.Key.ToString(),
                    .GrossSales = methodGross,
                    .TransactionCount = methodCount,
                    .PercentageOfTotal = methodPct
                })
            Next

            Dim creditSales = revRecords.Where(Function(r) r.PaymentMethod = PaymentMethod.Credit).Sum(Function(r) r.TotalRevenue)
            Dim creditPct = If(totalGrossSales > 0D, (creditSales / totalGrossSales) * 100D, 0D)
            Dim hasCreditWarn = (creditPct >= 30.0D)

            Dim dailyList As New List(Of DailySalesDto)()
            Dim dayGroups = revRecords.GroupBy(Function(r) r.RecordDate.Date).OrderBy(Function(g) g.Key)
            For Each dg In dayGroups
                Dim dayGross = dg.Sum(Function(r) r.TotalRevenue)
                Dim dayCount = dg.Select(Function(r) r.SourceTransactionId).Distinct().Count()
                dailyList.Add(New DailySalesDto With {
                    .SaleDate = dg.Key,
                    .DayLabel = dg.Key.ToString("ddd, MMM dd"),
                    .GrossSales = dayGross,
                    .Discounts = 0D,
                    .Returns = 0D,
                    .NetSales = dayGross,
                    .TransactionCount = dayCount
                })
            Next

            Return New AccountingSalesSummaryDto With {
                .PeriodLabel = periodLabel,
                .StartDate = startDate,
                .EndDate = endDate,
                .TotalGrossSales = totalGrossSales,
                .TotalNetSales = totalNetSales,
                .TotalReturns = totalReturns,
                .TransactionCount = transactionCount,
                .AvgTransactionValue = avgTransactionValue,
                .CreditSalesPercentage = creditPct,
                .HasCreditWarning = hasCreditWarn,
                .PaymentBreakdowns = breakdowns,
                .DailyBreakdown = dailyList
            }
        End Function

        Public Async Function GetSalesSummaryAsync(startDate As DateTime, endDate As DateTime, Optional periodLabel As String = Nothing, Optional cancellationToken As CancellationToken = Nothing) As Task(Of AccountingSalesSummaryDto) Implements ISalesSummaryService.GetSalesSummaryAsync
            If String.IsNullOrEmpty(periodLabel) Then
                periodLabel = $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}"
            End If
            Return Await BuildSalesSummaryAsync(startDate, endDate, periodLabel, cancellationToken)
        End Function

        Public Async Function GetDailySummaryAsync(targetDate As DateTime, Optional cancellationToken As CancellationToken = Nothing) As Task(Of AccountingSalesSummaryDto) Implements ISalesSummaryService.GetDailySummaryAsync
            Dim startDay = targetDate.Date
            Dim endDay = targetDate.Date.AddDays(1).AddTicks(-1)
            Dim pLabel = targetDate.ToString("MMMM dd, yyyy")
            Return Await BuildSalesSummaryAsync(startDay, endDay, pLabel, cancellationToken)
        End Function

        Public Async Function GetWeeklySummaryAsync(anchorDate As DateTime, Optional cancellationToken As CancellationToken = Nothing) As Task(Of AccountingSalesSummaryDto) Implements ISalesSummaryService.GetWeeklySummaryAsync
            Dim sunday = anchorDate.Date.AddDays(-(CInt(anchorDate.DayOfWeek)))
            Dim startWeek = sunday
            Dim endWeek = sunday.AddDays(7).AddTicks(-1)
            Dim pLabel = $"{startWeek:yyyy-MM-dd} to {endWeek:yyyy-MM-dd}"
            Return Await BuildSalesSummaryAsync(startWeek, endWeek, pLabel, cancellationToken)
        End Function

        Public Async Function GetMonthlySummaryAsync(year As Integer, month As Integer, Optional cancellationToken As CancellationToken = Nothing) As Task(Of AccountingSalesSummaryDto) Implements ISalesSummaryService.GetMonthlySummaryAsync
            Dim startMonth = New DateTime(year, month, 1)
            Dim endMonth = startMonth.AddMonths(1).AddTicks(-1)
            Dim pLabel = startMonth.ToString("MMMM yyyy")
            Return Await BuildSalesSummaryAsync(startMonth, endMonth, pLabel, cancellationToken)
        End Function

        Public Async Function GetPaymentTrendsAsync(monthsBack As Integer, Optional cancellationToken As CancellationToken = Nothing) As Task(Of List(Of PaymentTrendDto)) Implements ISalesSummaryService.GetPaymentTrendsAsync
            If monthsBack <= 0 Then
                monthsBack = 6
            End If

            Dim trends As New List(Of PaymentTrendDto)()
            Dim currentMonthStart = New DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1)
            For i = monthsBack - 1 To 0 Step -1
                Dim mStart = currentMonthStart.AddMonths(-i)
                Dim mEnd = mStart.AddMonths(1).AddTicks(-1)
                Dim mRecords = Await _dbContext.RevenueRecords.
                    Where(Function(r) r.RecordDate >= mStart AndAlso r.RecordDate <= mEnd).
                    ToListAsync(cancellationToken)

                Dim mTotal = mRecords.Sum(Function(r) r.TotalRevenue)
                Dim mCash = mRecords.Where(Function(r) r.PaymentMethod = PaymentMethod.Cash).Sum(Function(r) r.TotalRevenue)
                Dim mCredit = mRecords.Where(Function(r) r.PaymentMethod = PaymentMethod.Credit).Sum(Function(r) r.TotalRevenue)
                Dim mOther = mRecords.Where(Function(r) r.PaymentMethod <> PaymentMethod.Cash AndAlso r.PaymentMethod <> PaymentMethod.Credit).Sum(Function(r) r.TotalRevenue)
                Dim mCreditPct = If(mTotal > 0D, (mCredit / mTotal) * 100D, 0D)

                trends.Add(New PaymentTrendDto With {
                    .PeriodLabel = mStart.ToString("MMM yyyy"),
                    .CashSales = mCash,
                    .CreditSales = mCredit,
                    .OtherSales = mOther,
                    .CreditPercentage = mCreditPct
                })
            Next
            Return trends
        End Function

    End Class

End Namespace
