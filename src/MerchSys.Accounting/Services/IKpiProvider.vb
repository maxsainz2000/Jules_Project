Imports System.Threading

Namespace Services

    Public Enum KpiSeverity
        Info = 0
        Warning = 1
        Critical = 2
    End Enum

    Public Class KpiValue
        Public Property Key As String
        Public Property Amount As Decimal
        Public Property Label As String
        Public Property DueDate As DateTime?
        Public Property Severity As KpiSeverity
    End Class

    Public Interface IKpiProvider
        Function GetKpiAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of KpiValue)
    End Interface

End Namespace
