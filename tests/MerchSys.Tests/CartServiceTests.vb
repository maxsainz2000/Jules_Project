Imports Xunit
Imports System
Imports System.Linq
Imports System.Collections.Generic
Imports System.Threading
Imports System.Threading.Tasks
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.POS.Services
Imports MerchSys.SharedKernel.Exceptions
Imports MerchSys.SharedKernel.Queries

''' <summary>
''' Regression test for C9.
'''
''' <c>CartService.FinalizeTransactionAsync</c> built each <c>SalesTransactionLine</c> without setting
''' <c>ProductId</c>, so it defaulted to 0 on every line. That is silent — the compiler cannot catch it,
''' because a non-nullable Integer has a perfectly valid default — and it broke several things
''' downstream: the sale-completion handler deducted stock for product 0 and threw
''' <c>InsufficientStockException</c>, and Accounting, which keys <c>RevenueRecord</c> on
''' (SourceTransactionId, ProductId), collapsed every line of a multi-line sale into one row.
'''
''' Two different products on one sale is the case that exposes it: with ProductId stuck at 0 the
''' lines are indistinguishable from each other.
'''
'''   dotnet test --filter "Category=Integration"    # needs a live MariaDB
''' </summary>
<Trait("Category", "Integration")>
<Collection("IntegrationDatabase")>
Public Class CartServiceTests

    <Fact>
    Public Async Function FinalizeTransactionPersistsTheProductIdOnEveryLine() As Task
        Const firstProductId As Integer = 8
        Const secondProductId As Integer = 9

        ' CartService keeps carts in a shared static dictionary, so the id must be unique per run.
        Dim cartId = "cart-" & Guid.NewGuid().ToString("N")

        Using fixture As New TestDatabaseFixture()
            Dim transactionId As Integer

            Using ctx = fixture.NewPOS()
                Dim mediator = New FakeStockMediator(New Dictionary(Of Integer, Integer) From {
                    {firstProductId, 10},
                    {secondProductId, 10}
                })
                Dim service As New CartService(ctx, mediator)
                service.AddToCart(cartId, firstProductId, "First Product", 10D, 2)
                service.AddToCart(cartId, secondProductId, "Second Product", 20D, 1)

                Dim transaction = Await service.FinalizeTransactionAsync(cartId, "Cash")
                transactionId = transaction.Id

                Assert.Equal(2, transaction.Lines.Count)
                Assert.All(transaction.Lines,
                    Sub(l) Assert.True(l.ProductId > 0, "a checkout line was created with ProductId 0"))
            End Using

            ' Read it back, so the assertion covers what was actually persisted rather than the graph.
            Using ctx = fixture.NewPOS()
                Dim persisted = Await ctx.SalesTransactions.
                    Include(Function(t) t.Lines).
                    FirstAsync(Function(t) t.Id = transactionId)

                Assert.Equal(2, persisted.Lines.Count)
                Assert.Contains(persisted.Lines, Function(l) l.ProductId = firstProductId)
                Assert.Contains(persisted.Lines, Function(l) l.ProductId = secondProductId)

                ' The heart of the fix: the two lines stay distinguishable, which is what the
                ' (sale, product) keys downstream depend on.
                Assert.Equal(2, persisted.Lines.Select(Function(l) l.ProductId).Distinct().Count())
            End Using
        End Using
    End Function

    <Fact>
    Public Async Function FinalizeTransactionThrowsIfStockIsInsufficient() As Task
        Const productId As Integer = 10
        Dim cartId = "cart-insufficient-" & Guid.NewGuid().ToString("N")

        Using fixture As New TestDatabaseFixture()
            Using ctx = fixture.NewPOS()
                ' Mock only 2 items in stock
                Dim mediator = New FakeStockMediator(New Dictionary(Of Integer, Integer) From {
                    {productId, 2}
                })
                Dim service As New CartService(ctx, mediator)

                ' Try to buy 3 items
                service.AddToCart(cartId, productId, "Test Product", 10D, 3)

                Dim ex = Await Assert.ThrowsAsync(Of InsufficientStockException)(
                    Function() service.FinalizeTransactionAsync(cartId, "Cash")
                )

                Assert.Contains("Insufficient stock", ex.Message)
            End Using
        End Using
    End Function

    Private Class FakeStockMediator
        Implements IMediator

        Private ReadOnly _stockLevels As Dictionary(Of Integer, Integer)

        Public Sub New(stockLevels As Dictionary(Of Integer, Integer))
            _stockLevels = stockLevels
        End Sub

        Public Function Send(Of TResponse)(request As IRequest(Of TResponse), Optional cancellationToken As CancellationToken = Nothing) As Task(Of TResponse) Implements ISender.Send
            If TypeOf request Is GetCurrentStockQuery Then
                Dim result = New GetCurrentStockResult()
                For Each kvp In _stockLevels
                    result.StockLevels.Add(New ProductStockLevel With {
                        .ProductId = kvp.Key,
                        .CurrentQuantity = kvp.Value
                    })
                Next
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

End Class
