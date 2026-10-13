Imports System.Threading
Imports System.Threading.Tasks
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Inventory.Data
Imports MerchSys.SharedKernel.Queries

Namespace Handlers

    ''' <summary>
    ''' Values the remaining stock at the cost of the batches actually on hand.
    '''
    ''' <c>FinancialOverviewService</c> sends this query twice (lines 55 and 135) and sums
    ''' <c>Valuations.TotalValue</c>, but no handler existed — so <c>Task.WhenAll</c> faulted and the
    ''' Financial Overview screen threw. It lives in Inventory because Inventory owns the batches,
    ''' following the same pattern as GetTotalARQueryHandler (POS) and GetTotalAPQueryHandler
    ''' (Purchasing).
    '''
    ''' Valuation uses each batch's own UnitCost rather than a product-level average: remaining stock
    ''' is literally a set of batches bought at different prices, and valuing it at an average would
    ''' discard that.
    ''' </summary>
    Public Class GetInventoryValuationQueryHandler
        Implements IRequestHandler(Of GetInventoryValuationQuery, GetInventoryValuationResult)

        Private ReadOnly _dbContext As InventoryDbContext

        Public Sub New(dbContext As InventoryDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function Handle(request As GetInventoryValuationQuery, cancellationToken As CancellationToken) As Task(Of GetInventoryValuationResult) Implements IRequestHandler(Of GetInventoryValuationQuery, GetInventoryValuationResult).Handle
            ' Aggregated in the database rather than by loading every batch: a product's batches are
            ' the only rows needed, and the sums are what we actually want.
            Dim totals = Await _dbContext.StockBatches.
                Where(Function(b) b.QuantityRemaining > 0).
                GroupBy(Function(b) b.ProductId).
                Select(Function(g) New With {
                    .ProductId = g.Key,
                    .Quantity = g.Sum(Function(b) b.QuantityRemaining),
                    .TotalValue = g.Sum(Function(b) b.QuantityRemaining * b.UnitCost)
                }).
                ToListAsync(cancellationToken)

            Dim result As New GetInventoryValuationResult()
            If totals.Count = 0 Then Return result

            Dim productIds = totals.Select(Function(t) t.ProductId).ToList()
            Dim names = Await _dbContext.Products.
                Where(Function(p) productIds.Contains(p.Id)).
                Select(Function(p) New With {p.Id, p.Name}).
                ToListAsync(cancellationToken)

            For Each total In totals
                Dim name = names.FirstOrDefault(Function(n) n.Id = total.ProductId)?.Name

                result.Valuations.Add(New ProductValuation With {
                    .ProductId = total.ProductId,
                    .ProductName = If(name, String.Empty),
                    .Quantity = total.Quantity,
                    .UnitCost = If(total.Quantity > 0, total.TotalValue / CDec(total.Quantity), 0D),
                    .TotalValue = total.TotalValue
                })
            Next

            Return result
        End Function

    End Class

End Namespace
