Imports System

Namespace Exceptions
    Public Class VatReturnLockedException
        Inherits InvalidOperationException

        Public Property ReturnId As Integer
        Public Property Year As Integer
        Public Property Period As Integer
        Public Property FormType As Enums.VatReturnFormType

        Public Sub New(message As String, returnId As Integer, year As Integer, period As Integer, formType As Enums.VatReturnFormType)
            MyBase.New(message)
            Me.ReturnId = returnId
            Me.Year = year
            Me.Period = period
            Me.FormType = formType
        End Sub
    End Class
End Namespace
