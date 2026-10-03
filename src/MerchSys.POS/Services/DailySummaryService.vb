Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.POS.Data
Imports MerchSys.POS.Entities

Namespace Services
    Public Class DailySummaryService
        Implements IDailySummaryService

        Private ReadOnly _db As POSDbContext

        Public Sub New(db As POSDbContext)
            _db = db
        End Sub

        Public Async Function GetDailySummaryAsync(targetDate As DateTime) As Task(Of DailySummaryDto) Implements IDailySummaryService.GetDailySummaryAsync
            Dim startOfDay = targetDate.Date
            Dim endOfDay = startOfDay.AddDays(1).AddTicks(-1)

            Return Await ComputeDailySummaryAsync(startOfDay, endOfDay)
        End Function

        Public Async Function GetWeeklySummaryAsync(weekStartDate As DateTime) As Task(Of PeriodSummaryDto) Implements IDailySummaryService.GetWeeklySummaryAsync
            Dim startOfWeek = weekStartDate.Date
            Dim endOfWeek = startOfWeek.AddDays(7).AddTicks(-1)

            Return Await ComputePeriodSummaryAsync(startOfWeek, endOfWeek)
        End Function

        Public Async Function GetMonthlySummaryAsync(year As Integer, month As Integer) As Task(Of PeriodSummaryDto) Implements IDailySummaryService.GetMonthlySummaryAsync
            Dim startOfMonth = New DateTime(year, month, 1)
            Dim endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1)

            Return Await ComputePeriodSummaryAsync(startOfMonth, endOfMonth)
        End Function

        Private Async Function ComputeDailySummaryAsync(startDate As DateTime, endDate As DateTime) As Task(Of DailySummaryDto)
            ' Note: Circumvent EF Core ToListAsync empty projection bug by projecting via anonymous types before returning strong types if necessary.
            Dim transactions = Await _db.SalesTransactions _
                .Include(Function(t) t.Lines) _
                .Where(Function(t) t.TransactionDate >= startDate AndAlso t.TransactionDate <= endDate) _
                .Select(Function(t) New With {
                    .Id = t.Id,
                    .TransactionDate = t.TransactionDate,
                    .TotalAmount = t.TotalAmount,
                    .PaymentMethod = t.PaymentMethod,
                    .IsVoid = t.IsVoid,
                    .Lines = t.Lines.Select(Function(l) New With {
                        .ProductId = l.ProductId,
                        .ProductName = l.ProductName,
                        .Quantity = l.Quantity,
                        .LineTotal = l.LineTotal
                    }).ToList()
                }) _
                .ToListAsync()

            Dim returns = Await _db.SalesReturns _
                .Include(Function(r) r.SalesTransaction) _
                .Where(Function(r) r.CreatedAt >= startDate AndAlso r.CreatedAt <= endDate) _
                .Select(Function(r) New With {
                    .Id = r.Id,
                    .SalesTransactionId = r.SalesTransactionId,
                    .OriginalTransactionTotalAmount = r.SalesTransaction.TotalAmount
                }) _
                .ToListAsync()

            Dim summary As New DailySummaryDto With {
                .TargetDate = startDate.Date
            }

            Dim validTransactions = transactions.Where(Function(t) Not t.IsVoid).ToList()
            Dim voidedTransactions = transactions.Where(Function(t) t.IsVoid).ToList()

            summary.TotalTransactions = validTransactions.Count
            summary.TotalSalesAmount = validTransactions.Sum(Function(t) t.TotalAmount)
            summary.VoidedTransactions = voidedTransactions.Count
            summary.VoidedAmount = voidedTransactions.Sum(Function(t) t.TotalAmount)

            ' Calculate payment breakdown
            Dim totalAmount = summary.TotalSalesAmount
            If totalAmount > 0 Then
                Dim paymentGroup = validTransactions.GroupBy(Function(t) t.PaymentMethod)
                For Each group In paymentGroup
                    Dim methodAmount = group.Sum(Function(t) t.TotalAmount)
                    summary.PaymentBreakdown.Add(New PaymentMethodBreakdownDto With {
                        .Method = group.Key,
                        .Amount = methodAmount,
                        .Percentage = (methodAmount / totalAmount) * 100
                    })
                Next
            End If

            ' Calculate top products
            Dim productSales = New Dictionary(Of Integer, TopProductDto)()
            For Each t In validTransactions
                For Each l In t.Lines
                    If Not productSales.ContainsKey(l.ProductId) Then
                        productSales(l.ProductId) = New TopProductDto With {
                            .ProductId = l.ProductId,
                            .ProductName = l.ProductName,
                            .QuantitySold = 0,
                            .TotalSales = 0
                        }
                    End If
                    productSales(l.ProductId).QuantitySold += l.Quantity
                    productSales(l.ProductId).TotalSales += l.LineTotal
                Next
            Next
            summary.TopProducts = productSales.Values.OrderByDescending(Function(p) p.QuantitySold).Take(5).ToList()

            summary.ReturnedItemsCount = returns.Count
            summary.ReturnedAmount = returns.Sum(Function(r) r.OriginalTransactionTotalAmount)

            Return summary
        End Function

        Private Async Function ComputePeriodSummaryAsync(startDate As DateTime, endDate As DateTime) As Task(Of PeriodSummaryDto)
            Dim transactions = Await _db.SalesTransactions _
                .Include(Function(t) t.Lines) _
                .Where(Function(t) t.TransactionDate >= startDate AndAlso t.TransactionDate <= endDate) _
                .Select(Function(t) New With {
                    .Id = t.Id,
                    .TransactionDate = t.TransactionDate,
                    .TotalAmount = t.TotalAmount,
                    .PaymentMethod = t.PaymentMethod,
                    .IsVoid = t.IsVoid,
                    .Lines = t.Lines.Select(Function(l) New With {
                        .ProductId = l.ProductId,
                        .ProductName = l.ProductName,
                        .Quantity = l.Quantity,
                        .LineTotal = l.LineTotal
                    }).ToList()
                }) _
                .ToListAsync()

            Dim returns = Await _db.SalesReturns _
                .Include(Function(r) r.SalesTransaction) _
                .Where(Function(r) r.CreatedAt >= startDate AndAlso r.CreatedAt <= endDate) _
                .Select(Function(r) New With {
                    .Id = r.Id,
                    .CreatedAt = r.CreatedAt,
                    .SalesTransactionId = r.SalesTransactionId,
                    .OriginalTransactionTotalAmount = r.SalesTransaction.TotalAmount
                }) _
                .ToListAsync()

            Dim periodSummary As New PeriodSummaryDto With {
                .StartDate = startDate,
                .EndDate = endDate
            }

            Dim validTransactions = transactions.Where(Function(t) Not t.IsVoid).ToList()

            periodSummary.TotalTransactions = validTransactions.Count
            periodSummary.TotalSalesAmount = validTransactions.Sum(Function(t) t.TotalAmount)

            Dim daysDiff = (endDate - startDate).Days + 1
            If daysDiff > 0 Then
                periodSummary.AverageDailySales = periodSummary.TotalSalesAmount / daysDiff
            End If

            ' Payment Breakdown
            Dim totalAmount = periodSummary.TotalSalesAmount
            If totalAmount > 0 Then
                Dim paymentGroup = validTransactions.GroupBy(Function(t) t.PaymentMethod)
                For Each group In paymentGroup
                    Dim methodAmount = group.Sum(Function(t) t.TotalAmount)
                    periodSummary.PaymentBreakdown.Add(New PaymentMethodBreakdownDto With {
                        .Method = group.Key,
                        .Amount = methodAmount,
                        .Percentage = (methodAmount / totalAmount) * 100
                    })
                Next
            End If

            ' Top Products
            Dim productSales = New Dictionary(Of Integer, TopProductDto)()
            For Each t In validTransactions
                For Each l In t.Lines
                    If Not productSales.ContainsKey(l.ProductId) Then
                        productSales(l.ProductId) = New TopProductDto With {
                            .ProductId = l.ProductId,
                            .ProductName = l.ProductName,
                            .QuantitySold = 0,
                            .TotalSales = 0
                        }
                    End If
                    productSales(l.ProductId).QuantitySold += l.Quantity
                    productSales(l.ProductId).TotalSales += l.LineTotal
                Next
            Next
            periodSummary.TopProducts = productSales.Values.OrderByDescending(Function(p) p.QuantitySold).Take(5).ToList()

            ' Daily Breakdown
            ' Create a set of all dates with either transactions or returns
            Dim allDates As New HashSet(Of DateTime)()
            For Each t In validTransactions
                allDates.Add(t.TransactionDate.Date)
            Next
            For Each r In returns
                allDates.Add(r.CreatedAt.Date)
            Next

            For Each currentDate In allDates
                Dim dailyTransactions = validTransactions.Where(Function(t) t.TransactionDate.Date = currentDate).ToList()
                Dim dailyReturns = returns.Where(Function(r) r.CreatedAt.Date = currentDate).ToList()

                Dim dailySummary As New DailySummaryDto With {
                    .TargetDate = currentDate,
                    .TotalTransactions = dailyTransactions.Count,
                    .TotalSalesAmount = dailyTransactions.Sum(Function(t) t.TotalAmount),
                    .ReturnedItemsCount = dailyReturns.Count,
                    .ReturnedAmount = dailyReturns.Sum(Function(r) r.OriginalTransactionTotalAmount)
                }

                periodSummary.DailyBreakdown.Add(dailySummary)
            Next

            ' Sort daily breakdown by date
            periodSummary.DailyBreakdown = periodSummary.DailyBreakdown.OrderBy(Function(d) d.TargetDate).ToList()

            Return periodSummary
        End Function
    End Class
End Namespace
