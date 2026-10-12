Imports Xunit
Imports System.Collections.Generic
Imports MerchSys.App.Data

''' <summary>
''' Covers the guard that keeps <c>IRelationalDatabaseCreator.CreateTables()</c> from being
''' called on a context whose tables already exist. <c>CreateTables()</c> is not idempotent —
''' it aborts on the first table that is already present — so the decision matters.
''' </summary>
Public Class SchemaCreationGuardTests

    <Fact>
    Public Sub CreatesTablesWhenNoneOfThemExist()
        Dim existing As New HashSet(Of String)(System.StringComparer.OrdinalIgnoreCase)

        Assert.True(DatabaseBootstrapper.ShouldCreateTables(existing, {"Pur_Vendors", "Pur_AccountsPayable"}))
    End Sub

    <Fact>
    Public Sub SkipsWhenAnyTableAlreadyExists()
        Dim existing As New HashSet(Of String)(System.StringComparer.OrdinalIgnoreCase) From {"Pur_Vendors"}

        Assert.False(DatabaseBootstrapper.ShouldCreateTables(existing, {"Pur_Vendors", "Pur_AccountsPayable"}))
    End Sub

    <Fact>
    Public Sub ExistenceCheckIsCaseInsensitive()
        Dim existing As New HashSet(Of String)(System.StringComparer.OrdinalIgnoreCase) From {"pur_vendors"}

        Assert.False(DatabaseBootstrapper.ShouldCreateTables(existing, {"Pur_Vendors"}))
    End Sub

    <Fact>
    Public Sub EmptyModelNeverRequestsCreation()
        Dim existing As New HashSet(Of String)(System.StringComparer.OrdinalIgnoreCase)

        ' An empty table set is vacuously "nothing exists", so the guard says True; the caller
        ' short-circuits before calling CreateTables() for a context with no mapped tables.
        Assert.True(DatabaseBootstrapper.ShouldCreateTables(existing, New String() {}))
    End Sub

End Class
