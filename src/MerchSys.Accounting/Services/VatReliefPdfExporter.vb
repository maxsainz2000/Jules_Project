Imports QuestPDF.Fluent
Imports QuestPDF.Helpers
Imports QuestPDF.Infrastructure
Imports System.Collections.Generic
Imports System.Linq
Imports System

Namespace Services

    Public Class VatReliefPdfExporter
        Implements IVatReliefPdfExporter

        Public Sub New()
            QuestPDF.Settings.License = LicenseType.Community
        End Sub

        Public Function ExportPdf(summary As VatReliefSummary, Optional trailingMonths As List(Of VatReliefSummary) = Nothing, Optional interpretation As String = Nothing) As Byte() Implements IVatReliefPdfExporter.ExportPdf
            Return Document.Create(Sub(container)
                                       container.Page(Sub(page)
                                                          page.Size(PageSizes.A4)
                                                          page.Margin(1, Unit.Centimetre)
                                                          page.PageColor(Colors.White)
                                                          page.DefaultTextStyle(Function(x As TextStyle) x.FontSize(11))

                                                          page.Header().Element(Sub(c) ComposeHeader(c, summary))
                                                          page.Content().Element(Sub(c) ComposeContent(c, summary, trailingMonths, interpretation))
                                                          page.Footer().Element(Sub(c) ComposeFooter(c))
                                                      End Sub)
                                   End Sub).GeneratePdf()
        End Function

        Private Sub ComposeHeader(container As IContainer, summary As VatReliefSummary)
            container.Row(Sub(row)
                              row.RelativeItem().Column(Sub(column)
                                                            column.Item().Text("BIR VAT Relief Report").FontSize(20).SemiBold().FontColor(Colors.Blue.Darken2)
                                                            column.Item().Text(summary.PeriodLabel).FontSize(14)
                                                        End Sub)
                          End Sub)
        End Sub

        Private Sub ComposeContent(container As IContainer, summary As VatReliefSummary, trailingMonths As List(Of VatReliefSummary), interpretation As String)
            container.PaddingVertical(1, Unit.Centimetre).Column(Sub(column)
                                                                     column.Spacing(20)
                                                                     column.Item().Element(Sub(c) ComposeNetVatPayableCard(c, summary))
                                                                     column.Item().Element(Sub(c) ComposeSalesAndPurchasesTables(c, summary))

                                                                     If trailingMonths IsNot Nothing AndAlso trailingMonths.Any() Then
                                                                         column.Item().Element(Sub(c) ComposeTrailingMonthsTable(c, trailingMonths))
                                                                     End If

                                                                     If Not String.IsNullOrWhiteSpace(interpretation) Then
                                                                         column.Item().Element(Sub(c) ComposeInterpretation(c, interpretation))
                                                                     End If
                                                                 End Sub)
        End Sub

        Private Sub ComposeNetVatPayableCard(container As IContainer, summary As VatReliefSummary)
            Dim isCredit As Boolean = summary.NetVatPayable < 0
            Dim bgColor As String = If(isCredit, Colors.Green.Lighten5, Colors.Red.Lighten5)
            Dim textColor As String = If(isCredit, Colors.Green.Darken2, Colors.Red.Darken2)
            Dim labelText As String = If(isCredit, "Excess Input Tax Credit", "Net VAT Payable")
            Dim amountText As String = $"₱{Math.Abs(summary.NetVatPayable):N2}"

            container.Background(bgColor).Padding(15).Column(Sub(column)
                                                                 column.Item().AlignCenter().Text(labelText).FontSize(16).FontColor(textColor).SemiBold()
                                                                 column.Item().AlignCenter().Text(amountText).FontSize(24).FontColor(textColor).Bold()
                                                             End Sub)
        End Sub

        Private Sub ComposeSalesAndPurchasesTables(container As IContainer, summary As VatReliefSummary)
            container.Row(Sub(row)
                              row.RelativeItem().PaddingRight(10).Element(Sub(c) ComposeSalesTable(c, summary))
                              row.RelativeItem().PaddingLeft(10).Element(Sub(c) ComposePurchasesTable(c, summary))
                          End Sub)
        End Sub

        Private Sub ComposeSalesTable(container As IContainer, summary As VatReliefSummary)
            container.Column(Sub(column)
                                 column.Item().PaddingBottom(5).Text("Sales Summary").FontSize(14).SemiBold()
                                 column.Item().Table(Sub(table)
                                                         table.ColumnsDefinition(Sub(columns)
                                                                                     columns.RelativeColumn()
                                                                                     columns.RelativeColumn()
                                                                                 End Sub)

                                                         table.Header(Sub(header)
                                                                          header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).Text("Item").SemiBold()
                                                                          header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("Amount").SemiBold()
                                                                      End Sub)

                                                         table.Cell().PaddingVertical(3).Text("Vatable Sales")
                                                         table.Cell().PaddingVertical(3).AlignRight().Text($"₱{summary.VatableSales:N2}")

                                                         table.Cell().PaddingVertical(3).Text("VAT-Exempt Sales")
                                                         table.Cell().PaddingVertical(3).AlignRight().Text($"₱{summary.VatExemptSales:N2}")

                                                         table.Cell().PaddingVertical(3).Text("Zero-Rated Sales")
                                                         table.Cell().PaddingVertical(3).AlignRight().Text($"₱{summary.ZeroRatedSales:N2}")

                                                         table.Cell().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(3).Text("Total Sales").SemiBold()
                                                         table.Cell().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(3).AlignRight().Text($"₱{summary.TotalSales:N2}").SemiBold()

                                                         table.Cell().PaddingVertical(3).Text("Output VAT").SemiBold().FontColor(Colors.Blue.Darken2)
                                                         table.Cell().PaddingVertical(3).AlignRight().Text($"₱{summary.OutputVat:N2}").SemiBold().FontColor(Colors.Blue.Darken2)
                                                     End Sub)
                             End Sub)
        End Sub

        Private Sub ComposePurchasesTable(container As IContainer, summary As VatReliefSummary)
            container.Column(Sub(column)
                                 column.Item().PaddingBottom(5).Text("Purchases Summary").FontSize(14).SemiBold()
                                 column.Item().Table(Sub(table)
                                                         table.ColumnsDefinition(Sub(columns)
                                                                                     columns.RelativeColumn()
                                                                                     columns.RelativeColumn()
                                                                                 End Sub)

                                                         table.Header(Sub(header)
                                                                          header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).Text("Item").SemiBold()
                                                                          header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("Amount").SemiBold()
                                                                      End Sub)

                                                         table.Cell().PaddingVertical(3).Text("Vatable Purchases")
                                                         table.Cell().PaddingVertical(3).AlignRight().Text($"₱{summary.VatablePurchases:N2}")

                                                         table.Cell().PaddingVertical(3).Text("VAT-Exempt Purchases")
                                                         table.Cell().PaddingVertical(3).AlignRight().Text($"₱{summary.VatExemptPurchases:N2}")

                                                         table.Cell().PaddingVertical(3).Text("Zero-Rated Purchases")
                                                         table.Cell().PaddingVertical(3).AlignRight().Text($"₱{summary.ZeroRatedPurchases:N2}")

                                                         table.Cell().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(3).Text("Total Purchases").SemiBold()
                                                         table.Cell().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(3).AlignRight().Text($"₱{summary.TotalPurchases:N2}").SemiBold()

                                                         table.Cell().PaddingVertical(3).Text("Input VAT").SemiBold().FontColor(Colors.Green.Darken2)
                                                         table.Cell().PaddingVertical(3).AlignRight().Text($"₱{summary.InputVat:N2}").SemiBold().FontColor(Colors.Green.Darken2)
                                                     End Sub)
                             End Sub)
        End Sub

        Private Sub ComposeTrailingMonthsTable(container As IContainer, trailingMonths As List(Of VatReliefSummary))
            container.Column(Sub(column)
                                 column.Item().PaddingBottom(5).Text("Trailing 12-Month Trend").FontSize(14).SemiBold()
                                 column.Item().Table(Sub(table)
                                                         table.ColumnsDefinition(Sub(columns)
                                                                                     columns.RelativeColumn(2)
                                                                                     columns.RelativeColumn()
                                                                                     columns.RelativeColumn()
                                                                                     columns.RelativeColumn()
                                                                                     columns.RelativeColumn()
                                                                                     columns.RelativeColumn()
                                                                                 End Sub)

                                                         table.Header(Sub(header)
                                                                          header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).Text("Period").SemiBold()
                                                                          header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("Total Sales").SemiBold()
                                                                          header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("Output VAT").SemiBold()
                                                                          header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("Purchases").SemiBold()
                                                                          header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("Input VAT").SemiBold()
                                                                          header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("Net VAT").SemiBold()
                                                                      End Sub)

                                                         For Each tm In trailingMonths
                                                             table.Cell().PaddingVertical(3).Text(tm.PeriodLabel)
                                                             table.Cell().PaddingVertical(3).AlignRight().Text($"₱{tm.TotalSales:N2}")
                                                             table.Cell().PaddingVertical(3).AlignRight().Text($"₱{tm.OutputVat:N2}")
                                                             table.Cell().PaddingVertical(3).AlignRight().Text($"₱{tm.TotalPurchases:N2}")
                                                             table.Cell().PaddingVertical(3).AlignRight().Text($"₱{tm.InputVat:N2}")
                                                             table.Cell().PaddingVertical(3).AlignRight().Text($"₱{tm.NetVatPayable:N2}")
                                                         Next
                                                     End Sub)
                             End Sub)
        End Sub

        Private Sub ComposeInterpretation(container As IContainer, interpretation As String)
            container.Column(Sub(column)
                                 column.Item().PaddingBottom(5).Text("What This Means").FontSize(14).SemiBold()
                                 column.Item().Text(interpretation)
                             End Sub)
        End Sub

        Private Sub ComposeFooter(container As IContainer)
            container.AlignCenter().Text(Sub(text)
                                             text.Span("Page ")
                                             text.CurrentPageNumber()
                                             text.Span(" of ")
                                             text.TotalPages()
                                         End Sub)
        End Sub

    End Class

End Namespace
