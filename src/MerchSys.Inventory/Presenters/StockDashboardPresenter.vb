Imports MerchSys.Inventory.Services
Imports MerchSys.Inventory.Views
Imports System.Windows.Forms

Namespace Presenters

    Public Class StockDashboardPresenter
        Implements IDisposable

        Private ReadOnly _view As IStockDashboardView
        Private ReadOnly _dashboardService As IStockDashboardService
        Private ReadOnly _estimationService As IStockoutEstimationService

        Private ReadOnly _refreshTimer As Timer

        Private _allProducts As List(Of ProductRowItem) = New List(Of ProductRowItem)()

        Public Sub New(view As IStockDashboardView, dashboardService As IStockDashboardService, estimationService As IStockoutEstimationService)
            _view = view
            _dashboardService = dashboardService
            _estimationService = estimationService

            AddHandler _view.FilterChanged, AddressOf OnFilterChanged
            AddHandler _view.ProductSelected, AddressOf OnProductSelected

            _refreshTimer = New Timer()
            _refreshTimer.Interval = 60000 ' 60 seconds
            AddHandler _refreshTimer.Tick, AddressOf OnRefreshTimerTick

            LoadDataAsync().ConfigureAwait(False)
            _refreshTimer.Start()
        End Sub

        Private Async Sub OnRefreshTimerTick(sender As Object, e As EventArgs)
            Await LoadDataAsync()
        End Sub

        Private Async Function LoadDataAsync() As Task
            Dim dashboardData = Await _dashboardService.GetDashboardSummaryAsync()
            Dim estimates = Await _estimationService.EstimateAllAsync()

            Dim criticalRiskCount = Enumerable.Count(estimates, Function(est) est.RiskLevel = StockoutRiskLevel.Critical)

            _view.TotalStockValue = dashboardData.TotalStockValue
            _view.LowStockCount = dashboardData.LowStockProductCount
            _view.ExpiredBatchCount = dashboardData.ExpiredOrNearExpiryBatchCount
            _view.CriticalRiskCount = criticalRiskCount
            _view.TotalProducts = dashboardData.ProductSummaries.Count

            _allProducts.Clear()

            For Each prod In dashboardData.ProductSummaries
                Dim est = estimates.FirstOrDefault(Function(e) e.ProductId = prod.ProductId)

                Dim row As New ProductRowItem() With {
                    .ProductId = prod.ProductId,
                    .ProductName = prod.ProductName,
                    .SKU = prod.SKU,
                    .CategoryId = prod.CategoryId,
                    .CurrentQuantity = prod.CurrentQuantity,
                    .TotalValue = prod.TotalValue,
                    .IsBelowThreshold = prod.IsBelowThreshold
                }

                If est IsNot Nothing Then
                    row.RiskLevel = est.RiskLevel
                    row.EstimatedDaysUntilStockout = est.EstimatedDaysUntilStockout
                    row.EstimatedStockoutDate = est.EstimatedStockoutDate
                Else
                    row.RiskLevel = StockoutRiskLevel.Low
                    row.EstimatedDaysUntilStockout = 999
                    row.EstimatedStockoutDate = DateTime.MaxValue
                End If

                _allProducts.Add(row)
            Next

            ApplyFilters()
        End Function

        Private Sub OnFilterChanged(sender As Object, e As EventArgs)
            ApplyFilters()
        End Sub

        Private Sub ApplyFilters()
            Dim filtered = _allProducts.AsEnumerable()

            If Not String.IsNullOrWhiteSpace(_view.FilterText) Then
                Dim filterLower = _view.FilterText.ToLower()
                filtered = filtered.Where(Function(p) p.ProductName.ToLower().Contains(filterLower) OrElse p.SKU.ToLower().Contains(filterLower))
            End If

            If Not String.IsNullOrWhiteSpace(_view.CategoryFilter) AndAlso _view.CategoryFilter <> "All" Then
                ' Simplistic category filter implementation for now. Assuming the user chooses based on category ID or similar text.
                ' Ideally, this would map text to an ID. For this mockup, we'll skip complex category mapping.
            End If

            If Not String.IsNullOrWhiteSpace(_view.StatusFilter) AndAlso _view.StatusFilter <> "All" Then
                If _view.StatusFilter = "Low Stock" Then
                    filtered = filtered.Where(Function(p) p.IsBelowThreshold)
                ElseIf _view.StatusFilter = "Critical Risk" Then
                    filtered = filtered.Where(Function(p) p.RiskLevel = StockoutRiskLevel.Critical)
                End If
            End If

            _view.SetProductRows(filtered.ToList())
        End Sub

        Private Async Sub OnProductSelected(sender As Object, productId As Integer)
            Try
                Dim details = Await _dashboardService.GetProductDetailAsync(productId)
                _view.SetBatchRows(details.StockBatches)
            Catch ex As Exception
                ' Error handling should be separated, but for Presenter just swallow or log
            End Try
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            _refreshTimer?.Stop()
            _refreshTimer?.Dispose()

            RemoveHandler _view.FilterChanged, AddressOf OnFilterChanged
            RemoveHandler _view.ProductSelected, AddressOf OnProductSelected

            If TypeOf _view Is IDisposable Then
                DirectCast(_view, IDisposable).Dispose()
            End If
        End Sub

    End Class

End Namespace
