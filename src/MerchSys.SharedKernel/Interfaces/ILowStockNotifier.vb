Namespace Interfaces
    Public Interface ILowStockNotifier
        Sub NotifyLowStock(productName As String, currentStock As Integer, threshold As Integer)
    End Interface
End Namespace
