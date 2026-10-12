Imports Xunit
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Purchasing.Data
Imports MerchSys.Purchasing.Entities
Imports MerchSys.Inventory.Data
Imports MerchSys.Inventory.Entities
Imports MerchSys.POS.Data
Imports MerchSys.POS.Entities

''' <summary>
''' Covers the AGENTS.md requirement that the mutable financial and inventory rows carry an
''' optimistic concurrency token.
'''
''' These assertions also prove that <c>BaseDbContext.IgnoreNonTokenRowVersionConvention</c>
''' did not strip the property: it removes any <c>RowVersion</c> that is not explicitly mapped
''' as a concurrency token, so an entity can declare the property and still end up without one.
''' </summary>
Public Class ConcurrencyTokenTests

    Private Const DummyConnection As String =
        "Server=localhost;Port=3306;Database=merchsys_central;Uid=u;Pwd=p;"

    Private Shared Sub AssertHasConcurrencyToken(db As DbContext, entityType As System.Type)
        Dim entity = db.Model.FindEntityType(entityType)
        Assert.NotNull(entity)

        Dim rowVersion = entity.FindProperty("RowVersion")
        Assert.NotNull(rowVersion)
        Assert.True(rowVersion.IsConcurrencyToken)
    End Sub

    <Fact>
    Public Sub AccountsPayableEntryHasAConcurrencyToken()
        Dim options = New DbContextOptionsBuilder(Of PurchasingDbContext)().
            UseMySQL(DummyConnection).Options

        Using db As New PurchasingDbContext(options)
            AssertHasConcurrencyToken(db, GetType(AccountsPayableEntry))
        End Using
    End Sub

    <Fact>
    Public Sub StockBatchHasAConcurrencyToken()
        Dim options = New DbContextOptionsBuilder(Of InventoryDbContext)().
            UseMySQL(DummyConnection).Options

        Using db As New InventoryDbContext(options)
            AssertHasConcurrencyToken(db, GetType(StockBatch))
        End Using
    End Sub

    <Fact>
    Public Sub CreditAccountHasAConcurrencyToken()
        Dim options = New DbContextOptionsBuilder(Of POSDbContext)().
            UseMySQL(DummyConnection).Options

        Using db As New POSDbContext(options)
            AssertHasConcurrencyToken(db, GetType(CreditAccount))
        End Using
    End Sub

End Class
