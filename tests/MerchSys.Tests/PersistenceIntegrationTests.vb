Imports Xunit
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Infrastructure
Imports Microsoft.EntityFrameworkCore.Storage
Imports MySqlConnector
Imports MerchSys.Purchasing.Data
Imports MerchSys.Purchasing.Entities
Imports MerchSys.Purchasing.Services
Imports MerchSys.SharedKernel.Paging
Imports System.Threading
Imports MerchSys.Inventory.Data
Imports MerchSys.Inventory.Entities
Imports MerchSys.Inventory.Handlers
Imports MerchSys.Inventory.Services
Imports MerchSys.SharedKernel.Queries
Imports MerchSys.POS.Data
Imports MerchSys.Accounting.Data
Imports MerchSys.App.Data

''' <summary>
''' Builds a throwaway database for the DB-backed tests: creates it, materialises the module
''' schema from the EF model the same way <c>DatabaseBootstrapper</c> does, and drops it again
''' on dispose. A real schema is never touched.
''' </summary>
Public Class TestDatabaseFixture
    Implements IDisposable

    Private Const DatabaseName As String = "vista_integrationtest"
    Private Const DefaultServer As String =
        "Server=127.0.0.1;Port=3306;Uid=root;Pwd=;"

    Public ReadOnly Property ConnectionString As String

    Public Sub New()
        ConnectionString = New MySqlConnectionStringBuilder(ServerConnectionString()) With {
            .Database = DatabaseName
        }.ConnectionString

        Using conn As New MySqlConnection(ServerConnectionString())
            conn.Open()
            Using cmd = conn.CreateCommand()
                cmd.CommandText = $"DROP DATABASE IF EXISTS `{DatabaseName}`;" &
                                  $"CREATE DATABASE `{DatabaseName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"
                cmd.ExecuteNonQuery()
            End Using
        End Using

        Using db = NewPurchasing() : db.Database.GetService(Of IRelationalDatabaseCreator)().CreateTables() : End Using
        Using db = NewInventory() : db.Database.GetService(Of IRelationalDatabaseCreator)().CreateTables() : End Using
        Using db = NewPOS() : db.Database.GetService(Of IRelationalDatabaseCreator)().CreateTables() : End Using
        Using db = NewAccounting() : db.Database.GetService(Of IRelationalDatabaseCreator)().CreateTables() : End Using

        ' The audit log is deliberately not an EF entity — CreateTables() is not idempotent, so a table
        ' mapped into more than one context would be created by the first and abort the second — so it
        ' is created here exactly the way the bootstrapper creates it.
        AuditLogTableBootstrapper.EnsureTable(ConnectionString, Nothing)
    End Sub

    Friend Shared Function ServerConnectionString() As String
        Dim configured = Environment.GetEnvironmentVariable("VISTA_TEST_CONNECTION")
        Return If(String.IsNullOrWhiteSpace(configured), DefaultServer, configured)
    End Function

    Public Function NewPurchasing() As PurchasingDbContext
        Return New PurchasingDbContext(
            New DbContextOptionsBuilder(Of PurchasingDbContext)().UseMySQL(ConnectionString).Options)
    End Function

    Public Function NewInventory() As InventoryDbContext
        Return New InventoryDbContext(
            New DbContextOptionsBuilder(Of InventoryDbContext)().UseMySQL(ConnectionString).Options)
    End Function

    Public Function NewPOS() As POSDbContext
        Return New POSDbContext(
            New DbContextOptionsBuilder(Of POSDbContext)().UseMySQL(ConnectionString).Options)
    End Function

    Public Function NewAccounting() As AccountingDbContext
        Return New AccountingDbContext(
            New DbContextOptionsBuilder(Of AccountingDbContext)().UseMySQL(ConnectionString).Options)
    End Function

    Public Function NewAppConnection() As System.Data.Common.DbConnection
        Return New MySqlConnection(ConnectionString)
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        Using conn As New MySqlConnection(ServerConnectionString())
            conn.Open()
            Using cmd = conn.CreateCommand()
                cmd.CommandText = $"DROP DATABASE IF EXISTS `{DatabaseName}`;"
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

End Class

''' <summary>
''' DB-backed coverage for the code whose raw-ADO workarounds were retired.
'''
''' These need a live MariaDB. They build and drop their own throwaway database
''' (<c>VISTA_TEST_CONNECTION</c> overrides the server; the default is a local XAMPP),
''' so they never touch a real schema.
'''
''' The default suite stays DB-free. Run these explicitly:
'''   dotnet test --filter "Category=Integration"
''' and exclude them from the unit run with:
'''   dotnet test --filter "Category!=Integration"
''' </summary>
<Trait("Category", "Integration")>
<Collection("IntegrationDatabase")>
Public Class PersistenceIntegrationTests
    Implements IClassFixture(Of TestDatabaseFixture)

    Private ReadOnly _fixture As TestDatabaseFixture

    Public Sub New(fixture As TestDatabaseFixture)
        _fixture = fixture
    End Sub

    ''' <summary>
    ''' The premise of the whole cleanup: plain EF queries materialise full entities correctly.
    ''' The retired workarounds existed because this supposedly returned an empty list.
    ''' </summary>
    <Fact>
    Public Async Function PlainEfQueriesMaterialiseSeededRows() As Task
        Using ctx = _fixture.NewInventory()
            Dim products = Await ctx.Products.ToListAsync()
            Assert.Equal(20, products.Count)
            Assert.Equal(20, Await ctx.Products.CountAsync())
            Assert.Equal(4, Await ctx.ProductCategories.CountAsync())
        End Using

        Using ctx = _fixture.NewPurchasing()
            Dim vendors = Await ctx.Vendors.ToListAsync()
            Assert.Equal(3, vendors.Count)
            Assert.Equal(3, Await ctx.Vendors.CountAsync())
        End Using
    End Function

    ''' <summary>
    ''' Covers GoodsReceivingService.GetReceiptsForPOAsync, which now uses
    ''' <c>Include(Function(r) r.Lines)</c> instead of the manual parent/child join it used to do
    ''' by hand. That join existed only because GoodsReceiptConfiguration mapped the relationship
    ''' without a navigation, which made the include join on a shadow GoodsReceiptId1 key.
    ''' </summary>
    <Fact>
    Public Async Function GetReceiptsForPOAsyncReturnsReceiptsWithTheirLines() As Task
        Dim ids = Await CreateReceiptWithTwoLines()

        Using ctx = _fixture.NewPurchasing()
            ' The mediator, price-change service and VAT calculator are only used by the write
            ' paths; this query reads straight from the context.
            Dim service As New GoodsReceivingService(ctx, Nothing, Nothing, Nothing)
            Dim receipts = Await service.GetReceiptsForPOAsync(ids.OrderId)

            Dim onlyOne = Assert.Single(receipts)
            Assert.Equal(ids.ReceiptId, onlyOne.Id)
            Assert.Equal(2, onlyOne.Lines.Count)
        End Using
    End Function

    ''' <summary>
    ''' A purchase order with no receipts must come back as an empty list, not an error.
    ''' </summary>
    <Fact>
    Public Async Function GetReceiptsForPOAsyncReturnsEmptyForAnOrderWithNoReceipts() As Task
        Using ctx = _fixture.NewPurchasing()
            ' The mediator, price-change service and VAT calculator are only used by the write
            ' paths; this query reads straight from the context.
            Dim service As New GoodsReceivingService(ctx, Nothing, Nothing, Nothing)
            Dim receipts = Await service.GetReceiptsForPOAsync(-12345)

            Assert.Empty(receipts)
        End Using
    End Function

    ''' <summary>
    ''' GetReceiptByIdAsync shares the Include and was silently returning receipts with no lines.
    ''' PriceChangeService.vb:20 uses the same pattern and was affected too.
    ''' </summary>
    <Fact>
    Public Async Function GetReceiptByIdAsyncReturnsTheReceiptWithItsLines() As Task
        Dim ids = Await CreateReceiptWithTwoLines()

        Using ctx = _fixture.NewPurchasing()
            Dim service As New GoodsReceivingService(ctx, Nothing, Nothing, Nothing)
            Dim receipt = Await service.GetReceiptByIdAsync(ids.ReceiptId)

            Assert.NotNull(receipt)
            Assert.Equal(2, receipt.Lines.Count)
        End Using
    End Function

    ''' <summary>
    ''' Guards the mapping bug directly. A relationship configured without a navigation lets EF
    ''' discover a second one by convention, which materialises as a shadow "&lt;Fk&gt;1" column
    ''' and makes the navigation resolve on the wrong key.
    ''' </summary>
    <Fact>
    Public Async Function SchemaHasNoShadowForeignKeyColumns() As Task
        Using ctx = _fixture.NewPurchasing()
            Dim conn = ctx.Database.GetDbConnection()
            Await ctx.Database.OpenConnectionAsync()

            Dim strays As New List(Of String)()
            Using cmd = conn.CreateCommand()
                cmd.CommandText = "SELECT TABLE_NAME, COLUMN_NAME FROM information_schema.COLUMNS " &
                                  "WHERE TABLE_SCHEMA = DATABASE() AND COLUMN_NAME REGEXP 'Id[0-9]+$'"
                Using reader = Await cmd.ExecuteReaderAsync()
                    While Await reader.ReadAsync()
                        strays.Add(reader.GetString(0) & "." & reader.GetString(1))
                    End While
                End Using
            End Using

            Assert.Empty(strays)
        End Using
    End Function

    Private Class ReceiptFixture
        Public Property OrderId As Integer
        Public Property ReceiptId As Integer
    End Class

    ''' <summary>
    ''' Creates a purchase order with one receipt carrying two lines. The lines are attached
    ''' through <c>GoodsReceipt.Lines</c> on purpose: that only works once the relationship is
    ''' mapped with a navigation, so it exercises the fix in both directions.
    ''' </summary>
    Private Async Function CreateReceiptWithTwoLines() As Task(Of ReceiptFixture)
        Dim orderNumber = "PO-IT-" & Guid.NewGuid().ToString("N").Substring(0, 8)

        Using ctx = _fixture.NewPurchasing()
            Dim order As New PurchaseOrder With {
                .OrderNumber = orderNumber,
                .VendorId = 1,
                .TotalAmount = 100D,
                .CreatedAt = DateTime.UtcNow
            }
            ctx.PurchaseOrders.Add(order)
            Await ctx.SaveChangesAsync()

            Dim receipt As New GoodsReceipt With {
                .ReceiptNumber = "GR-IT-" & orderNumber,
                .PurchaseOrderId = order.Id,
                .Lines = New List(Of GoodsReceiptLine) From {
                    New GoodsReceiptLine With {
                        .ProductId = 1, .UnitCost = 10D, .QuantityReceived = 3,
                        .HasDiscrepancy = False, .VatAmount = 0D, .VatableSales = 30D
                    },
                    New GoodsReceiptLine With {
                        .ProductId = 2, .UnitCost = 5D, .QuantityReceived = 4,
                        .HasDiscrepancy = False, .VatAmount = 0D, .VatableSales = 20D
                    }
                }
            }
            ctx.GoodsReceipts.Add(receipt)
            Await ctx.SaveChangesAsync()

            Return New ReceiptFixture With {.OrderId = order.Id, .ReceiptId = receipt.Id}
        End Using
    End Function

    ''' <summary>
    ''' Pages a keyset-paginated history to exhaustion, asserting that no id appears on two pages
    ''' and that paging terminates. Returns the number of distinct rows seen.
    ''' </summary>
    Private Shared Async Function PageThroughAll(Of T)(
            fetch As Func(Of PageRequest, Task(Of PagedResult(Of T))),
            idOf As Func(Of T, Integer),
            pageSize As Integer) As Task(Of Integer)

        Dim seen As New HashSet(Of Integer)()
        Dim request As New PageRequest With {.PageSize = pageSize, .IsFirstPage = True}
        Dim pages = 0

        Do
            Dim page = Await fetch(request)

            For Each item In page.Items
                Assert.True(seen.Add(idOf(item)), "id " & idOf(item) & " appeared on two pages")
            Next

            pages += 1
            Assert.True(pages < 50, "paging did not terminate")

            If Not page.HasMore Then Exit Do

            request = New PageRequest With {
                .PageSize = pageSize,
                .IsFirstPage = False,
                .CursorDate = page.NextCursorDate,
                .CursorId = page.NextCursorId
            }
        Loop

        Return seen.Count
    End Function

    <Fact>
    Public Async Function VendorHistoryPagesWithoutOverlap() As Task
        Using ctx = _fixture.NewPurchasing()
            Dim service As New VendorService(ctx)

            Dim total = Await PageThroughAll(Of Vendor)(
                Function(r) service.GetHistoryAsync(r), Function(v As Vendor) v.Id, 2)

            Assert.True(total >= 3, "expected at least the 3 seeded vendors, saw " & total)
        End Using
    End Function

    ''' <summary>
    ''' All three orders share one CreatedAt, so paging has to fall back on the Id tiebreak —
    ''' the part of the keyset predicate that is easiest to get wrong.
    ''' </summary>
    <Fact>
    Public Async Function PurchaseOrderHistoryPagesWithoutOverlapWhenCreatedAtTies() As Task
        Dim stamp = DateTime.UtcNow

        Using ctx = _fixture.NewPurchasing()
            For i = 1 To 3
                ctx.PurchaseOrders.Add(New PurchaseOrder With {
                    .OrderNumber = "PO-PAGE-" & Guid.NewGuid().ToString("N").Substring(0, 8),
                    .VendorId = 1,
                    .TotalAmount = 10D,
                    .CreatedAt = stamp
                })
            Next
            Await ctx.SaveChangesAsync()
        End Using

        Using ctx = _fixture.NewPurchasing()
            Dim service As New PurchaseOrderService(ctx)

            Dim total = Await PageThroughAll(Of PurchaseOrder)(
                Function(r) service.GetHistoryAsync(r), Function(p As PurchaseOrder) p.Id, 2)

            Assert.True(total >= 3, "expected at least the 3 orders just created, saw " & total)
        End Using
    End Function

    <Fact>
    Public Async Function VendorProductHistoryPagesWithoutOverlap() As Task
        Using ctx = _fixture.NewPurchasing()
            For i = 1 To 3
                ctx.VendorProducts.Add(New VendorProduct With {
                    .VendorId = 1,
                    .ProductId = 100 + i,
                    .UnitCost = 5D,
                    .CreatedAt = DateTime.UtcNow
                })
            Next
            Await ctx.SaveChangesAsync()
        End Using

        Using ctx = _fixture.NewPurchasing()
            ' GetHistoryAsync does not touch the session service; only the write paths do.
            Dim service As New VendorProductService(ctx, Nothing)

            Dim total = Await PageThroughAll(Of VendorProduct)(
                Function(r) service.GetHistoryAsync(r), Function(vp As VendorProduct) vp.Id, 2)

            Assert.True(total >= 3, "expected at least the 3 entries just created, saw " & total)
        End Using
    End Function

    ''' <summary>
    ''' The seeded products have no batches, so this exercises the empty-group path — where a plain
    ''' Sum over Integer would fault on the NULL that SQL returns.
    ''' </summary>
    <Fact>
    Public Async Function CurrentStockLevelsCoalesceMissingBatchesToZero() As Task
        Using ctx = _fixture.NewInventory()
            Dim levels = Await New StockService(ctx).GetCurrentStockLevelsAsync()

            Assert.Equal(20, levels.Count)
            Assert.All(levels, Sub(l) Assert.Equal(0, l.CurrentQuantity))
        End Using

        Dim targetProductId As Integer
        Using ctx = _fixture.NewInventory()
            targetProductId = (Await ctx.Products.OrderBy(Function(p) p.Id).FirstAsync()).Id
            ctx.StockBatches.Add(New StockBatch With {
                .ProductId = targetProductId,
                .QuantityReceived = 5,
                .QuantityRemaining = 5,
                .UnitCost = 2D,
                .ReceiptDate = DateTime.UtcNow,
                .SourcePurchaseOrderId = 1
            })
            Await ctx.SaveChangesAsync()
        End Using

        Using ctx = _fixture.NewInventory()
            Dim levels = Await New StockService(ctx).GetCurrentStockLevelsAsync()
            Dim target = levels.First(Function(l) l.ProductId = targetProductId)

            Assert.Equal(5, target.CurrentQuantity)
        End Using
    End Function

    <Fact>
    Public Async Function CatalogQueryReturnsTheSeededProducts() As Task
        Using ctx = _fixture.NewInventory()
            Dim handler As New GetProductsForCatalogQueryHandler(ctx)

            Dim products = Await handler.Handle(New GetProductsForCatalogQuery(), CancellationToken.None)

            Assert.Equal(20, products.Count)
            Assert.All(products, Sub(p) Assert.False(String.IsNullOrWhiteSpace(p.Name)))
        End Using
    End Function

    ''' <summary>
    ''' The FIFO cost of a sale is only knowable at deduction time: nothing in the model records
    ''' which batches a sale consumed, so once the batches are decremented the information cannot be
    ''' reconstructed. DeductStockFIFOAsync therefore has to report the cost of what it consumed —
    ''' that value is what feeds COGS.
    '''
    ''' Uses product 2 rather than the lowest id, because the class fixture is shared and
    ''' CurrentStockLevelsCoalesceMissingBatchesToZero seeds a batch for the lowest-id product.
    ''' </summary>
    <Fact>
    Public Async Function FifoDeductionReportsTheCostOfTheBatchesItConsumed() As Task
        Const productId As Integer = 2

        Using ctx = _fixture.NewInventory()
            ' Explicit precondition — this test assumes it is the only source of batches here.
            Assert.Empty(Await ctx.StockBatches.Where(Function(b) b.ProductId = productId).ToListAsync())
        End Using

        Using ctx = _fixture.NewInventory()
            Dim service As New StockService(ctx)

            ' Two batches at different costs; the earlier one must be consumed first.
            Await service.AddStockBatchAsync(productId, 5, 10D, New DateTime(2026, 1, 1), 0, Nothing)
            Await service.AddStockBatchAsync(productId, 5, 14D, New DateTime(2026, 2, 1), 0, Nothing)

            ' Sell 7: all 5 of the cheap batch, then 2 of the dearer one.
            '   5 * 10.00 = 50.00
            '   2 * 14.00 = 28.00
            '               ------
            '                78.00
            Dim result = Await service.DeductStockFIFOAsync(productId, 7)

            Assert.Equal(7, result.DeductedQuantity)
            Assert.True(result.Success)
            Assert.Equal(78D, result.TotalCost)
        End Using
    End Function

End Class
