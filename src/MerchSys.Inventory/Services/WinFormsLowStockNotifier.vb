Imports MerchSys.SharedKernel.Interfaces

Namespace Services

    Public Class WinFormsLowStockNotifier
        Implements ILowStockNotifier

        Private ReadOnly _notificationManager As New Notification.WinForms.NotificationManager()

        Public Sub NotifyLowStock(productId As Integer, currentQuantity As Integer, threshold As Integer) Implements ILowStockNotifier.NotifyLowStock
            _notificationManager.Show($"Product {productId} has low stock: {currentQuantity} (threshold: {threshold})")
        End Sub

    End Class

End Namespace
