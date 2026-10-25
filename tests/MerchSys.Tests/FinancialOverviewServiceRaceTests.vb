Imports Xunit
Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.Extensions.Logging
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Diagnostics
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities
Imports MerchSys.Accounting.Services
Imports MediatR
Imports MerchSys.SharedKernel.Queries

<Trait("Category", "Integration")>
<Collection("IntegrationDatabase")>
Public Class FinancialOverviewServiceRaceTests
    Implements IClassFixture(Of TestDatabaseFixture)

    Private ReadOnly _fixture As TestDatabaseFixture

    Public Sub New(fixture As TestDatabaseFixture)
        _fixture = fixture
    End Sub

    Private Class FakeLogger
        Implements ILogger(Of FinancialOverviewService)
        Public Function BeginScope(Of TState)(state As TState) As IDisposable Implements ILogger.BeginScope
            Return Nothing
        End Function
        Public Function IsEnabled(logLevel As LogLevel) As Boolean Implements ILogger.IsEnabled
            Return True
        End Function
        Public Sub Log(Of TState)(logLevel As LogLevel, eventId As EventId, state As TState, exception As Exception, formatter As Func(Of TState, Exception, String)) Implements ILogger.Log
        End Sub
    End Class

    Private Class FakeMediator
        Implements IMediator

        Public Function Publish(notification As Object, Optional cancellationToken As CancellationToken = Nothing) As Task Implements IPublisher.Publish
            Return Task.CompletedTask
        End Function

        Public Function Publish(Of TNotification As INotification)(notification As TNotification, Optional cancellationToken As CancellationToken = Nothing) As Task Implements IPublisher.Publish
            Return Task.CompletedTask
        End Function

        Public Function Send(Of TResponse)(request As IRequest(Of TResponse), Optional cancellationToken As CancellationToken = Nothing) As Task(Of TResponse) Implements ISender.Send
            If TypeOf request Is GetTotalARQuery Then
                Return Task.FromResult(CType(CObj(0D), TResponse))
            End If
            If TypeOf request Is GetTotalAPQuery Then
                Return Task.FromResult(CType(CObj(0D), TResponse))
            End If
            If TypeOf request Is GetInventoryValuationQuery Then
                Dim val = New GetInventoryValuationResult()
                Return Task.FromResult(CType(CObj(val), TResponse))
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

    Private Class ThrowingInterceptor
        Inherits SaveChangesInterceptor
        Public Overrides Function SavingChangesAsync(eventData As DbContextEventData, result As InterceptionResult(Of Integer), Optional cancellationToken As CancellationToken = Nothing) As ValueTask(Of InterceptionResult(Of Integer))
            Throw New DbUpdateException("Simulated unique index collision on SnapshotDate")
        End Function
    End Class

    <Fact>
    Public Async Function RefreshSnapshotAsync_RaceCondition_RecoversAndReturnsExisting() As Task
        Dim today = DateTime.UtcNow.Date
        Dim existingId As Integer

        Dim optionsBuilderForSetup = New DbContextOptionsBuilder(Of AccountingDbContext)()
        optionsBuilderForSetup.UseMySQL(_fixture.ConnectionString)

        Using setupContext = New AccountingDbContext(optionsBuilderForSetup.Options)
            ' Ensure no snapshot exists for today
            Dim old = Await setupContext.Set(Of FinancialSnapshot)().FirstOrDefaultAsync(Function(s) s.SnapshotDate.Date = today)
            If old IsNot Nothing Then
                setupContext.Set(Of FinancialSnapshot)().Remove(old)
                Await setupContext.SaveChangesAsync()
            End If

            ' Insert existing
            Dim existing = New FinancialSnapshot With {
                .SnapshotDate = today,
                .TodayRevenue = 100D,
                .MtdRevenue = 100D,
                .YtdRevenue = 100D,
                .TotalAR = 0D,
                .TotalAP = 0D,
                .InventoryValue = 0D
            }
            setupContext.Set(Of FinancialSnapshot)().Add(existing)
            Await setupContext.SaveChangesAsync()
            existingId = existing.Id
        End Using

        Dim optionsBuilder = New DbContextOptionsBuilder(Of AccountingDbContext)()
        optionsBuilder.UseMySQL(_fixture.ConnectionString)
        optionsBuilder.AddInterceptors(New ThrowingInterceptor())

        Using raceContext = New AccountingDbContext(optionsBuilder.Options)
            Dim service = New FinancialOverviewService(raceContext, New FakeMediator(), New FakeLogger())
            Dim result = Await service.RefreshSnapshotAsync()

            Assert.NotNull(result)
            Assert.Equal(existingId, result.Id)
        End Using
    End Function

End Class
