Imports System.Windows.Forms
Imports MerchSys.App.Models

Namespace Views
    Public Interface IMainWindowView
        Sub RenderView(view As UserControl)
        Sub AddNavigationButtons(items As List(Of NavigationItem))
        Event NavigationRequested As EventHandler(Of Type)
        Event Load As EventHandler
        Event LogoutRequested As EventHandler
    End Interface
End Namespace
