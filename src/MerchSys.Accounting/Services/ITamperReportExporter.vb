Imports System.Collections.Generic
Imports MerchSys.Accounting.Presenters

Namespace Services
    Public Interface ITamperReportExporter
        Function Export(entries As IEnumerable(Of TamperAuditEntryDto), format As TamperReportFormat, Optional options As TamperReportExportOptions = Nothing) As Byte()
    End Interface
End Namespace
