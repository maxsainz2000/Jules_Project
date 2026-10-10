Imports MerchSys.Accounting.Services
Imports MerchSys.Accounting.Views
Imports MerchSys.SharedKernel.Interfaces
Imports System.Threading.Tasks

Namespace Presenters

    Public Class SalesSummaryPresenter
        Private ReadOnly _salesSummaryService As ISalesSummaryService
        Private ReadOnly _whatThisMeansService As IWhatThisMeansService
        Private ReadOnly _notificationService As INotificationService

        Private _view As ISalesSummaryView

        Public Sub New(
            salesSummaryService As ISalesSummaryService,
            whatThisMeansService As IWhatThisMeansService,
            notificationService As INotificationService)
            _salesSummaryService = salesSummaryService
            _whatThisMeansService = whatThisMeansService
            _notificationService = notificationService
        End Sub

        Public Property View As ISalesSummaryView
            Get
                Return _view
            End Get
            Set(value As ISalesSummaryView)
                If _view IsNot Nothing Then
                    RemoveHandler _view.PeriodChanged, AddressOf OnPeriodChanged
                    RemoveHandler _view.RefreshRequested, AddressOf OnRefreshRequested
                    RemoveHandler _view.ViewLoaded, AddressOf OnViewLoaded
                End If

                _view = value

                If _view IsNot Nothing Then
                    AddHandler _view.PeriodChanged, AddressOf OnPeriodChanged
                    AddHandler _view.RefreshRequested, AddressOf OnRefreshRequested
                    AddHandler _view.ViewLoaded, AddressOf OnViewLoaded
                End If
            End Set
        End Property

        Private Sub OnViewLoaded(sender As Object, e As EventArgs)
            Dim currentYear = DateTime.Today.Year
            Dim years = Enumerable.Range(currentYear - 5, 6).OrderByDescending(Function(y) y).ToList()
            Dim months = Enumerable.Range(1, 12).ToList()

            _view.SetAvailableYears(years)
            _view.SetAvailableMonths(months)

            _view.SelectedDate = DateTime.Today
            _view.SelectedYear = currentYear
            _view.SelectedMonth = DateTime.Today.Month
            _view.PeriodType = SalesSummaryPeriodType.Daily

            Dim dummy = LoadSummaryAsync()
        End Sub

        Private Sub OnPeriodChanged(sender As Object, e As EventArgs)
            Dim dummy = LoadSummaryAsync()
        End Sub

        Private Sub OnRefreshRequested(sender As Object, e As EventArgs)
            Dim dummy = LoadSummaryAsync()
        End Sub

        Public Async Function LoadSummaryAsync() As Task
            If _view Is Nothing Then Return

            _view.IsLoading = True
            _view.ErrorMessage = String.Empty

            _view.ShowDatePicker = (_view.PeriodType = SalesSummaryPeriodType.Daily OrElse _view.PeriodType = SalesSummaryPeriodType.Weekly)
            _view.ShowMonthSelectors = (_view.PeriodType = SalesSummaryPeriodType.Monthly)
            _view.ShowDailyBreakdown = (_view.PeriodType = SalesSummaryPeriodType.Weekly OrElse _view.PeriodType = SalesSummaryPeriodType.Monthly)

            Dim summary As AccountingSalesSummaryDto = Nothing
            Dim exceptionToHandle As Exception = Nothing

            Try
                If _view.PeriodType = SalesSummaryPeriodType.Daily Then
                    summary = Await _salesSummaryService.GetDailySummaryAsync(_view.SelectedDate)
                ElseIf _view.PeriodType = SalesSummaryPeriodType.Weekly Then
                    summary = Await _salesSummaryService.GetWeeklySummaryAsync(_view.SelectedDate)
                ElseIf _view.PeriodType = SalesSummaryPeriodType.Monthly Then
                    summary = Await _salesSummaryService.GetMonthlySummaryAsync(_view.SelectedYear, _view.SelectedMonth)
                End If

                If summary IsNot Nothing Then
                    _view.PeriodLabel = summary.PeriodLabel
                    _view.TotalGrossSalesDisplay = $"₱{summary.TotalGrossSales:N2}"
                    _view.TotalNetSalesDisplay = $"₱{summary.TotalNetSales:N2}"
                    _view.TotalReturnsDisplay = $"₱{summary.TotalReturns:N2}"
                    _view.TransactionCountDisplay = summary.TransactionCount.ToString("N0")
                    _view.AvgTransactionValueDisplay = $"₱{summary.AvgTransactionValue:N2}"
                    _view.CreditSalesPercentageDisplay = $"{summary.CreditSalesPercentage:N1}%"
                    _view.HasCreditWarning = summary.HasCreditWarning

                    _view.WhatThisMeansText = _whatThisMeansService.GenerateSalesSummaryInterpretation(summary)

                    _view.SetPaymentBreakdowns(summary.PaymentBreakdowns)
                    _view.SetDailyBreakdowns(summary.DailyBreakdown)

                    Dim trends = Await _salesSummaryService.GetPaymentTrendsAsync(6)
                    _view.SetPaymentTrends(trends)
                End If
            Catch ex As Exception
                exceptionToHandle = ex
            Finally
                _view.IsLoading = False
            End Try

            If exceptionToHandle IsNot Nothing Then
                _view.ErrorMessage = "Failed to load sales summary."
                _notificationService.ShowError(exceptionToHandle.Message)
            End If
        End Function

    End Class

End Namespace
