Namespace Exceptions
    Public Class UnauthorizedWriteException
        Inherits Exception

        Public Sub New()
            MyBase.New("Unauthorized write operation attempted.")
        End Sub

        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(message As String, innerException As Exception)
            MyBase.New(message, innerException)
        End Sub
    End Class
End Namespace
