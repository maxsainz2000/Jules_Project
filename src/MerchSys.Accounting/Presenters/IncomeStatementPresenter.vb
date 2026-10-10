Imports MerchSys.Accounting.Services
Imports MerchSys.Accounting.Views
Imports MerchSys.SharedKernel.Interfaces
Imports System.Threading.Tasks

Namespace Presenters

    Public Class IncomeStatementPresenter
        Private ReadOnly _service As IIncomeStatementService
        Private ReadOnly _whatThisMeansService As IWhatThisMeansService
        Private ReadOnly _notificationService As INotificationService

        Private _view As IIncomeStatementView

        Public Sub New(
            service As IIncomeStatementService,
            whatThisMeansService As IWhatThisMeansService,
            notificationService As INotificationService)
            _service = service
            _whatThisMeansService = whatThisMeansService
            _notificationService = notificationService
        End Sub

        Public Property View As IIncomeStatementView
            Get
                Return _view
            End Get
            Set(value As IIncomeStatementView)
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
            Dim currentYear = DateTime.Now.Year
            Dim years = Enumerable.Range(currentYear - 5, 6).OrderByDescending(Function(y) y).ToList()
            Dim months = Enumerable.Range(1, 12).ToList()
            Dim quarters = Enumerable.Range(1, 4).ToList()

            _view.SetAvailableYears(years)
            _view.SetAvailableMonths(months)
            _view.SetAvailableQuarters(quarters)

            _view.SelectedYear = currentYear
            _view.SelectedMonth = DateTime.Now.Month
            _view.SelectedQuarter = (DateTime.Now.Month - 1) \ 3 + 1
            _view.PeriodType = IncomeStatementPeriodType.Monthly

            Dim dummy = LoadStatementAsync()
        End Sub

        Private Sub OnPeriodChanged(sender As Object, e As EventArgs)
            Dim dummy = LoadStatementAsync()
        End Sub

        Private Sub OnRefreshRequested(sender As Object, e As EventArgs)
            Dim dummy = LoadStatementAsync()
        End Sub

        Public Async Function LoadStatementAsync() As Task
            If _view Is Nothing Then Return

            _view.IsLoading = True
            _view.ErrorMessage = String.Empty

            Dim statement As IncomeStatementDto = Nothing
            Dim previousMargin As Decimal? = Nothing
            Dim exceptionToHandle As Exception = Nothing

            Try
                If _view.PeriodType = IncomeStatementPeriodType.Monthly Then
                    statement = Await _service.GenerateMonthlyAsync(_view.SelectedYear, _view.SelectedMonth)

                    Dim prevMonth = _view.SelectedMonth - 1
                    Dim prevYear = _view.SelectedYear
                    If prevMonth = 0 Then
                        prevMonth = 12
                        prevYear -= 1
                    End If
                    Dim prevStatement = Await _service.GenerateMonthlyAsync(prevYear, prevMonth)
                    previousMargin = prevStatement.GrossMarginPercent
                ElseIf _view.PeriodType = IncomeStatementPeriodType.Quarterly Then
                    statement = Await _service.GenerateQuarterlyAsync(_view.SelectedYear, _view.SelectedQuarter)

                    Dim prevQuarter = _view.SelectedQuarter - 1
                    Dim prevYear = _view.SelectedYear
                    If prevQuarter = 0 Then
                        prevQuarter = 4
                        prevYear -= 1
                    End If
                    Dim prevStatement = Await _service.GenerateQuarterlyAsync(prevYear, prevQuarter)
                    previousMargin = prevStatement.GrossMarginPercent
                ElseIf _view.PeriodType = IncomeStatementPeriodType.Annual Then
                    statement = Await _service.GenerateAnnualAsync(_view.SelectedYear)

                    Dim prevStatement = Await _service.GenerateAnnualAsync(_view.SelectedYear - 1)
                    previousMargin = prevStatement.GrossMarginPercent
                End If

                If statement IsNot Nothing Then
                    _view.PeriodLabel = statement.PeriodLabel
                    _view.NetSalesDisplay = $"₱{statement.NetSales:N2}"
                    _view.CostOfGoodsSoldDisplay = $"₱{statement.CostOfGoodsSold:N2}"
                    _view.GrossProfitDisplay = $"₱{statement.GrossProfit:N2}"
                    _view.GrossMarginPercentDisplay = $"{statement.GrossMarginPercent:N1}%"
                    _view.OperatingExpensesDisplay = $"₱{statement.OperatingExpenses:N2}"
                    _view.NetIncomeDisplay = $"₱{statement.NetIncome:N2}"
                    _view.NetMarginPercentDisplay = $"{statement.NetMarginPercent:N1}%"

                    _view.WhatThisMeansText = _whatThisMeansService.GenerateIncomeStatementInterpretation(statement, previousMargin)

                    Dim margins = Await _service.GetPerProductMarginsAsync(statement.StartDate, statement.EndDate)
                    margins = margins.OrderBy(Function(m) m.GrossMarginPercent).ToList()
                    _view.SetProductMargins(margins)

                    _view.SetOperatingExpensesBreakdown(statement.OperatingExpensesByCategory)
                End If
            Catch ex As Exception
                exceptionToHandle = ex
            Finally
                _view.IsLoading = False
            End Try

            If exceptionToHandle IsNot Nothing Then
                _view.ErrorMessage = "Failed to load income statement."
                _notificationService.ShowError(exceptionToHandle.Message)
            End If
        End Function

    End Class

End Namespace
