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
Imports MerchSys.Accounting.Enums
Imports MerchSys.Accounting.Services
Imports MediatR
Imports MerchSys.SharedKernel.Interfaces

Public Class VatReportingServiceRaceTests
    Implements IDisposable

    Private _fixture As TestDatabaseFixture
    Private _dbContext As AccountingDbContext

    Public Sub New()
        _fixture = New TestDatabaseFixture()
        _dbContext = New AccountingDbContext(New DbContextOptionsBuilder(Of AccountingDbContext)().UseMySQL(_fixture.ConnectionString).Options)
        _dbContext.Database.EnsureCreated()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        _dbContext.Database.EnsureDeleted()
        _dbContext.Dispose()
        _fixture.Dispose()
    End Sub

    Private Class FakeLogger
        Implements ILogger(Of VatReportingService)
        Public Function BeginScope(Of TState)(state As TState) As IDisposable Implements ILogger.BeginScope
            Return Nothing
        End Function
        Public Function IsEnabled(logLevel As LogLevel) As Boolean Implements ILogger.IsEnabled
            Return True
        End Function
        Public Sub Log(Of TState)(logLevel As LogLevel, eventId As EventId, state As TState, exception As Exception, formatter As Func(Of TState, Exception, String)) Implements ILogger.Log
        End Sub
    End Class

    Private Class ThrowingInterceptor
        Inherits SaveChangesInterceptor
        Public Overrides Function SavingChangesAsync(eventData As DbContextEventData, result As InterceptionResult(Of Integer), Optional cancellationToken As CancellationToken = Nothing) As ValueTask(Of InterceptionResult(Of Integer))
            Throw New DbUpdateException("Simulated race condition")
        End Function
    End Class

    <Fact>
    Public Async Function GenerateVatReturnAsync_RaceCondition_RecoversAndReturnsExisting() As Task
        Dim existing = New VatReturn With {
            .Year = 2023,
            .Period = 5,
            .FormType = VatReturnFormType.Form2550M,
            .PeriodType = VatReturnPeriodType.Monthly,
            .FilingStatus = VatFilingStatus.Generated,
            .PeriodStart = New DateTime(2023, 1, 1),
            .PeriodEnd = New DateTime(2023, 1, 31)
        }
        _dbContext.VatReturns.Add(existing)
        Await _dbContext.SaveChangesAsync()

        Dim optionsBuilder = New DbContextOptionsBuilder(Of AccountingDbContext)()
        optionsBuilder.UseMySQL(_fixture.ConnectionString)
        optionsBuilder.AddInterceptors(New ThrowingInterceptor())

        Using raceContext = New AccountingDbContext(optionsBuilder.Options)
            Dim service = New VatReportingService(raceContext, New FakeVatMediator(0.12D), New FakeLogger())
            Dim result = Await service.GenerateVatReturnAsync(2023, 5, VatReturnFormType.Form2550M)

            Assert.NotNull(result)
            Assert.Equal(existing.Id, result.Id)
        End Using
    End Function

End Class
