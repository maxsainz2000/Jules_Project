Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Inventory.Data
Imports MerchSys.Inventory.Entities

Namespace Services

    Public Class StockDashboardService
        Implements IStockDashboardService

        Private ReadOnly _dbContext As InventoryDbContext

        Public Sub New(dbContext As InventoryDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function GetDashboardSummaryAsync() As Task(Of StockDashboardDto) Implements IStockDashboardService.GetDashboardSummaryAsync
            Dim productsData = Await _dbContext.Products.
                Where(Function(p) Not p.IsDeleted).
                Select(Function(p) New With {
                    .Id = p.Id,
                    .Name = p.Name,
                    .SKU = p.SKU,
                    .CategoryId = p.ProductCategoryId,
                    .MinimumThreshold = p.MinimumThreshold,
                    .RetailPrice = p.RetailPrice
                }).ToListAsync()

            Dim batchesData = Await _dbContext.StockBatches.
                Where(Function(b) b.QuantityRemaining > 0).
                Select(Function(b) New With {
                    .ProductId = b.ProductId,
                    .QuantityRemaining = b.QuantityRemaining,
                    .UnitCost = b.UnitCost,
                    .ExpiryDate = b.ExpiryDate,
                    .ReceiptDate = b.ReceiptDate
                }).ToListAsync()

            Dim result = New StockDashboardDto With {
                .ProductSummaries = New List(Of ProductSummaryDto)()
            }

            Dim today = DateTime.Today

            For Each p In productsData
                Dim productBatches = batchesData.Where(Function(b) b.ProductId = p.Id).ToList()
                Dim currentQty = productBatches.Sum(Function(b) b.QuantityRemaining)
                Dim totalVal = productBatches.Sum(Function(b) b.QuantityRemaining * b.UnitCost)

                Dim isBelow = currentQty < p.MinimumThreshold
                If isBelow Then
                    result.LowStockProductCount += 1
                End If

                result.TotalStockValue += totalVal

                Dim nonExpiredBatches = productBatches.Where(Function(batch) Not batch.ExpiryDate.HasValue OrElse batch.ExpiryDate.Value.Date > today).ToList()
                Dim nonExpiredQty = nonExpiredBatches.Sum(Function(batch) batch.QuantityRemaining)
                Dim nonExpiredVal = nonExpiredBatches.Sum(Function(batch) batch.QuantityRemaining * batch.UnitCost)
                Dim avgCost = If(nonExpiredQty > 0, nonExpiredVal / nonExpiredQty, 0D)

                Dim oldestBatch = nonExpiredBatches.OrderBy(Function(batch) batch.ReceiptDate).FirstOrDefault()
                Dim fifoCost = If(oldestBatch IsNot Nothing, oldestBatch.UnitCost, 0D)

                result.ProductSummaries.Add(New ProductSummaryDto With {
                    .ProductId = p.Id,
                    .ProductName = p.Name,
                    .SKU = p.SKU,
                    .CategoryId = p.CategoryId,
                    .CurrentQuantity = currentQty,
                    .TotalValue = totalVal,
                    .IsBelowThreshold = isBelow,
                    .RetailPrice = p.RetailPrice,
                    .AverageUnitCost = avgCost,
                    .FifoOldestUnitCost = fifoCost
                })
            Next

            result.ExpiredOrNearExpiryBatchCount = Enumerable.Count(batchesData.AsEnumerable(), Function(b) b.ExpiryDate.HasValue AndAlso b.ExpiryDate.Value.Date <= today.AddDays(30))

            Return result
        End Function

        Public Async Function GetProductDetailAsync(productId As Integer) As Task(Of ProductDetailDto) Implements IStockDashboardService.GetProductDetailAsync
            Dim productInfo = Await _dbContext.Products.
                Where(Function(p) p.Id = productId AndAlso Not p.IsDeleted).
                Select(Function(p) New With {
                    .Id = p.Id,
                    .Name = p.Name,
                    .SKU = p.SKU
                }).FirstOrDefaultAsync()

            If productInfo Is Nothing Then
                Return Nothing
            End If

            Dim batchesData = Await _dbContext.StockBatches.
                Where(Function(b) b.ProductId = productId).
                Select(Function(b) New StockBatchSummaryDto With {
                    .BatchId = b.Id,
                    .QuantityReceived = b.QuantityReceived,
                    .QuantityRemaining = b.QuantityRemaining,
                    .UnitCost = b.UnitCost,
                    .ReceiptDate = b.ReceiptDate,
                    .ExpiryDate = b.ExpiryDate
                }).ToListAsync()

            Dim shrinkageData = Await _dbContext.ShrinkageRecords.
                Where(Function(s) s.ProductId = productId).
                Select(Function(s) New ShrinkageSummaryDto With {
                    .ShrinkageId = s.Id,
                    .Reason = s.Reason,
                    .QuantityLost = s.QuantityLost,
                    .UnitCost = s.UnitCost,
                    .TotalValue = s.TotalValue,
                    .RecordedAt = s.CreatedAt
                }).ToListAsync()

            Dim currentQty = batchesData.Sum(Function(b) b.QuantityRemaining)

            Dim detailDto = New ProductDetailDto With {
                .ProductId = productInfo.Id,
                .ProductName = productInfo.Name,
                .SKU = productInfo.SKU,
                .CurrentQuantity = currentQty,
                .StockBatches = batchesData,
                .ShrinkageHistory = shrinkageData,
                .StockMovements = New List(Of StockMovementDto)()
            }

            Return detailDto
        End Function

    End Class

End Namespace
