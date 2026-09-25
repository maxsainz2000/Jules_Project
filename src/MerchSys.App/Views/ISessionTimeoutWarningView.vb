Imports System

Namespace Views
    Public Interface ISessionTimeoutWarningView
        Event StaySignedInRequested As EventHandler
        Event SignOutRequested As EventHandler

        Sub SetCountdown(formattedTime As String)
        Sub CloseView()
        Function ShowDialog() As System.Windows.Forms.DialogResult
        Sub Show(owner As System.Windows.Forms.IWin32Window)
    End Interface
End Namespace
