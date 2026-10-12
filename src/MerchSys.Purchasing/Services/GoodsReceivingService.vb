Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports MediatR
Imports MerchSys.Purchasing.Data
Imports MerchSys.Purchasing.Dtos
Imports MerchSys.Purchasing.Entities
Imports MerchSys.Purchasing.Helpers
Imports MerchSys.Purchasing.Services.Vat
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Events

Namespace Services

    Public Class GoodsReceivingService
        Implements IGoodsReceivingService

        Private ReadOnly _db As PurchasingDbContext
        Private ReadOnly _mediator As IMediator
        Private ReadOnly _priceChangeService As IPriceChangeService
        Private ReadOnly _vatCalculator As GoodsReceiptVatCalculator

        Public Sub New(db As PurchasingDbContext, mediator As IMediator, priceChangeService As IPriceChangeService, vatCalculator As GoodsReceiptVatCalculator)
            _db = db
            _mediator = mediator
            _priceChangeService = priceChangeService
            _vatCalculator = vatCalculator
        End Sub

        Public Async Function ReceiveGoodsAsync(dto As ReceiveGoodsDto) As Task(Of GoodsReceipt) Implements IGoodsReceivingService.ReceiveGoodsAsync
            Dim po = Await _db.PurchaseOrders.FirstOrDefaultAsync(Function(p) p.Id = dto.PurchaseOrderId)
            If po Is Nothing Then
                Throw New Exception("Purchase Order not found.")
            End If

            If po.Status <> PurchaseOrderStatus.Submitted Then
                Throw New Exception("Purchase Order must be in Submitted status to receive goods.")
            End If

            For Each lineDto In dto.Lines
                If lineDto.QuantityReceived <> lineDto.QuantityOrdered AndAlso String.IsNullOrWhiteSpace(lineDto.DiscrepancyNotes) Then
                    Throw New Exception($"Discrepancy notes are required for product {lineDto.ProductName} because quantity received ({lineDto.QuantityReceived}) differs from quantity ordered ({lineDto.QuantityOrdered}).")
                End If
            Next

            Dim existingReceipts = Await _db.GoodsReceipts.Select(Function(r) r.ReceiptNumber).ToListAsync()
            Dim receiptNumber = SequentialNumberGenerator.Generate("GR", DateTime.Now.Year, existingReceipts)

            Dim receipt = New GoodsReceipt With {
                .PurchaseOrderId = po.Id,
                .ReceiptNumber = receiptNumber,
                .Lines = New List(Of GoodsReceiptLine)()
            }

            For Each lineDto In dto.Lines
                Dim lineTotal = lineDto.UnitCost * lineDto.QuantityReceived
                Dim vatableSales As Decimal
                Dim vatAmount As Decimal

                If lineDto.VatClassification = VatTreatment.Vatable Then
                    vatableSales = Math.Round(lineTotal / 1.12D, 2)
                    vatAmount = lineTotal - vatableSales
                Else
                    vatableSales = lineTotal
                    vatAmount = 0
                End If

                Dim line = New GoodsReceiptLine With {
                    .ProductId = lineDto.ProductId,
                    .UnitCost = lineDto.UnitCost,
                    .ExpiryDate = lineDto.ExpiryDate,
                    .QuantityReceived = lineDto.QuantityReceived,
                    .DiscrepancyNotes = lineDto.DiscrepancyNotes,
                    .HasDiscrepancy = (lineDto.QuantityReceived <> lineDto.QuantityOrdered),
                    .VatClassification = lineDto.VatClassification,
                    .VatableSales = vatableSales,
                    .VatAmount = vatAmount
                }

                ' Also update the dto so the calculator uses the server-computed values
                lineDto.VatableSales = vatableSales
                lineDto.VatAmount = vatAmount

                receipt.Lines.Add(line)
            Next

            _db.GoodsReceipts.Add(receipt)
            po.Status = PurchaseOrderStatus.Received

            Await _db.SaveChangesAsync()

            Dim receivedEvent = New GoodsReceivedEvent With {
                .SourcePurchaseOrderId = po.Id,
                .ReceiptDate = DateTime.Now,
                .Items = receipt.Lines.Select(Function(l) New GoodsReceivedEvent.GoodsReceivedItem With {
                    .ProductId = l.ProductId,
                    .QuantityReceived = l.QuantityReceived,
                    .UnitCost = l.UnitCost,
                    .ExpiryDate = l.ExpiryDate
                }).ToList()
            }

            Await _mediator.Publish(receivedEvent)

            Dim breakdown = _vatCalculator.Calculate(dto)
            Dim vatEvent = New GoodsReceivedWithVatEvent With {
                .VatableInput = breakdown.VatableInputs,
                .VatExemptInput = breakdown.VatExemptInputs,
                .ZeroRatedInput = breakdown.ZeroRatedInputs,
                .InputVat = breakdown.InputVat
            }

            For Each line In receipt.Lines
                vatEvent.Items.Add(New GoodsReceivedWithVatEvent.GoodsReceivedItemWithVat With {
                    .Treatment = line.VatClassification,
                    .InputVat = line.VatAmount
                })
            Next

            Await _mediator.Publish(vatEvent)

            Await _priceChangeService.DetectChangesAsync(receipt.Id)

            Return receipt
        End Function

        Public Async Function GetReceiptByIdAsync(receiptId As Integer) As Task(Of GoodsReceipt) Implements IGoodsReceivingService.GetReceiptByIdAsync
            Return Await _db.GoodsReceipts.
                Include(Function(r) r.Lines).
                FirstOrDefaultAsync(Function(r) r.Id = receiptId)
        End Function

        Public Async Function GetReceiptsForPOAsync(purchaseOrderId As Integer) As Task(Of List(Of GoodsReceipt)) Implements IGoodsReceivingService.GetReceiptsForPOAsync
            ' Plain EF query. The manual parent/child join this replaced existed only because
            ' GoodsReceiptConfiguration mapped the relationship without a navigation, which made
            ' Include(Function(r) r.Lines) join on a shadow GoodsReceiptId1 key. That mapping is
            ' fixed, so the include now resolves on GoodsReceiptId.
            Return Await _db.GoodsReceipts.
                Include(Function(r) r.Lines).
                Where(Function(r) r.PurchaseOrderId = purchaseOrderId).
                ToListAsync()
        End Function
    End Class

End Namespace
