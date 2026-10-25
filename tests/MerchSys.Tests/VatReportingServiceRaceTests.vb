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

<Trait("Category", "Integration")>
<Collection("IntegrationDatabase")>
Public Class VatReportingServiceRaceTests
    Implements IClassFixture(Of TestDatabaseFixture)

    Private _fixture As TestDatabaseFixture


    Public Sub New(fixture As TestDatabaseFixture)
        _fixture = fixture
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
        Dim optionsBuilderForSetup = New DbContextOptionsBuilder(Of AccountingDbContext)()
        optionsBuilderForSetup.UseMySQL(_fixture.ConnectionString)

        Using setupContext = New AccountingDbContext(optionsBuilderForSetup.Options)
            ' Ensure the record doesn't already exist from another test
            Dim old = setupContext.VatReturns.FirstOrDefault(Function(r) r.Year = 2023 AndAlso r.Period = 5 AndAlso r.FormType = VatReturnFormType.Form2550M)
            If old IsNot Nothing Then
                setupContext.VatReturns.Remove(old)
                setupContext.SaveChanges()
            End If
        End Using

        Dim existing = New VatReturn With {
            .Year = 2023,
            .Period = 5,
            .FormType = VatReturnFormType.Form2550M,
            .PeriodType = VatReturnPeriodType.Monthly,
            .FilingStatus = VatFilingStatus.Generated,
            .PeriodStart = New DateTime(2023, 1, 1),
            .PeriodEnd = New DateTime(2023, 1, 31)
        }
        Using setupContext = New AccountingDbContext(optionsBuilderForSetup.Options)
            setupContext.VatReturns.Add(existing)
            Await setupContext.SaveChangesAsync()
        End Using

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
