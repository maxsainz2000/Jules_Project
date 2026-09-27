Imports MerchSys.Purchasing.Entities

Namespace Services
    Public Interface IReorderService
        Function GenerateSuggestionsAsync() As Task
        Function GetPendingSuggestionsAsync() As Task(Of List(Of ReorderSuggestion))
        Function AcceptSuggestionAsync(suggestionId As Integer, quantity As Integer) As Task(Of PurchaseOrder)
        Function DismissSuggestionAsync(suggestionId As Integer, reason As String) As Task
        Function UpdateConfigAsync(config As ReorderConfig) As Task
        Function GetAllConfigsAsync() As Task(Of List(Of ReorderConfig))
        Function GetAllSuggestionsAsync() As Task(Of List(Of ReorderSuggestion))
    End Interface
End Namespace
