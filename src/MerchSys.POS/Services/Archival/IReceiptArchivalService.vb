Imports System.Threading
Imports System.Threading.Tasks

Namespace Services.Archival
    Public Class ReceiptArchivalBatchResult
        Public Property ReceiptsMoved As Integer
        Public Property HadMoreEligible As Boolean
        Public Property LiveRemaining As Integer
        Public Property ArchiveCount As Integer
        Public Property ElapsedMilliseconds As Long
    End Class

    Public Interface IReceiptArchivalService
        Function ArchiveExpiredReceiptsAsync(Optional batchSize As Integer? = Nothing, Optional cancellationToken As CancellationToken = Nothing) As Task(Of ReceiptArchivalBatchResult)
    End Interface
End Namespace
