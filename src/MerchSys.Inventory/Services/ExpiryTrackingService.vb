Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Configuration
Imports MerchSys.Inventory.Data
Imports MerchSys.Inventory.Entities
Imports MerchSys.SharedKernel.Interfaces

Namespace Services

    Public Class ExpiryTrackingService
        Implements IExpiryTrackingService

        Private ReadOnly _dbContext As InventoryDbContext
        Private ReadOnly _writeContextScope As IWriteContextScope

        Public Sub New(dbContext As InventoryDbContext, writeContextScope As IWriteContextScope)
            _dbContext = dbContext
            _writeContextScope = writeContextScope
        End Sub

        Public Async Function GetNearExpiryAlertsAsync(daysThreshold As Integer) As Task(Of List(Of ExpiryAlertDto)) Implements IExpiryTrackingService.GetNearExpiryAlertsAsync
            Dim today = DateTime.Today
            Dim targetDate = today.AddDays(daysThreshold)

            Dim alerts = Await _dbContext.StockBatches.
                Where(Function(b) b.QuantityRemaining > 0 AndAlso
                                  b.ExpiryDate.HasValue AndAlso
                                  b.ExpiryDate.Value.Date >= today AndAlso
                                  b.ExpiryDate.Value.Date <= targetDate).
                Select(Function(b) New ExpiryAlertDto With {
                    .BatchId = b.Id,
                    .ProductId = b.ProductId,
                    .ExpiryDate = b.ExpiryDate.Value.Date,
                    .QuantityRemaining = b.QuantityRemaining
                }).
                ToListAsync()

            Return alerts
        End Function

        Public Async Function GetExpiredBatchesAsync() As Task(Of List(Of ExpiryAlertDto)) Implements IExpiryTrackingService.GetExpiredBatchesAsync
            Dim today = DateTime.Today

            Dim alerts = Await _dbContext.StockBatches.
                Where(Function(b) b.QuantityRemaining > 0 AndAlso
                                  b.ExpiryDate.HasValue AndAlso
                                  b.ExpiryDate.Value.Date < today).
                Select(Function(b) New ExpiryAlertDto With {
                    .BatchId = b.Id,
                    .ProductId = b.ProductId,
                    .ExpiryDate = b.ExpiryDate.Value.Date,
                    .QuantityRemaining = b.QuantityRemaining
                }).
                ToListAsync()

            Return alerts
        End Function

        Public Async Function GetProductExpiryStatusAsync(productId As Integer) As Task(Of ProductExpiryStatusDto) Implements IExpiryTrackingService.GetProductExpiryStatusAsync
            Dim today = DateTime.Today

            Dim stats = Await _dbContext.StockBatches.
                Where(Function(b) b.ProductId = productId AndAlso b.QuantityRemaining > 0).
                GroupBy(Function(b) b.ProductId).
                Select(Function(g) New ProductExpiryStatusDto With {
                    .ProductId = g.Key,
                    .TotalQuantity = g.Sum(Function(x) x.QuantityRemaining),
                    .ExpiredQuantity = g.Sum(Function(x) If(x.ExpiryDate.HasValue AndAlso x.ExpiryDate.Value.Date < today, x.QuantityRemaining, 0)),
                    .GoodQuantity = g.Sum(Function(x) If(Not x.ExpiryDate.HasValue OrElse x.ExpiryDate.Value.Date >= today, x.QuantityRemaining, 0))
                }).
                FirstOrDefaultAsync()

            If stats Is Nothing Then
                Return New ProductExpiryStatusDto With {
                    .ProductId = productId,
                    .TotalQuantity = 0,
                    .ExpiredQuantity = 0,
                    .GoodQuantity = 0
                }
            End If

            Return stats
        End Function

        Public Async Function WriteOffExpiredStockAsync(reason As String) As Task(Of Integer) Implements IExpiryTrackingService.WriteOffExpiredStockAsync
            Dim today = DateTime.Today

            Dim expiredBatchesData = Await _dbContext.StockBatches.
                Where(Function(b) b.QuantityRemaining > 0 AndAlso b.ExpiryDate.HasValue AndAlso b.ExpiryDate.Value.Date < today).
                Select(Function(b) New With { .Id = b.Id, .QuantityRemaining = b.QuantityRemaining, .UnitCost = b.UnitCost }).
                ToListAsync()

            If Not expiredBatchesData.Any() Then
                Return 0
            End If

            Using _writeContextScope.Enter(WriteContextKind.System)
                Using transaction = Await _dbContext.Database.BeginTransactionAsync()
                    For Each batch In expiredBatchesData
                        Dim shrinkage = New ShrinkageRecord With {
                            .Reason = reason,
                            .QuantityLost = batch.QuantityRemaining,
                            .UnitCost = batch.UnitCost,
                            .TotalValue = batch.QuantityRemaining * batch.UnitCost
                        }
                        _dbContext.ShrinkageRecords.Add(shrinkage)
                    Next

                    Dim batchIds = expiredBatchesData.Select(Function(b) b.Id).ToList()
                    Await _dbContext.StockBatches.
                        Where(Function(b) batchIds.Contains(b.Id)).
                        ExecuteUpdateAsync(Function(s) s.SetProperty(Function(b) b.QuantityRemaining, 0))

                    Await _dbContext.SaveChangesAsync()
                    Await transaction.CommitAsync()
                End Using
            End Using

            Return expiredBatchesData.Count
        End Function

    End Class

End Namespace
