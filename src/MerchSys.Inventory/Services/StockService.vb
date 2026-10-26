Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Inventory.Data
Imports MerchSys.Inventory.Entities
Imports MerchSys.SharedKernel.Exceptions

Namespace Services

    Public Class StockService
        Implements IStockService

        Private ReadOnly _dbContext As InventoryDbContext

        Public Sub New(dbContext As InventoryDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function AddStockBatchAsync(productId As Integer, quantity As Integer, unitCost As Decimal, receiptDate As DateTime, sourcePurchaseOrderId As Integer, expiryDate As DateTime?) As Task Implements IStockService.AddStockBatchAsync
            Dim batch = New StockBatch With {
                .ProductId = productId,
                .QuantityReceived = quantity,
                .QuantityRemaining = quantity,
                .UnitCost = unitCost,
                .ReceiptDate = receiptDate,
                .SourcePurchaseOrderId = sourcePurchaseOrderId,
                .ExpiryDate = expiryDate
            }
            _dbContext.StockBatches.Add(batch)
            Await _dbContext.SaveChangesAsync()
        End Function

        ''' <summary>
        ''' Deducts stock FIFO, serialized against the other client laptops.
        '''
        ''' AGENTS.md requires <c>SELECT ... FOR UPDATE</c> here, and this is why: without it the
        ''' method is an unlocked read-modify-write, so two clients can both read the same batches,
        ''' both pass the sufficiency check, and both try to decrement. The row-version token catches
        ''' the second one — but only after the sale is already committed, so the sale succeeds and
        ''' the stock silently fails to move.
        '''
        ''' The lock and the decrement share one transaction, so the second client blocks on the
        ''' first and then reads the post-decrement quantities instead of failing.
        '''
        ''' If the caller already has a transaction open — <c>SaleCompletedHandler</c> wraps a whole
        ''' sale's items in one, so a mid-sale failure cannot leave a partial deduction — this joins
        ''' it rather than beginning a nested one. EF throws if a second transaction is started on a
        ''' context that already has one, and joining is what makes the caller's unit of work atomic.
        ''' </summary>
        Public Async Function DeductStockFIFOAsync(productId As Integer, quantity As Integer) As Task(Of FIFODeductionResult) Implements IStockService.DeductStockFIFOAsync
            If _dbContext.Database.CurrentTransaction IsNot Nothing Then
                Return Await DeductStockFIFOCoreAsync(productId, quantity)
            End If

            ' No explicit commit path: disposing an uncommitted transaction rolls it back, so an
            ' exception anywhere below leaves the batches untouched.
            Using transaction = Await _dbContext.Database.BeginTransactionAsync()
                Dim result = Await DeductStockFIFOCoreAsync(productId, quantity)
                Await transaction.CommitAsync()
                Return result
            End Using
        End Function

        Private Async Function DeductStockFIFOCoreAsync(productId As Integer, quantity As Integer) As Task(Of FIFODeductionResult)
            ' Lock before reading. The ordering must match the read below: two clients that take the
            ' same locks in the same order simply queue, whereas different orders deadlock.
            Await StockBatchLock.LockInStockBatchesAsync(_dbContext, productId)

            Dim batches = Await _dbContext.StockBatches.
                Where(Function(b) b.ProductId = productId AndAlso b.QuantityRemaining > 0).
                OrderBy(Function(b) b.ReceiptDate).
                ThenBy(Function(b) b.Id).
                ToListAsync()

            Dim totalAvailable = batches.Sum(Function(b) b.QuantityRemaining)
            If totalAvailable < quantity Then
                Throw New InsufficientStockException($"Insufficient stock for product {productId}. Requested: {quantity}, Available: {totalAvailable}")
            End If

            Dim remainingToDeduct = quantity
            Dim deducted = 0
            Dim totalCost As Decimal = 0D
            Dim batchDeductions As New List(Of FIFOBatchDeduction)()

            For Each batch In batches
                If remainingToDeduct <= 0 Then
                    Exit For
                End If

                Dim deductAmount = Math.Min(batch.QuantityRemaining, remainingToDeduct)
                batch.QuantityRemaining -= deductAmount
                remainingToDeduct -= deductAmount
                deducted += deductAmount

                Dim batchCost = CDec(deductAmount) * batch.UnitCost

                batchDeductions.Add(New FIFOBatchDeduction With {
                    .BatchId = batch.Id,
                    .QuantityDeducted = deductAmount,
                    .UnitCost = batch.UnitCost,
                    .TotalCost = batchCost
                })

                ' Cost of the units taken from THIS batch, at this batch's own unit cost — that is
                ' what makes it FIFO costing rather than an average.
                totalCost += batchCost
            Next

            Await _dbContext.SaveChangesAsync()

            Return New FIFODeductionResult With {
                .ProductId = productId,
                .DeductedQuantity = deducted,
                .RemainingQuantityNeeded = remainingToDeduct,
                .Success = remainingToDeduct = 0,
                .TotalCost = totalCost,
                .BatchDeductions = batchDeductions
            }
        End Function

        Public Async Function GetCurrentStockLevelsAsync() As Task(Of List(Of StockLevelDto)) Implements IStockService.GetCurrentStockLevelsAsync
            ' Plain EF query. The raw SQL this replaced existed only for the "EF Core ToListAsync
            ' returns an empty list" workaround, retested against MariaDB on 2026-10-12 and not
            ' reproducible (see AGENTS.md). Soft-deleted rows are excluded by BaseDbContext's
            ' global query filter, which the raw SQL did by hand.
            ' The sum is taken over a nullable projection so an empty batch set yields NULL, which
            ' is then coalesced to zero — a plain Sum over Integer would fault on NULL.
            Dim rows = Await _dbContext.Products.
                Select(Function(p) New With {
                    .ProductId = p.Id,
                    .ThresholdQuantity = p.MinimumThreshold,
                    .CurrentQuantity = _dbContext.StockBatches.
                        Where(Function(b) b.ProductId = p.Id AndAlso b.QuantityRemaining > 0).
                        Sum(Function(b) CType(b.QuantityRemaining, Integer?))
                }).
                ToListAsync()

            Return rows.Select(Function(r) New StockLevelDto With {
                    .ProductId = r.ProductId,
                    .CurrentQuantity = If(r.CurrentQuantity, 0),
                    .ThresholdQuantity = r.ThresholdQuantity,
                    .IsBelowThreshold = If(r.CurrentQuantity, 0) < r.ThresholdQuantity
                }).ToList()
        End Function
    End Class

End Namespace
