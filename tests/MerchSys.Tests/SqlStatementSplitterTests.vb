Imports Xunit
Imports MerchSys.App.Data

''' <summary>
''' Covers the quote- and comment-aware SQL splitter (M13), which previously split on any
''' line ending with the active delimiter and could corrupt a script.
''' </summary>
Public Class SqlStatementSplitterTests

    <Fact>
    Public Sub SplitsSimpleStatements()
        Dim result = MariaDbSchemaInitializer.SplitSqlStatements("SELECT 1; SELECT 2;")
        Assert.Equal(2, result.Count)
    End Sub

    <Fact>
    Public Sub IgnoresSemicolonInsideStringLiteral()
        Dim result = MariaDbSchemaInitializer.SplitSqlStatements("INSERT INTO t VALUES ('a;b');")
        Assert.Equal(1, result.Count)
    End Sub

    <Fact>
    Public Sub IgnoresSemicolonInsideLineComment()
        Dim sql = "SELECT 1; -- a comment with ; inside" & vbLf & "SELECT 2;"
        Dim result = MariaDbSchemaInitializer.SplitSqlStatements(sql)
        Assert.Equal(2, result.Count)
    End Sub

    <Fact>
    Public Sub IgnoresSemicolonInsideBlockComment()
        Dim result = MariaDbSchemaInitializer.SplitSqlStatements("SELECT 1; /* c; */ SELECT 2;")
        Assert.Equal(2, result.Count)
    End Sub

    <Fact>
    Public Sub HandlesDelimiterBlocks()
        Dim sql = "DELIMITER //" & vbLf &
                  "CREATE TRIGGER t BEFORE INSERT ON x FOR EACH ROW BEGIN SET @a = 1; END//" & vbLf &
                  "DELIMITER ;" & vbLf

        Dim result = MariaDbSchemaInitializer.SplitSqlStatements(sql)

        Assert.Equal(1, result.Count)
        Assert.Contains("CREATE TRIGGER", result(0))
    End Sub

    <Fact>
    Public Sub DoesNotEmitEmptyStatementsForTrailingDelimiter()
        Dim result = MariaDbSchemaInitializer.SplitSqlStatements("SELECT 1;")
        Assert.Equal(1, result.Count)
    End Sub
End Class
