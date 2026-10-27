Imports System.Collections.Generic
Imports MerchSys.POS.Services

Namespace Views

    Public Enum SummaryPeriodMode
        Daily
        Weekly
        Monthly
    End Enum

    Public Interface IDailySummaryView
        ' Events
        Event Load As EventHandler
        Event PeriodModeChanged As EventHandler
        Event DateChanged As EventHandler
        Event NavigatePrevious As EventHandler
        Event NavigateNext As EventHandler

        ' Properties
        Property CurrentMode As SummaryPeriodMode
        Property CurrentDate As DateTime
        Property DateRangeDisplay As String
        Property TotalSales As Decimal
        Property TotalTransactions As Integer
        Property VoidedTransactions As Integer
        Property VoidedAmount As Decimal
        Property ReturnedItemsCount As Integer
        Property ReturnedAmount As Decimal
        Property AverageDailySales As Decimal

        Property PaymentBreakdown As IEnumerable(Of PaymentMethodBreakdownDto)
        Property TopProducts As IEnumerable(Of TopProductDto)
        Property TrendData As IEnumerable(Of DailySummaryDto)

        Property Message As String
        Property ErrorMessage As String

        Property SalesDeltaPercent As Decimal?

    End Interface

End Namespace
