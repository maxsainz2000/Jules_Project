Namespace Global.Notification.WinForms
    Public Class NotificationManager
        Public Sub Show(title As String, message As String)
            System.Windows.Forms.MessageBox.Show(message, title)
        End Sub
    End Class
End Namespace
