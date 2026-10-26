Imports System
Imports System.Collections.Generic
Imports System.Text
Imports MerchSys.Accounting.Presenters
Imports QuestPDF.Fluent
Imports QuestPDF.Helpers
Imports QuestPDF.Infrastructure

Namespace Services
    Public Class TamperReportExporter
        Implements ITamperReportExporter

        Public Function Export(entries As IEnumerable(Of TamperAuditEntryDto), format As TamperReportFormat, Optional options As TamperReportExportOptions = Nothing) As Byte() Implements ITamperReportExporter.Export
            If entries Is Nothing Then Return Array.Empty(Of Byte)()

            If format = TamperReportFormat.Csv Then
                Return ExportCsv(entries)
            ElseIf format = TamperReportFormat.Pdf Then
                Return ExportPdf(entries, options)
            Else
                Throw New ArgumentException($"Unsupported format: {format}")
            End If
        End Function

        Private Function ExportCsv(entries As IEnumerable(Of TamperAuditEntryDto)) As Byte()
            Dim sb As New StringBuilder()
            sb.AppendLine("DetectedAt,ReceiptNumber,ExpectedHash,ActualHash,TamperKind,Details")

            For Each entry In entries
                Dim detectedAt = FormatCsvField(entry.DetectedAt.ToString("yyyy-MM-dd HH:mm:ss"))
                Dim receiptNumber = FormatCsvField(entry.ReceiptNumber)
                Dim expectedHash = FormatCsvField(entry.ExpectedHash)
                Dim actualHash = FormatCsvField(entry.ActualHash)
                Dim tamperKind = FormatCsvField(entry.TamperKind)
                Dim details = FormatCsvField(entry.Details)

                sb.AppendLine($"{detectedAt},{receiptNumber},{expectedHash},{actualHash},{tamperKind},{details}")
            Next

            Return Encoding.UTF8.GetBytes(sb.ToString())
        End Function

        Private Function FormatCsvField(value As String) As String
            If String.IsNullOrEmpty(value) Then Return String.Empty

            Dim requiresEscaping As Boolean = value.Contains(","c) OrElse value.Contains(""""c) OrElse value.Contains(vbCr) OrElse value.Contains(vbLf)

            If requiresEscaping Then
                ' Escape inner double quotes by doubling them up
                Dim escapedValue = value.Replace("""", """""")
                ' Wrap in double quotes
                Return $"""{escapedValue}"""
            End If

            Return value
        End Function

        Private Function ExportPdf(entries As IEnumerable(Of TamperAuditEntryDto), options As TamperReportExportOptions) As Byte()
            QuestPDF.Settings.License = LicenseType.Community

            Dim opts = If(options, New TamperReportExportOptions())
            Dim title = If(String.IsNullOrEmpty(opts.Title), "Tamper Audit Incident Report", opts.Title)
            Dim businessName = If(String.IsNullOrEmpty(opts.BusinessName), "N/A", opts.BusinessName)
            Dim businessAddress = If(String.IsNullOrEmpty(opts.BusinessAddress), "N/A", opts.BusinessAddress)
            Dim businessTIN = If(String.IsNullOrEmpty(opts.BusinessTIN), "N/A", opts.BusinessTIN)

            Dim dateRange As String = "All Time"
            If opts.FromDate.HasValue AndAlso opts.ToDate.HasValue Then
                dateRange = $"{opts.FromDate.Value:yyyy-MM-dd} to {opts.ToDate.Value:yyyy-MM-dd}"
            ElseIf opts.FromDate.HasValue Then
                dateRange = $"From {opts.FromDate.Value:yyyy-MM-dd}"
            ElseIf opts.ToDate.HasValue Then
                dateRange = $"Up to {opts.ToDate.Value:yyyy-MM-dd}"
            End If

            Dim pdfDoc = Document.Create(Sub(container As IDocumentContainer)
                                             container.Page(Sub(page As PageDescriptor)
                                                                page.Size(PageSizes.A4.Landscape())
                                                                page.Margin(1.5, Unit.Centimetre)
                                                                page.PageColor(Colors.White)
                                                                page.DefaultTextStyle(Function(x As TextStyle) x.FontSize(10))

                                                                page.Header().Text(title).SemiBold().FontSize(18).FontColor(Colors.Red.Darken2)

                                                                page.Content().PaddingVertical(1, Unit.Centimetre).Column(Sub(x As ColumnDescriptor)
                                                                                                                              x.Item().Text($"Business Name: {businessName}")
                                                                                                                              x.Item().Text($"Address: {businessAddress}")
                                                                                                                              x.Item().Text($"TIN: {businessTIN}")
                                                                                                                              x.Item().Text($"Date Range: {dateRange}")

                                                                                                                              x.Item().PaddingTop(20).Table(Sub(t As TableDescriptor)
                                                                                                                                                                t.ColumnsDefinition(Sub(c)
                                                                                                                                                                                        c.ConstantColumn(100) ' Detected At
                                                                                                                                                                                        c.ConstantColumn(100) ' Receipt Number
                                                                                                                                                                                        c.ConstantColumn(150) ' Expected Hash
                                                                                                                                                                                        c.ConstantColumn(150) ' Actual Hash
                                                                                                                                                                                        c.ConstantColumn(80)  ' Tamper Kind
                                                                                                                                                                                        c.RelativeColumn()    ' Details
                                                                                                                                                                                    End Sub)

                                                                                                                                                                t.Header(Sub(h)
                                                                                                                                                                             h.Cell().Text("Detected At").SemiBold()
                                                                                                                                                                             h.Cell().Text("Receipt No.").SemiBold()
                                                                                                                                                                             h.Cell().Text("Expected Hash").SemiBold()
                                                                                                                                                                             h.Cell().Text("Actual Hash").SemiBold()
                                                                                                                                                                             h.Cell().Text("Tamper Kind").SemiBold()
                                                                                                                                                                             h.Cell().Text("Details").SemiBold()
                                                                                                                                                                         End Sub)

                                                                                                                                                                For Each entry In entries
                                                                                                                                                                    t.Cell().Text(entry.DetectedAt.ToString("yyyy-MM-dd HH:mm"))
                                                                                                                                                                    t.Cell().Text(entry.ReceiptNumber)

                                                                                                                                                                    ' Use full hash if possible, fallback to short if long, but we have wider columns here
                                                                                                                                                                    t.Cell().Text(If(String.IsNullOrEmpty(entry.ExpectedHash), "", entry.ExpectedHash.Substring(0, Math.Min(32, entry.ExpectedHash.Length))))
                                                                                                                                                                    t.Cell().Text(If(String.IsNullOrEmpty(entry.ActualHash), "", entry.ActualHash.Substring(0, Math.Min(32, entry.ActualHash.Length))))

                                                                                                                                                                    Dim kindColor = If(entry.TamperKind = "Critical" OrElse entry.TamperKind = "Tampered", Colors.Red.Darken2, Colors.Black)
                                                                                                                                                                    t.Cell().Text(entry.TamperKind).FontColor(kindColor)
                                                                                                                                                                    t.Cell().Text(entry.Details)
                                                                                                                                                                Next
                                                                                                                                                            End Sub)
                                                                                                                          End Sub)

                                                                page.Footer().AlignCenter().Text(Sub(x)
                                                                                                     x.Span("Page ")
                                                                                                     x.CurrentPageNumber()
                                                                                                     x.Span(" of ")
                                                                                                     x.TotalPages()
                                                                                                 End Sub)
                                                            End Sub)
                                         End Sub)

            Return pdfDoc.GeneratePdf()
        End Function
    End Class
End Namespace