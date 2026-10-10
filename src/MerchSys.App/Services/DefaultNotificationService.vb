Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.App.Notification.WinForms

Namespace Services
    Public Class DefaultNotificationService
        Implements INotificationService

        Public Sub ShowSuccess(message As String) Implements INotificationService.ShowSuccess
            NotificationManager.Show(message, "Success", NotificationType.Success)
        End Sub

        Public Sub ShowError(message As String) Implements INotificationService.ShowError
            NotificationManager.Show(message, "Error", NotificationType.Error)
        End Sub

        Public Sub ShowWarning(message As String) Implements INotificationService.ShowWarning
            NotificationManager.Show(message, "Warning", NotificationType.Warning)
        End Sub

        Public Sub ShowInfo(message As String) Implements INotificationService.ShowInfo
            NotificationManager.Show(message, "Information", NotificationType.Info)
        End Sub
    End Class
End Namespace
