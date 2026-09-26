Imports System.Collections.Generic

Namespace Paging

    Public Class PagedResult(Of T)
        Public Property Items As List(Of T) = New List(Of T)()
        Public Property HasMore As Boolean
        Public Property NextCursorDate As DateTime?
        Public Property NextCursorId As Integer?
    End Class

End Namespace
