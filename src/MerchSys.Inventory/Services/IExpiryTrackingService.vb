Namespace Services

    Public Class ExpiryAlertDto
        Public Property ProductId As Integer
        Public Property BatchId As Integer
        Public Property ExpiryDate As DateTime
        Public Property QuantityRemaining As Integer
    End Class

    Public Class ProductExpiryStatusDto
        Public Property ProductId As Integer
        Public Property TotalQuantity As Integer
        Public Property ExpiredQuantity As Integer
        Public Property GoodQuantity As Integer
    End Class

    Public Interface IExpiryTrackingService
        Function GetNearExpiryAlertsAsync(daysThreshold As Integer) As Task(Of List(Of ExpiryAlertDto))
        Function GetExpiredBatchesAsync() As Task(Of List(Of ExpiryAlertDto))
        Function GetProductExpiryStatusAsync(productId As Integer) As Task(Of ProductExpiryStatusDto)
        Function WriteOffExpiredStockAsync(reason As String) As Task(Of Integer)
    End Interface

End Namespace
