Imports System
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Collections.Generic
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Logging
Imports Moq
Imports Xunit
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities
Imports MerchSys.Accounting.Handlers
Imports MerchSys.Accounting.Services
Imports MerchSys.SharedKernel.Events

Public Class ReceiptTamperDetectedHandlerTests

    Private Function CreateDbContext() As AccountingDbContext
        Dim options = New DbContextOptionsBuilder(Of AccountingDbContext)().
            UseInMemoryDatabase(databaseName:=Guid.NewGuid().ToString()).
            Options
        Return New AccountingDbContext(options)
    End Function

    <Fact>
    Public Async Function Handler_SavesAuditEntry_WithCorrectProperties() As Task
        ' Arrange
        Dim dbContext = CreateDbContext()
        Dim loggerMock = New Mock(Of ILogger(Of ReceiptTamperDetectedHandler))()
        Dim handler = New ReceiptTamperDetectedHandler(dbContext, loggerMock.Object)

        Dim receiptId = Guid.NewGuid()
        Dim evt = New ReceiptTamperDetectedEvent() With {
            .ReceiptId = receiptId,
            .ReceiptNumber = "R-123",
            .ExpectedHash = "abc",
            .ActualHash = "def",
            .DetectedAt = DateTime.UtcNow,
            .DetectedBy = "System"
        }

        ' Act
        Await handler.Handle(evt, CancellationToken.None)

        ' Assert
        Dim entry = Await dbContext.TamperAuditEntries.FirstOrDefaultAsync()
        Assert.NotNull(entry)
        Assert.Equal(receiptId, entry.ReceiptId)
        Assert.Equal("R-123", entry.ReceiptNumber)
        Assert.Equal("abc", entry.ExpectedHash)
        Assert.Equal("def", entry.ActualHash)
        Assert.Equal(evt.DetectedAt, entry.DetectedAt)
        Assert.Equal("System", entry.DetectedBy)
        Assert.Equal("HashMismatch", entry.TamperKind)
        Assert.Equal(Environment.MachineName, entry.MachineName)
        Assert.Equal(Environment.UserName, entry.WindowsUser)
    End Function

    <Fact>
    Public Async Function QueryService_GetIncidentsAsync_ReturnsOrderedAndLimited() As Task
        ' Arrange
        Dim dbContext = CreateDbContext()

        For i = 1 To 10
            dbContext.TamperAuditEntries.Add(New TamperAuditEntry With {
                .ReceiptId = Guid.NewGuid(),
                .TamperKind = "HashMismatch",
                .DetectedAt = DateTime.UtcNow.AddMinutes(i)
            })
        Next
        Await dbContext.SaveChangesAsync()

        Dim service = New TamperAuditQueryService(dbContext)

        ' Act
        Dim results = Await service.GetIncidentsAsync(5)

        ' Assert
        Assert.Equal(5, results.Count)
        ' The most recent (highest AddMinutes) should be first
        Assert.True(results(0).DetectedAt > results(1).DetectedAt)
    End Function

    <Fact>
    Public Async Function QueryService_CountByKindAsync_GroupsCorrectly() As Task
        ' Arrange
        Dim dbContext = CreateDbContext()

        dbContext.TamperAuditEntries.Add(New TamperAuditEntry With {.TamperKind = "HashMismatch", .ReceiptId = Guid.NewGuid()})
        dbContext.TamperAuditEntries.Add(New TamperAuditEntry With {.TamperKind = "HashMismatch", .ReceiptId = Guid.NewGuid()})
        dbContext.TamperAuditEntries.Add(New TamperAuditEntry With {.TamperKind = "MissingFile", .ReceiptId = Guid.NewGuid()})
        Await dbContext.SaveChangesAsync()

        Dim service = New TamperAuditQueryService(dbContext)

        ' Act
        Dim counts = Await service.CountByKindAsync()

        ' Assert
        Assert.Equal(2, counts.Count)
        Assert.Equal(2, counts("HashMismatch"))
        Assert.Equal(1, counts("MissingFile"))
    End Function

    <Fact>
    Public Async Function Handler_CatchesAndRethrowsException_OutsideCatchBlock() As Task
        ' Arrange
        Dim mockDb = New Mock(Of AccountingDbContext)(New DbContextOptions(Of AccountingDbContext)())
        mockDb.Setup(Function(x) x.TamperAuditEntries).Throws(New Exception("Simulated DB error"))

        Dim loggerMock = New Mock(Of ILogger(Of ReceiptTamperDetectedHandler))()
        Dim handler = New ReceiptTamperDetectedHandler(mockDb.Object, loggerMock.Object)

        Dim evt = New ReceiptTamperDetectedEvent()

        ' Act & Assert
        Dim ex = Await Assert.ThrowsAsync(Of Exception)(Function() handler.Handle(evt, CancellationToken.None))
        Assert.Equal("Failed to record receipt tamper audit entry", ex.Message)
        Assert.Equal("Simulated DB error", ex.InnerException.Message)
    End Function

End Class