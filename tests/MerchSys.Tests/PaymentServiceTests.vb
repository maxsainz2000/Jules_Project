Imports Xunit
Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.POS.Services
Imports MerchSys.POS.Entities
Imports MerchSys.SharedKernel.Events
Imports MerchSys.SharedKernel.Interfaces

<Trait("Category", "Integration")>
<Collection("IntegrationDatabase")>
Public Class PaymentServiceTests

    Private ReadOnly _fixture As TestDatabaseFixture

    Public Sub New(fixture As TestDatabaseFixture)
        _fixture = fixture
    End Sub

    <Fact>
    Public Async Function ProcessPaymentAsyncParsesCashMethodCorrectly() As Task
        Using ctx = _fixture.NewPOS()
            Dim transaction = New SalesTransaction With {
                .TransactionDate = DateTime.Now,
                .TransactionNumber = "TXN-TEST-1",
                .TotalAmount = 100D,
                .Lines = New List(Of SalesTransactionLine) From {
                    New SalesTransactionLine With { .ProductId = 1, .Quantity = 1, .UnitPrice = 100D }
                }
            }
            ctx.SalesTransactions.Add(transaction)
            Await ctx.SaveChangesAsync()

            Dim eventBus = New FakeEventBus()
            Dim service As New PaymentService(ctx, eventBus)

            Dim result = Await service.ProcessPaymentAsync(transaction.Id, 100D, "Cash", "")

            Assert.True(result.Success)

            Dim completedEvent = eventBus.LastSaleCompletedEvent
            Assert.NotNull(completedEvent)
            Assert.Equal(MerchSys.SharedKernel.Enums.PaymentMethod.Cash, completedEvent.PaymentMethod)
        End Using
    End Function

    <Fact>
    Public Async Function ProcessPaymentAsyncParsesCreditMethodCorrectly() As Task
        Using ctx = _fixture.NewPOS()
            Dim creditAccount = New CreditAccount With {
                .CustomerName = "John Doe",
                .CurrentBalance = 0D,
                .IsBlocked = False
            }
            ctx.CreditAccounts.Add(creditAccount)
            Await ctx.SaveChangesAsync()

            Dim transaction = New SalesTransaction With {
                .TransactionDate = DateTime.Now,
                .TransactionNumber = "TXN-TEST-2",
                .TotalAmount = 100D,
                .CreditAccount = creditAccount,
                .Lines = New List(Of SalesTransactionLine) From {
                    New SalesTransactionLine With { .ProductId = 1, .Quantity = 1, .UnitPrice = 100D }
                }
            }
            ctx.SalesTransactions.Add(transaction)
            Await ctx.SaveChangesAsync()

            Dim eventBus = New FakeEventBus()
            Dim service As New PaymentService(ctx, eventBus)

            Dim result = Await service.ProcessPaymentAsync(transaction.Id, 100D, "Credit", "")

            Assert.True(result.Success)

            Dim completedEvent = eventBus.LastSaleCompletedEvent
            Assert.NotNull(completedEvent)
            Assert.Equal(MerchSys.SharedKernel.Enums.PaymentMethod.Credit, completedEvent.PaymentMethod)
        End Using
    End Function

    Private Class FakeEventBus
        Implements IEventBus

        Public Property LastSaleCompletedEvent As SaleCompletedEvent

        Public Function PublishAsync(Of TEvent As MediatR.INotification)(eventMsg As TEvent) As Task Implements IEventBus.PublishAsync
            If TypeOf eventMsg Is SaleCompletedEvent Then
                LastSaleCompletedEvent = DirectCast(DirectCast(eventMsg, Object), SaleCompletedEvent)
            End If
            Return Task.CompletedTask
        End Function
    End Class

End Class
