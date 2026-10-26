Imports MerchSys.Accounting.Services
Imports Xunit

Public Class IncomeStatementPdfExporterTests

    <Fact>
    Public Sub ExportPdf_ReturnsValidPdfBytes()
        ' Arrange
        Dim exporter As New IncomeStatementPdfExporter()
        Dim statement As New IncomeStatementDto With {
            .PeriodLabel = "March 2026",
            .StartDate = New DateTime(2026, 3, 1),
            .EndDate = New DateTime(2026, 3, 31),
            .NetSales = 100000,
            .CostOfGoodsSold = 40000,
            .GrossProfit = 60000,
            .GrossMarginPercent = 60D,
            .OperatingExpenses = 20000,
            .NetIncome = 40000,
            .NetMarginPercent = 40D,
            .OperatingExpensesByCategory = New Dictionary(Of String, Decimal) From {{"Rent", 10000}, {"Utilities", 10000}}
        }

        Dim margins = New List(Of ProductMarginDto) From {
            New ProductMarginDto With {.ProductName = "Product A", .QuantitySold = 100, .Revenue = 50000, .Cogs = 20000, .GrossProfit = 30000, .GrossMarginPercent = 60D}
        }

        Dim interpretation = "The company is performing well."

        ' Act
        Dim result = exporter.ExportPdf(statement, margins, interpretation)

        ' Assert
        Assert.NotNull(result)
        Assert.True(result.Length > 0)

        ' Verify starts with %PDF
        Assert.Equal(&H25, result(0))
        Assert.Equal(&H50, result(1))
        Assert.Equal(&H44, result(2))
        Assert.Equal(&H46, result(3))
    End Sub

End Class
