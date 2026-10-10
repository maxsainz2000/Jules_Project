Imports System
Imports System.Diagnostics
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting
Imports Microsoft.Extensions.Logging
Imports Microsoft.Extensions.Options
Imports MerchSys.POS.Data
Imports MerchSys.POS.Entities
Imports MerchSys.POS.Data.Interceptors

Namespace Services.Archival
    Public Class ReceiptArchivalService
        Inherits BackgroundService
        Implements IReceiptArchivalService

        Private ReadOnly _serviceScopeFactory As IServiceScopeFactory
        Private ReadOnly _options As IOptions(Of ReceiptArchivalOptions)
        Private ReadOnly _logger As ILogger(Of ReceiptArchivalService)

        Public Sub New(serviceScopeFactory As IServiceScopeFactory, options As IOptions(Of ReceiptArchivalOptions), logger As ILogger(Of ReceiptArchivalService))
            _serviceScopeFactory = serviceScopeFactory
            _options = options
            _logger = logger
        End Sub

        Protected Overrides Async Function ExecuteAsync(stoppingToken As CancellationToken) As Task
            While Not stoppingToken.IsCancellationRequested
                If _options.Value.Enabled Then
                    Try
                        Await ArchiveExpiredReceiptsAsync(Nothing, stoppingToken)
                    Catch ex As Exception
                        _logger.LogError(ex, "Error occurred while archiving receipts")
                    End Try
                End If
                Await Task.Delay(TimeSpan.FromHours(_options.Value.IntervalHours), stoppingToken)
            End While
        End Function

        Public Async Function ArchiveExpiredReceiptsAsync(Optional batchSize As Integer? = Nothing, Optional cancellationToken As CancellationToken = Nothing) As Task(Of ReceiptArchivalBatchResult) Implements IReceiptArchivalService.ArchiveExpiredReceiptsAsync
            Dim sw = Stopwatch.StartNew()
            Dim actualBatchSize = If(batchSize, _options.Value.BatchSize)
            Dim currentYear = DateTime.UtcNow.Year
            Dim utcNow = DateTime.UtcNow

            Dim result As New ReceiptArchivalBatchResult()

            Using scope = _serviceScopeFactory.CreateScope()
                Dim dbContext = scope.ServiceProvider.GetRequiredService(Of POSDbContext)()

                ' Get eligible records (RetentionExpiresAt <= UtcNow AND receipt year < currentYear)
                Dim eligibleQuery = dbContext.ReceiptIntegrities.
                    Include(Function(ri) ri.OfficialReceipt).
                    Where(Function(ri) ri.RetentionExpiresAt <= utcNow AndAlso ri.CreatedAt.Year < currentYear)

                Dim totalEligible = Await eligibleQuery.CountAsync(cancellationToken)

                If totalEligible = 0 Then
                    result.ElapsedMilliseconds = sw.ElapsedMilliseconds
                    result.LiveRemaining = Await dbContext.OfficialReceipts.CountAsync(cancellationToken)
                    result.ArchiveCount = Await dbContext.OfficialReceiptArchives.CountAsync(cancellationToken)
                    Return result
                End If

                result.HadMoreEligible = totalEligible > actualBatchSize

                Dim toArchive = Await eligibleQuery.
                    Take(actualBatchSize).
                    ToListAsync(cancellationToken)

                Dim commitError As Exception = Nothing
                Using transaction = Await dbContext.Database.BeginTransactionAsync(cancellationToken)
                    Try
                        ImmutableReceiptInterceptor.ArchivalBypass.Value = True

                        For Each item In toArchive
                            Dim orArchive As New OfficialReceiptArchive With {
                                .OriginalReceiptId = item.OfficialReceipt.Id,
                                .ReceiptNumber = item.OfficialReceipt.ReceiptNumber,
                                .ReceiptDate = item.OfficialReceipt.CreatedAt,
                                .SerializedLines = item.OfficialReceipt.SerializedLineItems,
                                .ArchivedAt = utcNow,
                                .ArchivedHash = item.IntegrityHash,
                                .TransactionId = 0 ' Fallback if not available
                            }

                            Dim riArchive As New ReceiptIntegrityArchive With {
                                .ReceiptId = item.ReceiptId,
                                .ReceiptNumber = item.OfficialReceipt.ReceiptNumber,
                                .IntegrityHash = item.IntegrityHash,
                                .PreviousHash = item.PreviousHash,
                                .RetentionExpiresAt = item.RetentionExpiresAt,
                                .CanonicalPayload = item.CanonicalPayload,
                                .HashAlgorithm = item.HashAlgorithm,
                                .ArchivedAt = utcNow,
                                .ArchivedByService = "ReceiptArchivalService"
                            }

                            dbContext.OfficialReceiptArchives.Add(orArchive)
                            dbContext.ReceiptIntegrityArchives.Add(riArchive)

                            dbContext.OfficialReceipts.Remove(item.OfficialReceipt)
                            dbContext.ReceiptIntegrities.Remove(item)

                            result.ReceiptsMoved += 1
                        Next

                        Await dbContext.SaveChangesAsync(cancellationToken)
                        Await transaction.CommitAsync(cancellationToken)
                    Catch ex As Exception
                        commitError = ex
                    Finally
                        ImmutableReceiptInterceptor.ArchivalBypass.Value = False
                    End Try

                    If commitError IsNot Nothing Then
                        Await transaction.RollbackAsync(cancellationToken)
                        Throw commitError
                    End If
                End Using

                sw.Stop()
                result.ElapsedMilliseconds = sw.ElapsedMilliseconds
                result.LiveRemaining = Await dbContext.OfficialReceipts.CountAsync(cancellationToken)
                result.ArchiveCount = Await dbContext.OfficialReceiptArchives.CountAsync(cancellationToken)

            End Using

            Return result
        End Function
    End Class
End Namespace
