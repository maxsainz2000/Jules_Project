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
    Public Sub HasAllRequiredTriggers_TrueWhenBothPresent()
        Assert.True(ReceiptArchiveTriggerBootstrapper.HasAllRequiredTriggers(
            {"pos_receipt_archive_no_update", "pos_receipt_archive_no_delete"}))
    End Sub

    <Fact>
    Public Sub HasAllRequiredTriggers_FalseWhenOneIsMissing()
        Assert.False(ReceiptArchiveTriggerBootstrapper.HasAllRequiredTriggers(
            {"pos_receipt_archive_no_update"}))
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
            {"POS_RECEIPT_ARCHIVE_NO_UPDATE", "Pos_Receipt_Archive_No_Delete"}))
    End Sub

    ''' <summary>
    ''' The DDL must actually be embedded under the logical name the bootstrapper looks for. If the
    ''' vbproj include is renamed, moved or its LogicalName changed, startup throws — this catches
    ''' that at test time instead of on a client machine.
    ''' </summary>
    <Fact>
    Public Sub TriggerScriptIsEmbeddedAndCarriesBothTriggers()
        Dim sql = ReceiptArchiveTriggerBootstrapper.ReadEmbeddedScript()

        Assert.False(String.IsNullOrWhiteSpace(sql))
        Assert.Contains("pos_receipt_archive_no_update", sql)
        Assert.Contains("pos_receipt_archive_no_delete", sql)
        Assert.Contains("Pos_OfficialReceiptArchive", sql)
        Assert.Contains("BIR-archive-immutable", sql)
    End Sub

    ''' <summary>
    ''' The script has to survive our own DELIMITER-aware splitter — that is what turns it into the
    ''' statements actually executed at startup. A script the splitter mis-handles would fail only
    ''' at runtime.
    ''' </summary>
    <Fact>
    Public Sub TriggerScriptSplitsIntoTwoDropsAndTwoCreates()
        Dim statements = MariaDbSchemaInitializer.SplitSqlStatements(
            ReceiptArchiveTriggerBootstrapper.ReadEmbeddedScript())

        ' The header comment is carried into the first statement (the splitter preserves comments),
        ' so match on the DDL phrases themselves rather than on "CREATE TRIGGER", which also occurs
        ' in the prose.
        Assert.Equal(4, statements.Count)
        Assert.Equal(2, statements.Where(Function(s) s.Contains("DROP TRIGGER IF EXISTS")).Count())
        Assert.Equal(2, statements.Where(Function(s) s.Contains("SIGNAL SQLSTATE '45000'")).Count())
    End Sub

End Class
