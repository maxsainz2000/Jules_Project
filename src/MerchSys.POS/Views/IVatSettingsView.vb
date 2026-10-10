Imports System

Namespace Views
    Public Interface IVatSettingsView
        Property IsVatRegistered As Boolean
        Property VatRatePercent As Decimal
        Property NonVatRatePercent As Decimal
        Property BusinessName As String
        Property BusinessAddress As String
        Property Tin As String
        Property ErrorMessage As String
        Property IsSaveEnabled As Boolean

        Event SaveRequested As EventHandler
        Event ReloadRequested As EventHandler
        Event VatRegistrationChanged As EventHandler
    End Interface
End Namespace
