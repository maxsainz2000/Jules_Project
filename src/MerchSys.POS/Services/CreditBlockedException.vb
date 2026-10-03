Imports System

Namespace Services

    ' Exception thrown when an account is blocked from further credit
    Public Class CreditBlockedException
        Inherits Exception

        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(message As String, innerException As Exception)
            MyBase.New(message, innerException)
        End Sub

    End Class

End Namespace
