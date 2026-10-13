Imports Xunit
Imports System
Imports System.Threading
Imports System.Threading.Tasks
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.DependencyInjection
Imports MerchSys.Accounting.Handlers
Imports MerchSys.SharedKernel.Events
Imports MerchSys.SharedKernel.Queries

''' <summary>
''' Regression tests for the sale-completion accounting handler.
'''
''' One sale can be described by two different events — <c>SaleCompletedEvent</c> (published by
''' <c>PaymentService</c>) and <c>SaleCompletedWithVatEvent</c> (published by
''' <c>VatAwareReceiptService</c>) — and MediatR does not guarantee the order its handlers run in.
''' The handler used to call <c>RevenueRecords.Add(...)</c> unconditionally, so whichever handler
''' ran second inserted a SECOND row for the same (sale, product) and revenue was double-counted.
'''
'''   dotnet test --filter "Category=Integration"    # needs a live MariaDB
'''
''' Shares the "IntegrationDatabase" collection: every fixture instance drops and recreates the
''' same throwaway database, and xUnit runs test classes in parallel by default, so a second
''' DB-backed class would tear the database out from under the first.
''' </summary>
<Trait("Category", "Integration")>
<Collection("IntegrationDatabase")>
Public Class SaleCompletedAccountingHandlerTests

    Private Const TestSaleId As Integer = 987654
    Private Const TestProductId As Integer = 1
    Private Const TestQuantitySold As Integer = 3
    Private Const FixedUnitCost As Decimal = 12.5D

    <Fact>
    Public Async Function RepeatedPublicationRecordsOneRevenueAndOneCogsRow() As Task
        Using fixture As New TestDatabaseFixture()
            Dim mediator = BuildMediator()

            Dim completedEvent = New SaleCompletedEvent With {
                .SaleId = TestSaleId,
                .SaleDate = New DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc)
            }
            completedEvent.Items.Add(New SaleCompletedEvent.SaleCompletedItem With {
                .ProductId = TestProductId,
                .QuantitySold = TestQuantitySold
            })

            ' Publish twice — exactly what happens when both sale-completed events describe one sale.
            For pass = 1 To 2
                Using ctx = fixture.NewAccounting()
                    Dim handler As New SaleCompletedAccountingHandler(ctx, mediator)
                    Await handler.Handle(completedEvent, CancellationToken.None)
                End Using
            Next

            Using ctx = fixture.NewAccounting()
                Dim revenues = Await ctx.RevenueRecords.
                    Where(Function(r) r.SourceTransactionId = TestSaleId).
                    ToListAsync()

                Dim expenses = Await ctx.ExpenseRecords.
                    Where(Function(e) e.SourceReferenceId.HasValue AndAlso e.SourceReferenceId.Value = TestSaleId).
                    ToListAsync()

                Assert.Equal(1, revenues.Count)
                Assert.Equal(1, expenses.Count)

                Dim expectedCogs = CDec(TestQuantitySold) * FixedUnitCost
                Assert.Equal(expectedCogs, revenues(0).TotalCogs)
                Assert.Equal(expectedCogs, expenses(0).Amount)
            End Using
        End Using
    End Function

    ''' <summary>
    ''' The real <c>GetProductCostQuery</c> handler lives in MerchSys.Inventory, which this test
    ''' project does not reference, so a fixed unit cost is supplied instead. Cost is orthogonal to
    ''' the idempotency under test.
    ''' </summary>
    Private Shared Function BuildMediator() As IMediator
        Dim services As New ServiceCollection()
        ' MediatR 14 resolves ILoggerFactory during registration.
        services.AddLogging()
        services.AddMediatR(Sub(cfg) cfg.RegisterServicesFromAssembly(GetType(SaleCompletedAccountingHandler).Assembly))

        ' Registered by Type rather than the generic overload: VB parses the comma inside
        ' AddTransient(Of IRequestHandler(Of A, B), Impl) as its own type-argument separator.
        services.AddTransient(
            GetType(IRequestHandler(Of GetProductCostQuery, GetProductCostResult)),
            GetType(FixedCostQueryHandler))

        Return services.BuildServiceProvider().GetRequiredService(Of IMediator)()
    End Function

    Private Class FixedCostQueryHandler
        Implements IRequestHandler(Of GetProductCostQuery, GetProductCostResult)

        Public Function Handle(request As GetProductCostQuery, cancellationToken As CancellationToken) As Task(Of GetProductCostResult) Implements IRequestHandler(Of GetProductCostQuery, GetProductCostResult).Handle
            Return Task.FromResult(New GetProductCostResult With {
                .ProductId = request.ProductId,
                .FifoUnitCost = FixedUnitCost
            })
        End Function
    End Class

End Class
