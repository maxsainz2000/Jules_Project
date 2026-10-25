Imports Xunit
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks
Imports MediatR
Imports MerchSys.POS.Entities
Imports MerchSys.POS.Services
Imports MerchSys.SharedKernel.Queries

''' <summary>
''' DB-backed coverage for the POS identifier races: C7 (transaction number) and C8 (receipt number
''' and integrity chain).
'''
''' These run the operations concurrently and assert the correct end state. They are not
''' deterministic reproductions — the window is between a read and a write, so whether a given run
''' overlaps is up to the scheduler. They fail loudly when the race does occur, which is what a
''' regression would look like.
'''
''' Needs a live MariaDB; builds and drops its own throwaway database via
''' <see cref="TestDatabaseFixture"/>. Run with:
'''   dotnet test --filter "Category=Integration"
''' </summary>
<Trait("Category", "Integration")>
<Collection("IntegrationDatabase")>
Public Class PosIdentifierConcurrencyTests
    Implements IClassFixture(Of TestDatabaseFixture)

    Private Const ParallelOperations As Integer = 4

    ''' <summary>A year no other test touches, so the sequence row is genuinely absent.</summary>
    Private Const UntouchedYear As Integer = 2027

    Private ReadOnly _fixture As TestDatabaseFixture

    Public Sub New(fixture As TestDatabaseFixture)
        _fixture = fixture
    End Sub

    ''' <summary>
    ''' C7: the number is derived from the current maximum, so two clients finalising at the same
    ''' moment derive the same one. The unique index rejected the loser, and that escaped as an
    ''' unhandled DbUpdateException — the sale was lost.
    ''' </summary>
    <Fact>
    Public Async Function ConcurrentSalesAllocateDistinctTransactionNumbers() As Task
        Dim tasks = Enumerable.Range(1, ParallelOperations).
            Select(Function(i) FinalizeOneSaleAsync("CONC-" & i)).
            ToList()

        Dim numbers = Await Task.WhenAll(tasks)

        Assert.Equal(ParallelOperations, numbers.Distinct().Count())
    End Function

    ''' <summary>
    ''' C8b: the first receipt of a year is the sharp case — both clients find no sequence row and
    ''' both insert one, so the loser hits the unique index on Year instead of the row-version token.
    ''' That used to escape unhandled and fail receipt generation outright.
    ''' </summary>
    <Fact>
    Public Async Function TheFirstReceiptOfAYearSurvivesTwoClientsRacingIt() As Task
        Dim tasks = {
            NextReceiptNumberAsync(UntouchedYear),
            NextReceiptNumberAsync(UntouchedYear)
        }

        Dim numbers = Await Task.WhenAll(tasks)

        Assert.Equal(2, numbers.Distinct().Count())
    End Function

    ''' <summary>Concurrent increments of an existing sequence must not hand out the same number.</summary>
    <Fact>
    Public Async Function ConcurrentReceiptNumbersAreAllDistinct() As Task
        Dim year = DateTime.UtcNow.Year

        Dim tasks = Enumerable.Range(1, ParallelOperations).
            Select(Function(i) NextReceiptNumberAsync(year)).
            ToList()

        Dim numbers = Await Task.WhenAll(tasks)

        Assert.Equal(ParallelOperations, numbers.Distinct().Count())
    End Function

    ''' <summary>
    ''' C8a: appending to the chain reads the tail and then writes a row pointing at it. Two clients
    ''' issuing receipts at the same moment read the same tail and the chain forks — each row
    ''' validates alone, and the chain check then reports a break.
    ''' </summary>
    <Fact>
    Public Async Function ConcurrentIntegrityAppendsDoNotForkTheChain() As Task
        Dim year = DateTime.UtcNow.Year

        ' The lock is that year's sequence row, which the real flow creates when it issues the first
        ' receipt number of the year. Create it the same way.
        Await NextReceiptNumberAsync(year)

        Dim receiptIds = Await CreateReceiptsAsync(ParallelOperations)

        Await Task.WhenAll(receiptIds.Select(Function(id) AppendIntegrityAsync(id)))

        Using ctx = _fixture.NewPOS()
            Dim result = Await New ReceiptIntegrityService(ctx, Nothing, Nothing).ValidateChainAsync()
            Assert.True(result.IsValid, $"the integrity chain forked: {result.ErrorMessage}")
        End Using
    End Function

    Private Async Function FinalizeOneSaleAsync(cartId As String) As Task(Of String)
        Using ctx = _fixture.NewPOS()
            Dim mediator = New FakeStockMediator()
            Dim service As New CartService(ctx, mediator)
            service.AddToCart(cartId, 1, "Concurrency Test Product", 10D, 1)
            Dim sale = Await service.FinalizeTransactionAsync(cartId, "Cash")
            Return sale.TransactionNumber
        End Using
    End Function

    Private Class FakeStockMediator
        Implements IMediator

        Public Function Send(Of TResponse)(request As IRequest(Of TResponse), Optional cancellationToken As CancellationToken = Nothing) As Task(Of TResponse) Implements ISender.Send
            If TypeOf request Is GetCurrentStockQuery Then
                Dim result = New GetCurrentStockResult()
                ' Just provide enough stock for the test product (productId 1)
                result.StockLevels.Add(New ProductStockLevel With {
                    .ProductId = 1,
                    .CurrentQuantity = 100
                })
                Return Task.FromResult(DirectCast(DirectCast(result, Object), TResponse))
            End If
            If TypeOf request Is GetVatConfigurationQuery Then
                Dim result = New GetVatConfigurationResult With {
                    .VatRate = 0.12D,
                    .IsVatRegistered = True
                }
                Return Task.FromResult(DirectCast(DirectCast(result, Object), TResponse))
            End If
            Throw New NotSupportedException()
        End Function

        Public Function Send(Of TRequest As IRequest)(request As TRequest, Optional cancellationToken As CancellationToken = Nothing) As Task Implements ISender.Send
            Throw New NotSupportedException()
        End Function

        Public Function Send(request As Object, Optional cancellationToken As CancellationToken = Nothing) As Task(Of Object) Implements ISender.Send
            Throw New NotSupportedException()
        End Function

        Public Function Publish(notification As Object, Optional cancellationToken As CancellationToken = Nothing) As Task Implements IPublisher.Publish
            Throw New NotSupportedException()
        End Function

        Public Function Publish(Of TNotification As INotification)(notification As TNotification, Optional cancellationToken As CancellationToken = Nothing) As Task Implements IPublisher.Publish
            Throw New NotSupportedException()
        End Function

        Public Function CreateStream(Of TResponse)(request As IStreamRequest(Of TResponse), Optional cancellationToken As CancellationToken = Nothing) As IAsyncEnumerable(Of TResponse) Implements ISender.CreateStream
            Throw New NotSupportedException()
        End Function

        Public Function CreateStream(request As Object, Optional cancellationToken As CancellationToken = Nothing) As IAsyncEnumerable(Of Object) Implements ISender.CreateStream
            Throw New NotSupportedException()
        End Function
    End Class

    Private Async Function NextReceiptNumberAsync(year As Integer) As Task(Of String)
        Using ctx = _fixture.NewPOS()
            ' GetNextReceiptNumberAsync touches neither the mediator nor the logger.
            Return Await New ReceiptIntegrityService(ctx, Nothing, Nothing).GetNextReceiptNumberAsync(year)
        End Using
    End Function

    Private Async Function AppendIntegrityAsync(receiptId As Integer) As Task
        Using ctx = _fixture.NewPOS()
            Await New ReceiptIntegrityService(ctx, Nothing, Nothing).ComputeAndPersistAsync(receiptId)
        End Using
    End Function

    Private Async Function CreateReceiptsAsync(count As Integer) As Task(Of List(Of Integer))
        Dim ids As New List(Of Integer)()

        Using ctx = _fixture.NewPOS()
            For i = 1 To count
                Dim receipt As New OfficialReceipt With {
                    .ReceiptNumber = $"OR-{DateTime.UtcNow.Year}-TEST{i:D4}",
                    .SerializedLineItems = "[]",
                    .CreatedAt = DateTime.UtcNow
                }
                ctx.OfficialReceipts.Add(receipt)
                Await ctx.SaveChangesAsync()
                ids.Add(receipt.Id)
            Next
        End Using

        Return ids
    End Function

End Class
