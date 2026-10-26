Imports QuestPDF.Fluent
Imports QuestPDF.Helpers
Imports QuestPDF.Infrastructure
Imports System.Collections.Generic
Imports System.Linq

Namespace Services

    Public Class IncomeStatementPdfExporter
        Implements IIncomeStatementPdfExporter

        Public Sub New()
            QuestPDF.Settings.License = LicenseType.Community
        End Sub

        Public Function ExportPdf(statement As IncomeStatementDto, Optional productMargins As IEnumerable(Of ProductMarginDto) = Nothing, Optional interpretation As String = Nothing) As Byte() Implements IIncomeStatementPdfExporter.ExportPdf
            Return Document.Create(Sub(container)
                                       container.Page(Sub(page)
                                                          page.Size(PageSizes.A4)
                                                          page.Margin(1, Unit.Centimetre)
                                                          page.PageColor(Colors.White)
                                                          page.DefaultTextStyle(Function(x As TextStyle) x.FontSize(11))

                                                          page.Header().Element(Sub(c) ComposeHeader(c, statement))
                                                          page.Content().Element(Sub(c) ComposeContent(c, statement, productMargins, interpretation))
                                                          page.Footer().Element(Sub(c) ComposeFooter(c))
                                                      End Sub)
                                   End Sub).GeneratePdf()
        End Function

        Private Sub ComposeHeader(container As IContainer, statement As IncomeStatementDto)
            container.Row(Sub(row)
                              row.RelativeItem().Column(Sub(column)
                                                            column.Item().Text("Income Statement (Profit & Loss)").FontSize(20).SemiBold().FontColor(Colors.Blue.Darken2)
                                                            column.Item().Text(statement.PeriodLabel).FontSize(14)
                                                        End Sub)
                          End Sub)
        End Sub

        Private Sub ComposeContent(container As IContainer, statement As IncomeStatementDto, productMargins As IEnumerable(Of ProductMarginDto), interpretation As String)
            container.PaddingVertical(1, Unit.Centimetre).Column(Sub(column)
                                                                     column.Spacing(20)

                                                                     column.Item().Element(Sub(c) ComposeSummaryTable(c, statement))

                                                                     If statement.OperatingExpensesByCategory IsNot Nothing AndAlso statement.OperatingExpensesByCategory.Count > 0 Then
                                                                         column.Item().Element(Sub(c) ComposeOperatingExpensesTable(c, statement.OperatingExpensesByCategory))
                                                                     End If

                                                                     If productMargins IsNot Nothing AndAlso productMargins.Any() Then
                                                                         column.Item().Element(Sub(c) ComposeProductMarginsTable(c, productMargins))
                                                                     End If

                                                                     If Not String.IsNullOrWhiteSpace(interpretation) Then
                                                                         column.Item().Element(Sub(c) ComposeInterpretation(c, interpretation))
                                                                     End If
                                                                 End Sub)
        End Sub

        Private Sub ComposeSummaryTable(container As IContainer, statement As IncomeStatementDto)
            container.Table(Sub(table)
                                table.ColumnsDefinition(Sub(columns)
                                                            columns.RelativeColumn()
                                                            columns.RelativeColumn()
                                                        End Sub)

                                table.Header(Sub(header)
                                                 header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).Text("Item").SemiBold()
                                                 header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("Amount").SemiBold()
                                             End Sub)

                                ' Net Sales
                                table.Cell().PaddingVertical(5).Text("Net Sales")
                                table.Cell().PaddingVertical(5).AlignRight().Text($"₱{statement.NetSales:N2}")

                                ' Cost of Goods Sold
                                table.Cell().PaddingVertical(5).Text("Cost of Goods Sold")
                                table.Cell().PaddingVertical(5).AlignRight().Text($"₱{statement.CostOfGoodsSold:N2}")

                                ' Gross Profit & Margin
                                table.Cell().PaddingVertical(5).Text("Gross Profit")
                                table.Cell().PaddingVertical(5).AlignRight().Text($"₱{statement.GrossProfit:N2}")

                                table.Cell().PaddingVertical(5).Text("Gross Margin %")
                                table.Cell().PaddingVertical(5).AlignRight().Text($"{statement.GrossMarginPercent:N1}%")

                                ' Operating Expenses
                                table.Cell().PaddingVertical(5).Text("Operating Expenses")
                                table.Cell().PaddingVertical(5).AlignRight().Text($"₱{statement.OperatingExpenses:N2}")

                                ' Net Income & Margin
                                table.Cell().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).Text("Net Income").SemiBold()
                                table.Cell().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).AlignRight().Text($"₱{statement.NetIncome:N2}").SemiBold()

                                table.Cell().PaddingVertical(5).Text("Net Margin %").SemiBold()
                                table.Cell().PaddingVertical(5).AlignRight().Text($"{statement.NetMarginPercent:N1}%").SemiBold()
                            End Sub)
        End Sub

        Private Sub ComposeOperatingExpensesTable(container As IContainer, expenses As IDictionary(Of String, Decimal))
            container.Column(Sub(column)
                                 column.Item().PaddingBottom(10).Text("Operating Expenses Breakdown").FontSize(14).SemiBold()
                                 column.Item().Table(Sub(table)
                                                         table.ColumnsDefinition(Sub(columns)
                                                                                     columns.RelativeColumn()
                                                                                     columns.RelativeColumn()
                                                                                 End Sub)

                                                         table.Header(Sub(header)
                                                                          header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).Text("Category").SemiBold()
                                                                          header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("Amount").SemiBold()
                                                                      End Sub)

                                                         For Each kvp In expenses
                                                             table.Cell().PaddingVertical(5).Text(kvp.Key)
                                                             table.Cell().PaddingVertical(5).AlignRight().Text($"₱{kvp.Value:N2}")
                                                         Next
                                                     End Sub)
                             End Sub)
        End Sub

        Private Sub ComposeProductMarginsTable(container As IContainer, margins As IEnumerable(Of ProductMarginDto))
            container.Column(Sub(column)
                                 column.Item().PaddingBottom(10).Text("Product Margins").FontSize(14).SemiBold()
                                 column.Item().Table(Sub(table)
                                                         table.ColumnsDefinition(Sub(columns)
                                                                                     columns.RelativeColumn(3)
                                                                                     columns.RelativeColumn()
                                                                                     columns.RelativeColumn()
                                                                                     columns.RelativeColumn()
                                                                                     columns.RelativeColumn()
                                                                                     columns.RelativeColumn()
                                                                                 End Sub)

                                                         table.Header(Sub(header)
                                                                          header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).Text("Product").SemiBold()
                                                                          header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("Units Sold").SemiBold()
                                                                          header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("Revenue").SemiBold()
                                                                          header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("COGS").SemiBold()
                                                                          header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("Gross Profit").SemiBold()
                                                                          header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("Margin %").SemiBold()
                                                                      End Sub)

                                                         For Each m In margins
                                                             table.Cell().PaddingVertical(5).Text(m.ProductName)
                                                             table.Cell().PaddingVertical(5).AlignRight().Text(m.QuantitySold.ToString("N0"))
                                                             table.Cell().PaddingVertical(5).AlignRight().Text($"₱{m.Revenue:N2}")
                                                             table.Cell().PaddingVertical(5).AlignRight().Text($"₱{m.Cogs:N2}")
                                                             table.Cell().PaddingVertical(5).AlignRight().Text($"₱{m.GrossProfit:N2}")
                                                             table.Cell().PaddingVertical(5).AlignRight().Text($"{m.GrossMarginPercent:N1}%")
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
