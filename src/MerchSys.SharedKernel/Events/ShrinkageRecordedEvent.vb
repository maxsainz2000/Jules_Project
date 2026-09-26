Imports MediatR

Namespace Events

    Public Class ShrinkageRecordedEvent
        Implements INotification

        Public Property ProductId As Integer
        Public Property Quantity As Integer
        Public Property TotalValue As Decimal
        Public Property Reason As String

    End Class

End Namespace
