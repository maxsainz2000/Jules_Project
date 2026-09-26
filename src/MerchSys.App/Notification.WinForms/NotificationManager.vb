Imports System.Windows.Forms

Namespace Notification.WinForms
    Public Enum NotificationType
        Info
        Success
        Warning
        [Error]
    End Enum

    Public Class NotificationManager
        Public Shared Sub Show(message As String, title As String, type As NotificationType)
            Dim icon As MessageBoxIcon = MessageBoxIcon.Information
            Select Case type
                Case NotificationType.Success
                    icon = MessageBoxIcon.Information
                Case NotificationType.Warning
                    icon = MessageBoxIcon.Warning
                Case NotificationType.Error
                    icon = MessageBoxIcon.Error
                Case NotificationType.Info
                    icon = MessageBoxIcon.Information
            End Select
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon)
        End Sub
    End Class
End Namespace
