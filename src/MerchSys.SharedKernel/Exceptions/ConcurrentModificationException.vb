Imports System

Namespace Exceptions

    ''' <summary>
    ''' Raised when a write loses a race with another client — EF's <c>DbUpdateConcurrencyException</c>
    ''' translated into the operator-facing wording AGENTS.md mandates.
    '''
    ''' AGENTS.md is explicit that the conflict must be surfaced and never silently overwritten, so
    ''' this is a <em>stop</em> signal, not a retry signal: the operator has to refresh and see the
    ''' current state before deciding what to do.
    ''' </summary>
    Public Class ConcurrentModificationException
        Inherits Exception

        ''' <summary>The wording the UI shows. Kept here so tests and presenters share one string.</summary>
        Public Const OperatorMessage As String = "Data changed elsewhere — refresh and retry."

        Public Sub New(Optional detail As String = Nothing)
            MyBase.New(If(String.IsNullOrWhiteSpace(detail), OperatorMessage, OperatorMessage & " " & detail))
        End Sub
    End Class

End Namespace
