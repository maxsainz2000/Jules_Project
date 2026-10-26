Imports System
Imports System.Collections.Generic
Imports QuestPDF.Fluent
Imports QuestPDF.Helpers
Imports QuestPDF.Infrastructure

Namespace Services

    Public Class VatReliefPdfExporter
        Implements IVatReliefPdfExporter

        Public Sub New()
            QuestPDF.Settings.License = LicenseType.Community
        End Sub

        Public Function ExportPdf(summary As VatReliefSummary, Optional trailingMonths As IEnumerable(Of VatReliefSummary) = Nothing, Optional interpretation As String = Nothing) As Byte() Implements IVatReliefPdfExporter.ExportPdf
            Dim doc = Document.Create(Sub(container As IDocumentContainer)
                container.Page(Sub(page As PageDescriptor)
                    page.Size(PageSizes.A4)
                    page.Margin(2, Unit.Centimetre)
                    page.PageColor(Colors.White)
                    page.DefaultTextStyle(Function(x As TextStyle) x.FontSize(10).FontFamily("Segoe UI"))

                    page.Header().Element(Sub(h) ComposeHeader(h, summary))
                    page.Content().Element(Sub(c) ComposeContent(c, summary, trailingMonths, interpretation))
                    page.Footer().Element(Sub(f) ComposeFooter(f))
                End Sub)
            End Sub)

            Return doc.GeneratePdf()
        End Function

        Private Sub ComposeHeader(container As IContainer, summary As VatReliefSummary)
            container.Column(Sub(c As ColumnDescriptor)
                c.Item().Text("BIR VAT Relief Report").FontSize(16).SemiBold()
                c.Item().Text($"Period: {summary.PeriodLabel}").FontSize(12)
            End Sub)
        End Sub

        Private Sub ComposeContent(container As IContainer, summary As VatReliefSummary, trailingMonths As IEnumerable(Of VatReliefSummary), interpretation As String)
            container.PaddingVertical(1, Unit.Centimetre).Column(Sub(col As ColumnDescriptor)

                ' Net VAT Payable Banner
                col.Item().PaddingBottom(15).Background(Colors.Grey.Lighten3).Padding(10).Column(Sub(banner As ColumnDescriptor)
                    banner.Item().AlignCenter().Text("Net VAT Payable").FontSize(12).SemiBold()
                    If summary.NetVatPayable > 0 Then
                        banner.Item().AlignCenter().Text($"₱{summary.NetVatPayable:N2}").FontSize(16).FontColor(Colors.Red.Darken2).SemiBold()
                    ElseIf summary.NetVatPayable < 0 Then
                        banner.Item().AlignCenter().Text($"₱{Math.Abs(summary.NetVatPayable):N2} (Credit)").FontSize(16).FontColor(Colors.Green.Darken2).SemiBold()
                    Else
                        banner.Item().AlignCenter().Text("₱0.00").FontSize(16).FontColor(Colors.Grey.Darken2).SemiBold()
                    End If
                End Sub)

                col.Item().PaddingBottom(10).Text("Sales Summary").FontSize(14).SemiBold()
                col.Item().Table(Sub(table As TableDescriptor)
                    table.ColumnsDefinition(Sub(cols)
                        cols.RelativeColumn(3)
                        cols.RelativeColumn(1)
                    End Sub)

                    table.Cell().Text("Vatable Sales:")
                    table.Cell().AlignRight().Text($"₱{summary.VatableSales:N2}")

                    table.Cell().Text("VAT-Exempt Sales:")
                    table.Cell().AlignRight().Text($"₱{summary.VatExemptSales:N2}")

                    table.Cell().Text("Zero-Rated Sales:")
                    table.Cell().AlignRight().Text($"₱{summary.ZeroRatedSales:N2}")

                    table.Cell().Text("Total Sales:")
                    table.Cell().AlignRight().Text($"₱{summary.TotalSales:N2}").SemiBold()

                    table.Cell().Text("Output VAT:")
                    table.Cell().AlignRight().Text($"₱{summary.OutputVat:N2}").SemiBold()
                End Sub)

                col.Item().PaddingTop(15).PaddingBottom(10).Text("Purchases Summary").FontSize(14).SemiBold()
                col.Item().Table(Sub(table As TableDescriptor)
                    table.ColumnsDefinition(Sub(cols)
                        cols.RelativeColumn(3)
                        cols.RelativeColumn(1)
                    End Sub)

                    table.Cell().Text("Vatable Purchases:")
                    table.Cell().AlignRight().Text($"₱{summary.VatablePurchases:N2}")

                    table.Cell().Text("VAT-Exempt Purchases:")
                    table.Cell().AlignRight().Text($"₱{summary.VatExemptPurchases:N2}")

                    table.Cell().Text("Zero-Rated Purchases:")
                    table.Cell().AlignRight().Text($"₱{summary.ZeroRatedPurchases:N2}")

                    table.Cell().Text("Total Purchases:")
                    table.Cell().AlignRight().Text($"₱{summary.TotalPurchases:N2}").SemiBold()

                    table.Cell().Text("Input VAT:")
                    table.Cell().AlignRight().Text($"₱{summary.InputVat:N2}").SemiBold()
                End Sub)

                If trailingMonths IsNot Nothing Then
                    col.Item().PaddingTop(15).PaddingBottom(10).Text("Trailing Trend").FontSize(14).SemiBold()
                    col.Item().Table(Sub(table As TableDescriptor)
                        table.ColumnsDefinition(Sub(cols)
                            cols.RelativeColumn(2)
                            cols.RelativeColumn(2)
                            cols.RelativeColumn(2)
                            cols.RelativeColumn(2)
                            cols.RelativeColumn(2)
                            cols.RelativeColumn(2)
                        End Sub)

                        table.Header(Sub(header)
                            header.Cell().Text("Month").SemiBold()
                            header.Cell().AlignRight().Text("Total Sales").SemiBold()
                            header.Cell().AlignRight().Text("Output VAT").SemiBold()
                            header.Cell().AlignRight().Text("Total Purch").SemiBold()
                            header.Cell().AlignRight().Text("Input VAT").SemiBold()
                            header.Cell().AlignRight().Text("Net VAT").SemiBold()

                            header.Cell().ColumnSpan(6).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black)
                        End Sub)

                        For Each m In trailingMonths
                            table.Cell().Text(m.PeriodLabel)
                            table.Cell().AlignRight().Text($"₱{m.TotalSales:N2}")
                            table.Cell().AlignRight().Text($"₱{m.OutputVat:N2}")
                            table.Cell().AlignRight().Text($"₱{m.TotalPurchases:N2}")
                            table.Cell().AlignRight().Text($"₱{m.InputVat:N2}")
                            table.Cell().AlignRight().Text($"₱{m.NetVatPayable:N2}")
                        Next
                    End Sub)
                End If

                If Not String.IsNullOrEmpty(interpretation) Then
                    col.Item().PaddingTop(15).PaddingBottom(5).Text("What This Means").FontSize(14).SemiBold()
                    col.Item().Text(interpretation)
                End If

            End Sub)
        End Sub

        Private Sub ComposeFooter(container As IContainer)
            container.AlignCenter().Text(Sub(text As TextDescriptor)
                text.Span("Page ")
                text.CurrentPageNumber()
                text.Span(" of ")
                text.TotalPages()
            End Sub)
        End Sub

    End Class

End Namespace
