Namespace Services

    Public Enum StockoutRiskLevel
        Critical
        High
        Medium
        Low
    End Enum

    Public Class StockoutEstimateDto
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property CurrentStock As Integer
        Public Property AvgDailySales As Decimal
        Public Property EstimatedDaysUntilStockout As Integer
        Public Property RiskLevel As StockoutRiskLevel
        Public Property EstimatedStockoutDate As DateTime
    End Class

    Public Interface IStockoutEstimationService
        Function EstimateAllAsync() As Task(Of List(Of StockoutEstimateDto))
        Function EstimateForProductAsync(productId As Integer) As Task(Of StockoutEstimateDto)
        Function GetCriticalProductsAsync() As Task(Of List(Of StockoutEstimateDto))
    End Interface

End Namespace
