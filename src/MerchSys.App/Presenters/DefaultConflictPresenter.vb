Imports System.Threading.Tasks
Imports MerchSys.App.Services.Theming
Imports MerchSys.App.Views.Dialogs
Imports System.Windows.Forms
Imports MerchSys.SharedKernel.Interfaces

Namespace Presenters
    Public Class DefaultConflictPresenter
        Implements IConflictPresenter

        Private ReadOnly _themeService As IThemeService

        Public Sub New(themeService As IThemeService)
            _themeService = themeService
        End Sub

        Public Function PromptConflict(entityName As String) As ConflictResolution Implements IConflictPresenter.PromptConflict
            Using dialog As New ConcurrencyConflictPrompt(entityName)
                _themeService.ApplyTheme(dialog)
                Dim result = dialog.ShowDialog()
                If result = DialogResult.Retry OrElse result = DialogResult.OK Then
                    Return ConflictResolution.Refresh
                Else
                    Return ConflictResolution.Cancel
                End If
            End Using
        End Function

        Public Function PromptAsync() As Task(Of Boolean) Implements IConflictPresenter.PromptAsync
            Dim result = PromptConflict("the data")
            Return Task.FromResult(result = ConflictResolution.Refresh)
        End Function
    End Class
End Namespace
