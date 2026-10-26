Imports System.Collections.Generic

Namespace Services

    Public Interface IVatReliefPdfExporter
        Function ExportPdf(summary As VatReliefSummary, Optional trailingMonths As List(Of VatReliefSummary) = Nothing, Optional interpretation As String = Nothing) As Byte()
    End Interface

End Namespace
