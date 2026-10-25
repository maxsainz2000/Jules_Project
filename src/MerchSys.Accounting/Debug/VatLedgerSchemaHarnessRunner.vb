#If DEBUG Then
Imports System.IO
Imports MerchSys.Accounting.Data

Namespace Debug
    Public Module VatLedgerSchemaHarnessRunner
        Public Async Function RunAndReportAsync(context As AccountingDbContext) As Task(Of String)
            Dim harness As New VatLedgerSchemaHarness()
            Dim report = Await harness.ExecuteAsync(context)

            Dim filePath = Path.Combine(Path.GetTempPath(), $"vat-ledger-schema-report-{DateTime.UtcNow:yyyyMMddHHmmss}.md")
            File.WriteAllText(filePath, report.ToMarkdown())

            Return filePath
        End Function
    End Module
End Namespace
#End If