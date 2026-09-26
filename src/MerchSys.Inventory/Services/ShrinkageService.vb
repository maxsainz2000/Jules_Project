Imports System.Threading.Tasks
Imports System.Collections.Generic
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Inventory.Data
Imports MerchSys.Inventory.Entities
Imports MerchSys.SharedKernel.Events
Imports MediatR

Namespace Services

    Public Class ShrinkageService
        Implements IShrinkageService

        Private ReadOnly _dbContext As InventoryDbContext
        Private ReadOnly _mediator As IMediator

        Public Sub New(dbContext As InventoryDbContext, mediator As IMediator)
            _dbContext = dbContext
            _mediator = mediator
        End Sub

        Public Async Function RecordShrinkageAsync(productId As Integer, quantity As Integer, reason As String, batchId As Integer?) As Task Implements IShrinkageService.RecordShrinkageAsync
            If String.IsNullOrWhiteSpace(reason) Then
                Throw New ArgumentException("Shrinkage reason must be provided.")
            End If
            If quantity <= 0 Then
                Throw New ArgumentException("Quantity must be greater than zero.")
            End If

            Dim totalValue As Decimal = 0D

            If batchId.HasValue Then
                Dim batch = Await _dbContext.StockBatches.FirstOrDefaultAsync(Function(b) b.Id = batchId.Value AndAlso b.ProductId = productId)
                If batch Is Nothing Then
                    Throw New InvalidOperationException("Specified batch not found.")
                End If
                If batch.QuantityRemaining < quantity Then
                    Throw New InvalidOperationException("Insufficient quantity in specified batch.")
                End If

                batch.QuantityRemaining -= quantity
                totalValue = quantity * batch.UnitCost
            Else
                Dim batchIds = Await _dbContext.StockBatches.
                    Where(Function(b) b.ProductId = productId AndAlso b.QuantityRemaining > 0).
                    OrderBy(Function(b) b.ReceiptDate).
                    Select(Function(b) b.Id).
                    ToListAsync()

                Dim remainingToDeduct = quantity

                For Each id In batchIds
                    If remainingToDeduct <= 0 Then
                        Exit For
                    End If

                    Dim batch = Await _dbContext.StockBatches.FindAsync(id)
                    If batch IsNot Nothing AndAlso batch.QuantityRemaining > 0 Then
                        Dim deductAmount = Math.Min(batch.QuantityRemaining, remainingToDeduct)
                        batch.QuantityRemaining -= deductAmount
                        remainingToDeduct -= deductAmount
                        totalValue += deductAmount * batch.UnitCost
                    End If
                Next

                If remainingToDeduct > 0 Then
                    Throw New InvalidOperationException("Insufficient stock for FIFO deduction.")
                End If
            End If

            Dim unitCost As Decimal = totalValue / quantity

            Dim record = New ShrinkageRecord With {
                .ProductId = productId,
                .Reason = reason,
                .QuantityLost = quantity,
                .UnitCost = unitCost,
                .TotalValue = totalValue
            }

            _dbContext.ShrinkageRecords.Add(record)
            Await _dbContext.SaveChangesAsync()

            Dim shrinkageEvent = New ShrinkageRecordedEvent With {
                .ProductId = productId,
                .Quantity = quantity,
                .TotalValue = totalValue,
                .Reason = reason
            }
            Await _mediator.Publish(shrinkageEvent)
        End Function

        Public Async Function GetShrinkageHistoryAsync() As Task(Of List(Of ShrinkageHistoryDto)) Implements IShrinkageService.GetShrinkageHistoryAsync
            Dim query = From sr In _dbContext.ShrinkageRecords
                        Join p In _dbContext.Products On sr.ProductId Equals p.Id
                        Order By sr.CreatedAt Descending
                        Select New ShrinkageHistoryDto With {
                            .Id = sr.Id,
                            .ProductId = sr.ProductId,
                            .ProductName = p.Name,
                            .Reason = sr.Reason,
                            .QuantityLost = sr.QuantityLost,
                            .UnitCost = sr.UnitCost,
                            .TotalValue = sr.TotalValue,
                            .CreatedAt = sr.CreatedAt
                        }

            Return Await query.ToListAsync()
        End Function

        Public Async Function GetTotalShrinkageValueAsync() As Task(Of Decimal) Implements IShrinkageService.GetTotalShrinkageValueAsync
            Return Await _dbContext.ShrinkageRecords.SumAsync(Function(sr) sr.TotalValue)
        End Function

    End Class

End Namespace
