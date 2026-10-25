Imports Xunit
Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Logging.Abstractions
Imports MediatR
Imports MerchSys.Inventory.Data
Imports MerchSys.Inventory.Handlers
Imports System.Text.Json
Imports MerchSys.Inventory.Services
Imports MerchSys.SharedKernel.Events
Imports MerchSys.SharedKernel.Exceptions

''' <summary>
''' DB-backed coverage for the C5 concurrency fixes: the FIFO decrement is serialized with
''' <c>SELECT ... FOR UPDATE</c>, a sale's item set is atomic, and a rolled-back deduction is never
''' announced.
'''
''' Needs a live MariaDB; builds and drops its own throwaway database via
''' <see cref="TestDatabaseFixture"/>. Run with:
'''   dotnet test --filter "Category=Integration"
''' </summary>
<Trait("Category", "Integration")>
<Collection("IntegrationDatabase")>
Public Class StockConcurrencyTests
    Implements IClassFixture(Of TestDatabaseFixture)

    Private ReadOnly _fixture As TestDatabaseFixture

    Public Sub New(fixture As TestDatabaseFixture)
        _fixture = fixture
    End Sub

    ''' <summary>
    ''' Two clients deducting the same product must both succeed, with the second seeing the first's
    ''' decrement — the point of serializing with <c>FOR UPDATE</c>.
    '''
    ''' Client A deducts inside a transaction it deliberately leaves open. Two things are asserted:
    ''' <list type="bullet">
    '''   <item>B does not finish while A still holds the transaction — which proves A's deduction
    '''     <em>joined</em> A's transaction rather than committing its own (the behaviour the atomic
    '''     item set in <c>SaleCompletedHandler</c> depends on);</item>
    '''   <item>once A commits, B's FIFO cost is 52.00 — 1 unit left in the cheap batch plus 3 from
    '''     the dearer one — rather than 40.00 for 4 from the cheap batch. Without the lock B races A
    '''     and fails outright with a concurrency exception (verified by removing the lock).</item>
    ''' </list>
    ''' </summary>
    <Fact>
    Public Async Function ADeductionWaitsForAnotherClientsLockInsteadOfFailing() As Task
        Const productId As Integer = 5
        Await SeedTwoBatchesAsync(productId)

        Using ctxA = _fixture.NewInventory()
            Dim serviceA As New StockService(ctxA)
            Dim transactionA = Await ctxA.Database.BeginTransactionAsync()

            ' Joins transactionA rather than committing its own, so the lock is still held below.
            Dim resultA = Await serviceA.DeductStockFIFOAsync(productId, 4)
            Assert.Equal(40D, resultA.TotalCost)

            Using ctxB = _fixture.NewInventory()
                Dim serviceB As New StockService(ctxB)
                Dim taskB = serviceB.DeductStockFIFOAsync(productId, 4)

                Dim delay = Task.Delay(1500)
                Dim firstToFinish = Await Task.WhenAny(taskB, delay)
                Assert.True(firstToFinish Is delay,
                            "the second deduction did not wait for the first client's lock")

                Await transactionA.CommitAsync()

                Dim resultB = Await taskB
                Assert.Equal(4, resultB.DeductedQuantity)
                Assert.Equal(52D, resultB.TotalCost)
            End Using
        End Using

        Using ctx = _fixture.NewInventory()
            Dim batches = Await ctx.StockBatches.
                Where(Function(b) b.ProductId = productId).
                OrderBy(Function(b) b.ReceiptDate).
                ToListAsync()

            Assert.Equal(0, batches(0).QuantityRemaining)   ' 5 - 4 (A) - 1 (B)
            Assert.Equal(2, batches(1).QuantityRemaining)   ' 5 - 3 (B)
        End Using
    End Function

    ''' <summary>
    ''' A sale's items share one transaction, so a failure on a later item must undo the earlier
    ''' ones. It previously saved each item independently: item 1 stayed deducted, and because the
    ''' deducted event is published only after the loop it was never reported to Accounting either —
    ''' revenue with no COGS.
    ''' </summary>
    <Fact>
    Public Async Function AMidSaleFailureLeavesNoDeductionAndPublishesNothing() As Task
        Const stockedProduct As Integer = 6
        Const unstockedProduct As Integer = 7

        Using ctx = _fixture.NewInventory()
            Await New StockService(ctx).AddStockBatchAsync(stockedProduct, 5, 10D, New DateTime(2026, 1, 1), 0, Nothing)
        End Using

        Dim mediator As New CapturingMediator()
        Dim failure As InsufficientStockException = Nothing

        Using ctx = _fixture.NewInventory()
            Dim handler As New SaleCompletedHandler(New StockService(ctx), ctx, NullLogger(Of SaleCompletedHandler).Instance, mediator)

            Dim saleEvent = New SaleCompletedEvent With {.SaleId = 991001, .SaleDate = DateTime.UtcNow}
            saleEvent.Items.Add(New SaleCompletedEvent.SaleCompletedItem With {.ProductId = stockedProduct, .QuantitySold = 2, .UnitPrice = 20D})
            ' This one cannot be satisfied, so the whole sale must roll back.
            saleEvent.Items.Add(New SaleCompletedEvent.SaleCompletedItem With {.ProductId = unstockedProduct, .QuantitySold = 1, .UnitPrice = 20D})

            Try
                Await handler.Handle(saleEvent, CancellationToken.None)
            Catch ex As InsufficientStockException
                failure = ex
            End Try
        End Using

        Assert.NotNull(failure)

        Using ctx = _fixture.NewInventory()
            Dim batch = Await ctx.StockBatches.FirstAsync(Function(b) b.ProductId = stockedProduct)
            Assert.Equal(5, batch.QuantityRemaining)
        End Using

        Assert.Empty(mediator.Published)
    End Function

    <Fact>
    Public Async Function AMidSaleFailureLogsAnAuditRecord() As Task
        Const stockedProduct As Integer = 12
        Const unstockedProduct As Integer = 13
        Const saleId As Integer = 882001

        Using ctx = _fixture.NewInventory()
            Await New StockService(ctx).AddStockBatchAsync(stockedProduct, 5, 10D, New DateTime(2026, 1, 1), 0, Nothing)
        End Using

        Dim mediator As New CapturingMediator()

        Using ctx = _fixture.NewInventory()
            Dim handler As New SaleCompletedHandler(New StockService(ctx), ctx, NullLogger(Of SaleCompletedHandler).Instance, mediator)

            Dim saleEvent = New SaleCompletedEvent With {.SaleId = saleId, .SaleDate = DateTime.UtcNow}
            saleEvent.Items.Add(New SaleCompletedEvent.SaleCompletedItem With {.ProductId = stockedProduct, .QuantitySold = 2, .UnitPrice = 20D})
            saleEvent.Items.Add(New SaleCompletedEvent.SaleCompletedItem With {.ProductId = unstockedProduct, .QuantitySold = 1, .UnitPrice = 20D})

            Dim ex = Await Assert.ThrowsAsync(Of InsufficientStockException)(
                Function() handler.Handle(saleEvent, CancellationToken.None))
        End Using

        ' Verify the audit log was written
        Using connection = _fixture.NewAppConnection()
            Await connection.OpenAsync()
            Using command = connection.CreateCommand()
                command.CommandText = "SELECT Action, EntityId, ChangesJson FROM App_AuditLog WHERE EntityType = 'SalesTransaction' AND EntityId = @EntityId"

                Dim pEntityId = command.CreateParameter()
                pEntityId.ParameterName = "@EntityId"
                pEntityId.Value = saleId.ToString()
                command.Parameters.Add(pEntityId)

                Using reader = Await command.ExecuteReaderAsync()
                    Assert.True(Await reader.ReadAsync(), "No audit log found for the failed sale deduction")

                    Assert.Equal("DedFail", reader.GetString(0))
                    Assert.Equal(saleId.ToString(), reader.GetString(1))

                    Dim json = reader.GetString(2)
                    Assert.Contains("Insufficient stock", json)
                    Assert.Contains(unstockedProduct.ToString(), json)
                End Using
            End Using
        End Using
    End Function

    ''' <summary>
    ''' Shrinkage decrements the same column a sale does, so it takes the same lock — and it must
    ''' still pick batches FIFO and report the value of what it consumed.
    ''' </summary>
    <Fact>
    Public Async Function ShrinkageDeductsFifoAndReportsTheValue() As Task
        Const productId As Integer = 8
        Await SeedTwoBatchesAsync(productId)

        Dim mediator As New CapturingMediator()

        Using ctx = _fixture.NewInventory()
            Await New ShrinkageService(ctx, mediator).RecordShrinkageAsync(productId, 7, "Spoiled", Nothing)
        End Using

        Using ctx = _fixture.NewInventory()
            Dim batches = Await ctx.StockBatches.
                Where(Function(b) b.ProductId = productId).
                OrderBy(Function(b) b.ReceiptDate).
                ToListAsync()

            Assert.Equal(0, batches(0).QuantityRemaining)
            Assert.Equal(3, batches(1).QuantityRemaining)

            Dim record = Await ctx.ShrinkageRecords.FirstAsync(Function(s) s.ProductId = productId)
            Assert.Equal(7, record.QuantityLost)
            Assert.Equal(78D, record.TotalValue)   ' 5 * 10.00 + 2 * 14.00
        End Using

        Assert.Single(mediator.Published)
    End Function

    ''' <summary>Five units at 10.00 received in January, five at 14.00 in February.</summary>
    Private Async Function SeedTwoBatchesAsync(productId As Integer) As Task
        Using ctx = _fixture.NewInventory()
            Dim service As New StockService(ctx)
            Await service.AddStockBatchAsync(productId, 5, 10D, New DateTime(2026, 1, 1), 0, Nothing)
            Await service.AddStockBatchAsync(productId, 5, 14D, New DateTime(2026, 2, 1), 0, Nothing)
        End Using
    End Function

    ''' <summary>
    ''' Records what the handler publishes, so a test can assert that a rolled-back unit of work
    ''' announced nothing. Only Publish is used; the rest of IMediator is not.
    ''' </summary>
    Private Class CapturingMediator
        Implements IMediator

        Public ReadOnly Published As New List(Of Object)()

        Public Function Publish(notification As Object, Optional cancellationToken As CancellationToken = Nothing) As Task Implements IPublisher.Publish
            Published.Add(notification)
            Return Task.CompletedTask
        End Function

        Public Function Publish(Of TNotification As INotification)(notification As TNotification, Optional cancellationToken As CancellationToken = Nothing) As Task Implements IPublisher.Publish
            Published.Add(notification)
            Return Task.CompletedTask
        End Function

        Public Function Send(Of TResponse)(request As IRequest(Of TResponse), Optional cancellationToken As CancellationToken = Nothing) As Task(Of TResponse) Implements ISender.Send
            Throw New NotSupportedException()
        End Function

        Public Function Send(Of TRequest As IRequest)(request As TRequest, Optional cancellationToken As CancellationToken = Nothing) As Task Implements ISender.Send
            Throw New NotSupportedException()
        End Function

        Public Function Send(request As Object, Optional cancellationToken As CancellationToken = Nothing) As Task(Of Object) Implements ISender.Send
            Throw New NotSupportedException()
        End Function

        Public Function CreateStream(Of TResponse)(request As IStreamRequest(Of TResponse), Optional cancellationToken As CancellationToken = Nothing) As IAsyncEnumerable(Of TResponse) Implements ISender.CreateStream
            Throw New NotSupportedException()
        End Function

        Public Function CreateStream(request As Object, Optional cancellationToken As CancellationToken = Nothing) As IAsyncEnumerable(Of Object) Implements ISender.CreateStream
            Throw New NotSupportedException()
        End Function
    End Class

End Class
