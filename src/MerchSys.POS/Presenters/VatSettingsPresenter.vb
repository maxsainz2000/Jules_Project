Imports MerchSys.SharedKernel.Persistence
Imports System.Threading.Tasks
Imports MerchSys.POS.Services
Imports MerchSys.POS.Views
Imports MerchSys.SharedKernel.Interfaces

Namespace Presenters
    Public Class VatSettingsPresenter
        Private _view As IVatSettingsView
        Private ReadOnly _conflictPresenter As IConflictPresenter
        Private ReadOnly _writer As IVatConfigurationWriter
        Private ReadOnly _configLoader As VatConfigurationLoader
        Private ReadOnly _notificationService As INotificationService

        Public Property View As IVatSettingsView
            Get
                Return _view
            End Get
            Set(value As IVatSettingsView)
                If _view IsNot Nothing Then
                    RemoveHandler _view.SaveRequested, AddressOf OnSaveRequested
                    RemoveHandler _view.ReloadRequested, AddressOf OnReloadRequested
                    RemoveHandler _view.VatRegistrationChanged, AddressOf OnVatRegistrationChanged
                End If
                _view = value
                If _view IsNot Nothing Then
                    AddHandler _view.SaveRequested, AddressOf OnSaveRequested
                    AddHandler _view.ReloadRequested, AddressOf OnReloadRequested
                    AddHandler _view.VatRegistrationChanged, AddressOf OnVatRegistrationChanged

                    ' Fire and forget initial load
                    Dim dummy = LoadConfigurationAsync()
                End If
            End Set
        End Property

        Public Sub New(writer As IVatConfigurationWriter, configLoader As VatConfigurationLoader, notificationService As INotificationService, Optional conflictPresenter As IConflictPresenter = Nothing)
            _writer = writer
            _configLoader = configLoader
            _notificationService = notificationService
            _conflictPresenter = conflictPresenter
        End Sub

        Public Sub New(writer As IVatConfigurationWriter, configLoader As VatConfigurationLoader, notificationService As INotificationService)
            _writer = writer
            _configLoader = configLoader
            _notificationService = notificationService
        End Sub

        Public Async Function LoadConfigurationAsync() As Task
            Try
                Dim config = Await _configLoader.GetConfigurationAsync()

                _view.IsVatRegistered = config.IsVatRegistered
                _view.VatRatePercent = config.VatRate * 100D
                _view.NonVatRatePercent = config.NonVatPercentageTaxRate * 100D
                _view.BusinessName = config.BusinessName
                _view.BusinessAddress = config.Address
                _view.Tin = config.Tin

                UpdateSaveEnabledState()
            Catch ex As Exception
                _view.ErrorMessage = "Failed to load VAT settings."
                _notificationService.ShowError(ex.Message)
            End Try
        End Function

        Public Async Function SaveAsync() As Task
            Try
                _view.ErrorMessage = String.Empty

                Dim request As New VatConfigurationUpdateRequest() With {
                    .IsVatRegistered = _view.IsVatRegistered,
                    .VatRate = _view.VatRatePercent / 100D,
                    .NonVatPercentageTaxRate = _view.NonVatRatePercent / 100D,
                    .BusinessName = _view.BusinessName,
                    .Address = _view.BusinessAddress,
                    .Tin = _view.Tin
                }

                Dim result As VatConfigurationUpdateResult = Nothing
                Dim success = Await ConcurrencyHelper.ExecuteWithConflictPromptAsync(
                    Async Function()
                        result = Await _writer.UpdateConfigurationAsync(request)
                    End Function, AddressOf LoadConfigurationAsync, _conflictPresenter)

                If success AndAlso result IsNot Nothing Then
                    If result.Success Then
                        _notificationService.ShowSuccess("VAT configuration saved successfully.")
                    Else
                        _view.ErrorMessage = result.ErrorMessage
                        _notificationService.ShowError(result.ErrorMessage)
                    End If
                End If
            Catch ex As Exception
                Dim errMsg As String = ex.Message
                _view.ErrorMessage = "Failed to save VAT settings."
                _notificationService.ShowError(errMsg)
            End Try
        End Function

        Public Async Function ReloadAsync() As Task
            Try
                _configLoader.Invalidate()
                Await LoadConfigurationAsync()
                _notificationService.ShowInfo("Configuration reloaded.")
            Catch ex As Exception
                Dim errMsg As String = ex.Message
                _view.ErrorMessage = "Failed to reload VAT settings."
                _notificationService.ShowError(errMsg)
            End Try
        End Function

        Private Async Sub OnSaveRequested(sender As Object, e As EventArgs)
            Await SaveAsync()
        End Sub

        Private Async Sub OnReloadRequested(sender As Object, e As EventArgs)
            Await ReloadAsync()
        End Sub

        Private Sub OnVatRegistrationChanged(sender As Object, e As EventArgs)
            UpdateSaveEnabledState()
        End Sub

        Private Sub UpdateSaveEnabledState()
            If _view IsNot Nothing Then
                _view.IsSaveEnabled = True
            End If
        End Sub
    End Class
End Namespace
