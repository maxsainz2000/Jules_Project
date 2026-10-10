Imports System.Text
Imports MerchSys.Accounting.Entities
Imports QuestPDF.Fluent
Imports QuestPDF.Helpers
Imports QuestPDF.Infrastructure

Namespace Services
    Public Class VatReturnExporter
        Implements IVatReturnExporter

        Public Function ExportCsv(vatReturn As VatReturn) As Byte() Implements IVatReturnExporter.ExportCsv
            If vatReturn Is Nothing Then Return Array.Empty(Of Byte)()

            Dim sb As New StringBuilder()
            sb.AppendLine("LineNumber,LineCode,Description,Amount,TaxRate,TaxAmount")
            
            If vatReturn.Lines IsNot Nothing Then
                For Each line In vatReturn.Lines
                    sb.AppendLine($"{line.LineNumber},{line.LineCode},{line.Description},{line.Amount},{line.TaxRate},{line.TaxAmount}")
                Next
            End If
            
            Return Encoding.UTF8.GetBytes(sb.ToString())
        End Function

        Public Function ExportPdf(vatReturn As VatReturn) As Byte() Implements IVatReturnExporter.ExportPdf
            If vatReturn Is Nothing Then Return Array.Empty(Of Byte)()
            
            QuestPDF.Settings.License = LicenseType.Community

            Dim pdfDoc = Document.Create(Sub(container As IDocumentContainer)
                container.Page(Sub(page As PageDescriptor)
                    page.Size(PageSizes.A4)
                    page.Margin(2, Unit.Centimetre)
                    page.PageColor(Colors.White)
                    page.DefaultTextStyle(Function(x As TextStyle) x.FontSize(11))
                    
                    page.Header().Text($"VAT Return {vatReturn.Year} - {vatReturn.Period} ({vatReturn.FormType})").SemiBold().FontSize(20).FontColor(Colors.Blue.Darken2)
                    
                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(Sub(x As ColumnDescriptor)
                        x.Item().Text($"Filing Status: {vatReturn.FilingStatus}")
                        x.Item().Text($"Period: {vatReturn.PeriodStart:yyyy-MM-dd} to {vatReturn.PeriodEnd:yyyy-MM-dd}")
                        x.Item().Text($"Total Sales: {vatReturn.TotalSales:N2}")
                        x.Item().Text($"Total Purchases: {vatReturn.TotalPurchases:N2}")
                        x.Item().Text($"Output VAT: {vatReturn.OutputVatTotal:N2}")
                        x.Item().Text($"Input VAT: {vatReturn.InputVatTotal:N2}")
                        x.Item().Text($"Net VAT Payable: {vatReturn.NetVatPayable:N2}")
                        
                        x.Item().PaddingTop(20).Table(Sub(t As TableDescriptor)
                            t.ColumnsDefinition(Sub(c)
                                c.ConstantColumn(50)
                                c.ConstantColumn(80)
                                c.RelativeColumn()
                                c.RelativeColumn()
                                c.RelativeColumn()
                                c.RelativeColumn()
                            End Sub)
                            
                            t.Header(Sub(h)
                                h.Cell().Text("Line")
                                h.Cell().Text("Code")
                                h.Cell().Text("Description")
                                h.Cell().Text("Amount")
                                h.Cell().Text("TaxRate")
                                h.Cell().Text("TaxAmount")
                            End Sub)
                            
                            If vatReturn.Lines IsNot Nothing Then
                                For Each line In vatReturn.Lines
                                    t.Cell().Text(line.LineNumber.ToString())
                                    t.Cell().Text(line.LineCode)
                                    t.Cell().Text(line.Description)
                                    t.Cell().Text(line.Amount.ToString("N2"))
                                    t.Cell().Text(line.TaxRate.ToString("N2"))
                                    t.Cell().Text(line.TaxAmount.ToString("N2"))
                                Next
                            End If
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
