Namespace Services

    Public Interface IStockService
        Function AddStockBatchAsync(productId As Integer, quantity As Integer, unitCost As Decimal, receiptDate As DateTime, sourcePurchaseOrderId As Integer, expiryDate As DateTime?) As Task
        Function DeductStockFIFOAsync(productId As Integer, quantity As Integer) As Task(Of FIFODeductionResult)
        Function GetCurrentStockLevelsAsync() As Task(Of List(Of StockLevelDto))
    End Interface

    Public Class FIFODeductionResult
        Public Property ProductId As Integer
        Public Property DeductedQuantity As Integer
        Public Property RemainingQuantityNeeded As Integer
        Public Property Success As Boolean
    End Class

    Public Class StockLevelDto
        Public Property ProductId As Integer
        Public Property CurrentQuantity As Integer
        Public Property ThresholdQuantity As Integer
        Public Property IsBelowThreshold As Boolean
    End Class

End Namespace
