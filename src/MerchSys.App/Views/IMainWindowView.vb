Imports System.Windows.Forms
Imports MerchSys.App.Models

Namespace Views
    Public Interface IMainWindowView
        Sub RenderView(view As UserControl)
        Sub AddNavigationButtons(items As List(Of NavigationItem))

        ''' <summary>Hosts the connection status indicator in the shell.</summary>
        Sub AddStatusIndicator(indicator As Control)

        ''' <summary>Closes the shell window (used on logout).</summary>
        Sub CloseWindow()

        ''' <summary>Sidebar header text showing the signed-in user and role.</summary>
        Property UserIdentityText As String

        Event NavigationRequested As EventHandler(Of NavigationItem)
        Event Load As EventHandler
        Event LogoutRequested As EventHandler
        Event ThemeToggleRequested As EventHandler

        Sub ApplyThemeTokens(tokens As MerchSys.App.Services.Theming.ThemeTokens)
        Sub SetThemeState(theme As MerchSys.App.Services.Theming.AppTheme)
    End Interface
End Namespace
