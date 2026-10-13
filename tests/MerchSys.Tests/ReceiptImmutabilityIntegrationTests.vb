Imports Xunit
Imports System
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports MySqlConnector
Imports MerchSys.App.Data
Imports MerchSys.POS.Data
Imports MerchSys.POS.Data.Interceptors
Imports MerchSys.POS.Entities
Imports MerchSys.SharedKernel.Exceptions

''' <summary>
''' DB-backed coverage for BIR receipt-archive immutability (H3 triggers, H4 interceptor).
'''
''' Needs a live MariaDB; builds and drops its own throwaway database via
''' <see cref="TestDatabaseFixture"/> (<c>VISTA_TEST_CONNECTION</c> overrides the server). Run with
'''   dotnet test --filter "Category=Integration"
''' and exclude from the unit run with:
'''   dotnet test --filter "Category!=Integration"
''' </summary>
<Trait("Category", "Integration")>
<Collection("IntegrationDatabase")>
Public Class ReceiptImmutabilityIntegrationTests
    Implements IClassFixture(Of TestDatabaseFixture)

    Private ReadOnly _fixture As TestDatabaseFixture

    Public Sub New(fixture As TestDatabaseFixture)
        _fixture = fixture
    End Sub

    ''' <summary>
    ''' Applying the triggers twice must be safe — the script drops before it creates — because
    ''' <c>DatabaseBootstrapper</c> runs this on every launch.
    ''' </summary>
    <Fact>
    Public Sub EnsureTriggersIsIdempotentAndLeavesBothTriggersPresent()
        ReceiptArchiveTriggerBootstrapper.EnsureTriggers(_fixture.ConnectionString, Nothing)
        ReceiptArchiveTriggerBootstrapper.EnsureTriggers(_fixture.ConnectionString, Nothing)

        Using conn As New MySqlConnection(_fixture.ConnectionString)
            conn.Open()
            Assert.True(ReceiptArchiveTriggerBootstrapper.HasAllRequiredTriggers(
                ReceiptArchiveTriggerBootstrapper.ReadTriggerNames(conn)))
        End Using
    End Sub

    ''' <summary>
    ''' The point of the whole exercise: once applied, the database itself refuses to let an archive
    ''' row be changed or removed. This is the enforcement that EF cannot express, and the only
    ''' protection for a direct-SQL writer that never loads the interceptor.
    ''' </summary>
    <Fact>
    Public Async Function TriggersRejectUpdateAndDeleteOnTheArchiveTable() As Task
        ReceiptArchiveTriggerBootstrapper.EnsureTriggers(_fixture.ConnectionString, Nothing)

        Dim id = Await InsertArchiveRow()

        Using conn As New MySqlConnection(_fixture.ConnectionString)
            conn.Open()
            Assert.True(Await FailsWithBirImmutable(conn, $"UPDATE Pos_OfficialReceiptArchive SET ReceiptNumber = 'tampered' WHERE Id = {id}"),
                        "an UPDATE on the archive table was allowed")
            Assert.True(Await FailsWithBirImmutable(conn, $"DELETE FROM Pos_OfficialReceiptArchive WHERE Id = {id}"),
                        "a DELETE on the archive table was allowed")
        End Using
    End Function

    ''' <summary>
    ''' H4: the interceptor used to cover only <c>OfficialReceipt</c> and <c>ReceiptIntegrity</c>, so
    ''' the two archive entities could be edited or deleted through EF. It must now throw for both.
    ''' </summary>
    <Fact>
    Public Async Function InterceptorBlocksModifyingAndDeletingArchiveRows() As Task
        Dim options = New DbContextOptionsBuilder(Of POSDbContext)().
            UseMySQL(_fixture.ConnectionString).
            AddInterceptors(New ImmutableReceiptInterceptor()).
            Options

        Dim id As Integer
        Using ctx = New POSDbContext(options)
            Dim archive = NewArchiveRow()
            ctx.OfficialReceiptArchives.Add(archive)
            Await ctx.SaveChangesAsync()
            id = archive.Id
        End Using

        ' A fresh context each time: the first SaveChanges leaves the entry tracked and dirty.
        Using ctx = New POSDbContext(options)
            Dim archive = Await ctx.OfficialReceiptArchives.FirstAsync(Function(a) a.Id = id)
            archive.ReceiptNumber = "tampered"
            Await Assert.ThrowsAsync(Of ImmutableEntityException)(Async Function() Await ctx.SaveChangesAsync())
        End Using

        Using ctx = New POSDbContext(options)
            Dim archive = Await ctx.OfficialReceiptArchives.FirstAsync(Function(a) a.Id = id)
            ctx.OfficialReceiptArchives.Remove(archive)
            Await Assert.ThrowsAsync(Of ImmutableEntityException)(Async Function() Await ctx.SaveChangesAsync())
        End Using
    End Function

    ''' <summary>
    ''' Guards the other direction: the interceptor must not block the insert the archival service
    ''' performs, or archiving would break.
    ''' </summary>
    <Fact>
    Public Async Function InterceptorAllowsInsertingArchiveRows() As Task
        Dim options = New DbContextOptionsBuilder(Of POSDbContext)().
            UseMySQL(_fixture.ConnectionString).
            AddInterceptors(New ImmutableReceiptInterceptor()).
            Options

        Using ctx = New POSDbContext(options)
            ctx.OfficialReceiptArchives.Add(NewArchiveRow())
            Assert.Equal(1, Await ctx.SaveChangesAsync())
        End Using
    End Function

    Private Async Function InsertArchiveRow() As Task(Of Integer)
        Using ctx = _fixture.NewPOS()
            Dim archive = NewArchiveRow()
            ctx.OfficialReceiptArchives.Add(archive)
            Await ctx.SaveChangesAsync()
            Return archive.Id
        End Using
    End Function

    Private Shared Function NewArchiveRow() As OfficialReceiptArchive
        Return New OfficialReceiptArchive With {
            .OriginalReceiptId = 1,
            .ReceiptNumber = "OR-TRIG-" & Guid.NewGuid().ToString("N").Substring(0, 8),
            .TransactionId = 1,
            .ReceiptDate = DateTime.UtcNow,
            .SerializedLines = "[]",
            .ArchivedAt = DateTime.UtcNow,
            .ArchivedHash = "hash"
        }
    End Function

    Private Shared Async Function FailsWithBirImmutable(conn As MySqlConnection, sql As String) As Task(Of Boolean)
        Try
            Using cmd = conn.CreateCommand()
                cmd.CommandText = sql
                Await cmd.ExecuteNonQueryAsync()
            End Using
            Return False
        Catch ex As MySqlException
            Return ex.Message.Contains("BIR-archive-immutable")
        End Try
    End Function

End Class
