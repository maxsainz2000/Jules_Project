Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Threading.Tasks
Imports MerchSys.Accounting.Presenters
Imports QuestPDF.Fluent
Imports QuestPDF.Helpers
Imports QuestPDF.Infrastructure

Namespace Services

    Public Class TamperReportExporter
        Implements ITamperReportExporter

        Public Sub New()
            QuestPDF.Settings.License = LicenseType.Community
        End Sub

        Public Function ExportAsync(incidents As IEnumerable(Of TamperAuditEntryDto), format As TamperReportFormat, options As TamperReportExportOptions) As Task(Of Byte()) Implements ITamperReportExporter.ExportAsync
            If format = TamperReportFormat.Csv Then
                Return Task.FromResult(ExportCsv(incidents))
            Else
                Return Task.FromResult(ExportPdf(incidents, options))
            End If
        End Function

        Private Function ExportCsv(incidents As IEnumerable(Of TamperAuditEntryDto)) As Byte()
            Dim sb As New StringBuilder()

            ' Header
            sb.AppendLine("DetectedAt,ReceiptNumber,ExpectedHash,ActualHash,TamperKind,Details")

            If incidents IsNot Nothing Then
                For Each i In incidents
                    Dim row = $"{EscapeCsv(i.DetectedAt.ToString("yyyy-MM-dd HH:mm:ss"))},{EscapeCsv(i.ReceiptNumber)},{EscapeCsv(i.ExpectedHash)},{EscapeCsv(i.ActualHash)},{EscapeCsv(i.TamperKind)},{EscapeCsv(i.Details)}"
                    sb.AppendLine(row)
                Next
            End If

            Return Encoding.UTF8.GetBytes(sb.ToString())
        End Function

        Private Function EscapeCsv(value As String) As String
            If String.IsNullOrEmpty(value) Then Return """"""
            Dim escaped = value.Replace("""", """""")
            Return $"""{escaped}"""
        End Function

        Private Function ExportPdf(incidents As IEnumerable(Of TamperAuditEntryDto), options As TamperReportExportOptions) As Byte()
            Dim doc = Document.Create(Sub(container As IDocumentContainer)
                container.Page(Sub(page As PageDescriptor)
                    page.Size(PageSizes.A4.Landscape())
                    page.Margin(2, Unit.Centimetre)
                    page.PageColor(Colors.White)
                    page.DefaultTextStyle(Function(x As TextStyle) x.FontSize(10))

                    page.Header().Element(Sub(header As IContainer)
                        header.Column(Sub(c As ColumnDescriptor)
                            c.Item().Text(options.BusinessName).FontSize(16).SemiBold()
                            If Not String.IsNullOrEmpty(options.BusinessAddress) Then c.Item().Text(options.BusinessAddress)
                            If Not String.IsNullOrEmpty(options.BusinessTIN) Then c.Item().Text($"TIN: {options.BusinessTIN}")
                            c.Item().PaddingTop(10).Text(options.Title).FontSize(14).SemiBold()
                            c.Item().Text($"Period: {options.FromDate:yyyy-MM-dd} to {options.ToDate:yyyy-MM-dd}")
                        End Sub)
                    End Sub)

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(Sub(content As ColumnDescriptor)
                        content.Item().Table(Sub(table As TableDescriptor)
                            table.ColumnsDefinition(Sub(cols)
                                cols.RelativeColumn(1.5F) ' DetectedAt
                                cols.RelativeColumn(1.5F) ' ReceiptNumber
                                cols.RelativeColumn(2) ' ExpectedHash
                                cols.RelativeColumn(2) ' ActualHash
                                cols.RelativeColumn(1) ' TamperKind
                                cols.RelativeColumn(3) ' Details
                            End Sub)

                            table.Header(Sub(header)
                                header.Cell().Text("Date/Time").SemiBold()
                                header.Cell().Text("Receipt No").SemiBold()
                                header.Cell().Text("Expected Hash").SemiBold()
                                header.Cell().Text("Actual Hash").SemiBold()
                                header.Cell().Text("Kind").SemiBold()
                                header.Cell().Text("Details").SemiBold()

                                header.Cell().ColumnSpan(6).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black)
                            End Sub)

                            If incidents IsNot Nothing Then
                                For Each i In incidents
                                    table.Cell().Text(i.DetectedAt.ToString("yyyy-MM-dd HH:mm"))
                                    table.Cell().Text(i.ReceiptNumber)
                                    table.Cell().Text(i.ExpectedHashShort)
                                    table.Cell().Text(i.ActualHashShort)
                                    table.Cell().Text(i.TamperKind)
                                    table.Cell().Text(i.Details)
                                Next
                            End If
                        End Sub)
                    End Sub)

                    page.Footer().AlignCenter().Text(Sub(text As TextDescriptor)
                        text.Span("Page ")
                        text.CurrentPageNumber()
                        text.Span(" of ")
                        text.TotalPages()
                    End Sub)
                End Sub)
            End Sub)

            Return doc.GeneratePdf()
        End Function
    End Class

End Namespace
