Imports MerchSys.SharedKernel.Interfaces

Namespace Services
    Public Class WinFormsLowStockNotifier
        Implements ILowStockNotifier

        Private ReadOnly _notificationManager As MerchSys.App.Notification.WinForms.NotificationManager

        Public Sub New()
            _notificationManager = New MerchSys.App.Notification.WinForms.NotificationManager()
        End Sub

        Public Sub Notify(message As String) Implements ILowStockNotifier.Notify
            _notificationManager.Show("Low Stock Alert", message)
        End Sub
    End Class
End Namespace
