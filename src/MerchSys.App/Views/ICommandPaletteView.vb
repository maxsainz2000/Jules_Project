Imports MerchSys.App.Models

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
    End Interface
End Namespace
