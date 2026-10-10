Imports MediatR

Namespace Events
    Public Class VatConfigurationChangedEvent
        Implements INotification

        Public Property OccurredAt As DateTime
        Public Property IsVatRegistered As Boolean
        Public Property PreviousIsVatRegistered As Boolean
        Public Property VatRate As Decimal

        Public Sub New()
            OccurredAt = DateTime.UtcNow
        End Sub

        Public Sub New(p_isVatRegistered As Boolean, p_previousIsVatRegistered As Boolean, p_vatRate As Decimal)
            Me.OccurredAt = DateTime.UtcNow
            Me.IsVatRegistered = p_isVatRegistered
            Me.PreviousIsVatRegistered = p_previousIsVatRegistered
            Me.VatRate = p_vatRate
        End Sub
    End Class
End Namespace
