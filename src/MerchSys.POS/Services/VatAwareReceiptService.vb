Imports System.Threading.Tasks
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.POS.Data
Imports MerchSys.POS.Entities
Imports MerchSys.SharedKernel.Events
Imports MerchSys.SharedKernel.Interfaces

Namespace Services
    Public Class VatAwareReceiptService
        Implements IReceiptService

        Private ReadOnly _inner As ReceiptService
        Private ReadOnly _vatCalculator As IVatCalculator
        Private ReadOnly _configLoader As VatConfigurationLoader
        Private ReadOnly _dbContext As POSDbContext
        Private ReadOnly _receiptIntegrityService As IReceiptIntegrityService
        Private ReadOnly _mediator As IMediator
        Private ReadOnly _eventBus As IEventBus

        Public Sub New(inner As ReceiptService, vatCalculator As IVatCalculator, configLoader As VatConfigurationLoader, dbContext As POSDbContext, receiptIntegrityService As IReceiptIntegrityService, mediator As IMediator, eventBus As IEventBus)
            _inner = inner
            _vatCalculator = vatCalculator
            _configLoader = configLoader
            _dbContext = dbContext
            _receiptIntegrityService = receiptIntegrityService
            _mediator = mediator
            _eventBus = eventBus
        End Sub

        Public Async Function GenerateReceiptAsync(transactionId As Integer) As Task(Of OfficialReceipt) Implements IReceiptService.GenerateReceiptAsync
            Dim transaction = Await _dbContext.SalesTransactions _
                .Include(Function(t) t.Lines) _
                .Include(Function(t) t.Receipt) _
                .FirstOrDefaultAsync(Function(t) t.Id = transactionId)

            If transaction Is Nothing Then
                Throw New ArgumentException($"Transaction with ID {transactionId} not found.")
            End If

            Dim config = Await _configLoader.GetConfigurationAsync()

            ' 3 & 4. Decompose each line and aggregate totals
            Dim totals = _vatCalculator.AggregateTransaction(transaction, config.VatRate, config.IsVatRegistered)

            For Each line In transaction.Lines
                Dim lineVat = _vatCalculator.CalculateLine(line, config.VatRate, config.IsVatRegistered)
                line.VatableAmount = lineVat.VatableAmount
                line.VatExemptAmount = lineVat.VatExemptAmount
                line.ZeroRatedAmount = lineVat.ZeroRatedAmount
                line.OutputVat = lineVat.OutputVat
            Next

            transaction.VatableSales = totals.VatableSales
            transaction.VatExemptSales = totals.VatExemptSales
            transaction.ZeroRatedSales = totals.ZeroRatedSales
            transaction.VatRateSnapshot = config.VatRate
            transaction.IsVatRegisteredSnapshot = config.IsVatRegistered

            ' 5. Await dbContext.SaveChangesAsync()
            Await _dbContext.SaveChangesAsync()

            ' 6. Await inner.GenerateReceiptAsync
            Dim receipt = Await _inner.GenerateReceiptAsync(transactionId)

            ' 7. Await receiptIntegrityService.ComputeAndPersistAsync
            Await _receiptIntegrityService.ComputeAndPersistAsync(receipt.Id)

            ' 8. Construct and publish events
            ' SaleId and the per-item ProductId are load-bearing: the Accounting handler upserts its
            ' RevenueRecord on (SourceTransactionId, ProductId). Without them both defaulted to 0, so
            ' every sale's VAT revenue collapsed onto a single row keyed (0, 0).
            Dim vatEvent = New SaleCompletedWithVatEvent() With {
                .SaleId = transaction.Id,
                .VatableSales = totals.VatableSales,
                .VatExemptSales = totals.VatExemptSales,
                .ZeroRatedSales = totals.ZeroRatedSales,
                .OutputVat = totals.TotalOutputVat,
                .IsVatRegistered = config.IsVatRegistered
            }

            For Each line In transaction.Lines
                vatEvent.Items.Add(New SaleCompletedWithVatEvent.SaleItemWithVat() With {
                    .ProductId = line.ProductId,
                    .Treatment = line.Treatment,
                    .VatableAmount = line.VatableAmount,
                    .VatExemptAmount = line.VatExemptAmount,
                    .ZeroRatedAmount = line.ZeroRatedAmount,
                    .OutputVat = line.OutputVat
                })
            Next

            Await _mediator.Publish(vatEvent)

            Dim legacyEvent = New SaleCompletedEvent() With {
                .SaleId = transaction.Id,
                .SaleDate = transaction.TransactionDate
            }

            For Each line In transaction.Lines
                legacyEvent.Items.Add(New SaleCompletedEvent.SaleCompletedItem() With {
                    .ProductId = line.ProductId,
                    .QuantitySold = line.Quantity,
                    .UnitPrice = line.UnitPrice
                })
            Next

            Await _eventBus.PublishAsync(legacyEvent)

            Return receipt
        End Function

        Public Function GetReceiptAsync(receiptId As Integer) As Task(Of OfficialReceipt) Implements IReceiptService.GetReceiptAsync
            Return _inner.GetReceiptAsync(receiptId)
        End Function

        Public Function GetReceiptByTransactionAsync(transactionId As Integer) As Task(Of OfficialReceipt) Implements IReceiptService.GetReceiptByTransactionAsync
            Return _inner.GetReceiptByTransactionAsync(transactionId)
        End Function

        Public Function PrintReceiptAsync(receiptId As Integer) As Task(Of String) Implements IReceiptService.PrintReceiptAsync
            Return _inner.PrintReceiptAsync(receiptId)
        End Function
    End Class
End Namespace
