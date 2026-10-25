Imports System
Imports System.Threading
Imports System.Threading.Tasks
Imports MediatR
Imports Microsoft.Extensions.Logging
Imports MerchSys.SharedKernel.Events
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities

Namespace Handlers

    Public Class ReceiptTamperDetectedHandler
        Implements INotificationHandler(Of ReceiptTamperDetectedEvent)

        Private ReadOnly _dbContext As AccountingDbContext
        Private ReadOnly _logger As ILogger(Of ReceiptTamperDetectedHandler)

        Public Sub New(dbContext As AccountingDbContext, logger As ILogger(Of ReceiptTamperDetectedHandler))
            _dbContext = dbContext
            _logger = logger
        End Sub

        Public Async Function Handle(notification As ReceiptTamperDetectedEvent, cancellationToken As CancellationToken) As Task Implements INotificationHandler(Of ReceiptTamperDetectedEvent).Handle
            Dim caughtException As Exception = Nothing

            Try
                Dim entry As New TamperAuditEntry() With {
                    .ReceiptId = notification.ReceiptId,
                    .ReceiptNumber = notification.ReceiptNumber,
                    .ExpectedHash = notification.ExpectedHash,
                    .ActualHash = notification.ActualHash,
                    .DetectedAt = notification.DetectedAt,
                    .DetectedBy = notification.DetectedBy,
                    .TamperKind = "HashMismatch",
                    .MachineName = Environment.MachineName,
                    .WindowsUser = Environment.UserName
                }

                _dbContext.TamperAuditEntries.Add(entry)
                Await _dbContext.SaveChangesAsync(cancellationToken)

                _logger.LogInformation("Receipt tamper audit entry recorded for ReceiptId {ReceiptId}.", notification.ReceiptId)
            Catch ex As Exception
                _logger.LogCritical(ex, "Failed to record receipt tamper audit entry for ReceiptId {ReceiptId}.", notification.ReceiptId)
                caughtException = ex
            End Try

            If caughtException IsNot Nothing Then
                Throw New Exception("Failed to record receipt tamper audit entry", caughtException)
            End If
        End Function

    End Class

End Namespace