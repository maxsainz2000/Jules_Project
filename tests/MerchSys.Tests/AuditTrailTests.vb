Imports Xunit
Imports System
Imports System.Collections.Generic
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.DependencyInjection
Imports MySqlConnector
Imports MerchSys.Accounting.Data
Imports MerchSys.App.Data
Imports MerchSys.Inventory.Data
Imports MerchSys.Inventory.Entities
Imports MerchSys.POS.Data
Imports MerchSys.Purchasing.Data
Imports MerchSys.Purchasing.Entities
Imports MerchSys.SharedKernel.Data

''' <summary>
''' DB-backed coverage for the audit trail (H1).
'''
''' The audit rows are read with raw ADO because the audit table is deliberately not an EF entity —
''' see <c>AuditLogSchema</c> for why.
'''
''' Needs a live MariaDB; builds and drops its own throwaway database via
''' <see cref="TestDatabaseFixture"/>. Run with:
'''   dotnet test --filter "Category=Integration"
''' </summary>
<Trait("Category", "Integration")>
<Collection("IntegrationDatabase")>
Public Class AuditTrailTests
    Implements IClassFixture(Of TestDatabaseFixture)

    Private ReadOnly _fixture As TestDatabaseFixture

    Public Sub New(fixture As TestDatabaseFixture)
        _fixture = fixture
    End Sub

    ''' <summary>
    ''' The core of the requirement: an update records who changed what, and the values the row no
    ''' longer holds. Those previous values exist nowhere else once the update commits.
    ''' </summary>
    <Fact>
    Public Async Function UpdatingAnAuditedRecordWritesOneRowWithThePreviousValue() As Task
        Dim batchId = Await SeedBatchAsync(5)

        Using ctx = _fixture.NewInventory()
            Dim batch = Await ctx.StockBatches.FirstAsync(Function(b) b.Id = batchId)
            batch.QuantityRemaining = 3
            Await ctx.SaveChangesAsync()
        End Using

        Dim rows = Await ReadAuditRowsAsync("StockBatch", batchId.ToString())
        ' Two rows for the batch: its own insert (the seed is itself an audited change) and the
        ' update under test — exactly one audit row per change.
        Assert.Equal(2, rows.Count)
        Dim row = Assert.Single(rows.Where(Function(r) r.AuditAction = "Update"))

        Assert.Equal("Inv", row.ModuleTag)
        Assert.Equal("System", row.Actor)          ' no session in a test, so the actor falls back
        Assert.Contains("QuantityRemaining", row.ChangesJson)
        Assert.Contains("""from"":5", row.ChangesJson)
        Assert.Contains("""to"":3", row.ChangesJson)
    End Function

    ''' <summary>
    ''' The log must commit with the change. A rolled-back change leaving an audit row would make the
    ''' log actively misleading.
    ''' </summary>
    <Fact>
    Public Async Function ARolledBackChangeWritesNoAuditRow() As Task
        Dim batchId = Await SeedBatchAsync(5)

        ' The seed's own insert row is expected to be there; the rolled-back change must add nothing.
        Dim before = (Await ReadAuditRowsAsync("StockBatch", batchId.ToString())).Count

        Using ctx = _fixture.NewInventory()
            Dim transaction = Await ctx.Database.BeginTransactionAsync()
            Dim batch = Await ctx.StockBatches.FirstAsync(Function(b) b.Id = batchId)
            batch.QuantityRemaining = 1
            Await ctx.SaveChangesAsync()
            Await transaction.RollbackAsync()
        End Using

        Assert.Equal(before, (Await ReadAuditRowsAsync("StockBatch", batchId.ToString())).Count)

        ' And the change really did roll back.
        Using ctx = _fixture.NewInventory()
            Dim batch = Await ctx.StockBatches.FirstAsync(Function(b) b.Id = batchId)
            Assert.Equal(5, batch.QuantityRemaining)
        End Using
    End Function

    <Fact>
    Public Async Function InsertingAnAuditedRecordWritesAnInsertRow() As Task
        Dim orderId As Integer

        Using ctx = _fixture.NewPurchasing()
            Dim order As New PurchaseOrder With {
                .OrderNumber = "PO-AUDIT-" & Guid.NewGuid().ToString("N").Substring(0, 8),
                .VendorId = 1,
                .Status = MerchSys.SharedKernel.Enums.PurchaseOrderStatus.Draft,
                .TotalAmount = 10D,
                .CreatedAt = DateTime.UtcNow
            }
            ctx.PurchaseOrders.Add(order)
            Await ctx.SaveChangesAsync()
            orderId = order.Id
        End Using

        Dim rows = Await ReadAuditRowsAsync("PurchaseOrder", orderId.ToString())
        Dim row = Assert.Single(rows)

        Assert.Equal("Insert", row.AuditAction)
        Assert.Equal("Pur", row.ModuleTag)
    End Function

    ''' <summary>
    ''' A soft delete is audited as an update — the row survives, so the change worth recording is the
    ''' IsDeleted flip, not a set of values that vanished.
    ''' </summary>
    <Fact>
    Public Async Function ASoftDeleteIsAuditedAsAnUpdate() As Task
        Dim orderId As Integer

        Using ctx = _fixture.NewPurchasing()
            Dim order As New PurchaseOrder With {
                .OrderNumber = "PO-AUDIT-DEL-" & Guid.NewGuid().ToString("N").Substring(0, 8),
                .VendorId = 1,
                .Status = MerchSys.SharedKernel.Enums.PurchaseOrderStatus.Draft,
                .TotalAmount = 10D,
                .CreatedAt = DateTime.UtcNow
            }
            ctx.PurchaseOrders.Add(order)
            Await ctx.SaveChangesAsync()
            orderId = order.Id
        End Using

        Using ctx = _fixture.NewPurchasing()
            Dim order = Await ctx.PurchaseOrders.FirstAsync(Function(p) p.Id = orderId)
            ctx.PurchaseOrders.Remove(order)
            Await ctx.SaveChangesAsync()
        End Using

        Dim rows = Await ReadAuditRowsAsync("PurchaseOrder", orderId.ToString())
        Assert.Equal(2, rows.Count)                                   ' the insert, then the soft delete
        Assert.Contains(rows, Function(r) r.AuditAction = "Insert")
        Assert.Contains(rows, Function(r) r.AuditAction = "Update" AndAlso r.ChangesJson.Contains("IsDeleted"))
    End Function

    ''' <summary>
    ''' An audit log that can be silently edited is worth nothing, so the table rejects UPDATE and
    ''' DELETE at the database, not just in application code.
    ''' </summary>
    <Fact>
    Public Async Function AuditRowsCannotBeUpdatedOrDeleted() As Task
        ReceiptArchiveTriggerBootstrapper.EnsureTriggers(_fixture.ConnectionString, Nothing)

        ' INSERT must stay allowed — the log has to be writable.
        Using conn As New MySqlConnection(_fixture.ConnectionString)
            conn.Open()
            Using cmd = conn.CreateCommand()
                cmd.CommandText = "INSERT INTO `App_AuditLog` (`OccurredAt`,`Actor`,`WriteContext`,`Module`,`EntityType`,`EntityId`,`Action`,`ChangesJson`) " &
                                  "VALUES (UTC_TIMESTAMP(6),'test','User','Pos','TriggerProbe','1','Update','{}')"
                Assert.Equal(1, Await cmd.ExecuteNonQueryAsync())
            End Using

            Assert.True(Await FailsWith(conn, "UPDATE `App_AuditLog` SET `Actor` = 'tampered' WHERE `EntityType` = 'TriggerProbe'"),
                        "an UPDATE on the audit log was allowed")
            Assert.True(Await FailsWith(conn, "DELETE FROM `App_AuditLog` WHERE `EntityType` = 'TriggerProbe'"),
                        "a DELETE on the audit log was allowed")
        End Using
    End Function

    ''' <summary>
    ''' The regression §3 of Design_Audit_Trail_H1.md predicted, and the reason the audit log is not
    ''' an EF entity: had it been mapped in all four contexts, <c>CreateTables()</c> — not idempotent
    ''' — would abort on the second context, leaving a fresh database with only the first module's
    ''' schema. This exercises the real startup bootstrapper, layer by layer, against a genuinely
    ''' empty database.
    ''' </summary>
    <Fact>
    Public Async Function AFreshDatabaseBootstrapsEveryModuleSchemaAndTheAuditTable() As Task
        Const DatabaseName As String = "vista_bootstraptest"

        Try
            Using conn As New MySqlConnection(TestDatabaseFixture.ServerConnectionString())
                conn.Open()
                Using cmd = conn.CreateCommand()
                    cmd.CommandText = $"DROP DATABASE IF EXISTS `{DatabaseName}`;" &
                                      $"CREATE DATABASE `{DatabaseName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"
                    Await cmd.ExecuteNonQueryAsync()
                End Using
            End Using

            Dim connectionString = New MySqlConnectionStringBuilder(TestDatabaseFixture.ServerConnectionString()) With {
                .Database = DatabaseName
            }.ConnectionString

            Dim services As New ServiceCollection()
            services.AddDbContext(Of PurchasingDbContext)(Sub(sp, o) o.UseMySQL(connectionString))
            services.AddDbContext(Of InventoryDbContext)(Sub(sp, o) o.UseMySQL(connectionString))
            services.AddDbContext(Of POSDbContext)(Sub(sp, o) o.UseMySQL(connectionString))
            services.AddDbContext(Of AccountingDbContext)(Sub(sp, o) o.UseMySQL(connectionString))

            ' The whole real startup path: central schema, the shared audit table, all four module
            ' schemas from the EF models, then the immutability triggers.
            DatabaseBootstrapper.Initialize(connectionString, services.BuildServiceProvider(), Nothing)

            ' Case-insensitive on purpose: XAMPP's default lower_case_table_names=1 folds the stored
            ' names, and information_schema reports them folded. DatabaseBootstrapper itself compares
            ' the same way.
            Dim names As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Using conn As New MySqlConnection(connectionString)
                conn.Open()
                Using cmd = conn.CreateCommand()
                    cmd.CommandText = "SELECT TABLE_NAME FROM information_schema.TABLES " &
                                      "WHERE TABLE_SCHEMA = DATABASE() AND TABLE_TYPE = 'BASE TABLE'"
                    Using reader = Await cmd.ExecuteReaderAsync()
                        While Await reader.ReadAsync()
                            names.Add(reader.GetString(0))
                        End While
                    End Using
                End Using
            End Using

            Assert.Contains("Pur_Vendors", names)
            Assert.Contains("Inv_StockBatches", names)
            Assert.Contains("Pos_SalesTransactions", names)
            Assert.Contains("Acc_VatReturns", names)
            Assert.Contains("App_AuditLog", names)
            ' One shared log — not the four prefixed copies the prefixer would have produced.
            Assert.DoesNotContain(names, Function(n) n.EndsWith("_AuditLog", StringComparison.OrdinalIgnoreCase) AndAlso
                                                  Not n.Equals("App_AuditLog", StringComparison.OrdinalIgnoreCase))
        Finally
            ' Sync on purpose: Await is illegal in a Finally (BC36943).
            Using conn As New MySqlConnection(TestDatabaseFixture.ServerConnectionString())
                conn.Open()
                Using cmd = conn.CreateCommand()
                    cmd.CommandText = $"DROP DATABASE IF EXISTS `{DatabaseName}`;"
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Try
    End Function

    Private Async Function SeedBatchAsync(quantity As Integer) As Task(Of Integer)
        Using ctx = _fixture.NewInventory()
            Dim batch As New StockBatch With {
                .ProductId = 99,
                .QuantityReceived = quantity,
                .QuantityRemaining = quantity,
                .UnitCost = 1D,
                .ReceiptDate = New DateTime(2026, 1, 1),
                .SourcePurchaseOrderId = 0
            }
            ctx.StockBatches.Add(batch)
            Await ctx.SaveChangesAsync()
            Return batch.Id
        End Using
    End Function

    Private Async Function ReadAuditRowsAsync(entityType As String, entityId As String) As Task(Of List(Of AuditRowSnapshot))
        Dim rows As New List(Of AuditRowSnapshot)()

        Using conn As New MySqlConnection(_fixture.ConnectionString)
            conn.Open()
            Using cmd = conn.CreateCommand()
                cmd.CommandText = "SELECT `Action`,`Module`,`Actor`,`ChangesJson` FROM `App_AuditLog` " &
                                  "WHERE `EntityType` = @entityType AND `EntityId` = @entityId ORDER BY `Id`"
                cmd.Parameters.AddWithValue("@entityType", entityType)
                cmd.Parameters.AddWithValue("@entityId", entityId)

                Using reader = Await cmd.ExecuteReaderAsync()
                    While Await reader.ReadAsync()
                        rows.Add(New AuditRowSnapshot With {
                            .AuditAction = reader.GetString(0),
                            .ModuleTag = reader.GetString(1),
                            .Actor = reader.GetString(2),
                            .ChangesJson = If(reader.IsDBNull(3), String.Empty, reader.GetString(3))
                        })
                    End While
                End Using
            End Using
        End Using

        Return rows
    End Function

    Private Shared Async Function FailsWith(conn As MySqlConnection, sql As String) As Task(Of Boolean)
        Try
            Using cmd = conn.CreateCommand()
                cmd.CommandText = sql
                Await cmd.ExecuteNonQueryAsync()
            End Using
            Return False
        Catch ex As MySqlException
            Return ex.Message.Contains("audit-log-immutable")
        End Try
    End Function

    Private Class AuditRowSnapshot
        Public Property AuditAction As String
        Public Property ModuleTag As String
        Public Property Actor As String
        Public Property ChangesJson As String
    End Class

End Class
