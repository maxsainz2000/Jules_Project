Imports MerchSys.App.Models
Imports MerchSys.App.Services.Theming

Namespace Views
    Public Interface ICommandPaletteView
        Event SearchQueryChanged As EventHandler(Of String)
        Event ItemSelected As EventHandler(Of CommandPaletteItem)
        Event DismissRequested As EventHandler

        Property IsBusy As Boolean
        Property IsEmpty As Boolean

        Sub SetResults(results As IEnumerable(Of CommandPaletteItem))
        Sub ShowPalette()
        Sub HidePalette()
        Sub ApplyTheme(tokens As ThemeTokens)
    End Interface
End Namespace
