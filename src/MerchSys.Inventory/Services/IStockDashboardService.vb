Namespace Services

    Public Interface IStockDashboardService
        Function GetDashboardSummaryAsync() As Task(Of StockDashboardDto)
        Function GetProductDetailAsync(productId As Integer) As Task(Of ProductDetailDto)
    End Interface

    Public Class StockDashboardDto
        Public Property TotalStockValue As Decimal
        Public Property LowStockProductCount As Integer
        Public Property ExpiredOrNearExpiryBatchCount As Integer
        Public Property ProductSummaries As List(Of ProductSummaryDto)
    End Class

    Public Class ProductSummaryDto
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property SKU As String
        Public Property CategoryId As Integer
        Public Property CurrentQuantity As Integer
        Public Property TotalValue As Decimal
        Public Property IsBelowThreshold As Boolean
    End Class

    Public Class ProductDetailDto
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property SKU As String
        Public Property CurrentQuantity As Integer
        Public Property StockBatches As List(Of StockBatchSummaryDto)
        Public Property ShrinkageHistory As List(Of ShrinkageSummaryDto)
        Public Property StockMovements As List(Of StockMovementDto)
    End Class

    Public Class StockBatchSummaryDto
        Public Property BatchId As Integer
        Public Property QuantityReceived As Integer
        Public Property QuantityRemaining As Integer
        Public Property UnitCost As Decimal
        Public Property ReceiptDate As DateTime
        Public Property ExpiryDate As DateTime?
    End Class

    Public Class ShrinkageSummaryDto
        Public Property ShrinkageId As Integer
        Public Property Reason As String
        Public Property QuantityLost As Integer
        Public Property UnitCost As Decimal
        Public Property TotalValue As Decimal
        Public Property RecordedAt As DateTime
    End Class

    Public Class StockMovementDto
    End Class

End Namespace
