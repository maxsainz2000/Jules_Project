Imports Xunit
Imports System
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Accounting.Handlers
Imports MerchSys.SharedKernel.Events

''' <summary>
''' Regression tests for the two accounting handlers that share a RevenueRecord row.
'''
''' <c>SaleCompletedAccountingHandler</c> (revenue) and <c>StockDeductedAccountingHandler</c> (COGS)
''' are driven by different events, and MediatR gives no ordering guarantee between them. Both upsert
''' on (SourceTransactionId, ProductId) and recompute gross profit from the persisted row, so the
''' final state must be identical whichever runs first — and a repeat publication must not
''' double-count.
'''
'''   dotnet test --filter "Category=Integration"    # needs a live MariaDB
''' </summary>
<Trait("Category", "Integration")>
<Collection("IntegrationDatabase")>
Public Class SaleAccountingHandlerTests

    Private Const TestSaleId As Integer = 987655
    Private Const TestProductId As Integer = 1
    Private Const TestQuantity As Integer = 3
    Private Const TestUnitPrice As Decimal = 20D
    Private Const TestTotalCost As Decimal = 37.5D

    Private Shared Function RevenueEvent() As SaleCompletedEvent
        Dim completedEvent = New SaleCompletedEvent With {
            .SaleId = TestSaleId,
            .SaleDate = New DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc)
        }
        completedEvent.Items.Add(New SaleCompletedEvent.SaleCompletedItem With {
            .ProductId = TestProductId,
            .QuantitySold = TestQuantity,
            .UnitPrice = TestUnitPrice
        })
        Return completedEvent
    End Function

    Private Shared Function CostEvent() As StockDeductedEvent
        Dim deductedEvent = New StockDeductedEvent With {
            .SaleId = TestSaleId,
            .DeductedAt = New DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc)
        }
        deductedEvent.Items.Add(New StockDeductedEvent.StockDeductedItem With {
            .ProductId = TestProductId,
            .QuantityDeducted = TestQuantity,
            .TotalCost = TestTotalCost
        })
        Return deductedEvent
    End Function

    ''' <summary>COGS handler wins the race; each event is also published twice.</summary>
    <Fact>
    Public Async Function CostThenRevenueProducesOneConsistentRow() As Task
        Using fixture As New TestDatabaseFixture()
            Using ctx = fixture.NewAccounting()
                Await New StockDeductedAccountingHandler(ctx).Handle(CostEvent(), CancellationToken.None)
                Await New StockDeductedAccountingHandler(ctx).Handle(CostEvent(), CancellationToken.None)
            End Using

            Using ctx = fixture.NewAccounting()
                Await New SaleCompletedAccountingHandler(ctx).Handle(RevenueEvent(), CancellationToken.None)
                Await New SaleCompletedAccountingHandler(ctx).Handle(RevenueEvent(), CancellationToken.None)
            End Using

            Await AssertRowIsConsistent(fixture)
        End Using
    End Function

    ''' <summary>Revenue handler wins the race — the other interleaving.</summary>
    <Fact>
    Public Async Function RevenueThenCostProducesOneConsistentRow() As Task
        Using fixture As New TestDatabaseFixture()
            Using ctx = fixture.NewAccounting()
                Await New SaleCompletedAccountingHandler(ctx).Handle(RevenueEvent(), CancellationToken.None)
                Await New SaleCompletedAccountingHandler(ctx).Handle(RevenueEvent(), CancellationToken.None)
            End Using

            Using ctx = fixture.NewAccounting()
                Await New StockDeductedAccountingHandler(ctx).Handle(CostEvent(), CancellationToken.None)
                Await New StockDeductedAccountingHandler(ctx).Handle(CostEvent(), CancellationToken.None)
            End Using

            Await AssertRowIsConsistent(fixture)
        End Using
    End Function

    Private Shared Async Function AssertRowIsConsistent(fixture As TestDatabaseFixture) As Task
        Using ctx = fixture.NewAccounting()
            Dim revenues = Await ctx.RevenueRecords.
                Where(Function(r) r.SourceTransactionId = TestSaleId).
                ToListAsync()

            Dim expenses = Await ctx.ExpenseRecords.
                Where(Function(e) e.SourceReferenceId.HasValue AndAlso e.SourceReferenceId.Value = TestSaleId).
                ToListAsync()

            ' Idempotent: publishing either event twice must not create a second row.
            Assert.Equal(1, revenues.Count)
            Assert.Equal(1, expenses.Count)

            Dim expectedRevenue = CDec(TestQuantity) * TestUnitPrice

            Assert.Equal(expectedRevenue, revenues(0).TotalRevenue)
            Assert.Equal(TestUnitPrice, revenues(0).UnitPrice)
            Assert.Equal(CDec(TestQuantity), revenues(0).QuantitySold)
            Assert.Equal(TestTotalCost, revenues(0).TotalCogs)
            Assert.Equal(TestTotalCost / CDec(TestQuantity), revenues(0).UnitCost)

            ' The point of recomputing on both sides: this must hold whichever handler ran last.
            Assert.Equal(expectedRevenue - TestTotalCost, revenues(0).GrossProfit)
            Assert.Equal(TestTotalCost, expenses(0).Amount)
        End Using
    End Function

End Class
