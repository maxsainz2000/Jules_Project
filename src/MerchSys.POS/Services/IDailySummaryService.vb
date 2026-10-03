Imports System.Threading.Tasks

Namespace Services
    ''' <summary>
    ''' Represents a breakdown of total amounts by payment method.
    ''' </summary>
    Public Class PaymentMethodBreakdownDto
        Public Property Method As String
        Public Property Amount As Decimal
        Public Property Percentage As Decimal
    End Class

    ''' <summary>
    ''' Represents a top-selling product.
    ''' </summary>
    Public Class TopProductDto
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property QuantitySold As Integer
        Public Property TotalSales As Decimal
    End Class

    ''' <summary>
    ''' Represents a summary of sales and returns for a specific day.
    ''' </summary>
    Public Class DailySummaryDto
        Public Property TargetDate As DateTime
        Public Property TotalSalesAmount As Decimal
        Public Property TotalTransactions As Integer
        Public Property VoidedTransactions As Integer
        Public Property VoidedAmount As Decimal
        Public Property ReturnedItemsCount As Integer
        Public Property ReturnedAmount As Decimal
        Public Property PaymentBreakdown As List(Of PaymentMethodBreakdownDto) = New List(Of PaymentMethodBreakdownDto)()
        Public Property TopProducts As List(Of TopProductDto) = New List(Of TopProductDto)()
    End Class

    ''' <summary>
    ''' Represents an aggregated summary of sales and returns over a specific period (weekly/monthly).
    ''' </summary>
    Public Class PeriodSummaryDto
        Public Property StartDate As DateTime
        Public Property EndDate As DateTime
        Public Property TotalSalesAmount As Decimal
        Public Property TotalTransactions As Integer
        Public Property AverageDailySales As Decimal
        Public Property DailyBreakdown As List(Of DailySummaryDto) = New List(Of DailySummaryDto)()
        Public Property PaymentBreakdown As List(Of PaymentMethodBreakdownDto) = New List(Of PaymentMethodBreakdownDto)()
        Public Property TopProducts As List(Of TopProductDto) = New List(Of TopProductDto)()
    End Class

    ''' <summary>
    ''' Defines the contract for the daily sales summary service.
    ''' </summary>
    Public Interface IDailySummaryService
        ''' <summary>
        ''' Gets the sales summary for a specific day.
        ''' </summary>
        Function GetDailySummaryAsync(targetDate As DateTime) As Task(Of DailySummaryDto)

        ''' <summary>
        ''' Gets the sales summary for a specific week, starting from the specified date.
        ''' </summary>
        Function GetWeeklySummaryAsync(weekStartDate As DateTime) As Task(Of PeriodSummaryDto)

        ''' <summary>
        ''' Gets the sales summary for a specific month and year.
        ''' </summary>
        Function GetMonthlySummaryAsync(year As Integer, month As Integer) As Task(Of PeriodSummaryDto)
    End Interface
End Namespace
