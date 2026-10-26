Imports System.Drawing
Imports System.Windows.Forms

Namespace Views
    ''' <summary>
    ''' View contract for the connection status indicator, so the presenter depends on an
    ''' abstraction rather than the concrete WinForms control (M11).
    ''' </summary>
    Public Interface IConnectionStatusView
        ''' <summary>The underlying control, so the shell can host it.</summary>
        ReadOnly Property Control As Control

        Sub SetState(statusText As String, indicatorColor As Color, isRetryVisible As Boolean, foreColor As Color)

        Event RetryClicked As EventHandler
    End Interface
End Namespace
