Imports System.Threading.Tasks

Namespace Interfaces
    Public Interface IConflictPresenter
        Function PromptAsync() As Task(Of Boolean)
        Function PromptConflict(entityName As String) As ConflictResolution
    End Interface
End Namespace
