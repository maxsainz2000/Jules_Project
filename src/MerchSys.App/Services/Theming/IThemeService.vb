Imports System.Windows.Forms

Namespace Services.Theming
    Public Interface IThemeService
        ReadOnly Property CurrentTheme As AppTheme
        ReadOnly Property Tokens As ThemeTokens

        Event ThemeChanged As EventHandler(Of AppTheme)

        Sub SetTheme(theme As AppTheme)
        Sub ToggleTheme()
        Sub ApplyTheme(root As Control)
    End Interface
End Namespace
