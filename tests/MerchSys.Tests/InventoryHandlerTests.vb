Imports Xunit
Imports System
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Inventory.Handlers
Imports MerchSys.Inventory.Services
Imports MerchSys.SharedKernel.Events
Imports MerchSys.SharedKernel.Queries

''' <summary>
''' Regression tests for the two Inventory handlers that were missing entirely.
'''
''' C3: <c>GetInventoryValuationQuery</c> was sent by <c>FinancialOverviewService</c> (lines 55 and
''' 135) with no handler registered, so <c>Task.WhenAll</c> faulted and the Financial Overview screen
''' threw.
'''
''' C4: <c>StockReturnedEvent</c> was published by <c>SalesReturnService</c> with no handler, so a
''' sales return never put stock back and inventory stayed short.
'''
'''   dotnet test --filter "Category=Integration"    # needs a live MariaDB
''' </summary>
<Trait("Category", "Integration")>
<Collection("IntegrationDatabase")>
Public Class InventoryHandlerTests

    ''' <summary>
    ''' Remaining stock is valued at each batch's own cost, not a blended figure — the batches really
    ''' were bought at different prices.
    ''' </summary>
    <Fact>
    Public Async Function ValuationValuesRemainingStockAtEachBatchsOwnCost() As Task
        Const productId As Integer = 4
        Const expectedTotal As Decimal = 92D   ' 5 * 10.00 + 3 * 14.00

        Using fixture As New TestDatabaseFixture()
            Using ctx = fixture.NewInventory()
                Dim service As New StockService(ctx)
                Await service.AddStockBatchAsync(productId, 5, 10D, New DateTime(2026, 1, 1), 0, Nothing)
                Await service.AddStockBatchAsync(productId, 3, 14D, New DateTime(2026, 2, 1), 0, Nothing)
            End Using

            Using ctx = fixture.NewInventory()
                Dim result = Await New GetInventoryValuationQueryHandler(ctx).
                    Handle(New GetInventoryValuationQuery(), CancellationToken.None)

                Dim valuation = result.Valuations.First(Function(v) v.ProductId = productId)

                Assert.Equal(8, valuation.Quantity)
                Assert.Equal(expectedTotal, valuation.TotalValue)
                Assert.Equal(expectedTotal / 8D, valuation.UnitCost)
                Assert.False(String.IsNullOrWhiteSpace(valuation.ProductName))
            End Using
        End Using
    End Function

    ''' <summary>
    ''' The actual regression: with no stock at all the query must return an empty result, not throw.
    ''' </summary>
    <Fact>
    Public Async Function ValuationReturnsAnEmptyResultRatherThanThrowing() As Task
        Using fixture As New TestDatabaseFixture()
            Using ctx = fixture.NewInventory()
                Dim result = Await New GetInventoryValuationQueryHandler(ctx).
                    Handle(New GetInventoryValuationQuery(), CancellationToken.None)

                Assert.NotNull(result)
                Assert.Empty(result.Valuations)
            End Using
        End Using
    End Function

    ''' <summary>
    ''' A returned quantity goes back on the shelf. The cost is an estimate — see the handler — but
    ''' the quantity must be exact.
    ''' </summary>
    <Fact>
    Public Async Function SalesReturnRestocksTheReturnedQuantity() As Task
        Const productId As Integer = 5
        Const expectedUnitCost As Decimal = 15D   ' (4 * 10.00 + 4 * 20.00) / 8

        Using fixture As New TestDatabaseFixture()
            Using ctx = fixture.NewInventory()
                Dim service As New StockService(ctx)
                Await service.AddStockBatchAsync(productId, 4, 10D, New DateTime(2026, 1, 1), 0, Nothing)
                Await service.AddStockBatchAsync(productId, 4, 20D, New DateTime(2026, 2, 1), 0, Nothing)
            End Using

            Using ctx = fixture.NewInventory()
                Dim handler As New StockReturnedHandler(ctx, New StockService(ctx), Nothing)
                Await handler.Handle(New StockReturnedEvent(productId, 3), CancellationToken.None)
            End Using

            Using ctx = fixture.NewInventory()
                Dim batches = Await ctx.StockBatches.
                    Where(Function(b) b.ProductId = productId).
                    ToListAsync()

                Assert.Equal(3, batches.Count)

                Dim restocked = batches.OrderByDescending(Function(b) b.Id).First()
                Assert.Equal(3, restocked.QuantityReceived)
                Assert.Equal(3, restocked.QuantityRemaining)
                Assert.Equal(expectedUnitCost, restocked.UnitCost)

                ' 4 + 4 on hand, plus the 3 returned.
                Dim levels = Await New StockService(ctx).GetCurrentStockLevelsAsync()
                Assert.Equal(11, levels.First(Function(l) l.ProductId = productId).CurrentQuantity)
            End Using
        End Using
    End Function

    <Fact>
    Public Async Function SalesReturnWithNoStockOnHandRestocksAtZeroRatherThanFailing() As Task
        Const productId As Integer = 6

        Using fixture As New TestDatabaseFixture()
            Using ctx = fixture.NewInventory()
                Dim handler As New StockReturnedHandler(ctx, New StockService(ctx), Nothing)
                Await handler.Handle(New StockReturnedEvent(productId, 2), CancellationToken.None)
            End Using

            Using ctx = fixture.NewInventory()
                Dim batches = Await ctx.StockBatches.
                    Where(Function(b) b.ProductId = productId).
                    ToListAsync()

                Assert.Equal(1, batches.Count)
                Assert.Equal(0D, batches(0).UnitCost)
                Assert.Equal(2, batches(0).QuantityRemaining)
            End Using
        End Using
    End Function

    <Fact>
    Public Async Function SalesReturnOfNothingIsIgnored() As Task
        Const productId As Integer = 7

        Using fixture As New TestDatabaseFixture()
            Using ctx = fixture.NewInventory()
                Dim handler As New StockReturnedHandler(ctx, New StockService(ctx), Nothing)
                Await handler.Handle(New StockReturnedEvent(productId, 0), CancellationToken.None)
            End Using

            Using ctx = fixture.NewInventory()
                Assert.Empty(Await ctx.StockBatches.Where(Function(b) b.ProductId = productId).ToListAsync())
            End Using
        End Using
    End Function

End Class
