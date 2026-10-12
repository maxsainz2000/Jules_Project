Imports Xunit
Imports System.Linq
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Purchasing.Data
Imports MerchSys.Inventory.Data
Imports MerchSys.POS.Data
Imports MerchSys.Accounting.Data

''' <summary>
''' Locks in the single-prefix table naming contract.
'''
''' The module prefix must be applied exactly once. The raw-ADO queries
''' (for example <c>FROM Pur_VendorProducts</c> in VendorProductService) and the operator
''' runbooks use these names, so a table mapped as <c>Pur_Pur_VendorProducts</c> is invisible
''' to them.
''' </summary>
Public Class ModuleTableNameTests

    Private Const DummyConnection As String =
        "Server=localhost;Port=3306;Database=vista_central;Uid=u;Pwd=p;"

    Private Shared Function MappedTableNames(db As DbContext) As List(Of String)
        Return db.Model.GetEntityTypes().
            Select(Function(et) et.GetTableName()).
            Where(Function(n) n IsNot Nothing).
            Distinct().
            ToList()
    End Function

    <Fact>
    Public Sub PurchasingTablesArePrefixedExactlyOnce()
        Dim options = New DbContextOptionsBuilder(Of PurchasingDbContext)().
            UseMySQL(DummyConnection).Options

        Using db As New PurchasingDbContext(options)
            Dim names = MappedTableNames(db)

            Assert.Contains("Pur_VendorProducts", names)
            Assert.Contains("Pur_PriceChangeAlerts", names)
            Assert.Contains("Pur_AccountsPayable", names)
            Assert.Contains("Pur_PurchaseOrders", names)
            Assert.Contains("Pur_Vendors", names)
            Assert.False(names.Any(Function(n) n.StartsWith("Pur_Pur_")))
        End Using
    End Sub

    <Fact>
    Public Sub PosTablesArePrefixedExactlyOnce()
        Dim options = New DbContextOptionsBuilder(Of POSDbContext)().
            UseMySQL(DummyConnection).Options

        Using db As New POSDbContext(options)
            Dim names = MappedTableNames(db)

            Assert.Contains("Pos_SalesTransactions", names)
            Assert.Contains("Pos_CreditAccounts", names)
            Assert.Contains("Pos_OfficialReceipts", names)
            Assert.False(names.Any(Function(n) n.StartsWith("Pos_Pos_")))
        End Using
    End Sub

    <Fact>
    Public Sub InventoryTablesArePrefixedExactlyOnce()
        Dim options = New DbContextOptionsBuilder(Of InventoryDbContext)().
            UseMySQL(DummyConnection).Options

        Using db As New InventoryDbContext(options)
            Dim names = MappedTableNames(db)

            Assert.Contains("Inv_Products", names)
            Assert.Contains("Inv_StockBatches", names)
            Assert.Contains("Inv_ProductCategories", names)
            Assert.False(names.Any(Function(n) n.StartsWith("Inv_Inv_")))
        End Using
    End Sub

    <Fact>
    Public Sub AccountingTablesArePrefixedExactlyOnce()
        Dim options = New DbContextOptionsBuilder(Of AccountingDbContext)().
            UseMySQL(DummyConnection).Options

        Using db As New AccountingDbContext(options)
            Dim names = MappedTableNames(db)

            Assert.Contains("Acc_VatReturns", names)
            Assert.False(names.Any(Function(n) n.StartsWith("Acc_Acc_")))
        End Using
    End Sub

End Class
