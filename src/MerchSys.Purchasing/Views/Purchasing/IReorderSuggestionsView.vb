Imports MerchSys.Purchasing.Entities

Namespace Views.Purchasing
    Public Interface IReorderSuggestionsView
        Event LoadSuggestions As EventHandler
        Event GenerateSuggestionsClicked As EventHandler
        Event AcceptSuggestionClicked As Action(Of Integer, Integer)
        Event DismissSuggestionClicked As Action(Of Integer, String)

        Event LoadConfigurations As EventHandler
        Event EditConfigurationClicked As Action(Of ReorderConfig)

        Sub BindSuggestions(suggestions As List(Of ReorderSuggestion))
        Sub BindConfigurations(configs As List(Of ReorderConfig))

        Sub ShowMessage(message As String)
        Sub ShowError(message As String)
    End Interface
End Namespace
