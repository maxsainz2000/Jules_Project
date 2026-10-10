Imports System.Threading
Imports MerchSys.SharedKernel.Enums

Namespace Services

    Public Class PaymentBreakdownDto
        Public Property PaymentMethod As PaymentMethod
        Public Property PaymentMethodName As String
        Public Property GrossSales As Decimal
        Public Property TransactionCount As Integer
        Public Property PercentageOfTotal As Decimal
    End Class

    Public Class DailySalesDto
        Public Property SaleDate As DateTime
        Public Property DayLabel As String
        Public Property GrossSales As Decimal
        Public Property Discounts As Decimal
        Public Property Returns As Decimal
        Public Property NetSales As Decimal
        Public Property TransactionCount As Integer
    End Class

    Public Class PaymentTrendDto
        Public Property PeriodLabel As String
        Public Property CashSales As Decimal
        Public Property CreditSales As Decimal
        Public Property OtherSales As Decimal
        Public Property CreditPercentage As Decimal
    End Class

    Public Class AccountingSalesSummaryDto
        Public Property PeriodLabel As String
        Public Property StartDate As DateTime
        Public Property EndDate As DateTime
        Public Property TotalGrossSales As Decimal
        Public Property TotalNetSales As Decimal
        Public Property TotalReturns As Decimal
        Public Property TransactionCount As Integer
        Public Property AvgTransactionValue As Decimal
        Public Property CreditSalesPercentage As Decimal
        Public Property HasCreditWarning As Boolean
        Public Property PaymentBreakdowns As New List(Of PaymentBreakdownDto)
        Public Property DailyBreakdown As New List(Of DailySalesDto)
    End Class

    Public Interface ISalesSummaryService
        Function GetSalesSummaryAsync(startDate As DateTime, endDate As DateTime, Optional periodLabel As String = Nothing, Optional cancellationToken As CancellationToken = Nothing) As Task(Of AccountingSalesSummaryDto)
        Function GetDailySummaryAsync(targetDate As DateTime, Optional cancellationToken As CancellationToken = Nothing) As Task(Of AccountingSalesSummaryDto)
        Function GetWeeklySummaryAsync(anchorDate As DateTime, Optional cancellationToken As CancellationToken = Nothing) As Task(Of AccountingSalesSummaryDto)
        Function GetMonthlySummaryAsync(year As Integer, month As Integer, Optional cancellationToken As CancellationToken = Nothing) As Task(Of AccountingSalesSummaryDto)
        Function GetPaymentTrendsAsync(monthsBack As Integer, Optional cancellationToken As CancellationToken = Nothing) As Task(Of List(Of PaymentTrendDto))
    End Interface

End Namespace
