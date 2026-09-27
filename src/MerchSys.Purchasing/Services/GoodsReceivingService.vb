Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Purchasing.Data
Imports MerchSys.Purchasing.Dtos
Imports MerchSys.Purchasing.Entities
Imports MerchSys.Purchasing.Helpers
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Events

Namespace Services
    Public Class GoodsReceivingService
        Implements IGoodsReceivingService

        Private ReadOnly _dbContext As PurchasingDbContext
        Private ReadOnly _mediator As IMediator

        Public Sub New(dbContext As PurchasingDbContext, mediator As IMediator)
            _dbContext = dbContext
            _mediator = mediator
        End Sub

        Public Async Function ReceiveGoodsAsync(dto As ReceiveGoodsDto) As Task(Of GoodsReceipt) Implements IGoodsReceivingService.ReceiveGoodsAsync
            Dim po = Await _dbContext.PurchaseOrders.FirstOrDefaultAsync(Function(p) p.Id = dto.PurchaseOrderId)
            If po Is Nothing Then Throw New InvalidOperationException("Purchase order not found.")
            If po.Status <> PurchaseOrderStatus.Submitted Then Throw New InvalidOperationException("Purchase order must be in Submitted status to receive goods.")

            For Each line In dto.Lines
                If line.QuantityReceived <> line.QuantityOrdered AndAlso String.IsNullOrWhiteSpace(line.DiscrepancyNotes) Then
                    Throw New InvalidOperationException($"Discrepancy notes required for product {line.ProductId} as received quantity ({line.QuantityReceived}) differs from ordered quantity ({line.QuantityOrdered}).")
                End If
            Next

            Dim currentYear = DateTime.UtcNow.Year
            Dim yearPrefix = $"GR-{currentYear}-"

            Dim existingNumbers = Await _dbContext.GoodsReceipts.
                Where(Function(gr) gr.ReceiptNumber.StartsWith(yearPrefix)).
                Select(Function(gr) gr.ReceiptNumber).
                ToListAsync()

            Dim receiptNumber = SequentialNumberGenerator.Generate("GR", currentYear, existingNumbers)

            Dim goodsReceipt As New GoodsReceipt With {
                .PurchaseOrderId = dto.PurchaseOrderId,
                .ReceiptNumber = receiptNumber
            }

            For Each line In dto.Lines
                Dim hasDiscrepancy = (line.QuantityReceived <> line.QuantityOrdered)
                Dim receiptLine As New GoodsReceiptLine With {
                    .ProductId = line.ProductId,
                    .UnitCost = line.UnitCost,
                    .ExpiryDate = line.ExpiryDate,
                    .QuantityOrdered = line.QuantityOrdered,
                    .QuantityReceived = line.QuantityReceived,
                    .DiscrepancyNotes = line.DiscrepancyNotes,
                    .HasDiscrepancy = hasDiscrepancy
                }
                goodsReceipt.Lines.Add(receiptLine)
            Next

            _dbContext.GoodsReceipts.Add(goodsReceipt)

            po.Status = PurchaseOrderStatus.Received
            _dbContext.PurchaseOrders.Update(po)

            Await _dbContext.SaveChangesAsync()

            Dim goodsReceivedEvent As New GoodsReceivedEvent With {
                .SourcePurchaseOrderId = dto.PurchaseOrderId,
                .ReceiptDate = DateTime.UtcNow
            }

            For Each line In goodsReceipt.Lines
                goodsReceivedEvent.Items.Add(New GoodsReceivedEvent.GoodsReceivedItem With {
                    .ProductId = line.ProductId,
                    .QuantityReceived = line.QuantityReceived,
                    .UnitCost = line.UnitCost,
                    .ExpiryDate = line.ExpiryDate
                })
            Next

            Await _mediator.Publish(goodsReceivedEvent)

            Dim reloadedReceipt = Await _dbContext.GoodsReceipts.
                Include(Function(x) x.Lines).
                FirstOrDefaultAsync(Function(gr) gr.Id = goodsReceipt.Id)

            Return reloadedReceipt
        End Function

        Public Async Function GetReceiptByIdAsync(id As Integer) As Task(Of GoodsReceipt) Implements IGoodsReceivingService.GetReceiptByIdAsync
            Return Await _dbContext.GoodsReceipts.
                Include(Function(x) x.Lines).
                FirstOrDefaultAsync(Function(gr) gr.Id = id)
        End Function

        Public Async Function GetReceiptsForPOAsync(purchaseOrderId As Integer) As Task(Of List(Of GoodsReceipt)) Implements IGoodsReceivingService.GetReceiptsForPOAsync
            Return Await _dbContext.GoodsReceipts.
                Include(Function(x) x.Lines).
                Where(Function(gr) gr.PurchaseOrderId = purchaseOrderId).
                ToListAsync()
        End Function
    End Class
End Namespace
