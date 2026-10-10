Imports System.Threading

Namespace Services

    Public Class IncomeStatementDto
        Public Property PeriodLabel As String
        Public Property StartDate As DateTime
        Public Property EndDate As DateTime
        Public Property GrossSales As Decimal
        Public Property SalesReturns As Decimal
        Public Property NetSales As Decimal
        Public Property CostOfGoodsSold As Decimal
        Public Property GrossProfit As Decimal
        Public Property GrossMarginPercent As Decimal
        Public Property OperatingExpenses As Decimal
        Public Property OperatingExpensesByCategory As New Dictionary(Of String, Decimal)
        Public Property NetIncome As Decimal
        Public Property NetMarginPercent As Decimal
    End Class

    Public Class ProductMarginDto
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property QuantitySold As Decimal
        Public Property Revenue As Decimal
        Public Property Cogs As Decimal
        Public Property GrossProfit As Decimal
        Public Property GrossMarginPercent As Decimal
    End Class

    Public Interface IIncomeStatementService
        Function GenerateAsync(startDate As DateTime, endDate As DateTime, Optional periodLabel As String = Nothing, Optional cancellationToken As CancellationToken = Nothing) As Task(Of IncomeStatementDto)
        Function GenerateMonthlyAsync(year As Integer, month As Integer, Optional cancellationToken As CancellationToken = Nothing) As Task(Of IncomeStatementDto)
        Function GenerateQuarterlyAsync(year As Integer, quarter As Integer, Optional cancellationToken As CancellationToken = Nothing) As Task(Of IncomeStatementDto)
        Function GenerateAnnualAsync(year As Integer, Optional cancellationToken As CancellationToken = Nothing) As Task(Of IncomeStatementDto)
        Function GetPerProductMarginsAsync(startDate As DateTime, endDate As DateTime, Optional cancellationToken As CancellationToken = Nothing) As Task(Of List(Of ProductMarginDto))
    End Interface

End Namespace
