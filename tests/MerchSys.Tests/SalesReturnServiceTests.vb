Imports Xunit
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text.Json
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports MediatR
Imports MerchSys.POS.Entities
Imports MerchSys.POS.Services
Imports MerchSys.SharedKernel.Events
Imports MerchSys.SharedKernel.Interfaces

''' <summary>
''' Tests for the sales-return service, which is where items 2 and 3 begin.
'''
''' It previously published a bare <c>StockReturnedEvent</c> — product and quantity only — inside the
''' line loop and before <c>SaveChangesAsync</c>, and stored no line detail on <c>SalesReturn</c> at
''' all. That meant Accounting never heard about returns, the returned quantity for a line could not be
''' reconstructed, and a failed save left stock restocked with no return record.
'''
'''   dotnet test --filter "Category=Integration"    # needs a live MariaDB
''' </summary>
<Trait("Category", "Integration")>
<Collection("IntegrationDatabase")>
Public Class SalesReturnServiceTests

    Private Const TestProductId As Integer = 12
    Private Const PurchasedQuantity As Integer = 5
    Private Const TestUnitPrice As Decimal = 20D

    Private Class CapturingEventBus
        Implements IEventBus

        Public ReadOnly Published As New List(Of Object)()

        Public Function PublishAsync(Of TEvent As INotification)([event] As TEvent) As Task Implements IEventBus.PublishAsync
            Published.Add([event])
            Return Task.CompletedTask
        End Function
    End Class

    Private Shared Async Function CreateSaleAsync(fixture As TestDatabaseFixture) As Task(Of Tuple(Of Integer, Integer))
        Using ctx = fixture.NewPOS()
            Dim transaction As New SalesTransaction With {
                .TransactionNumber = "TX-RET-" & Guid.NewGuid().ToString("N").Substring(0, 8),
                .TransactionDate = New DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc),
                .TotalAmount = PurchasedQuantity * TestUnitPrice,
                .PaymentMethod = "Cash",
                .IsVoid = False
            }
            transaction.Lines.Add(New SalesTransactionLine With {
                .ProductId = TestProductId,
                .ProductName = "Return Test Product",
                .UnitPrice = TestUnitPrice,
                .Quantity = PurchasedQuantity,
                .LineTotal = PurchasedQuantity * TestUnitPrice
            })

            ctx.SalesTransactions.Add(transaction)
            Await ctx.SaveChangesAsync()

            Return Tuple.Create(transaction.Id, transaction.Lines(0).Id)
        End Using
    End Function

    <Fact>
    Public Async Function ReturnPublishesOneEventCarryingTheFullContext() As Task
        Using fixture As New TestDatabaseFixture()
            Dim sale = Await CreateSaleAsync(fixture)
            Dim bus As New CapturingEventBus()
            Dim serialized As String

            Using ctx = fixture.NewPOS()
                Dim service As New SalesReturnService(ctx, bus)
                Dim result = Await service.ProcessReturnAsync(
                    sale.Item1,
                    New Dictionary(Of Integer, Integer) From {{sale.Item2, 2}},
                    "Damaged",
                    True)

                serialized = result.SerializedLines
            End Using

            ' The line detail is now persisted, which is what makes the returned quantity reconstructable.
            Assert.False(String.IsNullOrWhiteSpace(serialized))

            Dim persistedLines = JsonSerializer.Deserialize(Of List(Of SalesReturnService.ReturnedLineRecord))(serialized)
            Assert.NotNull(persistedLines)
            Assert.Single(persistedLines)
            Assert.Equal(TestProductId, persistedLines(0).ProductId)
            Assert.Equal(2, persistedLines(0).Quantity)
            Assert.Equal(TestUnitPrice, persistedLines(0).UnitPrice)

            ' Exactly one event, and it carries everything both consumers need.
            Dim published = Assert.Single(bus.Published)
            Dim returnedEvent = Assert.IsType(Of SaleReturnedEvent)(published)

            Assert.Equal(sale.Item1, returnedEvent.SaleId)
            Assert.True(returnedEvent.IsRestocked)
            Assert.Equal(2D * TestUnitPrice, returnedEvent.TotalRefundAmount)

            Dim item = Assert.Single(returnedEvent.Items)
            Assert.Equal(TestProductId, item.ProductId)
            Assert.Equal(2, item.QuantityReturned)
            Assert.Equal(2, item.CumulativeQuantityReturned)
            Assert.Equal(PurchasedQuantity, item.OriginalQuantitySold)
            Assert.Equal(TestUnitPrice, item.UnitPrice)
        End Using
    End Function

    ''' <summary>
    ''' The cumulative figure is what lets Accounting set the net position, so it has to count earlier
    ''' returns too.
    ''' </summary>
    <Fact>
    Public Async Function SecondReturnReportsTheCumulativeQuantity() As Task
        Using fixture As New TestDatabaseFixture()
            Dim sale = Await CreateSaleAsync(fixture)

            Using ctx = fixture.NewPOS()
                Dim service As New SalesReturnService(ctx, New CapturingEventBus())
                Await service.ProcessReturnAsync(sale.Item1, New Dictionary(Of Integer, Integer) From {{sale.Item2, 2}}, "First", True)
            End Using

            Dim bus As New CapturingEventBus()
            Using ctx = fixture.NewPOS()
                Dim service As New SalesReturnService(ctx, bus)
                Await service.ProcessReturnAsync(sale.Item1, New Dictionary(Of Integer, Integer) From {{sale.Item2, 1}}, "Second", True)
            End Using

            Dim returnedEvent = Assert.IsType(Of SaleReturnedEvent)(Assert.Single(bus.Published))
            Dim item = Assert.Single(returnedEvent.Items)

            Assert.Equal(1, item.QuantityReturned)            ' this return only
            Assert.Equal(3, item.CumulativeQuantityReturned)  ' 2 earlier + 1 now
            Assert.Equal(PurchasedQuantity, item.OriginalQuantitySold)
        End Using
    End Function

    ''' <summary>
    ''' Previously the check compared only against the original line quantity, so a customer could
    ''' return more than they bought across several returns.
    ''' </summary>
    <Fact>
    Public Async Function ReturningMoreThanPurchasedAcrossReturnsIsRejected() As Task
        Using fixture As New TestDatabaseFixture()
            Dim sale = Await CreateSaleAsync(fixture)

            Using ctx = fixture.NewPOS()
                Dim service As New SalesReturnService(ctx, New CapturingEventBus())
                Await service.ProcessReturnAsync(sale.Item1, New Dictionary(Of Integer, Integer) From {{sale.Item2, 3}}, "First", True)
            End Using

            Using ctx = fixture.NewPOS()
                Dim service As New SalesReturnService(ctx, New CapturingEventBus())
                Dim ex = Await Assert.ThrowsAsync(Of InvalidOperationException)(
                    Function() service.ProcessReturnAsync(sale.Item1, New Dictionary(Of Integer, Integer) From {{sale.Item2, 3}}, "Too many", True))

                Assert.Contains("already been returned", ex.Message)
            End Using
        End Using
    End Function

    <Fact>
    Public Async Function ReturningTheWholeLineIsAllowed() As Task
        Using fixture As New TestDatabaseFixture()
            Dim sale = Await CreateSaleAsync(fixture)
            Dim bus As New CapturingEventBus()

            Using ctx = fixture.NewPOS()
                Dim service As New SalesReturnService(ctx, bus)
                Await service.ProcessReturnAsync(
                    sale.Item1,
                    New Dictionary(Of Integer, Integer) From {{sale.Item2, PurchasedQuantity}},
                    "Everything",
                    True)
            End Using

            Dim item = Assert.Single(Assert.IsType(Of SaleReturnedEvent)(Assert.Single(bus.Published)).Items)
            Assert.Equal(PurchasedQuantity, item.QuantityReturned)
            Assert.Equal(PurchasedQuantity, item.CumulativeQuantityReturned)
        End Using
    End Function

End Class
