Imports System.Threading.Tasks

Namespace Services

    Public Interface ITamperReportExporter
        Function ExportAsync(options As TamperReportExportOptions, format As TamperReportFormat) As Task(Of Byte())
    End Interface

End Namespace
