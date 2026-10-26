Imports System
Imports System.Collections.Generic
Imports QuestPDF.Fluent
Imports QuestPDF.Helpers
Imports QuestPDF.Infrastructure

Namespace Services

    Public Class IncomeStatementPdfExporter
        Implements IIncomeStatementPdfExporter

        Public Sub New()
            QuestPDF.Settings.License = LicenseType.Community
        End Sub

        Public Function ExportPdf(statement As IncomeStatementDto, Optional productMargins As IEnumerable(Of ProductMarginDto) = Nothing, Optional interpretation As String = Nothing) As Byte() Implements IIncomeStatementPdfExporter.ExportPdf
            Dim doc = Document.Create(Sub(container As IDocumentContainer)
                container.Page(Sub(page As PageDescriptor)
                    page.Size(PageSizes.A4)
                    page.Margin(2, Unit.Centimetre)
                    page.PageColor(Colors.White)
                    page.DefaultTextStyle(Function(x As TextStyle) x.FontSize(10).FontFamily("Segoe UI"))

                    page.Header().Element(Sub(h) ComposeHeader(h, statement))
                    page.Content().Element(Sub(c) ComposeContent(c, statement, productMargins, interpretation))
                    page.Footer().Element(Sub(f) ComposeFooter(f))
                End Sub)
            End Sub)

            Return doc.GeneratePdf()
        End Function

        Private Sub ComposeHeader(container As IContainer, statement As IncomeStatementDto)
            container.Column(Sub(c As ColumnDescriptor)
                c.Item().Text("Income Statement (Profit & Loss)").FontSize(16).SemiBold()
                c.Item().Text($"Period: {statement.PeriodLabel}").FontSize(12)
            End Sub)
        End Sub

        Private Sub ComposeContent(container As IContainer, statement As IncomeStatementDto, productMargins As IEnumerable(Of ProductMarginDto), interpretation As String)
            container.PaddingVertical(1, Unit.Centimetre).Column(Sub(col As ColumnDescriptor)

                col.Item().PaddingBottom(10).Text("Summary").FontSize(14).SemiBold()

                col.Item().Table(Sub(table As TableDescriptor)
                    table.ColumnsDefinition(Sub(cols)
                        cols.RelativeColumn(3)
                        cols.RelativeColumn(1)
                    End Sub)

                    table.Cell().Text("Net Sales:")
                    table.Cell().AlignRight().Text($"₱{statement.NetSales:N2}")

                    table.Cell().Text("Cost of Goods Sold:")
                    table.Cell().AlignRight().Text($"₱{statement.CostOfGoodsSold:N2}")

                    table.Cell().Text("Gross Profit:")
                    table.Cell().AlignRight().Text($"₱{statement.GrossProfit:N2}").SemiBold()

                    table.Cell().Text("Gross Margin %:")
                    table.Cell().AlignRight().Text($"{statement.GrossMarginPercent:N1}%")

                    table.Cell().Text("Operating Expenses:")
                    table.Cell().AlignRight().Text($"₱{statement.OperatingExpenses:N2}")

                    table.Cell().Text("Net Income:")
                    table.Cell().AlignRight().Text($"₱{statement.NetIncome:N2}").SemiBold()

                    table.Cell().Text("Net Margin %:")
                    table.Cell().AlignRight().Text($"{statement.NetMarginPercent:N1}%")
                End Sub)

                If statement.OperatingExpensesByCategory IsNot Nothing AndAlso statement.OperatingExpensesByCategory.Count > 0 Then
                    col.Item().PaddingTop(15).PaddingBottom(10).Text("Operating Expenses Breakdown").FontSize(14).SemiBold()
                    col.Item().Table(Sub(table As TableDescriptor)
                        table.ColumnsDefinition(Sub(cols)
                            cols.RelativeColumn(3)
                            cols.RelativeColumn(1)
                        End Sub)
                        For Each kvp In statement.OperatingExpensesByCategory
                            table.Cell().Text(kvp.Key)
                            table.Cell().AlignRight().Text($"₱{kvp.Value:N2}")
                        Next
                    End Sub)
                End If

                If productMargins IsNot Nothing Then
                    col.Item().PaddingTop(15).PaddingBottom(10).Text("Product Margins").FontSize(14).SemiBold()
                    col.Item().Table(Sub(table As TableDescriptor)
                        table.ColumnsDefinition(Sub(cols)
                            cols.RelativeColumn(2)
                            cols.RelativeColumn(1)
                            cols.RelativeColumn(1)
                            cols.RelativeColumn(1)
                            cols.RelativeColumn(1)
                            cols.RelativeColumn(1)
                        End Sub)

                        table.Header(Sub(header)
                            header.Cell().Text("Product").SemiBold()
                            header.Cell().AlignRight().Text("Units").SemiBold()
                            header.Cell().AlignRight().Text("Revenue").SemiBold()
                            header.Cell().AlignRight().Text("COGS").SemiBold()
                            header.Cell().AlignRight().Text("Gross Profit").SemiBold()
                            header.Cell().AlignRight().Text("Margin %").SemiBold()

                            header.Cell().ColumnSpan(6).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black)
                        End Sub)

                        For Each margin In productMargins
                            table.Cell().Text(margin.ProductName)
                            table.Cell().AlignRight().Text(margin.QuantitySold.ToString("N0"))
                            table.Cell().AlignRight().Text($"₱{margin.Revenue:N2}")
                            table.Cell().AlignRight().Text($"₱{margin.Cogs:N2}")
                            table.Cell().AlignRight().Text($"₱{margin.GrossProfit:N2}")
                            table.Cell().AlignRight().Text($"{margin.GrossMarginPercent:N1}%")
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
