Imports System

Namespace Exceptions
    Public Class ImmutableEntityException
        Inherits Exception

        Public Sub New(entityType As String, entityKey As String)
            MyBase.New($"Entity of type '{entityType}' with key '{entityKey}' is immutable under NIRC §235 and cannot be modified or deleted.")
        End Sub
    End Class
End Namespace