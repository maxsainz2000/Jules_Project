Imports MerchSys.Accounting.Services

Namespace Views

    Public Enum IncomeStatementPeriodType
        Monthly = 1
        Quarterly = 2
        Annual = 3
    End Enum

    Public Interface IIncomeStatementView

        Property PeriodType As IncomeStatementPeriodType
        Property SelectedYear As Integer
        Property SelectedMonth As Integer
        Property SelectedQuarter As Integer

        Property PeriodLabel As String
        Property NetSalesDisplay As String
        Property CostOfGoodsSoldDisplay As String
        Property GrossProfitDisplay As String
        Property GrossMarginPercentDisplay As String
        Property OperatingExpensesDisplay As String
        Property NetIncomeDisplay As String
        Property NetMarginPercentDisplay As String

        Property WhatThisMeansText As String
        Property IsLoading As Boolean
        Property ErrorMessage As String

        Sub SetAvailableYears(years As IEnumerable(Of Integer))
        Sub SetAvailableMonths(months As IEnumerable(Of Integer))
        Sub SetAvailableQuarters(quarters As IEnumerable(Of Integer))

        Sub SetProductMargins(margins As IEnumerable(Of ProductMarginDto))
        Sub SetOperatingExpensesBreakdown(breakdown As IDictionary(Of String, Decimal))

        Event PeriodChanged As EventHandler
        Event RefreshRequested As EventHandler
        Event ViewLoaded As EventHandler
        Event ExportPdfRequested As EventHandler(Of String)

    End Interface

End Namespace
