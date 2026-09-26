Namespace Interfaces

    Public Interface ILowStockNotifier
        Sub NotifyLowStock(productId As Integer, currentQuantity As Integer, threshold As Integer)
    End Interface

End Namespace
