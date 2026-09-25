Imports System.Windows.Forms

Namespace Models
    Public Class NavigationItem
        Public Property DisplayName As String
        Public Property TargetViewType As Type
    End Class

    Public Class NavigationGroup
        Public Property GroupName As String
        Public Property Items As New List(Of NavigationItem)
    End Class
End Namespace
