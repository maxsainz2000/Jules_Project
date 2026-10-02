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
                Dim line = New GoodsReceiptLine With {
                    .ProductId = lineDto.ProductId,
                    .UnitCost = lineDto.UnitCost,
                    .ExpiryDate = lineDto.ExpiryDate,
                    .QuantityReceived = lineDto.QuantityReceived,
                    .DiscrepancyNotes = lineDto.DiscrepancyNotes,
                    .HasDiscrepancy = (lineDto.QuantityReceived <> lineDto.QuantityOrdered)
                }
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

            For Each line In dto.Lines
                vatEvent.Items.Add(New GoodsReceivedWithVatEvent.GoodsReceivedItemWithVat With {
                    .Treatment = VatTreatment.Vatable,
                    .InputVat = Math.Round(((line.UnitCost * line.QuantityReceived) / 1.12D) * 0.12D, 2)
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
            Dim receipts = New List(Of GoodsReceipt)()
            Dim receiptLines = New List(Of GoodsReceiptLine)()

            Dim connection = _db.Database.GetDbConnection()
            Dim wasClosed = (connection.State = System.Data.ConnectionState.Closed)

            If wasClosed Then
                Await connection.OpenAsync()
            End If

            Try
                Using cmd = connection.CreateCommand()
                    cmd.CommandText = "SELECT Id, ReceiptNumber, PurchaseOrderId FROM Pur_GoodsReceipts WHERE PurchaseOrderId = @poId"
                    Dim p = cmd.CreateParameter()
                    p.ParameterName = "@poId"
                    p.Value = purchaseOrderId
                    cmd.Parameters.Add(p)

                    Using reader = Await cmd.ExecuteReaderAsync()
                        While Await reader.ReadAsync()
                            Dim r = New GoodsReceipt()
                            r.Id = Convert.ToInt32(reader("Id"))
                            r.ReceiptNumber = Convert.ToString(reader("ReceiptNumber"))
                            r.PurchaseOrderId = Convert.ToInt32(reader("PurchaseOrderId"))
                            r.Lines = New List(Of GoodsReceiptLine)()
                            receipts.Add(r)
                        End While
                    End Using
                End Using

                If receipts.Any() Then
                    Dim receiptIds = String.Join(",", receipts.Select(Function(r) r.Id))
                    Using cmdLines = connection.CreateCommand()
                        cmdLines.CommandText = $"SELECT Id, GoodsReceiptId, ProductId, UnitCost, ExpiryDate, HasDiscrepancy, QuantityReceived, DiscrepancyNotes FROM Pur_GoodsReceiptLines WHERE GoodsReceiptId IN ({receiptIds})"
                        Using readerLines = Await cmdLines.ExecuteReaderAsync()
                            While Await readerLines.ReadAsync()
                                Dim l = New GoodsReceiptLine()
                                l.Id = Convert.ToInt32(readerLines("Id"))
                                l.GoodsReceiptId = Convert.ToInt32(readerLines("GoodsReceiptId"))
                                l.ProductId = Convert.ToInt32(readerLines("ProductId"))
                                l.UnitCost = Convert.ToDecimal(readerLines("UnitCost"))

                                If Not readerLines.IsDBNull(readerLines.GetOrdinal("ExpiryDate")) Then
                                    l.ExpiryDate = Convert.ToDateTime(readerLines("ExpiryDate"))
                                End If

                                l.HasDiscrepancy = Convert.ToBoolean(readerLines("HasDiscrepancy"))
                                l.QuantityReceived = Convert.ToInt32(readerLines("QuantityReceived"))

                                If Not readerLines.IsDBNull(readerLines.GetOrdinal("DiscrepancyNotes")) Then
                                    l.DiscrepancyNotes = Convert.ToString(readerLines("DiscrepancyNotes"))
                                End If

                                Dim parent = receipts.FirstOrDefault(Function(r) r.Id = l.GoodsReceiptId)
                                If parent IsNot Nothing Then
                                    parent.Lines.Add(l)
                                End If
                            End While
                        End Using
                    End Using
                End If
            Finally
                If wasClosed Then
                    connection.Close()
                End If
            End Try

            Return receipts
        End Function
    End Class

End Namespace
