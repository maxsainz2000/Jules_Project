Namespace Services

    Public Interface ILowStockAlertService
        Function CheckAndGenerateAlertsAsync() As Task
        Function GetActiveAlertsAsync() As Task(Of List(Of LowStockAlertDto))
        Function DismissAlertAsync(productId As Integer) As Task
    End Interface

    Public Class LowStockAlertDto
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property CurrentStock As Integer
        Public Property MinimumThreshold As Integer
        Public Property Deficit As Integer
    End Class

End Namespace
