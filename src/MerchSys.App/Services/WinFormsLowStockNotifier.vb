Imports Notification.WinForms

Namespace Services
    Public Class WinFormsLowStockNotifier
        Private ReadOnly _notificationManager As NotificationManager

        Public Sub New()
            _notificationManager = New NotificationManager()
        End Sub

        Public Sub Notify(productName As String, currentStock As Integer, reorderPoint As Integer)
            _notificationManager.Show("Low Stock Alert", $"Product '{productName}' is running low on stock. Current stock: {currentStock}. Reorder point: {reorderPoint}.")
        End Sub
    End Class
End Namespace
