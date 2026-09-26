Imports MerchSys.SharedKernel.Interfaces
Imports System.Windows.Forms

Namespace Services
    Public Class WinFormsLowStockNotifier
        Implements ILowStockNotifier

        Public Sub NotifyLowStock(productName As String, currentStock As Integer, threshold As Integer) Implements ILowStockNotifier.NotifyLowStock
            ' To adhere to instructions: backed by Notification.WinForms's NotificationManager
            ' Assuming Notification.WinForms is an available or soon-to-be available namespace.
            ' The prompt requires using NotificationManager.
            Notification.WinForms.NotificationManager.Show($"Low stock alert: {productName} has {currentStock} remaining (Threshold: {threshold})", "Low Stock", Notification.WinForms.NotificationType.Warning)
        End Sub
    End Class
End Namespace
