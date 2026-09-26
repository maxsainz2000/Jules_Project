Imports System

Namespace Views

    Public Interface IExpiryMonitorView
        Property NearExpiryCount As Integer
        Property ExpiredCount As Integer
        Property TotalValueAtRisk As Decimal

        Property ThresholdDays As Integer

        Event RefreshRequested As EventHandler
        Event ThresholdChanged As EventHandler
        Event WriteOffRequested As EventHandler(Of ExpiryActionArgs)

        Sub DisplayNearExpiry(batches As Object)
        Sub DisplayExpired(batches As Object)
    End Interface

    Public Class ExpiryActionArgs
        Inherits EventArgs

        Public Property BatchId As Integer
        Public Property ProductId As Integer
        Public Property Quantity As Integer
    End Class

End Namespace
