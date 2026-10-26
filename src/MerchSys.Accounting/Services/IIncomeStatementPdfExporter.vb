Imports System.Collections.Generic

Namespace Services

    Public Interface IIncomeStatementPdfExporter
        Function ExportPdf(statement As IncomeStatementDto, Optional productMargins As IEnumerable(Of ProductMarginDto) = Nothing, Optional interpretation As String = Nothing) As Byte()
    End Interface

End Namespace
