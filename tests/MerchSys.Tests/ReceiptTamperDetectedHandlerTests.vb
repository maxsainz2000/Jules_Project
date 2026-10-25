Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Logging
Imports Moq
Imports Xunit
Imports MerchSys.SharedKernel.Events
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities
Imports MerchSys.Accounting.Handlers
Imports MerchSys.Accounting.Services

Public Class ReceiptTamperDetectedHandlerTests

    Private Function CreateDbContext() As AccountingDbContext
        Dim options = New DbContextOptionsBuilder(Of AccountingDbContext)() _
            .UseInMemoryDatabase(databaseName:=Guid.NewGuid().ToString()) _
            .Options
        Return New AccountingDbContext(options)
    End Function

    <Fact>
    Public Async Function Handler_RecordsAuditEntry_WithDiagnostics() As Task
        ' Arrange
        Dim dbContext = CreateDbContext()
        Dim loggerMock = New Mock(Of ILogger(Of ReceiptTamperDetectedHandler))()
        Dim handler = New ReceiptTamperDetectedHandler(dbContext, loggerMock.Object)
        Dim evt = New ReceiptTamperDetectedEvent With {
            .ReceiptId = Guid.NewGuid(),
            .ReceiptNumber = "RCPT-100",
            .ExpectedHash = "Expected123",
            .ActualHash = "Actual456",
            .DetectedAt = DateTime.UtcNow,
            .DetectedBy = "SystemWatcher"
        }

        ' Act
        Await handler.Handle(evt, CancellationToken.None)

        ' Assert
        Dim entry = Await dbContext.TamperAuditEntries.SingleAsync()
        Assert.Equal(evt.ReceiptId, entry.ReceiptId)
        Assert.Equal("RCPT-100", entry.ReceiptNumber)
        Assert.Equal("Expected123", entry.ExpectedHash)
        Assert.Equal("Actual456", entry.ActualHash)
        Assert.Equal("HashMismatch", entry.TamperKind)
        Assert.Equal(evt.DetectedAt, entry.DetectedAt)
        Assert.Equal("SystemWatcher", entry.DetectedBy)
        Assert.Equal(Environment.MachineName, entry.MachineName)
        Assert.Equal(Environment.UserName, entry.WindowsUser)
    End Function

    <Fact>
    Public Async Function Handler_CatchesAndRethrows_ExceptionsOutsideCatch() As Task
        ' Arrange
        ' Force an exception by passing a mocked context that throws on SaveChangesAsync
        Dim options = New DbContextOptionsBuilder(Of AccountingDbContext)().Options
        Dim dbContextMock = New Mock(Of AccountingDbContext)(options)

        Dim dbSetMock = New Mock(Of DbSet(Of TamperAuditEntry))()
        dbContextMock.Setup(Function(x) x.TamperAuditEntries).Returns(dbSetMock.Object)

        dbContextMock.Setup(Function(x) x.SaveChangesAsync(It.IsAny(Of CancellationToken)())).ThrowsAsync(New InvalidOperationException("DB Failure"))

        Dim loggerMock = New Mock(Of ILogger(Of ReceiptTamperDetectedHandler))()
        Dim handler = New ReceiptTamperDetectedHandler(dbContextMock.Object, loggerMock.Object)
        Dim evt = New ReceiptTamperDetectedEvent With { .ReceiptId = Guid.NewGuid() }

        ' Act & Assert
        Dim ex = Await Assert.ThrowsAsync(Of InvalidOperationException)(Function() handler.Handle(evt, CancellationToken.None))
        Assert.Equal("DB Failure", ex.Message)
    End Function

    <Fact>
    Public Async Function QueryService_GetIncidentsAsync_OrdersByDetectedAtDescending() As Task
        ' Arrange
        Dim dbContext = CreateDbContext()
        dbContext.TamperAuditEntries.AddRange(
            New TamperAuditEntry With { .DetectedAt = New DateTime(2023, 1, 1), .TamperKind = "HashMismatch" },
            New TamperAuditEntry With { .DetectedAt = New DateTime(2023, 1, 3), .TamperKind = "HashMismatch" },
            New TamperAuditEntry With { .DetectedAt = New DateTime(2023, 1, 2), .TamperKind = "HashMismatch" }
        )
        Await dbContext.SaveChangesAsync()

        Dim service = New TamperAuditQueryService(dbContext)

        ' Act
        Dim results = Await service.GetIncidentsAsync()

        ' Assert
        Assert.Equal(3, results.Count)
        Assert.Equal(New DateTime(2023, 1, 3), results(0).DetectedAt)
        Assert.Equal(New DateTime(2023, 1, 2), results(1).DetectedAt)
        Assert.Equal(New DateTime(2023, 1, 1), results(2).DetectedAt)
    End Function

    <Fact>
    Public Async Function QueryService_CountByKindAsync_ReturnsCorrectCounts() As Task
        ' Arrange
        Dim dbContext = CreateDbContext()
        dbContext.TamperAuditEntries.AddRange(
            New TamperAuditEntry With { .TamperKind = "HashMismatch" },
            New TamperAuditEntry With { .TamperKind = "HashMismatch" },
            New TamperAuditEntry With { .TamperKind = "SignatureInvalid" }
        )
        Await dbContext.SaveChangesAsync()

        Dim service = New TamperAuditQueryService(dbContext)

        ' Act
        Dim counts = Await service.CountByKindAsync()

        ' Assert
        Assert.Equal(2, counts.Count)
        Assert.Equal(2, counts("HashMismatch"))
        Assert.Equal(1, counts("SignatureInvalid"))
    End Function

End Class
