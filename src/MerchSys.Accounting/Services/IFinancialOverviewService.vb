Imports System.Threading

Namespace Services

    Partial Public Class FinancialOverviewDto
        Public Property TodayRevenue As Decimal
        Public Property MtdRevenue As Decimal
        Public Property YtdRevenue As Decimal
        Public Property TotalAR As Decimal
        Public Property TotalAP As Decimal
        Public Property InventoryValue As Decimal
        Public Property GrossMarginToday As Decimal
        Public Property GrossMarginMtd As Decimal
        Public Property GrossMarginYtd As Decimal
        Public Property MonthlyTrends As New List(Of MonthlyTrendDto)
        Public Property TopSellingProducts As New List(Of TopProductDto)
        Public Property SnapshotDate As DateTime
    End Class

    Public Class MonthlyTrendDto
        Public Property MonthLabel As String
        Public Property MonthStart As DateTime
        Public Property Revenue As Decimal
        Public Property Cogs As Decimal
        Public Property GrossProfit As Decimal
        Public Property GrossMarginPercent As Decimal
    End Class

    Public Class TopProductDto
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property QuantitySold As Decimal
        Public Property TotalRevenue As Decimal
        Public Property TotalGrossProfit As Decimal
    End Class

    Public Interface IFinancialOverviewService
        Function GetOverviewAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of FinancialOverviewDto)
        Function RefreshSnapshotAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of Entities.FinancialSnapshot)
    End Interface

End Namespace