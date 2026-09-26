Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Inventory.Data
Imports MerchSys.SharedKernel.Interfaces

Namespace Services

    Public Class LowStockAlertService
        Implements ILowStockAlertService

        Private ReadOnly _dbContext As InventoryDbContext
        Private ReadOnly _notifier As ILowStockNotifier
        Private Shared ReadOnly _dismissedAlerts As New HashSet(Of Integer)()

        Public Sub New(dbContext As InventoryDbContext, notifier As ILowStockNotifier)
            _dbContext = dbContext
            _notifier = notifier
        End Sub

        Public Async Function GetActiveAlertsAsync() As Task(Of List(Of LowStockAlertDto)) Implements ILowStockAlertService.GetActiveAlertsAsync
            Dim dismissedIds As Integer()
            SyncLock _dismissedAlerts
                dismissedIds = _dismissedAlerts.ToArray()
            End SyncLock

            Dim productsQuery = Await _dbContext.Products.
                Where(Function(p) Not p.IsDeleted).
                Select(Function(p) New With {
                    .Id = p.Id,
                    .Name = p.Name,
                    .MinimumThreshold = p.MinimumThreshold,
                    .CurrentStock = _dbContext.StockBatches.
                        Where(Function(b) b.ProductId = p.Id AndAlso b.QuantityRemaining > 0).
                        Sum(Function(b) CType(b.QuantityRemaining, Integer?))
                }).
                ToListAsync()

            Dim alerts = productsQuery.
                Select(Function(p) New With {
                    .Id = p.Id,
                    .Name = p.Name,
                    .MinimumThreshold = p.MinimumThreshold,
                    .CurrentStock = If(p.CurrentStock, 0)
                }).
                Where(Function(p) p.CurrentStock <= p.MinimumThreshold AndAlso Not dismissedIds.Contains(p.Id)).
                Select(Function(p) New LowStockAlertDto With {
                    .ProductId = p.Id,
                    .ProductName = p.Name,
                    .CurrentStock = p.CurrentStock,
                    .MinimumThreshold = p.MinimumThreshold,
                    .Deficit = p.MinimumThreshold - p.CurrentStock
                }).
                OrderByDescending(Function(a) a.Deficit).
                ToList()

            Return alerts
        End Function

        Public Async Function CheckAndGenerateAlertsAsync() As Task Implements ILowStockAlertService.CheckAndGenerateAlertsAsync
            Dim alerts = Await GetActiveAlertsAsync()

            For Each alert In alerts
                _notifier.NotifyLowStock(alert.ProductName, alert.CurrentStock, alert.MinimumThreshold)
            Next
        End Function

        Public Function DismissAlertAsync(productId As Integer) As Task Implements ILowStockAlertService.DismissAlertAsync
            SyncLock _dismissedAlerts
                _dismissedAlerts.Add(productId)
            End SyncLock
            Return Task.CompletedTask
        End Function

    End Class

End Namespace
