Imports Xunit
Imports System
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Accounting.Handlers
Imports MerchSys.SharedKernel.Enums
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
''' same answer. The same assignment pattern covers the output VAT reversal and the damaged-goods
''' expense booked for a non-restocked return.
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

    ' The sale books 100.00 of revenue: 90.00 vatable + 10.00 output VAT.
    Private Const TestVatableAmount As Decimal = 90D
    Private Const TestOutputVat As Decimal = 10D

    ''' <summary>
    ''' Books a sale of 5 at 20.00 each costing 10.00, so revenue 100.00 / COGS 50.00, with the VAT
    ''' breakdown the VAT handler records (90.00 vatable, 10.00 output VAT).
    ''' </summary>
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

        Dim vatEvent = New SaleCompletedWithVatEvent With {
            .SaleId = TestSaleId,
            .VatableSales = TestVatableAmount,
            .OutputVat = TestOutputVat,
            .IsVatRegistered = True
        }
        vatEvent.Items.Add(New SaleCompletedWithVatEvent.SaleItemWithVat With {
            .ProductId = TestProductId,
            .Treatment = VatTreatment.Vatable,
            .VatableAmount = TestVatableAmount,
            .VatExemptAmount = 0D,
            .ZeroRatedAmount = 0D,
            .OutputVat = TestOutputVat
        })

        Using ctx = fixture.NewAccounting()
            Await New SaleCompletedWithVatHandler(ctx).Handle(vatEvent, CancellationToken.None)
        End Using
    End Function

    Private Shared Function ReturnEvent(returnedNow As Integer, cumulative As Integer, original As Integer,
                                        Optional isRestocked As Boolean = True,
                                        Optional salesReturnId As Integer = 1,
                                        Optional originalVatable As Decimal = TestVatableAmount,
                                        Optional originalOutputVat As Decimal = TestOutputVat) As SaleReturnedEvent
        Dim returnedEvent = New SaleReturnedEvent With {
            .SaleId = TestSaleId,
            .SalesReturnId = salesReturnId,
            .ReturnedAt = New DateTime(2026, 4, 2, 0, 0, 0, DateTimeKind.Utc),
            .TotalRefundAmount = CDec(returnedNow) * TestUnitPrice,
            .IsRestocked = isRestocked
        }
        returnedEvent.Items.Add(New SaleReturnedEvent.ReturnedItem With {
            .ProductId = TestProductId,
            .QuantityReturned = returnedNow,
            .CumulativeQuantityReturned = cumulative,
            .OriginalQuantitySold = original,
            .UnitPrice = TestUnitPrice,
            .OriginalVatableAmount = originalVatable,
            .OriginalVatExemptAmount = 0D,
            .OriginalZeroRatedAmount = 0D,
            .OriginalOutputVat = originalOutputVat
        })
        Return returnedEvent
    End Function

    Private Shared Async Function ApplyReturnAsync(fixture As TestDatabaseFixture, returnedEvent As SaleReturnedEvent) As Task
        Using ctx = fixture.NewAccounting()
            Await New SaleReturnedAccountingHandler(ctx, Nothing).Handle(returnedEvent, CancellationToken.None)
        End Using
    End Function

    <Fact>
    Public Async Function PartialReturnReducesRevenueAndCogs() As Task
        Using fixture As New TestDatabaseFixture()
            Await BookSaleAsync(fixture)
            Await ApplyReturnAsync(fixture, ReturnEvent(returnedNow:=2, cumulative:=2, original:=5))

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
                Await ApplyReturnAsync(fixture, ReturnEvent(returnedNow:=2, cumulative:=2, original:=5))
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

            Await ApplyReturnAsync(fixture, ReturnEvent(returnedNow:=2, cumulative:=2, original:=5))
            Await ApplyReturnAsync(fixture, ReturnEvent(returnedNow:=1, cumulative:=3, original:=5))

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
            Await ApplyReturnAsync(fixture, ReturnEvent(returnedNow:=5, cumulative:=5, original:=5))

            Using ctx = fixture.NewAccounting()
                Dim record = Await ctx.RevenueRecords.FirstAsync(
                    Function(r) r.SourceTransactionId = TestSaleId AndAlso r.ProductId = TestProductId)

                Assert.Equal(0D, record.QuantitySold)
                Assert.Equal(0D, record.TotalRevenue)
                Assert.Equal(0D, record.TotalCogs)
                Assert.Equal(0D, record.GrossProfit)
                Assert.Equal(0D, record.OutputVat)
                Assert.Equal(0D, record.VatableAmount)

                Dim expense = Await ctx.ExpenseRecords.FirstAsync(
                    Function(e) e.SourceReferenceId.HasValue AndAlso
                                 e.SourceReferenceId.Value = TestSaleId AndAlso
                                 e.Category = "COGS")
                Assert.Equal(0D, expense.Amount)
            End Using
        End Using
    End Function

    ''' <summary>
    ''' The VAT return sums <c>RevenueRecord.OutputVat</c> and the VAT base it is reported with, so a
    ''' return has to bring both down — otherwise output VAT (and net VAT payable) stays overstated.
    ''' Reversed in proportion to the units returned: 3 of 5 kept → 60% of 90.00 and 10.00.
    ''' </summary>
    <Fact>
    Public Async Function ReturnReversesOutputVatProportionally() As Task
        Using fixture As New TestDatabaseFixture()
            Await BookSaleAsync(fixture)
            Await ApplyReturnAsync(fixture, ReturnEvent(returnedNow:=2, cumulative:=2, original:=5))

            Using ctx = fixture.NewAccounting()
                Dim record = Await ctx.RevenueRecords.FirstAsync(
                    Function(r) r.SourceTransactionId = TestSaleId AndAlso r.ProductId = TestProductId)

                Assert.Equal(54D, record.VatableAmount)            ' 90.00 * 3/5
                Assert.Equal(6D, record.OutputVat)                 ' 10.00 * 3/5
                Assert.Equal(0D, record.VatExemptAmount)
                Assert.Equal(0D, record.ZeroRatedAmount)
            End Using
        End Using
    End Function

    ''' <summary>
    ''' The VAT base is taken from the event, never from the record's own columns — which the first
    ''' pass has already overwritten. A subtracting or record-based implementation would compound
    ''' here and drive the VAT to 2.40 instead of 6.00.
    ''' </summary>
    <Fact>
    Public Async Function VatReversalIsIdempotent() As Task
        Using fixture As New TestDatabaseFixture()
            Await BookSaleAsync(fixture)

            For pass = 1 To 2
                Await ApplyReturnAsync(fixture, ReturnEvent(returnedNow:=2, cumulative:=2, original:=5))
            Next

            Using ctx = fixture.NewAccounting()
                Dim record = Await ctx.RevenueRecords.FirstAsync(
                    Function(r) r.SourceTransactionId = TestSaleId AndAlso r.ProductId = TestProductId)

                Assert.Equal(54D, record.VatableAmount)
                Assert.Equal(6D, record.OutputVat)
            End Using
        End Using
    End Function

    ''' <summary>
    ''' Damaged goods that are discarded are a loss, not a cost of sale. COGS is brought down to the
    ''' net quantity, so without this expense the cost of the two discarded units (2 * 10.00) would
    ''' leave the P&amp;L altogether.
    ''' </summary>
    <Fact>
    Public Async Function NonRestockedReturnBooksADamagedGoodsExpense() As Task
        Using fixture As New TestDatabaseFixture()
            Await BookSaleAsync(fixture)
            Await ApplyReturnAsync(fixture, ReturnEvent(returnedNow:=2, cumulative:=2, original:=5, isRestocked:=False))

            Using ctx = fixture.NewAccounting()
                Dim damaged = Await ctx.ExpenseRecords.FirstAsync(Function(e) e.Category = "Damaged Goods")
                Assert.Equal(20D, damaged.Amount)                  ' 2 * 10.00
                Assert.Equal(TestSaleId, damaged.SourceReferenceId)
                Assert.Equal("POS", damaged.SourceModule)
            End Using
        End Using
    End Function

    ''' <summary>
    ''' A restocked return puts the goods back in inventory, so there is no loss to book — the cost
    ''' stays on the balance sheet as stock.
    ''' </summary>
    <Fact>
    Public Async Function RestockedReturnBooksNoDamagedGoodsExpense() As Task
        Using fixture As New TestDatabaseFixture()
            Await BookSaleAsync(fixture)
            Await ApplyReturnAsync(fixture, ReturnEvent(returnedNow:=2, cumulative:=2, original:=5, isRestocked:=True))

            Using ctx = fixture.NewAccounting()
                Assert.Empty(Await ctx.ExpenseRecords.Where(Function(e) e.Category = "Damaged Goods").ToListAsync())
            End Using
        End Using
    End Function

    ''' <summary>
    ''' Keyed per return, so a repeated publication re-sets the same amount instead of booking the
    ''' loss a second time.
    ''' </summary>
    <Fact>
    Public Async Function DamagedGoodsExpenseIsIdempotent() As Task
        Using fixture As New TestDatabaseFixture()
            Await BookSaleAsync(fixture)

            For pass = 1 To 2
                Await ApplyReturnAsync(fixture, ReturnEvent(returnedNow:=2, cumulative:=2, original:=5, isRestocked:=False))
            Next

            Using ctx = fixture.NewAccounting()
                Dim damaged = Await ctx.ExpenseRecords.Where(Function(e) e.Category = "Damaged Goods").ToListAsync()
                Assert.Single(damaged)
                Assert.Equal(20D, damaged(0).Amount)
            End Using
        End Using
    End Function

    ''' <summary>
    ''' Each return is its own loss event, so two damaged returns produce two expense rows rather
    ''' than one that silently grows.
    ''' </summary>
    <Fact>
    Public Async Function TwoDamagedReturnsBookTwoExpenses() As Task
        Using fixture As New TestDatabaseFixture()
            Await BookSaleAsync(fixture)

            Await ApplyReturnAsync(fixture, ReturnEvent(returnedNow:=2, cumulative:=2, original:=5, isRestocked:=False, salesReturnId:=1))
            Await ApplyReturnAsync(fixture, ReturnEvent(returnedNow:=1, cumulative:=3, original:=5, isRestocked:=False, salesReturnId:=2))

            Using ctx = fixture.NewAccounting()
                Dim damaged = Await ctx.ExpenseRecords.Where(Function(e) e.Category = "Damaged Goods").OrderBy(Function(e) e.Amount).ToListAsync()
                Assert.Equal(2, damaged.Count)
                Assert.Equal(10D, damaged(0).Amount)               ' 1 * 10.00
                Assert.Equal(20D, damaged(1).Amount)               ' 2 * 10.00
            End Using
        End Using
    End Function

End Class
