Imports Xunit
Imports System
Imports System.Linq
Imports MerchSys.App.Data

''' <summary>
''' DB-free coverage for the BIR receipt-archive trigger bootstrapper (H3). The DB-touching half
''' lives in <c>ReceiptImmutabilityIntegrationTests</c>.
''' </summary>
Public Class ReceiptArchiveTriggerBootstrapperTests

    <Fact>
    Public Sub HasAllRequiredTriggers_TrueWhenEveryTriggerIsPresent()
        Assert.True(ReceiptArchiveTriggerBootstrapper.HasAllRequiredTriggers(
            ReceiptArchiveTriggerBootstrapper.RequiredTriggers))
    End Sub

    ''' <summary>
    ''' The two tables are guarded independently, so having only the official-receipt triggers — the
    ''' state the codebase was actually in before this change — must not read as protected.
    ''' </summary>
    <Fact>
    Public Sub HasAllRequiredTriggers_FalseWhenOnlyTheOfficialReceiptTriggersArePresent()
        Assert.False(ReceiptArchiveTriggerBootstrapper.HasAllRequiredTriggers(
            {"pos_receipt_archive_no_update", "pos_receipt_archive_no_delete"}))
    End Sub

    <Fact>
    Public Sub HasAllRequiredTriggers_FalseWhenOneIsMissing()
        Assert.False(ReceiptArchiveTriggerBootstrapper.HasAllRequiredTriggers(
            {"pos_receipt_archive_no_update",
             "pos_receipt_archive_no_delete",
             "pos_receipt_integrity_archive_no_update"}))
    End Sub

    <Fact>
    Public Sub HasAllRequiredTriggers_FalseWhenEmptyOrNothing()
        Assert.False(ReceiptArchiveTriggerBootstrapper.HasAllRequiredTriggers(New String() {}))
        Assert.False(ReceiptArchiveTriggerBootstrapper.HasAllRequiredTriggers(Nothing))
    End Sub

    ''' <summary>
    ''' MariaDB treats trigger names case-insensitively, so a name that differs only in case must
    ''' still count as present — otherwise a healthy database would be reported as unprotected.
    ''' </summary>
    <Fact>
    Public Sub HasAllRequiredTriggers_IsCaseInsensitive()
        Assert.True(ReceiptArchiveTriggerBootstrapper.HasAllRequiredTriggers(
            {"POS_RECEIPT_ARCHIVE_NO_UPDATE", "Pos_Receipt_Archive_No_Delete",
             "Pos_Receipt_Integrity_Archive_No_Update", "POS_RECEIPT_INTEGRITY_ARCHIVE_NO_DELETE"}))
    End Sub

    ''' <summary>
    ''' Every required trigger must be declared by the script, and each must target a table the
    ''' bootstrapper knows about. A script that declared a trigger for the wrong table would apply
    ''' cleanly and protect nothing.
    ''' </summary>
    <Fact>
    Public Sub TriggerScriptIsEmbeddedAndCarriesEveryTriggerOnItsTable()
        Dim sql = ReceiptArchiveTriggerBootstrapper.ReadEmbeddedScript()

        Assert.False(String.IsNullOrWhiteSpace(sql))
        For Each triggerName In ReceiptArchiveTriggerBootstrapper.RequiredTriggers
            Assert.Contains(triggerName, sql)
        Next
        For Each tableName In ReceiptArchiveTriggerBootstrapper.ArchiveTables
            Assert.Contains("ON " & tableName, sql)
        Next
        Assert.Contains("BIR-archive-immutable", sql)
    End Sub

    ''' <summary>
    ''' The script has to survive our own DELIMITER-aware splitter — that is what turns it into the
    ''' statements actually executed at startup. A script the splitter mis-handles would fail only
    ''' at runtime.
    ''' </summary>
    <Fact>
    Public Sub TriggerScriptSplitsIntoADropAndCreatePerTrigger()
        Dim statements = MariaDbSchemaInitializer.SplitSqlStatements(
            ReceiptArchiveTriggerBootstrapper.ReadEmbeddedScript())

        ' The header comment is carried into the first statement (the splitter preserves comments),
        ' so match on the DDL phrases themselves rather than on "CREATE TRIGGER", which also occurs
        ' in the prose.
        Assert.Equal(8, statements.Count)
        Assert.Equal(4, statements.Where(Function(s) s.Contains("DROP TRIGGER IF EXISTS")).Count())
        Assert.Equal(4, statements.Where(Function(s) s.Contains("SIGNAL SQLSTATE '45000'")).Count())
    End Sub

End Class
