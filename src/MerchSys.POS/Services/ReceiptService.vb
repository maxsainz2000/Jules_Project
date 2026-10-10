Imports System.Text
Imports System.Text.Json
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.POS.Data
Imports MerchSys.POS.Entities
Imports MerchSys.POS.Services.ReceiptFormatting

Namespace Services

    ''' <summary>
    ''' Core service for handling receipt generation and formatting.
    ''' Interacts with POSDbContext for persistence and IReceiptIntegrityService for sequential receipt numbering.
    ''' </summary>
    Public Class ReceiptService
        Implements IReceiptService

        Private ReadOnly _dbContext As POSDbContext
        Private ReadOnly _receiptIntegrity As IReceiptIntegrityService
        Private ReadOnly _bodyComposer As IReceiptBodyComposer

        ''' <summary>
        ''' Initializes a new instance of the ReceiptService.
        ''' </summary>
        Public Sub New(dbContext As POSDbContext, receiptIntegrity As IReceiptIntegrityService, bodyComposer As IReceiptBodyComposer)
            _dbContext = dbContext
            _receiptIntegrity = receiptIntegrity
            _bodyComposer = bodyComposer
        End Sub

        ''' <summary>
        ''' Generates an official receipt for a given sales transaction.
        ''' Uses IReceiptIntegrityService to fetch the next contiguous receipt number safely.
        ''' </summary>
        Public Async Function GenerateReceiptAsync(transactionId As Integer) As Task(Of OfficialReceipt) Implements IReceiptService.GenerateReceiptAsync
            Dim transaction = Await _dbContext.SalesTransactions _
                .Include(Function(t) t.Lines) _
                .Include(Function(t) t.Receipt) _
                .FirstOrDefaultAsync(Function(t) t.Id = transactionId)

            If transaction Is Nothing Then
                Throw New ArgumentException($"Transaction with ID {transactionId} not found.")
            End If

            If transaction.Receipt IsNot Nothing Then
                Throw New InvalidOperationException("Transaction already has a receipt.")
            End If

            Dim currentYear = DateTime.UtcNow.Year

            ' Delegate sequential number generation to integrity service
            Dim receiptNumber = Await _receiptIntegrity.GetNextReceiptNumberAsync(currentYear)

            Dim receipt = New OfficialReceipt With {
                .ReceiptNumber = receiptNumber,
                .SerializedLineItems = JsonSerializer.Serialize(transaction.Lines.Select(Function(l) New With { l.ProductName, l.UnitPrice, l.Quantity, l.LineTotal }))
            }

            transaction.Receipt = receipt
            _dbContext.OfficialReceipts.Add(receipt)
            Await _dbContext.SaveChangesAsync()

            Return receipt
        End Function

        Public Async Function GetReceiptAsync(receiptId As Integer) As Task(Of OfficialReceipt) Implements IReceiptService.GetReceiptAsync
            Return Await _dbContext.OfficialReceipts.FirstOrDefaultAsync(Function(r) r.Id = receiptId)
        End Function

        Public Async Function GetReceiptByTransactionAsync(transactionId As Integer) As Task(Of OfficialReceipt) Implements IReceiptService.GetReceiptByTransactionAsync
            Dim transaction = Await _dbContext.SalesTransactions _
                .Include(Function(t) t.Receipt) _
                .FirstOrDefaultAsync(Function(t) t.Id = transactionId)

            Return transaction?.Receipt
        End Function

        Public Async Function PrintReceiptAsync(receiptId As Integer) As Task(Of String) Implements IReceiptService.PrintReceiptAsync
            ' Get receipt with transaction and lines
            Dim transaction = Await _dbContext.SalesTransactions _
                .Include(Function(t) t.Receipt) _
                .Include(Function(t) t.Lines) _
                .FirstOrDefaultAsync(Function(t) t.Receipt.Id = receiptId)

            If transaction Is Nothing Then
                Throw New ArgumentException($"Receipt with ID {receiptId} not found or has no associated transaction.")
            End If

            Dim vatConfig = Await _dbContext.VatConfigurations.FirstOrDefaultAsync(Function(c) c.Id = 1)
            If vatConfig Is Nothing Then
                vatConfig = New VatConfiguration() With {
                    .IsVatRegistered = False,
                    .VatRate = 0.12D,
                    .BusinessName = "VISTA POS MERCHANDISING",
                    .Tin = "123-456-789-000"
                }
            End If

            Dim body = _bodyComposer.Compose(transaction.Receipt, transaction, vatConfig)
            Return body.ToString()
        End Function

    End Class

End Namespace
