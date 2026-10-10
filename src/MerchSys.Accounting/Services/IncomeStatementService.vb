Imports System.Threading
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Accounting.Data

Namespace Services

    Public Class IncomeStatementService
        Implements IIncomeStatementService

        Private ReadOnly _dbContext As AccountingDbContext

        Public Sub New(dbContext As AccountingDbContext)
            _dbContext = dbContext
        End Sub

        Private Async Function BuildStatementAsync(startDate As DateTime, endDate As DateTime, periodLabel As String, cancellationToken As CancellationToken) As Task(Of IncomeStatementDto)
            Dim revRecords = Await _dbContext.RevenueRecords.
                Where(Function(r) r.RecordDate >= startDate AndAlso r.RecordDate <= endDate).
                ToListAsync(cancellationToken)

            Dim grossSales As Decimal = revRecords.Sum(Function(r) r.TotalRevenue)
            Dim costOfGoodsSold As Decimal = revRecords.Sum(Function(r) r.TotalCogs)

            ' In this system, sales returns are implicit or handle separately if they exist, but for now we set to 0 as instructed if no separate return records
            ' Assuming we don't have separate return records based on "SalesReturns = 0 if no separate return records"
            Dim salesReturns As Decimal = 0D

            Dim expRecords = Await _dbContext.ExpenseRecords.
                Where(Function(e) e.ExpenseDate >= startDate AndAlso e.ExpenseDate <= endDate AndAlso
                                  e.Category <> "COGS" AndAlso e.Category <> "Purchase" AndAlso e.Category <> "AR Reduction").
                ToListAsync(cancellationToken)

            Dim operatingExpensesByCategory = expRecords.
                GroupBy(Function(e) e.Category).
                ToDictionary(Function(g) g.Key, Function(g) g.Sum(Function(e) e.Amount))

            Dim operatingExpenses As Decimal = operatingExpensesByCategory.Values.Sum()

            Dim netSales = grossSales - salesReturns
            Dim grossProfit = netSales - costOfGoodsSold
            Dim grossMarginPercent = If(netSales > 0D, (grossProfit / netSales) * 100D, 0D)
            Dim netIncome = grossProfit - operatingExpenses
            Dim netMarginPercent = If(netSales > 0D, (netIncome / netSales) * 100D, 0D)

            Return New IncomeStatementDto With {
                .PeriodLabel = periodLabel,
                .StartDate = startDate,
                .EndDate = endDate,
                .GrossSales = grossSales,
                .SalesReturns = salesReturns,
                .NetSales = netSales,
                .CostOfGoodsSold = costOfGoodsSold,
                .GrossProfit = grossProfit,
                .GrossMarginPercent = grossMarginPercent,
                .OperatingExpenses = operatingExpenses,
                .OperatingExpensesByCategory = operatingExpensesByCategory,
                .NetIncome = netIncome,
                .NetMarginPercent = netMarginPercent
            }
        End Function

        Public Function GenerateAsync(startDate As DateTime, endDate As DateTime, Optional periodLabel As String = Nothing, Optional cancellationToken As CancellationToken = Nothing) As Task(Of IncomeStatementDto) Implements IIncomeStatementService.GenerateAsync
            If String.IsNullOrEmpty(periodLabel) Then
                periodLabel = $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}"
            End If
            Return BuildStatementAsync(startDate, endDate, periodLabel, cancellationToken)
        End Function

        Public Function GenerateMonthlyAsync(year As Integer, month As Integer, Optional cancellationToken As CancellationToken = Nothing) As Task(Of IncomeStatementDto) Implements IIncomeStatementService.GenerateMonthlyAsync
            Dim startDate As New DateTime(year, month, 1)
            Dim endDate As DateTime = startDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59)
            Dim periodLabel As String = startDate.ToString("MMMM yyyy")
            Return BuildStatementAsync(startDate, endDate, periodLabel, cancellationToken)
        End Function

        Public Function GenerateQuarterlyAsync(year As Integer, quarter As Integer, Optional cancellationToken As CancellationToken = Nothing) As Task(Of IncomeStatementDto) Implements IIncomeStatementService.GenerateQuarterlyAsync
            Dim startMonth As Integer = (quarter - 1) * 3 + 1
            Dim startDate As New DateTime(year, startMonth, 1)
            Dim endDate As DateTime = startDate.AddMonths(3).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59)
            Dim periodLabel As String = $"Q{quarter} {year}"
            Return BuildStatementAsync(startDate, endDate, periodLabel, cancellationToken)
        End Function

        Public Function GenerateAnnualAsync(year As Integer, Optional cancellationToken As CancellationToken = Nothing) As Task(Of IncomeStatementDto) Implements IIncomeStatementService.GenerateAnnualAsync
            Dim startDate As New DateTime(year, 1, 1)
            Dim endDate As New DateTime(year, 12, 31, 23, 59, 59)
            Dim periodLabel As String = year.ToString()
            Return BuildStatementAsync(startDate, endDate, periodLabel, cancellationToken)
        End Function

        Public Async Function GetPerProductMarginsAsync(startDate As DateTime, endDate As DateTime, Optional cancellationToken As CancellationToken = Nothing) As Task(Of List(Of ProductMarginDto)) Implements IIncomeStatementService.GetPerProductMarginsAsync
            Dim revRecords = Await _dbContext.RevenueRecords.
                Where(Function(r) r.RecordDate >= startDate AndAlso r.RecordDate <= endDate).
                ToListAsync(cancellationToken)

            Dim margins = revRecords.
                GroupBy(Function(r) New With {Key r.ProductId, Key r.ProductName}).
                Select(Function(g)
                           Dim rev As Decimal = g.Sum(Function(r) r.TotalRevenue)
                           Dim cogs As Decimal = g.Sum(Function(r) r.TotalCogs)
                           Dim profit As Decimal = rev - cogs
                           Dim marginPercent As Decimal = If(rev > 0D, (profit / rev) * 100D, 0D)

                           Return New ProductMarginDto With {
                               .ProductId = g.Key.ProductId,
                               .ProductName = g.Key.ProductName,
                               .QuantitySold = g.Sum(Function(r) r.QuantitySold),
                               .Revenue = rev,
                               .Cogs = cogs,
                               .GrossProfit = profit,
                               .GrossMarginPercent = marginPercent
                           }
                       End Function).
                OrderByDescending(Function(m) m.Revenue).
                ToList()

            Return margins
        End Function
    End Class

End Namespace
