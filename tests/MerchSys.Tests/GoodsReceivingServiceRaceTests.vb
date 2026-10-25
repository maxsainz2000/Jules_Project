Imports Xunit
Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Diagnostics
Imports MerchSys.Purchasing.Data
Imports MerchSys.Purchasing.Entities
Imports MerchSys.Purchasing.Services
Imports MerchSys.Purchasing.Services.Vat
Imports MerchSys.Purchasing.Dtos
Imports MediatR
Imports MerchSys.SharedKernel.Queries
Imports MerchSys.SharedKernel.Enums

<Trait("Category", "Integration")>
<Collection("IntegrationDatabase")>
Public Class GoodsReceivingServiceRaceTests
    Implements IClassFixture(Of TestDatabaseFixture)

    Private ReadOnly _fixture As TestDatabaseFixture

    Public Sub New(fixture As TestDatabaseFixture)
        _fixture = fixture
    End Sub

    Private Class FakeMediator
        Implements IMediator

        Public Function Publish(notification As Object, Optional cancellationToken As CancellationToken = Nothing) As Task Implements IPublisher.Publish
            Return Task.CompletedTask
        End Function

        Public Function Publish(Of TNotification As INotification)(notification As TNotification, Optional cancellationToken As CancellationToken = Nothing) As Task Implements IPublisher.Publish
            Return Task.CompletedTask
        End Function

        Public Function Send(Of TResponse)(request As IRequest(Of TResponse), Optional cancellationToken As CancellationToken = Nothing) As Task(Of TResponse) Implements ISender.Send
            If TypeOf request Is GetVatConfigurationQuery Then
                Dim config = New MerchSys.SharedKernel.Queries.GetVatConfigurationResult With {
                    .VatRate = 0.12D,
                    .IsVatRegistered = True
                }
                Return Task.FromResult(CType(CObj(config), TResponse))
            End If
            Return Task.FromResult(CType(Nothing, TResponse))
        End Function

        Public Function Send(request As Object, Optional cancellationToken As CancellationToken = Nothing) As Task(Of Object) Implements ISender.Send
            Return Task.FromResult(Of Object)(Nothing)
        End Function

        Public Function Send(Of TRequest As IRequest)(request As TRequest, Optional cancellationToken As CancellationToken = Nothing) As Task Implements ISender.Send
            Return Task.CompletedTask
        End Function

        Public Function CreateStream(Of TResponse)(request As IStreamRequest(Of TResponse), Optional cancellationToken As CancellationToken = Nothing) As IAsyncEnumerable(Of TResponse) Implements ISender.CreateStream
            Throw New NotImplementedException()
        End Function

        Public Function CreateStream(request As Object, Optional cancellationToken As CancellationToken = Nothing) As IAsyncEnumerable(Of Object) Implements ISender.CreateStream
            Throw New NotImplementedException()
        End Function
    End Class

    Private Class RetryInterceptor
        Inherits SaveChangesInterceptor

        Public AttemptCount As Integer = 0

        Public Overrides Function SavingChangesAsync(eventData As DbContextEventData, result As InterceptionResult(Of Integer), Optional cancellationToken As CancellationToken = Nothing) As ValueTask(Of InterceptionResult(Of Integer))
            AttemptCount += 1
            If AttemptCount = 1 Then
                Throw New DbUpdateException("Simulated unique index collision on ReceiptNumber")
            End If
            Return MyBase.SavingChangesAsync(eventData, result, cancellationToken)
        End Function
    End Class

    <Fact>
    Public Async Function ReceiveGoodsAsync_RaceCondition_RetriesAndPersists() As Task
        Dim poId As Integer

        Dim optionsBuilderForSetup = New DbContextOptionsBuilder(Of PurchasingDbContext)()
        optionsBuilderForSetup.UseMySQL(_fixture.ConnectionString)

        Using setupContext = New PurchasingDbContext(optionsBuilderForSetup.Options)
            Dim po = New PurchaseOrder With {
                .OrderNumber = "PO-RACE-1",
                .VendorId = 1,
                .Status = PurchaseOrderStatus.Submitted,
                .TotalAmount = 100D,
                .CreatedAt = DateTime.UtcNow
            }
            setupContext.PurchaseOrders.Add(po)
            Await setupContext.SaveChangesAsync()
            poId = po.Id
        End Using

        Dim interceptor = New RetryInterceptor()
        Dim optionsBuilder = New DbContextOptionsBuilder(Of PurchasingDbContext)()
        optionsBuilder.UseMySQL(_fixture.ConnectionString)
        optionsBuilder.AddInterceptors(interceptor)

        Using raceContext = New PurchasingDbContext(optionsBuilder.Options)
            Dim mediator = New FakeMediator()
            Dim vatCalculator = New GoodsReceiptVatCalculator()
            Dim service = New GoodsReceivingService(raceContext, mediator, Nothing, vatCalculator)

            Dim dto = New ReceiveGoodsDto With {
                .PurchaseOrderId = poId,
                .Lines = New List(Of ReceiveGoodsLineDto) From {
                    New ReceiveGoodsLineDto With {
                        .ProductId = 1,
                        .QuantityOrdered = 1,
                        .QuantityReceived = 1,
                        .UnitCost = 100D,
                        .VatClassification = VatTreatment.Vatable
                    }
                }
            }

            Dim result = Await service.ReceiveGoodsAsync(dto)

            Assert.NotNull(result)
            Assert.True(result.Id > 0)
            Assert.Equal(2, interceptor.AttemptCount)
        End Using
    End Function

End Class
