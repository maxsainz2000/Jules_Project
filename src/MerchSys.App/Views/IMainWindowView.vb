Imports System.Windows.Forms
Imports MerchSys.App.Models
Imports MerchSys.App.Services.Theming

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

        Sub ApplyThemeTokens(tokens As ThemeTokens)
        Sub SetThemeState(theme As AppTheme)

        Event ThemeToggleRequested As EventHandler
        Event NavigationRequested As EventHandler(Of NavigationItem)
        Event Load As EventHandler
        Event LogoutRequested As EventHandler
        ReadOnly Property PaletteView As ICommandPaletteView
    End Interface
End Namespace
