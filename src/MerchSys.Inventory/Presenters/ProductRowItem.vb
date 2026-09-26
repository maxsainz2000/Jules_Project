Imports MerchSys.Inventory.Services

Namespace Presenters

    Public Class ProductRowItem
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property SKU As String
        Public Property CategoryId As Integer
        Public Property CurrentQuantity As Integer
        Public Property TotalValue As Decimal
        Public Property IsBelowThreshold As Boolean

        ' From StockoutEstimateDto
        Public Property RiskLevel As StockoutRiskLevel
        Public Property EstimatedDaysUntilStockout As Integer
        Public Property EstimatedStockoutDate As DateTime
    End Class

End Namespace
