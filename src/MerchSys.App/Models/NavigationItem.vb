Imports System.Windows.Forms

Namespace Models
    Public Class NavigationItem
        Public Property Name As String
        Public Property ViewType As Type
        Public Property IsActive As Boolean
        Public Property NavigationGroup As String
    End Class

    Public Class NavigationGroup
        Public Property GroupName As String
        Public Property Items As New List(Of NavigationItem)
    End Class
End Namespace
