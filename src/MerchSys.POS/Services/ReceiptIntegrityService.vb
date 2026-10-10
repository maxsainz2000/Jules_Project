Imports System
Imports System.Security.Cryptography
Imports System.Text
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Logging
Imports MediatR
Imports MerchSys.POS.Data
Imports MerchSys.POS.Entities
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

            Dim lastIntegrity = Await _db.ReceiptIntegrities.OrderByDescending(Function(ri) ri.ReceiptId).FirstOrDefaultAsync()
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

            Return integrity
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
            Dim integrities = Await _db.ReceiptIntegrities.Include(Function(ri) ri.OfficialReceipt).OrderBy(Function(ri) ri.ReceiptId).ToListAsync()
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
            Dim retries = 0
            While retries < 10
                Dim concurrencyException As DbUpdateConcurrencyException = Nothing
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
                    concurrencyException = ex
                End Try

                If concurrencyException IsNot Nothing Then
                    retries += 1
                    For Each dbEntry In concurrencyException.Entries
                        Await dbEntry.ReloadAsync()
                    Next
                End If
            End While

            Throw New Exception("Failed to generate next receipt number due to high concurrency.")
        End Function

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