Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks
Imports Xunit
Imports Moq
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Logging
Imports MerchSys.SharedKernel.Events
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities
Imports MerchSys.Accounting.Handlers
Imports MerchSys.Accounting.Services
Imports MerchSys.Tests.Fakes

Namespace Tests

    Public Class ReceiptTamperDetectedHandlerTests

        Private Function SetupMockDbSet(Of T As Class)(elements As List(Of T)) As Mock(Of DbSet(Of T))
            Dim mockSet = New Mock(Of DbSet(Of T))()
            Dim queryable = elements.AsQueryable()

            mockSet.As(Of IQueryable(Of T))().Setup(Function(m) m.Provider).Returns(New TestAsyncQueryProvider(Of T)(queryable.Provider))
            mockSet.As(Of IQueryable(Of T))().Setup(Function(m) m.Expression).Returns(queryable.Expression)
            mockSet.As(Of IQueryable(Of T))().Setup(Function(m) m.ElementType).Returns(queryable.ElementType)
            mockSet.As(Of IQueryable(Of T))().Setup(Function(m) m.GetEnumerator()).Returns(queryable.GetEnumerator())

            mockSet.As(Of IAsyncEnumerable(Of T))().Setup(Function(m) m.GetAsyncEnumerator(It.IsAny(Of CancellationToken)())).Returns(New TestAsyncEnumerator(Of T)(queryable.GetEnumerator()))

            mockSet.Setup(Function(m) m.Add(It.IsAny(Of T)())).Callback(New Action(Of T)(Sub(item) elements.Add(item)))
            Return mockSet
        End Function

        <Fact>
        Public Async Function Handler_SavesTamperAuditEntry_WithCorrectValues() As Task
            Dim options = New DbContextOptionsBuilder(Of AccountingDbContext)().Options
            Dim mockContext = New Mock(Of AccountingDbContext)(options, Nothing, Nothing)
            Dim entries = New List(Of TamperAuditEntry)()
            Dim mockDbSet = SetupMockDbSet(entries)

            mockContext.SetupProperty(Function(c) c.TamperAuditEntries, mockDbSet.Object)
            mockContext.Setup(Function(c) c.SaveChangesAsync(It.IsAny(Of CancellationToken)())).ReturnsAsync(1)

            Dim mockLogger = New Mock(Of ILogger(Of ReceiptTamperDetectedHandler))()
            Dim handler = New ReceiptTamperDetectedHandler(mockContext.Object, mockLogger.Object)

            Dim evt = New ReceiptTamperDetectedEvent With {
                .ReceiptId = Guid.NewGuid(),
                .ReceiptNumber = "R-12345",
                .ExpectedHash = "abc",
                .ActualHash = "def",
                .DetectedAt = DateTime.UtcNow,
                .DetectedBy = "POS-1"
            }

            Await handler.Handle(evt, CancellationToken.None)

            Assert.Single(entries)
            Dim entry = entries.First()
            Assert.Equal(evt.ReceiptId, entry.ReceiptId)
            Assert.Equal(evt.ReceiptNumber, entry.ReceiptNumber)
            Assert.Equal(evt.ExpectedHash, entry.ExpectedHash)
            Assert.Equal(evt.ActualHash, entry.ActualHash)
            Assert.Equal(evt.DetectedAt, entry.DetectedAt)
            Assert.Equal(evt.DetectedBy, entry.DetectedBy)
            Assert.Equal("HashMismatch", entry.TamperKind)
            Assert.NotNull(entry.MachineName)
            mockContext.Verify(Function(c) c.SaveChangesAsync(It.IsAny(Of CancellationToken)()), Times.Once())
        End Function

        <Fact>
        Public Async Function GetIncidentsAsync_OrdersByDetectedAtDesc() As Task
            Dim options = New DbContextOptionsBuilder(Of AccountingDbContext)().Options
            Dim mockContext = New Mock(Of AccountingDbContext)(options, Nothing, Nothing)

            Dim entries = New List(Of TamperAuditEntry) From {
                New TamperAuditEntry With { .Id = 1, .DetectedAt = New DateTime(2023, 1, 1) },
                New TamperAuditEntry With { .Id = 2, .DetectedAt = New DateTime(2023, 1, 3) },
                New TamperAuditEntry With { .Id = 3, .DetectedAt = New DateTime(2023, 1, 2) }
            }
            Dim mockDbSet = SetupMockDbSet(entries)
            mockContext.SetupProperty(Function(c) c.TamperAuditEntries, mockDbSet.Object)

            Dim service = New TamperAuditQueryService(mockContext.Object)

            Dim result = Await service.GetIncidentsAsync()

            Assert.Equal(3, result.Count)
            Assert.Equal(2, result(0).Id)
            Assert.Equal(3, result(1).Id)
            Assert.Equal(1, result(2).Id)
        End Function

        <Fact>
        Public Async Function CountByKindAsync_ReturnsCorrectCountMappings() As Task
            Dim options = New DbContextOptionsBuilder(Of AccountingDbContext)().Options
            Dim mockContext = New Mock(Of AccountingDbContext)(options, Nothing, Nothing)

            Dim entries = New List(Of TamperAuditEntry) From {
                New TamperAuditEntry With { .Id = 1, .TamperKind = "KindA" },
                New TamperAuditEntry With { .Id = 2, .TamperKind = "KindB" },
                New TamperAuditEntry With { .Id = 3, .TamperKind = "KindA" }
            }
            Dim mockDbSet = SetupMockDbSet(entries)
            mockContext.SetupProperty(Function(c) c.TamperAuditEntries, mockDbSet.Object)

            Dim service = New TamperAuditQueryService(mockContext.Object)

            Dim result = Await service.CountByKindAsync()

            Assert.Equal(2, result.Count)
            Assert.Equal(2, result("KindA"))
            Assert.Equal(1, result("KindB"))
        End Function

    End Class

End Namespace
