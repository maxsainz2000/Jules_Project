Imports System
Imports System.IO
Imports System.Text
Imports System.Threading.Tasks
Imports QuestPDF.Fluent
Imports QuestPDF.Helpers
Imports QuestPDF.Infrastructure

Namespace Services

    Public Class TamperReportExporter
        Implements ITamperReportExporter

        Public Sub New()
            QuestPDF.Settings.License = LicenseType.Community
        End Sub

        Public Async Function ExportAsync(options As TamperReportExportOptions, format As TamperReportFormat) As Task(Of Byte()) Implements ITamperReportExporter.ExportAsync
            Select Case format
                Case TamperReportFormat.Csv
                    Return Await ExportCsvAsync(options)
                Case TamperReportFormat.Pdf
                    Return Await ExportPdfAsync(options)
                Case Else
                    Throw New NotSupportedException($"Format {format} is not supported.")
            End Select
        End Function

        Private Async Function ExportCsvAsync(options As TamperReportExportOptions) As Task(Of Byte())
            Using ms As New MemoryStream()
                Using writer As New StreamWriter(ms, Encoding.UTF8)
                    ' Write Header
                    Await writer.WriteLineAsync("Date/Time,Receipt Number,Expected Hash,Actual Hash,Tamper Kind,Details")

                    If options.Incidents IsNot Nothing Then
                        For Each incident In options.Incidents
                            Dim line = $"{EscapeCsv(incident.DetectedAt.ToString("g"))},{EscapeCsv(incident.ReceiptNumber)},{EscapeCsv(incident.ExpectedHash)},{EscapeCsv(incident.ActualHash)},{EscapeCsv(incident.TamperKind)},{EscapeCsv(incident.Details)}"
                            Await writer.WriteLineAsync(line)
                        Next
                    End If
                    Await writer.FlushAsync()
                End Using
                Return ms.ToArray()
            End Using
        End Function

        Private Function EscapeCsv(field As String) As String
            If String.IsNullOrEmpty(field) Then
                Return String.Empty
            End If

            If field.Contains(",") OrElse field.Contains("""") OrElse field.Contains(vbCr) OrElse field.Contains(vbLf) Then
                Return """" & field.Replace("""", """""") & """"
            End If

            Return field
        End Function

        Private Async Function ExportPdfAsync(options As TamperReportExportOptions) As Task(Of Byte())
            Dim document = QuestPDF.Fluent.Document.Create(Sub(container As IDocumentContainer)
                                                               container.Page(Sub(page As PageDescriptor)
                                                                                  page.Size(PageSizes.A4)
                                                                                  page.Margin(2, Unit.Centimetre)
                                                                                  page.PageColor(Colors.White)
                                                                                  page.DefaultTextStyle(Function(x As TextStyle) x.FontSize(11).FontFamily("Arial"))

                                                                                  page.Header().Element(Sub(headerContainer As IContainer)
                                                                                                            headerContainer.Column(Sub(col As ColumnDescriptor)
                                                                                                                                       col.Item().Text(options.BusinessName).FontSize(18).SemiBold()
                                                                                                                                       col.Item().Text(options.BusinessAddress)
                                                                                                                                       col.Item().Text($"TIN: {options.BusinessTIN}")
                                                                                                                                       col.Item().PaddingTop(10).Text(options.Title).FontSize(14).SemiBold()
                                                                                                                                       col.Item().Text($"Period: {options.FromDate:d} to {options.ToDate:d}")
                                                                                                                                   End Sub)
                                                                                                        End Sub)

                                                                                  page.Content().PaddingVertical(1, Unit.Centimetre).Table(Sub(table As TableDescriptor)
                                                                                                                                               table.ColumnsDefinition(Sub(cols)
                                                                                                                                                                           cols.RelativeColumn()
                                                                                                                                                                           cols.RelativeColumn()
                                                                                                                                                                           cols.RelativeColumn()
                                                                                                                                                                           cols.RelativeColumn()
                                                                                                                                                                           cols.RelativeColumn()
                                                                                                                                                                           cols.RelativeColumn()
                                                                                                                                                                       End Sub)

                                                                                                                                               table.Header(Sub(header)
                                                                                                                                                                header.Cell().Element(AddressOf CellStyle).Text("Date/Time")
                                                                                                                                                                header.Cell().Element(AddressOf CellStyle).Text("Receipt Number")
                                                                                                                                                                header.Cell().Element(AddressOf CellStyle).Text("Expected Hash")
                                                                                                                                                                header.Cell().Element(AddressOf CellStyle).Text("Actual Hash")
                                                                                                                                                                header.Cell().Element(AddressOf CellStyle).Text("Tamper Kind")
                                                                                                                                                                header.Cell().Element(AddressOf CellStyle).Text("Details")
                                                                                                                                                            End Sub)

                                                                                                                                               If options.Incidents IsNot Nothing Then
                                                                                                                                                   For Each incident In options.Incidents
                                                                                                                                                       table.Cell().Element(AddressOf CellStyle).Text(incident.DetectedAt.ToString("g"))
                                                                                                                                                       table.Cell().Element(AddressOf CellStyle).Text(incident.ReceiptNumber)
                                                                                                                                                       table.Cell().Element(AddressOf CellStyle).Text(incident.ExpectedHashShort)
                                                                                                                                                       table.Cell().Element(AddressOf CellStyle).Text(incident.ActualHashShort)
                                                                                                                                                       table.Cell().Element(AddressOf CellStyle).Text(incident.TamperKind)
                                                                                                                                                       table.Cell().Element(AddressOf CellStyle).Text(incident.Details)
                                                                                                                                                   Next
                                                                                                                                               End If
                                                                                                                                           End Sub)

                                                                                  page.Footer().AlignCenter().Text(Sub(text As TextDescriptor)
                                                                                                                       text.Span("Page ")
                                                                                                                       text.CurrentPageNumber()
                                                                                                                       text.Span(" of ")
                                                                                                                       text.TotalPages()
                                                                                                                   End Sub)
                                                                              End Sub)
                                                           End Sub)

            Return Await Task.Run(Function() document.GeneratePdf())
        End Function

        Private Function CellStyle(container As IContainer) As IContainer
            Return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5)
        End Function

    End Class

End Namespace
