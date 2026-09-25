Imports System

Namespace Services
    Public Class IdleMonitorOptions
        Public Property IdleTimeoutMinutes As Integer
        Public Property WarningLeadSeconds As Integer
    End Class

    Public Class IdleMonitorWarningEventArgs
        Inherits EventArgs
        Public Property RemainingSeconds As Integer

        Public Sub New(remainingSeconds As Integer)
            Me.RemainingSeconds = remainingSeconds
        End Sub
    End Class

    Public Interface IIdleMonitor
        Event IdleWarning As EventHandler(Of IdleMonitorWarningEventArgs)
        Event SessionExpired As EventHandler

        Sub Start()
        Sub [Stop]()
        Sub Reset()
    End Interface
End Namespace
