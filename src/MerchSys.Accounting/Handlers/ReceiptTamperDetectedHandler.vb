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
                Dim entry As New TamperAuditEntry With {
                    .ReceiptId = notification.ReceiptId,
                    .ReceiptNumber = notification.ReceiptNumber,
                    .ExpectedHash = notification.ExpectedHash,
                    .ActualHash = notification.ActualHash,
                    .DetectedAt = notification.DetectedAt,
                    .DetectedBy = notification.DetectedBy,
                    .TamperKind = "HashMismatch",
                    .MachineName = Environment.MachineName,
                    .WindowsUser = System.Security.Principal.WindowsIdentity.GetCurrent()?.Name
                }

                _dbContext.TamperAuditEntries.Add(entry)
                Await _dbContext.SaveChangesAsync(cancellationToken)

            Catch ex As Exception
                caughtException = ex
            End Try

            If caughtException IsNot Nothing Then
                _logger.LogCritical(caughtException, "Failed to record receipt tamper audit entry for Receipt {ReceiptId}.", notification.ReceiptId)
                Throw caughtException
            End If
        End Function

    End Class

End Namespace
