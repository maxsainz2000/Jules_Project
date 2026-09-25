Imports System
Imports System.Windows.Forms
Imports Microsoft.Extensions.Options

Namespace Services
    Public Class WinFormsIdleMonitor
        Implements IIdleMonitor
        Implements IMessageFilter

        Private ReadOnly _options As IdleMonitorOptions
        Private ReadOnly _timer As Timer
        Private _lastInputTime As DateTime
        Private _isWarningActive As Boolean

        Public Event IdleWarning As EventHandler(Of IdleMonitorWarningEventArgs) Implements IIdleMonitor.IdleWarning
        Public Event SessionExpired As EventHandler Implements IIdleMonitor.SessionExpired

        Public Sub New(options As IOptions(Of IdleMonitorOptions))
            _options = options.Value
            _timer = New Timer()
            _timer.Interval = 1000 ' Tick every second
            AddHandler _timer.Tick, AddressOf OnTimerTick
        End Sub

        Public Sub Start() Implements IIdleMonitor.Start
            _lastInputTime = DateTime.UtcNow
            _isWarningActive = False
            _timer.Start()
            Application.AddMessageFilter(Me)
        End Sub

        Public Sub [Stop]() Implements IIdleMonitor.Stop
            _timer.Stop()
            Application.RemoveMessageFilter(Me)
        End Sub

        Public Sub Reset() Implements IIdleMonitor.Reset
            _lastInputTime = DateTime.UtcNow
            _isWarningActive = False
        End Sub

        Private Sub OnTimerTick(sender As Object, e As EventArgs)
            Dim idleTime = DateTime.UtcNow - _lastInputTime
            Dim totalTimeoutSeconds = _options.IdleTimeoutMinutes * 60
            Dim warningThresholdSeconds = totalTimeoutSeconds - _options.WarningLeadSeconds

            If idleTime.TotalSeconds >= totalTimeoutSeconds Then
                [Stop]()
                RaiseEvent SessionExpired(Me, EventArgs.Empty)
            ElseIf idleTime.TotalSeconds >= warningThresholdSeconds Then
                _isWarningActive = True
                Dim remainingSeconds = CInt(Math.Ceiling(totalTimeoutSeconds - idleTime.TotalSeconds))
                RaiseEvent IdleWarning(Me, New IdleMonitorWarningEventArgs(remainingSeconds))
            End If
        End Sub

        Public Function PreFilterMessage(ByRef m As Message) As Boolean Implements IMessageFilter.PreFilterMessage
            ' Constants for mouse and keyboard events
            Const WM_MOUSEMOVE As Integer = &H200
            Const WM_LBUTTONDOWN As Integer = &H201
            Const WM_RBUTTONDOWN As Integer = &H204
            Const WM_MBUTTONDOWN As Integer = &H207
            Const WM_KEYDOWN As Integer = &H100
            Const WM_KEYUP As Integer = &H101

            If m.Msg = WM_MOUSEMOVE OrElse m.Msg = WM_LBUTTONDOWN OrElse m.Msg = WM_RBUTTONDOWN OrElse m.Msg = WM_MBUTTONDOWN OrElse m.Msg = WM_KEYDOWN OrElse m.Msg = WM_KEYUP Then
                If Not _isWarningActive Then
                    Reset()
                End If
            End If

            Return False
        End Function
    End Class

#If DEBUG Then
    Public Class NoOpIdleMonitor
        Implements IIdleMonitor

        Public Event IdleWarning As EventHandler(Of IdleMonitorWarningEventArgs) Implements IIdleMonitor.IdleWarning
        Public Event SessionExpired As EventHandler Implements IIdleMonitor.SessionExpired

        Public Sub Start() Implements IIdleMonitor.Start
        End Sub

        Public Sub [Stop]() Implements IIdleMonitor.Stop
        End Sub

        Public Sub Reset() Implements IIdleMonitor.Reset
        End Sub
    End Class
#End If

End Namespace
