Imports MerchSys.Accounting.Services

Namespace Views

    Public Interface IFinancialOverviewView
        Property TodayRevenueDisplay As String
        Property MtdRevenueDisplay As String
        Property YtdRevenueDisplay As String
        Property TotalARDisplay As String
        Property TotalAPDisplay As String
        Property InventoryValueDisplay As String
        Property GrossMarginTodayDisplay As String
        Property WhatThisMeansText As String
        Property OverdueARCount As Integer
        Property OverdueAPCount As Integer
        Property LowStockAlertCount As Integer
        Property IsLoading As Boolean
        Property ErrorMessage As String
        Property LastRefreshedText As String

        Property VatPayableDisplay As String
        Property VatDueDateDisplay As String
        Property VatSeverity As KpiSeverity

        Sub SetTopSellingProducts(products As IEnumerable(Of TopProductDto))
        Sub SetMonthlyTrends(trends As IEnumerable(Of MonthlyTrendDto))

        Event RefreshRequested As EventHandler
        Event ViewLoaded As EventHandler
        Event VatTileClicked As EventHandler
    End Interface

End Namespace
