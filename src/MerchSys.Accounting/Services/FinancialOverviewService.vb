Imports System.Threading
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Logging
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities
Imports MerchSys.SharedKernel.Queries

Namespace Services

    Public Class FinancialOverviewService
        Implements IFinancialOverviewService

        Private ReadOnly _dbContext As AccountingDbContext
        Private ReadOnly _mediator As IMediator
        Private ReadOnly _logger As ILogger(Of FinancialOverviewService)

        Public Sub New(dbContext As AccountingDbContext, mediator As IMediator, logger As ILogger(Of FinancialOverviewService))
            _dbContext = dbContext
            _mediator = mediator
            _logger = logger
        End Sub

        Public Async Function GetOverviewAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of FinancialOverviewDto) Implements IFinancialOverviewService.GetOverviewAsync
            Dim dto As New FinancialOverviewDto()
            dto.SnapshotDate = DateTime.UtcNow

            Dim today = DateTime.UtcNow.Date
            Dim firstOfMonth = New DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc)
            Dim firstOfYear = New DateTime(today.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc)

            ' Calculate Today, MTD, YTD Revenue & Margins
            Dim allRecords = Await _dbContext.RevenueRecords.
                Where(Function(r) r.RecordDate >= firstOfYear).
                ToListAsync(cancellationToken)

            Dim todayRecords = allRecords.Where(Function(r) r.RecordDate.Date = today).ToList()
            Dim mtdRecords = allRecords.Where(Function(r) r.RecordDate >= firstOfMonth).ToList()

            dto.TodayRevenue = todayRecords.Sum(Function(r) r.TotalRevenue)
            Dim todayProfit = todayRecords.Sum(Function(r) r.GrossProfit)
            dto.GrossMarginToday = If(dto.TodayRevenue > 0, (todayProfit / dto.TodayRevenue) * 100D, 0D)

            dto.MtdRevenue = mtdRecords.Sum(Function(r) r.TotalRevenue)
            Dim mtdProfit = mtdRecords.Sum(Function(r) r.GrossProfit)
            dto.GrossMarginMtd = If(dto.MtdRevenue > 0, (mtdProfit / dto.MtdRevenue) * 100D, 0D)

            dto.YtdRevenue = allRecords.Sum(Function(r) r.TotalRevenue)
            Dim ytdProfit = allRecords.Sum(Function(r) r.GrossProfit)
            dto.GrossMarginYtd = If(dto.YtdRevenue > 0, (ytdProfit / dto.YtdRevenue) * 100D, 0D)

            ' Fetch AR, AP, Inventory Value
            Dim arTask = _mediator.Send(New GetTotalARQuery(), cancellationToken)
            Dim apTask = _mediator.Send(New GetTotalAPQuery(), cancellationToken)
            Dim invTask = _mediator.Send(New GetInventoryValuationQuery(), cancellationToken)

            Await Task.WhenAll(arTask, apTask, invTask)

            dto.TotalAR = arTask.Result
            dto.TotalAP = apTask.Result
            dto.InventoryValue = invTask.Result.Valuations.Sum(Function(v) v.TotalValue)

            ' Top 5 Products MTD/YTD (Using allRecords which is YTD)
            dto.TopSellingProducts = allRecords.
                GroupBy(Function(r) New With {Key .ProductId = r.ProductId, Key .ProductName = r.ProductName}).
                Select(Function(g) New TopProductDto With {
                    .ProductId = g.Key.ProductId,
                    .ProductName = g.Key.ProductName,
                    .QuantitySold = g.Sum(Function(r) r.QuantitySold),
                    .TotalRevenue = g.Sum(Function(r) r.TotalRevenue),
                    .TotalGrossProfit = g.Sum(Function(r) r.GrossProfit)
                }).
                OrderByDescending(Function(p) p.TotalRevenue).
                Take(5).
                ToList()

            ' Monthly Trends - Last 6 Calendar Months
            Dim startOf6MonthsAgo = firstOfMonth.AddMonths(-5)
            Dim sixMonthRecords = Await _dbContext.RevenueRecords.
                Where(Function(r) r.RecordDate >= startOf6MonthsAgo).
                ToListAsync(cancellationToken)

            For i As Integer = 5 To 0 Step -1
                Dim monthStart = firstOfMonth.AddMonths(-i)
                Dim nextMonth = monthStart.AddMonths(1)

                Dim records = sixMonthRecords.Where(Function(r) r.RecordDate >= monthStart AndAlso r.RecordDate < nextMonth).ToList()
                Dim mRevenue = records.Sum(Function(r) r.TotalRevenue)
                Dim mCogs = records.Sum(Function(r) r.TotalCogs)
                Dim mProfit = records.Sum(Function(r) r.GrossProfit)
                Dim mMargin = If(mRevenue > 0, (mProfit / mRevenue) * 100D, 0D)

                dto.MonthlyTrends.Add(New MonthlyTrendDto With {
                    .MonthStart = monthStart,
                    .MonthLabel = monthStart.ToString("MMM yyyy"),
                    .Revenue = mRevenue,
                    .Cogs = mCogs,
                    .GrossProfit = mProfit,
                    .GrossMarginPercent = mMargin
                })
            Next

            Return dto
        End Function

        Public Async Function RefreshSnapshotAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of FinancialSnapshot) Implements IFinancialOverviewService.RefreshSnapshotAsync
            Dim today = DateTime.UtcNow.Date

            Dim snapshot = Await _dbContext.Set(Of FinancialSnapshot)().
                FirstOrDefaultAsync(Function(s) s.SnapshotDate.Date = today, cancellationToken)

            Dim isNew = False
            If snapshot Is Nothing Then
                snapshot = New FinancialSnapshot() With {
                    .SnapshotDate = today
                }
                isNew = True
            End If

            ' Calculate Today, MTD, YTD Revenue
            Dim firstOfMonth = New DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc)
            Dim firstOfYear = New DateTime(today.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc)

            Dim ytdRecords = Await _dbContext.RevenueRecords.
                Where(Function(r) r.RecordDate >= firstOfYear).
                ToListAsync(cancellationToken)

            snapshot.TodayRevenue = ytdRecords.Where(Function(r) r.RecordDate.Date = today).Sum(Function(r) r.TotalRevenue)
            snapshot.MtdRevenue = ytdRecords.Where(Function(r) r.RecordDate >= firstOfMonth).Sum(Function(r) r.TotalRevenue)
            snapshot.YtdRevenue = ytdRecords.Sum(Function(r) r.TotalRevenue)

            ' Fetch KPIs
            Dim arTask = _mediator.Send(New GetTotalARQuery(), cancellationToken)
            Dim apTask = _mediator.Send(New GetTotalAPQuery(), cancellationToken)
            Dim invTask = _mediator.Send(New GetInventoryValuationQuery(), cancellationToken)

            Await Task.WhenAll(arTask, apTask, invTask)

            snapshot.TotalAR = arTask.Result
            snapshot.TotalAP = apTask.Result
            snapshot.InventoryValue = invTask.Result.Valuations.Sum(Function(v) v.TotalValue)

            If isNew Then
                _dbContext.Set(Of FinancialSnapshot)().Add(snapshot)
            End If

            Dim isRace As Boolean = False
            Try
                Await _dbContext.SaveChangesAsync(cancellationToken)
            Catch ex As DbUpdateException
                isRace = True
                _dbContext.Entry(snapshot).State = EntityState.Detached
            End Try

            If isRace Then
                snapshot = Await _dbContext.Set(Of FinancialSnapshot)().
                    FirstOrDefaultAsync(Function(s) s.SnapshotDate.Date = today, cancellationToken)
            End If

            Return snapshot
        End Function

    End Class

End Namespace