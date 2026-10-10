Imports MerchSys.Accounting.Services

Namespace Views

    Public Enum SalesSummaryPeriodType
        Daily = 1
        Weekly = 2
        Monthly = 3
    End Enum

    Public Interface ISalesSummaryView

        Property PeriodType As SalesSummaryPeriodType
        Property SelectedDate As DateTime
        Property SelectedYear As Integer
        Property SelectedMonth As Integer

        Property PeriodLabel As String
        Property TotalGrossSalesDisplay As String
        Property TotalNetSalesDisplay As String
        Property TotalReturnsDisplay As String
        Property TransactionCountDisplay As String
        Property AvgTransactionValueDisplay As String
        Property CreditSalesPercentageDisplay As String
        Property HasCreditWarning As Boolean
        Property WhatThisMeansText As String
        Property IsLoading As Boolean
        Property ErrorMessage As String
        Property ShowDatePicker As Boolean
        Property ShowMonthSelectors As Boolean
        Property ShowDailyBreakdown As Boolean

        Sub SetAvailableYears(years As IEnumerable(Of Integer))
        Sub SetAvailableMonths(months As IEnumerable(Of Integer))

        Sub SetPaymentBreakdowns(breakdowns As IEnumerable(Of PaymentBreakdownDto))
        Sub SetDailyBreakdowns(breakdowns As IEnumerable(Of DailySalesDto))
        Sub SetPaymentTrends(trends As IEnumerable(Of PaymentTrendDto))

        Event PeriodChanged As EventHandler
        Event RefreshRequested As EventHandler
        Event ViewLoaded As EventHandler

    End Interface

End Namespace
