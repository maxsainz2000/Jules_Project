Imports System.Collections.Generic
Imports System.Threading.Tasks
Imports MerchSys.Accounting.Presenters

Namespace Services

    Public Interface ITamperReportExporter
        Function ExportAsync(incidents As IEnumerable(Of TamperAuditEntryDto), format As TamperReportFormat, options As TamperReportExportOptions) As Task(Of Byte())
    End Interface

End Namespace
