Imports MerchSys.Purchasing.Services
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.Purchasing.Views.Purchasing
Imports MerchSys.Purchasing.Views.Dialogs
Imports MerchSys.Purchasing.Entities
Imports Microsoft.Extensions.DependencyInjection
Imports System.Windows.Forms

Namespace Presenters
    Public Class ReorderSuggestionsPresenter
        Private ReadOnly _reorderService As IReorderService
        Private ReadOnly _sessionService As ISessionService
        Private ReadOnly _serviceProvider As IServiceProvider

        Private _view As IReorderSuggestionsView

        Public Property View As IReorderSuggestionsView
            Get
                Return _view
            End Get
            Set(value As IReorderSuggestionsView)
                _view = value
                If _view IsNot Nothing Then
                    WireViewEvents()
                End If
            End Set
        End Property

        Public Sub New(reorderService As IReorderService, sessionService As ISessionService, serviceProvider As IServiceProvider)
            _reorderService = reorderService
            _sessionService = sessionService
            _serviceProvider = serviceProvider
        End Sub

        Private Sub WireViewEvents()
            AddHandler _view.LoadSuggestions, AddressOf OnLoadSuggestions
            AddHandler _view.GenerateSuggestionsClicked, AddressOf OnGenerateSuggestionsClicked
            AddHandler _view.AcceptSuggestionClicked, AddressOf OnAcceptSuggestionClicked
            AddHandler _view.DismissSuggestionClicked, AddressOf OnDismissSuggestionClicked
            AddHandler _view.LoadConfigurations, AddressOf OnLoadConfigurations
            AddHandler _view.EditConfigurationClicked, AddressOf OnEditConfigurationClicked
        End Sub

        Public Async Sub OnLoadSuggestions(sender As Object, e As EventArgs)
            Try
                Dim suggestions = Await _reorderService.GetAllSuggestionsAsync()
                _view.BindSuggestions(suggestions)
            Catch ex As Exception
                _view.ShowError(ex.Message)
            End Try
        End Sub

        Public Async Sub OnGenerateSuggestionsClicked(sender As Object, e As EventArgs)
            If Not HasWriteAccess() Then
                _view.ShowError("You do not have permission to generate suggestions.")
                Return
            End If

            Try
                Await _reorderService.GenerateSuggestionsAsync()
                OnLoadSuggestions(Me, EventArgs.Empty)
                _view.ShowMessage("Suggestions generated successfully.")
            Catch ex As Exception
                _view.ShowError(ex.Message)
            End Try
        End Sub

        Public Async Sub OnAcceptSuggestionClicked(suggestionId As Integer, quantity As Integer)
            If Not HasWriteAccess() Then
                _view.ShowError("You do not have permission to accept suggestions.")
                Return
            End If

            Try
                Await _reorderService.AcceptSuggestionAsync(suggestionId, quantity)
                OnLoadSuggestions(Me, EventArgs.Empty)
                _view.ShowMessage("Suggestion accepted.")
            Catch ex As Exception
                _view.ShowError(ex.Message)
            End Try
        End Sub

        Public Async Sub OnDismissSuggestionClicked(suggestionId As Integer, reason As String)
            If Not HasWriteAccess() Then
                _view.ShowError("You do not have permission to dismiss suggestions.")
                Return
            End If

            Try
                Await _reorderService.DismissSuggestionAsync(suggestionId, reason)
                OnLoadSuggestions(Me, EventArgs.Empty)
                _view.ShowMessage("Suggestion dismissed.")
            Catch ex As Exception
                _view.ShowError(ex.Message)
            End Try
        End Sub

        Public Async Sub OnLoadConfigurations(sender As Object, e As EventArgs)
            Try
                Dim configs = Await _reorderService.GetAllConfigsAsync()
                _view.BindConfigurations(configs)
            Catch ex As Exception
                _view.ShowError(ex.Message)
            End Try
        End Sub

        Public Async Sub OnEditConfigurationClicked(config As ReorderConfig)
            If Not HasWriteAccess() Then
                _view.ShowError("You do not have permission to edit configurations.")
                Return
            End If

            Dim dialog = _serviceProvider.GetRequiredService(Of IReorderConfigEditorDialog)()
            dialog.SetConfig(config)

            If dialog.ShowDialog() = DialogResult.OK Then
                Try
                    Dim updatedConfig = dialog.GetConfig()
                    Await _reorderService.UpdateConfigAsync(updatedConfig)
                    OnLoadConfigurations(Me, EventArgs.Empty)
                    _view.ShowMessage("Configuration updated successfully.")
                Catch ex As Exception
                    _view.ShowError(ex.Message)
                End Try
            End If
        End Sub

        Private Function HasWriteAccess() As Boolean
            Return _sessionService.CurrentUserRole = UserRole.Manager OrElse _sessionService.CurrentUserRole = UserRole.Developer
        End Function
    End Class
End Namespace
