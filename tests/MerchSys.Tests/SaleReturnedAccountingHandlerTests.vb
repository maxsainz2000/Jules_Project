Imports Xunit
Imports System
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Accounting.Handlers
Imports MerchSys.SharedKernel.Events

''' <summary>
''' Regression tests for the sales-return reversal.
'''
''' Nothing used to reverse a return in Accounting at all, so a returned sale kept its full revenue
''' and COGS and gross profit stayed overstated. <c>SaleReturnedAccountingHandler</c> sets the net
''' position for the line.
'''
''' The net figure is <em>assigned</em> from (original − cumulative returned) rather than subtracted,
''' which is what makes it idempotent — the second test publishes the same event twice and expects the
''' same answer.
'''
'''   dotnet test --filter "Category=Integration"    # needs a live MariaDB
''' </summary>
<Trait("Category", "Integration")>
<Collection("IntegrationDatabase")>
Public Class SaleReturnedAccountingHandlerTests

    Private Const TestSaleId As Integer = 882001
    Private Const TestProductId As Integer = 3
    Private Const TestUnitPrice As Decimal = 20D
    Private Const TestUnitCost As Decimal = 10D

    ''' <summary>Books a sale of 5 at 20.00 each costing 10.00, so revenue 100.00 / COGS 50.00.</summary>
    Private Shared Async Function BookSaleAsync(fixture As TestDatabaseFixture) As Task
        Dim completedEvent = New SaleCompletedEvent With {
            .SaleId = TestSaleId,
            .SaleDate = New DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc)
        }
        completedEvent.Items.Add(New SaleCompletedEvent.SaleCompletedItem With {
            .ProductId = TestProductId,
            .QuantitySold = 5,
            .UnitPrice = TestUnitPrice
        })

        Using ctx = fixture.NewAccounting()
            Await New SaleCompletedAccountingHandler(ctx).Handle(completedEvent, CancellationToken.None)
        End Using

        Dim deductedEvent = New StockDeductedEvent With {
            .SaleId = TestSaleId,
            .DeductedAt = New DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc)
        }
        deductedEvent.Items.Add(New StockDeductedEvent.StockDeductedItem With {
            .ProductId = TestProductId,
            .QuantityDeducted = 5,
            .TotalCost = 5D * TestUnitCost
        })

        Using ctx = fixture.NewAccounting()
            Await New StockDeductedAccountingHandler(ctx).Handle(deductedEvent, CancellationToken.None)
        End Using
    End Function

    Private Shared Function ReturnEvent(returnedNow As Integer, cumulative As Integer, original As Integer) As SaleReturnedEvent
        Dim returnedEvent = New SaleReturnedEvent With {
            .SaleId = TestSaleId,
            .SalesReturnId = 1,
            .ReturnedAt = New DateTime(2026, 4, 2, 0, 0, 0, DateTimeKind.Utc),
            .TotalRefundAmount = CDec(returnedNow) * TestUnitPrice,
            .IsRestocked = True
        }
        returnedEvent.Items.Add(New SaleReturnedEvent.ReturnedItem With {
            .ProductId = TestProductId,
            .QuantityReturned = returnedNow,
            .CumulativeQuantityReturned = cumulative,
            .OriginalQuantitySold = original,
            .UnitPrice = TestUnitPrice
        })
        Return returnedEvent
    End Function

    <Fact>
    Public Async Function PartialReturnReducesRevenueAndCogs() As Task
        Using fixture As New TestDatabaseFixture()
            Await BookSaleAsync(fixture)

            Using ctx = fixture.NewAccounting()
                Await New SaleReturnedAccountingHandler(ctx, Nothing).
                    Handle(ReturnEvent(returnedNow:=2, cumulative:=2, original:=5), CancellationToken.None)
            End Using

            Using ctx = fixture.NewAccounting()
                Dim record = Await ctx.RevenueRecords.FirstAsync(
                    Function(r) r.SourceTransactionId = TestSaleId AndAlso r.ProductId = TestProductId)

                Assert.Equal(3D, record.QuantitySold)              ' 5 sold, 2 returned
                Assert.Equal(60D, record.TotalRevenue)             ' 3 * 20.00
                Assert.Equal(30D, record.TotalCogs)                ' 3 * 10.00
                Assert.Equal(30D, record.GrossProfit)

                Dim expense = Await ctx.ExpenseRecords.FirstAsync(
                    Function(e) e.SourceReferenceId.HasValue AndAlso
                                 e.SourceReferenceId.Value = TestSaleId AndAlso
                                 e.Category = "COGS")
                Assert.Equal(30D, expense.Amount)
            End Using
        End Using
    End Function

    ''' <summary>
    ''' The reversal assigns the net position, so publishing the same event again must not reverse a
    ''' second time. A subtracting implementation would leave 1 on the books here instead of 3.
    ''' </summary>
    <Fact>
    Public Async Function ReversalIsIdempotent() As Task
        Using fixture As New TestDatabaseFixture()
            Await BookSaleAsync(fixture)

            For pass = 1 To 2
                Using ctx = fixture.NewAccounting()
                    Await New SaleReturnedAccountingHandler(ctx, Nothing).
                        Handle(ReturnEvent(returnedNow:=2, cumulative:=2, original:=5), CancellationToken.None)
                End Using
            Next

            Using ctx = fixture.NewAccounting()
                Dim record = Await ctx.RevenueRecords.FirstAsync(
                    Function(r) r.SourceTransactionId = TestSaleId AndAlso r.ProductId = TestProductId)

                Assert.Equal(3D, record.QuantitySold)
                Assert.Equal(60D, record.TotalRevenue)
                Assert.Equal(30D, record.TotalCogs)
            End Using
        End Using
    End Function

    ''' <summary>A later return carries the running total, so the net figure stays right.</summary>
    <Fact>
    Public Async Function SecondReturnUsesTheCumulativeQuantity() As Task
        Using fixture As New TestDatabaseFixture()
            Await BookSaleAsync(fixture)

            Using ctx = fixture.NewAccounting()
                Await New SaleReturnedAccountingHandler(ctx, Nothing).
                    Handle(ReturnEvent(returnedNow:=2, cumulative:=2, original:=5), CancellationToken.None)
            End Using

            Using ctx = fixture.NewAccounting()
                Await New SaleReturnedAccountingHandler(ctx, Nothing).
                    Handle(ReturnEvent(returnedNow:=1, cumulative:=3, original:=5), CancellationToken.None)
            End Using

            Using ctx = fixture.NewAccounting()
                Dim record = Await ctx.RevenueRecords.FirstAsync(
                    Function(r) r.SourceTransactionId = TestSaleId AndAlso r.ProductId = TestProductId)

                Assert.Equal(2D, record.QuantitySold)              ' 5 sold, 3 returned in total
                Assert.Equal(40D, record.TotalRevenue)
                Assert.Equal(20D, record.TotalCogs)
                Assert.Equal(20D, record.GrossProfit)
            End Using
        End Using
    End Function

    ''' <summary>Returning everything leaves nothing on the books.</summary>
    <Fact>
    Public Async Function FullReturnZeroesTheLine() As Task
        Using fixture As New TestDatabaseFixture()
            Await BookSaleAsync(fixture)

            Using ctx = fixture.NewAccounting()
                Await New SaleReturnedAccountingHandler(ctx, Nothing).
                    Handle(ReturnEvent(returnedNow:=5, cumulative:=5, original:=5), CancellationToken.None)
            End Using

            Using ctx = fixture.NewAccounting()
                Dim record = Await ctx.RevenueRecords.FirstAsync(
                    Function(r) r.SourceTransactionId = TestSaleId AndAlso r.ProductId = TestProductId)

                Assert.Equal(0D, record.QuantitySold)
                Assert.Equal(0D, record.TotalRevenue)
                Assert.Equal(0D, record.TotalCogs)
                Assert.Equal(0D, record.GrossProfit)

                Dim expense = Await ctx.ExpenseRecords.FirstAsync(
                    Function(e) e.SourceReferenceId.HasValue AndAlso
                                 e.SourceReferenceId.Value = TestSaleId AndAlso
                                 e.Category = "COGS")
                Assert.Equal(0D, expense.Amount)
            End Using
        End Using
    End Function

End Class
