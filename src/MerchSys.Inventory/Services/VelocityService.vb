Imports System.Linq
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Inventory.Data

Namespace Services

    Public Class VelocityService
        Implements IVelocityService

        Private ReadOnly _db As InventoryDbContext

        Public Sub New(db As InventoryDbContext)
            _db = db
        End Sub

        Public Async Function GetProductVelocitiesAsync() As Task(Of List(Of ProductVelocityDto)) Implements IVelocityService.GetProductVelocitiesAsync
            Dim products = Await _db.Products.
                Where(Function(prod) Not prod.IsDeleted).
                Select(Function(prod) New With { prod.Id, prod.Name, prod.SKU, .CurrentStock = _db.StockBatches.Where(Function(b) b.ProductId = prod.Id).Sum(Function(b) CType(b.QuantityRemaining, Integer?)) }).
                ToListAsync()

            Dim batches = Await _db.StockBatches.
                Select(Function(batch) New With { batch.ProductId, batch.QuantityReceived, batch.QuantityRemaining, batch.ReceiptDate }).
                ToListAsync()

            Dim batchLookup = batches.GroupBy(Function(batch) batch.ProductId).ToDictionary(Function(g) g.Key, Function(g) g.ToList())

            Dim results As New List(Of ProductVelocityDto)()

            For Each p In products
                Dim avgDailySales As Decimal = 0D

                If batchLookup.ContainsKey(p.Id) Then
                    Dim pBatches = batchLookup(p.Id)
                    For Each b In pBatches
                        Dim daysSince = (DateTime.Today - b.ReceiptDate.Date).TotalDays
                        Dim days = Math.Max(1D, daysSince)
                        Dim sold = b.QuantityReceived - b.QuantityRemaining

                        If sold > 0 Then
                            avgDailySales += CDec(sold / days)
                        End If
                    Next
                End If

                Dim cat As VelocityCategory
                If avgDailySales <= 0D Then
                    cat = VelocityCategory.Dead
                ElseIf avgDailySales < 1D Then
                    cat = VelocityCategory.Slow
                ElseIf avgDailySales < 5D Then
                    cat = VelocityCategory.Moderate
                Else
                    cat = VelocityCategory.Fast
                End If

                results.Add(New ProductVelocityDto With {
                    .ProductId = p.Id,
                    .ProductName = p.Name,
                    .SKU = p.SKU,
                    .CurrentStock = If(p.CurrentStock.HasValue, p.CurrentStock.Value, 0),
                    .AverageDailySales = avgDailySales,
                    .Category = cat
                })
            Next

            Return results
        End Function

    End Class

End Namespace
