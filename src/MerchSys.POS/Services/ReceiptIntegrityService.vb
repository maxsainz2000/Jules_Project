Imports System
Imports System.Data.Common
Imports System.Security.Cryptography
Imports System.Text
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Storage
Imports Microsoft.Extensions.Logging
Imports MediatR
Imports MerchSys.POS.Data
Imports MerchSys.POS.Entities
Imports MerchSys.SharedKernel.Data
Imports MerchSys.SharedKernel.Events

Namespace Services
    Public Class ReceiptIntegrityService
        Implements IReceiptIntegrityService

        Private ReadOnly _db As POSDbContext
        Private ReadOnly _mediator As IMediator
        Private ReadOnly _logger As ILogger(Of ReceiptIntegrityService)
        Private Const GenesisHash As String = "0000000000000000000000000000000000000000000000000000000000000000"

        Public Sub New(db As POSDbContext, mediator As IMediator, logger As ILogger(Of ReceiptIntegrityService))
            _db = db
            _mediator = mediator
            _logger = logger
        End Sub

        Private Async Function BuildCanonicalStringAsync(receiptId As Integer, receiptNumber As String) As Task(Of String)
            Dim salesTransaction = Await _db.SalesTransactions.Include(Function(st) st.Lines).FirstOrDefaultAsync(Function(st) st.Receipt.Id = receiptId)

            Dim linesStr = ""
            If salesTransaction IsNot Nothing AndAlso salesTransaction.Lines IsNot Nothing Then
                Dim sortedLines = salesTransaction.Lines.OrderBy(Function(l) l.ProductId).ToList()
                Dim linesArray = sortedLines.Select(Function(l) $"{l.ProductId}:{l.Quantity}:{l.LineTotal}").ToArray()
                linesStr = String.Join(",", linesArray)
            End If

            Return $"{receiptId}|{receiptNumber}|{linesStr}"
        End Function

        Public Async Function ComputeAndPersistAsync(receiptId As Integer) As Task(Of ReceiptIntegrity) Implements IReceiptIntegrityService.ComputeAndPersistAsync
            Dim receipt = Await _db.OfficialReceipts.FirstOrDefaultAsync(Function(r) r.Id = receiptId)
            If receipt Is Nothing Then
                Throw New ArgumentException($"Receipt with ID {receiptId} not found.")
            End If

            Using transaction = Await _db.Database.BeginTransactionAsync()
                ' Serialize the append. Reading the tail and then writing a row that points at it is a
                ' read-modify-write: two clients issuing a receipt at the same moment read the same
                ' PreviousHash and the chain forks — each row validates on its own, and
                ' ValidateChainAsync then reports a break.
                Await LockReceiptSequenceAsync(receipt.CreatedAt.Year)

                ' The chain follows APPEND order — this table's own identity column, not ReceiptId.
                ' They are two different sequences: a receipt is created before its integrity row is
                ' appended, so two clients can append in the opposite order to their ReceiptIds.
                ' Chaining on ReceiptId is what let the chain fork even once appends were serialized.
                Dim lastIntegrity = Await _db.ReceiptIntegrities.OrderByDescending(Function(ri) ri.Id).FirstOrDefaultAsync()
                Dim prevHash = If(lastIntegrity IsNot Nothing, lastIntegrity.IntegrityHash, GenesisHash)

                Dim canonicalStr = Await BuildCanonicalStringAsync(receipt.Id, receipt.ReceiptNumber)
                Dim computedHash = ComputeSha256(canonicalStr + prevHash)

                Dim integrity As New ReceiptIntegrity With {
                    .ReceiptId = receipt.Id,
                    .IntegrityHash = computedHash,
                    .PreviousHash = prevHash,
                    .RetentionExpiresAt = receipt.CreatedAt.AddYears(10),
                    .CanonicalPayload = canonicalStr,
                    .IsImmutable = True,
                    .HashAlgorithm = "SHA-256-v1"
                }

                _db.ReceiptIntegrities.Add(integrity)
                Await _db.SaveChangesAsync()
                Await transaction.CommitAsync()

                Return integrity
            End Using
        End Function

        ''' <summary>
        ''' Locks the receipt-sequence row for a year, which serializes receipt issuance for that year.
        '''
        ''' That row is the right lock: the receipt number was issued from it moments earlier, so it
        ''' exists, and every receipt of the year contends on it. Locking "the last integrity row"
        ''' instead would have no row to lock when the chain is empty — which is exactly when the
        ''' first two receipts race.
        '''
        ''' The lock is per year while the chain is global, so two receipts issued in different
        ''' calendar years within the same instant are not serialized against each other — a residual
        ''' narrower than the defect it replaces.
        '''
        ''' Same raw-ADO approach as the inventory batch lock: EF cannot express FOR UPDATE, and the
        ''' connection belongs to MySql.Data (casting it to MySqlConnector throws).
        ''' </summary>
        Private Async Function LockReceiptSequenceAsync(year As Integer) As Task
            Dim connection = _db.Database.GetDbConnection()
            Dim currentTransaction = _db.Database.CurrentTransaction
            Dim tableName = _db.Model.FindEntityType(GetType(ReceiptSequence)).GetTableName()

            Using command = connection.CreateCommand()
                If currentTransaction IsNot Nothing Then
                    command.Transaction = currentTransaction.GetDbTransaction()
                End If

                command.CommandText = $"SELECT Id FROM `{tableName}` WHERE Year = @year FOR UPDATE"
                command.AddParameter("@year", year)

                Using reader = Await command.ExecuteReaderAsync()
                    While Await reader.ReadAsync()
                        ' Reading the row is what acquires the lock.
                    End While
                End Using
            End Using
        End Function

        Public Async Function ValidateAsync(receiptId As Integer) As Task(Of IntegrityValidationResult) Implements IReceiptIntegrityService.ValidateAsync
            Dim integrity = Await _db.ReceiptIntegrities.Include(Function(ri) ri.OfficialReceipt).FirstOrDefaultAsync(Function(ri) ri.ReceiptId = receiptId)
            If integrity Is Nothing Then
                Return New IntegrityValidationResult With {.IsValid = False, .ErrorMessage = "Integrity record not found."}
            End If

            Dim receipt = integrity.OfficialReceipt
            Dim canonicalStr = Await BuildCanonicalStringAsync(receipt.Id, receipt.ReceiptNumber)
            Dim expectedHash = ComputeSha256(canonicalStr + integrity.PreviousHash)

            If expectedHash <> integrity.IntegrityHash Then
                _logger.LogWarning($"Tamper detected for receipt {receipt.ReceiptNumber}.")

                Dim tamperEvent As New ReceiptTamperDetectedEvent With {
                    .ReceiptId = Guid.NewGuid(),
                    .ReceiptNumber = receipt.ReceiptNumber,
                    .ExpectedHash = integrity.IntegrityHash,
                    .ActualHash = expectedHash,
                    .DetectedAt = DateTime.UtcNow,
                    .DetectedBy = "System"
                }
                Await _mediator.Publish(tamperEvent)

                Return New IntegrityValidationResult With {
                    .IsValid = False,
                    .ExpectedHash = integrity.IntegrityHash,
                    .ActualHash = expectedHash,
                    .ErrorMessage = "Hash mismatch detected."
                }
            End If

            Return New IntegrityValidationResult With {
                .IsValid = True,
                .ExpectedHash = integrity.IntegrityHash,
                .ActualHash = expectedHash
            }
        End Function

        Public Async Function ValidateChainAsync() As Task(Of ChainValidationResult) Implements IReceiptIntegrityService.ValidateChainAsync
            ' Walk in APPEND order (this table's own Id), matching how ComputeAndPersistAsync builds
            ' the chain. Walking ReceiptId instead traverses a different sequence and reports a break
            ' for a chain that is in fact intact.
            Dim integrities = Await _db.ReceiptIntegrities.Include(Function(ri) ri.OfficialReceipt).OrderBy(Function(ri) ri.Id).ToListAsync()
            Dim currentPrevHash = GenesisHash
            Dim verifiedCount = 0

            For Each item In integrities
                If item.PreviousHash <> currentPrevHash Then
                    Return New ChainValidationResult With {
                        .IsValid = False,
                        .BrokenAtReceiptNumber = item.OfficialReceipt?.ReceiptNumber,
                        .ErrorMessage = "Previous hash mismatch in chain.",
                        .TotalVerified = verifiedCount
                    }
                End If

                Dim receipt = item.OfficialReceipt
                Dim canonicalStr = Await BuildCanonicalStringAsync(receipt.Id, receipt.ReceiptNumber)
                Dim expectedHash = ComputeSha256(canonicalStr + item.PreviousHash)

                If expectedHash <> item.IntegrityHash Then
                    Return New ChainValidationResult With {
                        .IsValid = False,
                        .BrokenAtReceiptNumber = receipt.ReceiptNumber,
                        .ErrorMessage = "Hash mismatch in chain.",
                        .TotalVerified = verifiedCount
                    }
                End If

                currentPrevHash = item.IntegrityHash
                verifiedCount += 1
            Next

            Return New ChainValidationResult With {.IsValid = True, .TotalVerified = verifiedCount}
        End Function

        Public Async Function GetNextReceiptNumberAsync(year As Integer) As Task(Of String) Implements IReceiptIntegrityService.GetNextReceiptNumberAsync
            Const maxAttempts As Integer = 10

            For attempt = 1 To maxAttempts
                Dim failure As Exception = Nothing
                Try
                    Dim seq = Await _db.ReceiptSequences.FirstOrDefaultAsync(Function(rs) rs.Year = year)
                    If seq Is Nothing Then
                        seq = New ReceiptSequence With {.Year = year, .LastSequenceNumber = 0}
                        _db.ReceiptSequences.Add(seq)
                    End If

                    seq.LastSequenceNumber += 1
                    Await _db.SaveChangesAsync()

                    Return $"OR-{year}-{seq.LastSequenceNumber:D4}"
                Catch ex As DbUpdateConcurrencyException
                    failure = ex
                Catch ex As DbUpdateException When DuplicateKeyDetector.IsDuplicateKey(ex)
                    ' Two clients issued the first receipt of the year at the same moment: both found
                    ' no sequence row and both tried to insert one. The unique index on Year rejects
                    ' the loser — and that used to escape unhandled, failing receipt generation
                    ' outright rather than retrying.
                    failure = ex
                End Try

                ' Discard the rejected insert, or the retry would repeat exactly the same failing
                ' insert for ever.
                DetachAddedReceiptSequences()

                Dim concurrencyFailure = TryCast(failure, DbUpdateConcurrencyException)
                If concurrencyFailure IsNot Nothing Then
                    For Each dbEntry In concurrencyFailure.Entries
                        Await dbEntry.ReloadAsync()
                    Next
                End If
            Next

            Throw New Exception("Failed to generate next receipt number due to high concurrency.")
        End Function

        ''' <summary>
        ''' Discards receipt-sequence rows that exist only as a failed insert, so a retry starts from
        ''' the persisted state rather than repeating the same rejected insert.
        ''' </summary>
        Private Sub DetachAddedReceiptSequences()
            For Each dbEntry In _db.ChangeTracker.Entries(Of ReceiptSequence)().ToList()
                If dbEntry.State = EntityState.Added Then
                    dbEntry.State = EntityState.Detached
                End If
            Next
        End Sub

        Private Function ComputeSha256(input As String) As String
            Using sha256 As SHA256 = SHA256.Create()
                Dim bytes = Encoding.UTF8.GetBytes(input)
                Dim hash = sha256.ComputeHash(bytes)
                Dim builder As New StringBuilder()
                For Each b In hash
                    builder.Append(b.ToString("x2"))
                Next
                Return builder.ToString()
            End Using
        End Function
    End Class
End Namespace