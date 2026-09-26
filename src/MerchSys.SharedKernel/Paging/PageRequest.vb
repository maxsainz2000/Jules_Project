Imports System

Namespace Paging

    Public Class PageRequest
        Public Property PageSize As Integer = 100
        Public Property CursorDate As DateTime?
        Public Property CursorId As Integer?
        Public Property FromUtc As DateTime?
        Public Property ToUtc As DateTime?
        Public Property IsFirstPage As Boolean = True
    End Class

End Namespace
