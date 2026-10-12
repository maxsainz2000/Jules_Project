Imports System.Windows.Forms

Namespace Models
    Public Class NavigationItem
        Public Property Name As String

        ''' <summary>
        ''' Resolves the view from the DI container. Using a factory (instead of a raw
        ''' <see cref="Type"/>) lets each item resolve its view through the registered
        ''' interface, so the shell never needs the concrete view type to be registered
        ''' and stays decoupled from it.
        ''' </summary>
        Public Property ViewFactory As Func(Of IServiceProvider, UserControl)

        Public Property IsActive As Boolean
        Public Property NavigationGroup As String
    End Class

    Public Class NavigationGroup
        Public Property GroupName As String
        Public Property Items As New List(Of NavigationItem)
    End Class
End Namespace
