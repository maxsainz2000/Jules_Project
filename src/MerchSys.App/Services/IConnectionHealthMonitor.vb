Imports System
Imports System.Threading.Tasks

Namespace Services
    Public Enum ConnectionState
        Online
        Reconnecting
        Offline
    End Enum

    Public Class ConnectionStateChangedEventArgs
        Inherits EventArgs

        Public ReadOnly Property State As ConnectionState

        Public Sub New(state As ConnectionState)
            Me.State = state
        End Sub
    End Class

    Public Interface IConnectionHealthMonitor
        Event StateChanged As EventHandler(Of ConnectionStateChangedEventArgs)
        ReadOnly Property CurrentState As ConnectionState

        Sub Start()
        Sub [Stop]()
        Function RetryNowAsync() As Task
    End Interface
End Namespace
