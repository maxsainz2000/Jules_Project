Imports System

Namespace Services
    ''' <summary>
    ''' Session idle-timeout settings, bound from the <c>Session</c> configuration section.
    '''
    ''' Both properties sanitise on assignment. Without that, a missing section (or a
    ''' hand-edited zero) yields a timeout of zero, and <c>WinFormsIdleMonitor</c> then finds
    ''' that any elapsed time is greater than zero — so the session expires on its first timer
    ''' tick, about a second after the user logs in.
    ''' </summary>
    Public Class IdleMonitorOptions
        Public Const DefaultIdleTimeoutMinutes As Integer = 20
        Public Const DefaultWarningLeadSeconds As Integer = 60

        Private _idleTimeoutMinutes As Integer = DefaultIdleTimeoutMinutes
        Private _warningLeadSeconds As Integer = DefaultWarningLeadSeconds

        ''' <summary>
        ''' Idle timeout in minutes. A non-positive value is treated as "not configured" and
        ''' falls back to <see cref="DefaultIdleTimeoutMinutes"/>.
        ''' </summary>
        Public Property IdleTimeoutMinutes As Integer
            Get
                Return _idleTimeoutMinutes
            End Get
            Set(value As Integer)
                _idleTimeoutMinutes = If(value > 0, value, DefaultIdleTimeoutMinutes)
            End Set
        End Property

        ''' <summary>
        ''' How long before expiry the warning appears. A negative value falls back to
        ''' <see cref="DefaultWarningLeadSeconds"/>; zero is legitimate (warn at expiry).
        ''' </summary>
        Public Property WarningLeadSeconds As Integer
            Get
                Return _warningLeadSeconds
            End Get
            Set(value As Integer)
                _warningLeadSeconds = If(value >= 0, value, DefaultWarningLeadSeconds)
            End Set
        End Property
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
