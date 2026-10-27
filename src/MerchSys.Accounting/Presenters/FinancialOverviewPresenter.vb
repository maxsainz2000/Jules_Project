Imports System.Threading
Imports MediatR
Imports MerchSys.Accounting.Services
Imports MerchSys.Accounting.Views
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.SharedKernel.Queries

Namespace Presenters

    Public Class FinancialOverviewPresenter
        Private _view As IFinancialOverviewView
        Private ReadOnly _overviewService As IFinancialOverviewService
        Private ReadOnly _whatThisMeansService As IWhatThisMeansService
        Private ReadOnly _notificationService As INotificationService
        Private ReadOnly _mediator As IMediator
        Private ReadOnly _sessionService As ISessionService
        Private ReadOnly _refreshTimer As Timer

        Public Event NavigateToVatReturnRequested As EventHandler

        Public Sub New(overviewService As IFinancialOverviewService,
                       whatThisMeansService As IWhatThisMeansService,
                       notificationService As INotificationService,
                       mediator As IMediator,
                       sessionService As ISessionService)
            _overviewService = overviewService
            _whatThisMeansService = whatThisMeansService
            _notificationService = notificationService
            _mediator = mediator
            _sessionService = sessionService
            _refreshTimer = New Timer(AddressOf OnTimerTick, Nothing, Timeout.Infinite, Timeout.Infinite)
        End Sub

        Public Property View As IFinancialOverviewView
            Get
                Return _view
            End Get
            Set(value As IFinancialOverviewView)
                If _view IsNot Nothing Then
                    RemoveHandler _view.RefreshRequested, AddressOf OnRefreshRequested
                    RemoveHandler _view.ViewLoaded, AddressOf OnViewLoaded
                    RemoveHandler _view.VatTileClicked, AddressOf OnVatTileClicked
                End If
                _view = value
                If _view IsNot Nothing Then
                    AddHandler _view.RefreshRequested, AddressOf OnRefreshRequested
                    AddHandler _view.ViewLoaded, AddressOf OnViewLoaded
                    AddHandler _view.VatTileClicked, AddressOf OnVatTileClicked
                End If
            End Set
        End Property

        Private Sub OnVatTileClicked(sender As Object, e As EventArgs)
            If _sessionService.CurrentUserRole = MerchSys.SharedKernel.Enums.UserRole.Manager OrElse _sessionService.CurrentUserRole = MerchSys.SharedKernel.Enums.UserRole.Developer Then
                RaiseEvent NavigateToVatReturnRequested(Me, EventArgs.Empty)
            End If
        End Sub

        Private Async Sub OnViewLoaded(sender As Object, e As EventArgs)
            Await LoadOverviewAsync()
            ' Start the 5-minute periodic auto-refresh timer (300000 ms)
            _refreshTimer.Change(300000, 300000)
        End Sub

        Private Async Sub OnRefreshRequested(sender As Object, e As EventArgs)
            ' Reset the timer so we don't immediately refresh again
            _refreshTimer.Change(300000, 300000)
            Await LoadOverviewAsync()
        End Sub

        Private Sub OnTimerTick(state As Object)
            ' We need to ensure we don't try to access UI elements on a background thread if the view doesn't marshal properly,
            ' but WinForms views typically have an InvokeRequired check.
            ' Assuming LoadOverviewAsync updates the view properly. In WinForms MVP, the Presenter usually invokes via ISynchronizeInvoke or the view handles it.
            ' A System.Threading.Timer callback happens on a ThreadPool thread.
            ' We will just call the async method.
            Try
                If TypeOf _view Is System.ComponentModel.ISynchronizeInvoke AndAlso DirectCast(_view, System.ComponentModel.ISynchronizeInvoke).InvokeRequired Then
                    DirectCast(_view, System.ComponentModel.ISynchronizeInvoke).Invoke(Sub()
                                                                                           Dim dummy = LoadOverviewAsync()
                                                                                       End Sub, Nothing)
                Else
                    Dim dummy = LoadOverviewAsync()
                End If
            Catch ex As Exception
                ' Background thread exception handling
            End Try
        End Sub

        Public Async Function LoadOverviewAsync() As Task
            If _view Is Nothing Then Return

            ' Store exception to handle outside Try/Catch for Await restrictions if necessary (VB Await in Catch/Finally is illegal)
            Dim loadEx As Exception = Nothing

            Try
                _view.IsLoading = True
                _view.ErrorMessage = String.Empty

                Dim overview = Await _overviewService.GetOverviewAsync()

                _view.TodayRevenueDisplay = $"₱{overview.TodayRevenue:N2}"
                _view.MtdRevenueDisplay = $"₱{overview.MtdRevenue:N2}"
                _view.YtdRevenueDisplay = $"₱{overview.YtdRevenue:N2}"
                _view.TotalARDisplay = $"₱{overview.TotalAR:N2}"
                _view.TotalAPDisplay = $"₱{overview.TotalAP:N2}"
                _view.InventoryValueDisplay = $"₱{overview.InventoryValue:N2}"
                _view.GrossMarginTodayDisplay = $"{overview.GrossMarginToday:N1}%"

                If overview.IsVatRegistered Then
                    _view.VatPayableDisplay = $"₱{overview.VatPayable:N2}"
                    _view.VatDueDateDisplay = If(overview.VatFilingDueDate.HasValue, overview.VatFilingDueDate.Value.ToString("MMM dd, yyyy"), "N/A")
                    _view.VatSeverity = overview.VatPayableSeverity
                Else
                    _view.VatPayableDisplay = "N/A"
                    _view.VatDueDateDisplay = ""
                    _view.VatSeverity = KpiSeverity.Info
                End If

                _view.WhatThisMeansText = _whatThisMeansService.GenerateOverviewInterpretation(overview)

                _view.SetTopSellingProducts(overview.TopSellingProducts)
                _view.SetMonthlyTrends(overview.MonthlyTrends)

                If overview.MonthlyTrends IsNot Nothing AndAlso overview.MonthlyTrends.Count >= 2 Then
                    Dim latestMonth = overview.MonthlyTrends.Last()
                    Dim prevMonth = overview.MonthlyTrends.Skip(overview.MonthlyTrends.Count - 2).First()
                    If prevMonth.Revenue > 0 Then
                        _view.RevenueDeltaPercent = ((latestMonth.Revenue - prevMonth.Revenue) / prevMonth.Revenue) * 100D
                    Else
                        _view.RevenueDeltaPercent = Nothing
                    End If
                    _view.RevenueSparkPoints = overview.MonthlyTrends.Select(Function(m) m.Revenue).ToList()
                Else
                    _view.RevenueDeltaPercent = Nothing
                    _view.RevenueSparkPoints = New List(Of Decimal)()
                End If

                Dim lowStock = Await _mediator.Send(New GetLowStockAlertCountQuery())
                _view.LowStockAlertCount = lowStock
                _view.OverdueARCount = 0
                _view.OverdueAPCount = 0

                _view.LastRefreshedText = $"Last updated: {DateTime.Now:hh:mm:ss tt}"

            Catch ex As Exception
                loadEx = ex
            Finally
                _view.IsLoading = False
            End Try

            If loadEx IsNot Nothing Then
                _view.ErrorMessage = "Failed to load financial overview."
                _notificationService.ShowError(loadEx.Message)
            End If
        End Function
    End Class

End Namespace
