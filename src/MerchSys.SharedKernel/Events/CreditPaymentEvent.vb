Imports MediatR

Namespace Events

    Public Class CreditPaymentEvent
        Implements INotification

        Public Property CreditAccountId As Integer
        Public Property Amount As Decimal

        Public Sub New(creditAccountId As Integer, amount As Decimal)
            Me.CreditAccountId = creditAccountId
            Me.Amount = amount
        End Sub

    End Class

End Namespace
