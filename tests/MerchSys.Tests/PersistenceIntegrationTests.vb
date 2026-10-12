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
Imports MerchSys.Inventory.Data
Imports MerchSys.POS.Data
Imports MerchSys.Accounting.Data

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
    End Sub

    Private Function ServerConnectionString() As String
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
    ''' Covers GoodsReceivingService.GetReceiptsForPOAsync.
    '''
    ''' This method deliberately keeps its manual parent/child join. Replacing it with
    ''' <c>Include(Function(r) r.Lines)</c> was tried on 2026-10-12 and returned an empty
    ''' collection: GoodsReceiptConfiguration maps the relationship as
    ''' <c>HasMany(Of GoodsReceiptLine)().WithOne()</c> — no navigation on the principal — so
    ''' <c>GoodsReceipt.Lines</c> is not a usable navigation and the raw join was compensating for
    ''' that, not for the disproven ToListAsync claim. Fixing the mapping is a prerequisite.
    ''' </summary>
    <Fact>
    Public Async Function GetReceiptsForPOAsyncReturnsReceiptsWithTheirLines() As Task
        Dim orderNumber = "PO-IT-" & Guid.NewGuid().ToString("N").Substring(0, 8)
        Dim orderId As Integer
        Dim receiptId As Integer

        Using ctx = _fixture.NewPurchasing()
            Dim order As New PurchaseOrder With {
                .OrderNumber = orderNumber,
                .VendorId = 1,
                .TotalAmount = 100D,
                .CreatedAt = DateTime.UtcNow
            }
            ctx.PurchaseOrders.Add(order)
            Await ctx.SaveChangesAsync()
            orderId = order.Id

            Dim receipt As New GoodsReceipt With {
                .ReceiptNumber = "GR-IT-" & orderNumber,
                .PurchaseOrderId = orderId
            }
            ctx.GoodsReceipts.Add(receipt)
            Await ctx.SaveChangesAsync()
            receiptId = receipt.Id

            ' The GoodsReceiptLine -> GoodsReceipt relationship is configured without an inverse
            ' navigation, so adding lines to GoodsReceipt.Lines does not fix up the child FK.
            ' Set it explicitly to isolate the Include() behaviour under test.
            ctx.GoodsReceiptLines.Add(New GoodsReceiptLine With {
                .GoodsReceiptId = receiptId, .ProductId = 1, .UnitCost = 10D,
                .QuantityReceived = 3, .HasDiscrepancy = False,
                .VatAmount = 0D, .VatableSales = 30D
            })
            ctx.GoodsReceiptLines.Add(New GoodsReceiptLine With {
                .GoodsReceiptId = receiptId, .ProductId = 2, .UnitCost = 5D,
                .QuantityReceived = 4, .HasDiscrepancy = False,
                .VatAmount = 0D, .VatableSales = 20D
            })
            Await ctx.SaveChangesAsync()
        End Using

        Using ctx = _fixture.NewPurchasing()
            ' The mediator, price-change service and VAT calculator are only used by the write
            ' paths; this query reads straight from the context.
            Dim service As New GoodsReceivingService(ctx, Nothing, Nothing, Nothing)
            Dim receipts = Await service.GetReceiptsForPOAsync(orderId)

            Dim onlyOne = Assert.Single(receipts)
            Assert.Equal(receiptId, onlyOne.Id)
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

End Class
