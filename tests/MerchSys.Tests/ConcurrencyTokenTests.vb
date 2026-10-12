Imports Xunit
Imports System
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Purchasing.Data
Imports MerchSys.Purchasing.Entities
Imports MerchSys.Inventory.Data
Imports MerchSys.Inventory.Entities
Imports MerchSys.POS.Data
Imports MerchSys.POS.Entities
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities

''' <summary>
''' Covers the AGENTS.md requirement that mutable financial and inventory rows carry an
''' optimistic concurrency token, and that the token is actually usable.
'''
''' Two traps are locked out here:
'''  - <c>BaseDbContext.IgnoreNonTokenRowVersionConvention</c> removes any <c>RowVersion</c> that
'''    is not explicitly mapped, so an entity can declare the property and still end up without it.
'''  - A <c>byte[]</c> RowVersion maps to a <c>longblob</c> column with no auto-update, so the
'''    token never changes and can never detect a conflict. Only a <c>DateTime</c> mapped to
'''    <c>TIMESTAMP(6)</c> produces a column MariaDB keeps current via ON UPDATE.
''' </summary>
Public Class ConcurrencyTokenTests

    Private Const DummyConnection As String =
        "Server=localhost;Port=3306;Database=merchsys_central;Uid=u;Pwd=p;"

    Private Shared Sub AssertWorkingConcurrencyToken(db As DbContext, entityType As Type)
        Dim entity = db.Model.FindEntityType(entityType)
        Assert.NotNull(entity)

        Dim rowVersion = entity.FindProperty("RowVersion")
        Assert.NotNull(rowVersion)
        Assert.True(rowVersion.IsConcurrencyToken)

        ' The column type is what makes the token functional: a longblob never changes, so the
        ' concurrency check can never fail.
        Assert.Contains("timestamp", rowVersion.GetColumnType(), StringComparison.OrdinalIgnoreCase)
    End Sub

    Private Shared Function Purchasing() As PurchasingDbContext
        Return New PurchasingDbContext(
            New DbContextOptionsBuilder(Of PurchasingDbContext)().UseMySQL(DummyConnection).Options)
    End Function

    Private Shared Function Inventory() As InventoryDbContext
        Return New InventoryDbContext(
            New DbContextOptionsBuilder(Of InventoryDbContext)().UseMySQL(DummyConnection).Options)
    End Function

    Private Shared Function Pos() As POSDbContext
        Return New POSDbContext(
            New DbContextOptionsBuilder(Of POSDbContext)().UseMySQL(DummyConnection).Options)
    End Function

    Private Shared Function Accounting() As AccountingDbContext
        Return New AccountingDbContext(
            New DbContextOptionsBuilder(Of AccountingDbContext)().UseMySQL(DummyConnection).Options)
    End Function

    <Fact>
    Public Sub PurchasingEntitiesHaveWorkingConcurrencyTokens()
        Using db = Purchasing()
            AssertWorkingConcurrencyToken(db, GetType(AccountsPayableEntry))
            AssertWorkingConcurrencyToken(db, GetType(PurchaseOrderLine))
            AssertWorkingConcurrencyToken(db, GetType(ReorderConfig))
            AssertWorkingConcurrencyToken(db, GetType(ReorderSuggestion))
        End Using
    End Sub

    <Fact>
    Public Sub InventoryEntitiesHaveWorkingConcurrencyTokens()
        Using db = Inventory()
            AssertWorkingConcurrencyToken(db, GetType(StockBatch))
            AssertWorkingConcurrencyToken(db, GetType(StockAlertConfig))
        End Using
    End Sub

    <Fact>
    Public Sub PosEntitiesHaveWorkingConcurrencyTokens()
        Using db = Pos()
            AssertWorkingConcurrencyToken(db, GetType(CreditAccount))
            AssertWorkingConcurrencyToken(db, GetType(SalesTransaction))
            AssertWorkingConcurrencyToken(db, GetType(ReceiptSequence))
            AssertWorkingConcurrencyToken(db, GetType(VatConfiguration))
        End Using
    End Sub

    <Fact>
    Public Sub AccountingEntitiesHaveWorkingConcurrencyTokens()
        Using db = Accounting()
            AssertWorkingConcurrencyToken(db, GetType(VatReturn))
            AssertWorkingConcurrencyToken(db, GetType(FinancialPeriod))
        End Using
    End Sub

End Class
