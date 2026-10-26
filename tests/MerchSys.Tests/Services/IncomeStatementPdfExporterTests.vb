Imports MerchSys.Accounting.Services
Imports Xunit
Imports System.Text
Imports System.Collections.Generic
Imports System.Linq

Namespace Services
Public Class IncomeStatementPdfExporterTests

    <Fact>
    Public Sub ExportPdf_ReturnsValidPdfBytes()
        ' Arrange
        Dim exporter As New IncomeStatementPdfExporter()
        Dim statement As New IncomeStatementDto With {
            .PeriodLabel = "March 2026",
            .StartDate = New DateTime(2026, 3, 1),
            .EndDate = New DateTime(2026, 3, 31),
            .GrossSales = 100000,
            .NetSales = 100000,
            .CostOfGoodsSold = 40D,
            .GrossProfit = 60D,
            .GrossMarginPercent = 60D,
            .OperatingExpenses = 20000,
            .NetIncome = 40D,
            .NetMarginPercent = 40D
        }

        Dim margins = New List(Of ProductMarginDto) From {
            New ProductMarginDto With {.ProductName = "Product A", .QuantitySold = 100, .Revenue = 50000, .Cogs = 20000, .GrossProfit = 30000, .GrossMarginPercent = 60D}
        }

        Dim interpretation = "Excellent performance this month."

        ' Act
        Dim pdfBytes = exporter.ExportPdf(statement, margins, interpretation)

        ' Assert
        Assert.NotNull(pdfBytes)
        Assert.True(pdfBytes.Length > 0)

        ' Check PDF magic number signature %PDF
        Dim headerBytes = pdfBytes.Take(4).ToArray()
        Dim headerString = Encoding.ASCII.GetString(headerBytes)
        Assert.Equal("%PDF", headerString)
    End Sub

End Class
End Namespace
