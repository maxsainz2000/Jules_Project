Imports Xunit
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.DependencyInjection
Imports MerchSys.Inventory.Handlers
Imports MerchSys.Inventory.Services
Imports MerchSys.SharedKernel.Events
Imports MerchSys.SharedKernel.Queries

''' <summary>
''' Tests for the Inventory handlers behind the two follow-up items.
'''
''' C3: <c>GetInventoryValuationQuery</c> was sent by <c>FinancialOverviewService</c> with no handler
''' registered, so <c>Task.WhenAll</c> faulted and the Financial Overview screen threw.
'''
''' Item 3: <c>SaleReturnedEvent</c> now carries enough for Inventory to restock, and the returned goods
''' are valued at the cost the sale was booked at — fetched from Accounting — rather than the cost of
''' whatever batches happen to be left.
'''
'''   dotnet test --filter "Category=Integration"    # needs a live MariaDB
''' </summary>
<Trait("Category", "Integration")>
<Collection("IntegrationDatabase")>
Public Class InventoryHandlerTests

    Private Const TestSaleId As Integer = 771001

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
    ''' Item 3: the returned goods take the cost the sale was booked at, not the weighted average of
    ''' what is left. Here the remaining stock averages 15.00 while the sale cost 12.50 — the recorded
    ''' figure is the correct one, and using the average would inflate inventory.
    ''' </summary>
    <Fact>
    Public Async Function SalesReturnRestocksAtTheCostTheSaleWasBookedAt() As Task
        Const productId As Integer = 5
        Const recordedCost As Decimal = 12.5D

        Using fixture As New TestDatabaseFixture()
            Using ctx = fixture.NewInventory()
                Dim service As New StockService(ctx)
                Await service.AddStockBatchAsync(productId, 4, 10D, New DateTime(2026, 1, 1), 0, Nothing)
                Await service.AddStockBatchAsync(productId, 4, 20D, New DateTime(2026, 2, 1), 0, Nothing)
            End Using

            Using ctx = fixture.NewInventory()
                Dim mediator = BuildMediator(New Dictionary(Of Integer, Decimal) From {{productId, recordedCost}})
                Dim handler As New StockReturnedHandler(ctx, New StockService(ctx), mediator, Nothing)
                Await handler.Handle(ReturnEvent(productId, 3, True), CancellationToken.None)
            End Using

            Using ctx = fixture.NewInventory()
                Dim batches = Await ctx.StockBatches.Where(Function(b) b.ProductId = productId).ToListAsync()
                Assert.Equal(3, batches.Count)

                Dim restocked = batches.OrderByDescending(Function(b) b.Id).First()
                Assert.Equal(3, restocked.QuantityRemaining)
                Assert.Equal(recordedCost, restocked.UnitCost)

                ' 4 + 4 on hand, plus the 3 returned.
                Dim levels = Await New StockService(ctx).GetCurrentStockLevelsAsync()
                Assert.Equal(11, levels.First(Function(l) l.ProductId = productId).CurrentQuantity)
            End Using
        End Using
    End Function

    ''' <summary>
    ''' Only when no cost was ever recorded does it fall back to the weighted average of what is on
    ''' hand — an approximation, but better than zero.
    ''' </summary>
    <Fact>
    Public Async Function SalesReturnFallsBackToTheAverageWhenNoCostWasRecorded() As Task
        Const productId As Integer = 6
        Const expectedAverage As Decimal = 15D   ' (4 * 10.00 + 4 * 20.00) / 8

        Using fixture As New TestDatabaseFixture()
            Using ctx = fixture.NewInventory()
                Dim service As New StockService(ctx)
                Await service.AddStockBatchAsync(productId, 4, 10D, New DateTime(2026, 1, 1), 0, Nothing)
                Await service.AddStockBatchAsync(productId, 4, 20D, New DateTime(2026, 2, 1), 0, Nothing)
            End Using

            Using ctx = fixture.NewInventory()
                Dim mediator = BuildMediator(New Dictionary(Of Integer, Decimal)())
                Dim handler As New StockReturnedHandler(ctx, New StockService(ctx), mediator, Nothing)
                Await handler.Handle(ReturnEvent(productId, 3, True), CancellationToken.None)
            End Using

            Using ctx = fixture.NewInventory()
                Dim restocked = (Await ctx.StockBatches.
                    Where(Function(b) b.ProductId = productId).
                    ToListAsync()).
                    OrderByDescending(Function(b) b.Id).
                    First()

                Assert.Equal(expectedAverage, restocked.UnitCost)
            End Using
        End Using
    End Function

    ''' <summary>
    ''' Last resort: nothing recorded and nothing on hand. Zero is wrong but it is logged, and the
    ''' quantity is still exact.
    ''' </summary>
    <Fact>
    Public Async Function SalesReturnWithNoCostAndNoStockRestocksAtZero() As Task
        Const productId As Integer = 7

        Using fixture As New TestDatabaseFixture()
            Using ctx = fixture.NewInventory()
                Dim mediator = BuildMediator(New Dictionary(Of Integer, Decimal)())
                Dim handler As New StockReturnedHandler(ctx, New StockService(ctx), mediator, Nothing)
                Await handler.Handle(ReturnEvent(productId, 2, True), CancellationToken.None)
            End Using

            Using ctx = fixture.NewInventory()
                Dim batches = Await ctx.StockBatches.Where(Function(b) b.ProductId = productId).ToListAsync()
                Assert.Equal(1, batches.Count)
                Assert.Equal(0D, batches(0).UnitCost)
                Assert.Equal(2, batches(0).QuantityRemaining)
            End Using
        End Using
    End Function

    ''' <summary>
    ''' A return that was not put back on the shelf must not touch inventory.
    ''' </summary>
    <Fact>
    Public Async Function SalesReturnIsIgnoredWhenNotRestocked() As Task
        Const productId As Integer = 10

        Using fixture As New TestDatabaseFixture()
            Using ctx = fixture.NewInventory()
                Dim mediator = BuildMediator(New Dictionary(Of Integer, Decimal) From {{productId, 5D}})
                Dim handler As New StockReturnedHandler(ctx, New StockService(ctx), mediator, Nothing)
                Await handler.Handle(ReturnEvent(productId, 2, False), CancellationToken.None)
            End Using

            Using ctx = fixture.NewInventory()
                Assert.Empty(Await ctx.StockBatches.Where(Function(b) b.ProductId = productId).ToListAsync())
            End Using
        End Using
    End Function

    <Fact>
    Public Async Function SalesReturnOfNothingIsIgnored() As Task
        Const productId As Integer = 11

        Using fixture As New TestDatabaseFixture()
            Using ctx = fixture.NewInventory()
                Dim mediator = BuildMediator(New Dictionary(Of Integer, Decimal) From {{productId, 5D}})
                Dim handler As New StockReturnedHandler(ctx, New StockService(ctx), mediator, Nothing)
                Await handler.Handle(ReturnEvent(productId, 0, True), CancellationToken.None)
            End Using

            Using ctx = fixture.NewInventory()
                Assert.Empty(Await ctx.StockBatches.Where(Function(b) b.ProductId = productId).ToListAsync())
            End Using
        End Using
    End Function

    Private Shared Function ReturnEvent(productId As Integer, quantity As Integer, isRestocked As Boolean) As SaleReturnedEvent
        Dim returnedEvent = New SaleReturnedEvent With {
            .SaleId = TestSaleId,
            .SalesReturnId = 1,
            .ReturnedAt = New DateTime(2026, 3, 5, 0, 0, 0, DateTimeKind.Utc),
            .IsRestocked = isRestocked
        }
        returnedEvent.Items.Add(New SaleReturnedEvent.ReturnedItem With {
            .ProductId = productId,
            .QuantityReturned = quantity,
            .CumulativeQuantityReturned = quantity,
            .OriginalQuantitySold = quantity,
            .UnitPrice = 20D
        })
        Return returnedEvent
    End Function

    ''' <summary>
    ''' Stands in for Accounting, which owns the recorded costs.
    ''' </summary>
    Private Shared Function BuildMediator(recordedCosts As Dictionary(Of Integer, Decimal)) As IMediator
        Dim services As New ServiceCollection()
        services.AddLogging()
        services.AddMediatR(Sub(cfg) cfg.RegisterServicesFromAssembly(GetType(StockReturnedHandler).Assembly))
        services.AddSingleton(Of IRequestHandler(Of GetSaleLineCostsQuery, GetSaleLineCostsResult))(New FakeCostQueryHandler(recordedCosts))
        Return services.BuildServiceProvider().GetRequiredService(Of IMediator)()
    End Function

    Private Class FakeCostQueryHandler
        Implements IRequestHandler(Of GetSaleLineCostsQuery, GetSaleLineCostsResult)

        Private ReadOnly _recordedCosts As Dictionary(Of Integer, Decimal)

        Public Sub New(recordedCosts As Dictionary(Of Integer, Decimal))
            _recordedCosts = recordedCosts
        End Sub

        Public Function Handle(request As GetSaleLineCostsQuery, cancellationToken As CancellationToken) As Task(Of GetSaleLineCostsResult) Implements IRequestHandler(Of GetSaleLineCostsQuery, GetSaleLineCostsResult).Handle
            Dim result As New GetSaleLineCostsResult()
            For Each productId In _recordedCosts.Keys
                result.Costs.Add(New SaleLineCost With {.ProductId = productId, .UnitCost = _recordedCosts(productId)})
            Next
            Return Task.FromResult(result)
        End Function
    End Class

End Class
