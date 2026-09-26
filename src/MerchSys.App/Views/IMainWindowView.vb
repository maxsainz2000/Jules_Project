Imports System.Windows.Forms
Imports MerchSys.App.Models

Namespace Views
    Public Interface IMainWindowView
        Event NavigationRequested As EventHandler(Of NavigationItem)

        Sub ShowView(view As UserControl)
        Sub RenderNavigation(groups As Dictionary(Of String, List(Of NavigationItem)))
    End Interface
End Namespace