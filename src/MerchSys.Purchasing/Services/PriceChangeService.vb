Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Purchasing.Data
Imports MerchSys.Purchasing.Entities

Namespace Services
    Public Class PriceChangeService
        Implements IPriceChangeService

        Private ReadOnly _db As PurchasingDbContext

        Public Sub New(db As PurchasingDbContext)
            _db = db
        End Sub

        Public Async Function DetectChangesAsync(goodsReceiptId As Integer) As Task Implements IPriceChangeService.DetectChangesAsync
            Dim receipt = Await _db.GoodsReceipts.Include(Function(r) r.Lines).FirstOrDefaultAsync(Function(r) r.Id = goodsReceiptId)
            If receipt Is Nothing Then Return

            Dim po = Await _db.PurchaseOrders.FirstOrDefaultAsync(Function(p) p.Id = receipt.PurchaseOrderId)
            If po Is Nothing Then Return

            Dim poLinesData = Await _db.PurchaseOrderLines.
                Where(Function(pl) pl.PurchaseOrderId = po.Id).
                Select(Function(pl) New With { pl.ProductId, pl.UnitCost }).
                ToListAsync()

            Dim alerts = New List(Of PriceChangeAlert)()

            For Each line In receipt.Lines
                Dim poLine = poLinesData.FirstOrDefault(Function(pl) pl.ProductId = line.ProductId)
                If poLine IsNot Nothing AndAlso poLine.UnitCost <> line.UnitCost Then
                    Dim prevCost = poLine.UnitCost
                    Dim newCost = line.UnitCost
                    Dim diff = newCost - prevCost
                    Dim changePercent = Math.Round((diff / prevCost) * 100D, 4)
                    Dim direction = If(newCost > prevCost, "Increase", "Decrease")

                    alerts.Add(New PriceChangeAlert With {
                        .ProductId = line.ProductId,
                        .VendorId = po.VendorId,
                        .PreviousUnitCost = prevCost,
                        .NewUnitCost = newCost,
                        .ChangePercent = changePercent,
                        .ChangeDirection = direction,
                        .GoodsReceiptId = goodsReceiptId,
                        .IsAcknowledged = False
                    })
                End If
            Next

            If alerts.Any() Then
                _db.PriceChangeAlerts.AddRange(alerts)
                Await _db.SaveChangesAsync()
            End If
        End Function

        Public Async Function GetUnacknowledgedAsync() As Task(Of List(Of PriceChangeAlert)) Implements IPriceChangeService.GetUnacknowledgedAsync
            ' Project to an anonymous type first to avoid the VB.NET ToListAsync bug
            Dim anonymousAlerts = Await _db.PriceChangeAlerts.
                Where(Function(a) Not a.IsAcknowledged).
                Select(Function(a) New With {
                    a.Id,
                    a.ProductId,
                    a.VendorId,
                    a.PreviousUnitCost,
                    a.NewUnitCost,
                    a.ChangePercent,
                    a.ChangeDirection,
                    a.GoodsReceiptId,
                    a.IsAcknowledged,
                    a.AcknowledgedAt,
                    a.CreatedAt,
                    a.CreatedBy,
                    a.ModifiedAt,
                    a.ModifiedBy
                }).ToListAsync()

            Return anonymousAlerts.Select(Function(a) New PriceChangeAlert With {
                .Id = a.Id,
                .ProductId = a.ProductId,
                .VendorId = a.VendorId,
                .PreviousUnitCost = a.PreviousUnitCost,
                .NewUnitCost = a.NewUnitCost,
                .ChangePercent = a.ChangePercent,
                .ChangeDirection = a.ChangeDirection,
                .GoodsReceiptId = a.GoodsReceiptId,
                .IsAcknowledged = a.IsAcknowledged,
                .AcknowledgedAt = a.AcknowledgedAt,
                .CreatedAt = a.CreatedAt,
                .CreatedBy = a.CreatedBy,
                .ModifiedAt = a.ModifiedAt,
                .ModifiedBy = a.ModifiedBy
            }).ToList()
        End Function

        Public Async Function AcknowledgeAsync(alertId As Integer) As Task Implements IPriceChangeService.AcknowledgeAsync
            Dim alert = Await _db.PriceChangeAlerts.FirstOrDefaultAsync(Function(a) a.Id = alertId)
            If alert IsNot Nothing AndAlso Not alert.IsAcknowledged Then
                alert.IsAcknowledged = True
                alert.AcknowledgedAt = DateTime.UtcNow
                Await _db.SaveChangesAsync()
            End If
        End Function

        Public Async Function GetHistoryForProductAsync(productId As Integer) As Task(Of List(Of PriceChangeAlert)) Implements IPriceChangeService.GetHistoryForProductAsync
            ' Project to an anonymous type first to avoid the VB.NET ToListAsync bug
            Dim anonymousAlerts = Await _db.PriceChangeAlerts.
                Where(Function(a) a.ProductId = productId).
                OrderByDescending(Function(a) a.CreatedAt).
                Select(Function(a) New With {
                    a.Id,
                    a.ProductId,
                    a.VendorId,
                    a.PreviousUnitCost,
                    a.NewUnitCost,
                    a.ChangePercent,
                    a.ChangeDirection,
                    a.GoodsReceiptId,
                    a.IsAcknowledged,
                    a.AcknowledgedAt,
                    a.CreatedAt,
                    a.CreatedBy,
                    a.ModifiedAt,
                    a.ModifiedBy
                }).ToListAsync()

            Return anonymousAlerts.Select(Function(a) New PriceChangeAlert With {
                .Id = a.Id,
                .ProductId = a.ProductId,
                .VendorId = a.VendorId,
                .PreviousUnitCost = a.PreviousUnitCost,
                .NewUnitCost = a.NewUnitCost,
                .ChangePercent = a.ChangePercent,
                .ChangeDirection = a.ChangeDirection,
                .GoodsReceiptId = a.GoodsReceiptId,
                .IsAcknowledged = a.IsAcknowledged,
                .AcknowledgedAt = a.AcknowledgedAt,
                .CreatedAt = a.CreatedAt,
                .CreatedBy = a.CreatedBy,
                .ModifiedAt = a.ModifiedAt,
                .ModifiedBy = a.ModifiedBy
            }).ToList()
        End Function
    End Class
End Namespace
