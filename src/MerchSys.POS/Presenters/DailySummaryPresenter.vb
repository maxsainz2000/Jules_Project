Imports System.Threading.Tasks
Imports MerchSys.POS.Services
Imports MerchSys.POS.Views

Namespace Presenters

    Public Class DailySummaryPresenter
        Private ReadOnly _service As IDailySummaryService
        Private WithEvents _view As IDailySummaryView

        Public Sub New(service As IDailySummaryService)
            _service = service
        End Sub

        Public Property View As IDailySummaryView
            Get
                Return _view
            End Get
            Set(value As IDailySummaryView)
                _view = value
                If _view IsNot Nothing Then
                    WireEvents()
                End If
            End Set
        End Property

        Private Sub WireEvents()
            ' Events are wired automatically via WithEvents
        End Sub

        Private Sub OnLoad(sender As Object, e As EventArgs) Handles _view.Load
            RefreshDataAsync()
        End Sub

        Private Sub OnPeriodModeChanged(sender As Object, e As EventArgs) Handles _view.PeriodModeChanged
            RefreshDataAsync()
        End Sub

        Private Sub OnDateChanged(sender As Object, e As EventArgs) Handles _view.DateChanged
            RefreshDataAsync()
        End Sub

        Private Sub OnNavigatePrevious(sender As Object, e As EventArgs) Handles _view.NavigatePrevious
            If _view Is Nothing Then Return

            Select Case _view.CurrentMode
                Case SummaryPeriodMode.Daily
                    _view.CurrentDate = _view.CurrentDate.AddDays(-1)
                Case SummaryPeriodMode.Weekly
                    _view.CurrentDate = _view.CurrentDate.AddDays(-7)
                Case SummaryPeriodMode.Monthly
                    _view.CurrentDate = _view.CurrentDate.AddMonths(-1)
            End Select
            RefreshDataAsync()
        End Sub

        Private Sub OnNavigateNext(sender As Object, e As EventArgs) Handles _view.NavigateNext
            If _view Is Nothing Then Return

            Select Case _view.CurrentMode
                Case SummaryPeriodMode.Daily
                    _view.CurrentDate = _view.CurrentDate.AddDays(1)
                Case SummaryPeriodMode.Weekly
                    _view.CurrentDate = _view.CurrentDate.AddDays(7)
                Case SummaryPeriodMode.Monthly
                    _view.CurrentDate = _view.CurrentDate.AddMonths(1)
            End Select
            RefreshDataAsync()
        End Sub

        Private Sub RefreshDataAsync()
            If _view Is Nothing Then Return

            Dim scheduler = TaskScheduler.FromCurrentSynchronizationContext()
            _view.ErrorMessage = String.Empty

            Select Case _view.CurrentMode
                Case SummaryPeriodMode.Daily
                    Dim taskCurrent = _service.GetDailySummaryAsync(_view.CurrentDate)
                    Dim taskPrev = _service.GetDailySummaryAsync(_view.CurrentDate.AddDays(-1))

                    Task.WhenAll(taskCurrent, taskPrev).ContinueWith(
                        Sub(t)
                            If t.IsFaulted Then
                                _view.ErrorMessage = "Error loading daily summary: " & t.Exception.GetBaseException().Message
                                Return
                            End If

                            Dim result = taskCurrent.Result
                            Dim prevResult = taskPrev.Result

                            If result IsNot Nothing Then
                                _view.DateRangeDisplay = result.TargetDate.ToString("MMMM dd, yyyy")
                                _view.TotalSales = result.TotalSalesAmount

                                If prevResult IsNot Nothing AndAlso prevResult.TotalSalesAmount > 0 Then
                                    _view.SalesDeltaPercent = ((result.TotalSalesAmount - prevResult.TotalSalesAmount) / prevResult.TotalSalesAmount) * 100D
                                Else
                                    _view.SalesDeltaPercent = Nothing
                                End If
                                _view.TotalTransactions = result.TotalTransactions
                                _view.VoidedTransactions = result.VoidedTransactions
                                _view.VoidedAmount = result.VoidedAmount
                                _view.ReturnedItemsCount = result.ReturnedItemsCount
                                _view.ReturnedAmount = result.ReturnedAmount
                                _view.AverageDailySales = 0 ' Not applicable for daily

                                _view.PaymentBreakdown = result.PaymentBreakdown
                                _view.TopProducts = result.TopProducts
                                _view.TrendData = Nothing ' No trend for single day
                            End If
                        End Sub, scheduler)

                Case SummaryPeriodMode.Weekly
                    Dim taskCurrent = _service.GetWeeklySummaryAsync(_view.CurrentDate)
                    Dim taskPrev = _service.GetWeeklySummaryAsync(_view.CurrentDate.AddDays(-7))

                    Task.WhenAll(taskCurrent, taskPrev).ContinueWith(
                        Sub(t)
                            If t.IsFaulted Then
                                _view.ErrorMessage = "Error loading weekly summary: " & t.Exception.GetBaseException().Message
                                Return
                            End If

                            Dim result = taskCurrent.Result
                            Dim prevResult = taskPrev.Result
                            If result IsNot Nothing Then
                                _view.DateRangeDisplay = $"{result.StartDate.ToString("MMM d")} - {result.EndDate.ToString("MMM d, yyyy")}"
                                _view.TotalSales = result.TotalSalesAmount

                                If prevResult IsNot Nothing AndAlso prevResult.TotalSalesAmount > 0 Then
                                    _view.SalesDeltaPercent = ((result.TotalSalesAmount - prevResult.TotalSalesAmount) / prevResult.TotalSalesAmount) * 100D
                                Else
                                    _view.SalesDeltaPercent = Nothing
                                End If
                                _view.TotalTransactions = result.TotalTransactions
                                _view.AverageDailySales = result.AverageDailySales

                                ' Aggregate returns and voids from DailyBreakdown
                                Dim voidedT = 0
                                Dim voidedA = 0D
                                Dim retItems = 0
                                Dim retA = 0D

                                If result.DailyBreakdown IsNot Nothing Then
                                    For Each d In result.DailyBreakdown
                                        voidedT += d.VoidedTransactions
                                        voidedA += d.VoidedAmount
                                        retItems += d.ReturnedItemsCount
                                        retA += d.ReturnedAmount
                                    Next
                                End If

                                _view.VoidedTransactions = voidedT
                                _view.VoidedAmount = voidedA
                                _view.ReturnedItemsCount = retItems
                                _view.ReturnedAmount = retA

                                _view.PaymentBreakdown = result.PaymentBreakdown
                                _view.TopProducts = result.TopProducts
                                _view.TrendData = result.DailyBreakdown
                            End If
                        End Sub, scheduler)

                Case SummaryPeriodMode.Monthly
                    Dim prevDate = _view.CurrentDate.AddMonths(-1)
                    Dim taskCurrent = _service.GetMonthlySummaryAsync(_view.CurrentDate.Year, _view.CurrentDate.Month)
                    Dim taskPrev = _service.GetMonthlySummaryAsync(prevDate.Year, prevDate.Month)

                    Task.WhenAll(taskCurrent, taskPrev).ContinueWith(
                        Sub(t)
                            If t.IsFaulted Then
                                _view.ErrorMessage = "Error loading monthly summary: " & t.Exception.GetBaseException().Message
                                Return
                            End If

                            Dim result = taskCurrent.Result
                            Dim prevResult = taskPrev.Result
                            If result IsNot Nothing Then
                                _view.DateRangeDisplay = result.StartDate.ToString("MMMM yyyy")
                                _view.TotalSales = result.TotalSalesAmount

                                If prevResult IsNot Nothing AndAlso prevResult.TotalSalesAmount > 0 Then
                                    _view.SalesDeltaPercent = ((result.TotalSalesAmount - prevResult.TotalSalesAmount) / prevResult.TotalSalesAmount) * 100D
                                Else
                                    _view.SalesDeltaPercent = Nothing
                                End If
                                _view.TotalTransactions = result.TotalTransactions
                                _view.AverageDailySales = result.AverageDailySales

                                ' Aggregate returns and voids from DailyBreakdown
                                Dim voidedT = 0
                                Dim voidedA = 0D
                                Dim retItems = 0
                                Dim retA = 0D

                                If result.DailyBreakdown IsNot Nothing Then
                                    For Each d In result.DailyBreakdown
                                        voidedT += d.VoidedTransactions
                                        voidedA += d.VoidedAmount
                                        retItems += d.ReturnedItemsCount
                                        retA += d.ReturnedAmount
                                    Next
                                End If

                                _view.VoidedTransactions = voidedT
                                _view.VoidedAmount = voidedA
                                _view.ReturnedItemsCount = retItems
                                _view.ReturnedAmount = retA

                                _view.PaymentBreakdown = result.PaymentBreakdown
                                _view.TopProducts = result.TopProducts
                                _view.TrendData = result.DailyBreakdown
                            End If
                        End Sub, scheduler)
            End Select
        End Sub

    End Class

End Namespace
