Imports MerchSys.Accounting.Services
Imports Xunit
Imports System.Collections.Generic

Public Class VatReliefPdfExporterTests

    <Fact>
    Public Sub ExportPdf_ReturnsValidPdfBytes()
        ' Arrange
        Dim exporter As New VatReliefPdfExporter()
        Dim summary As New VatReliefSummary With {
            .Year = 2026,
            .Month = 3,
            .PeriodLabel = "March 2026",
            .VatableSales = 100000,
            .OutputVat = 12000,
            .TotalSales = 100000,
            .VatablePurchases = 50000,
            .InputVat = 6000,
            .TotalPurchases = 50000,
            .NetVatPayable = 6000
        }

        Dim trailing As New List(Of VatReliefSummary) From {
            New VatReliefSummary With {
                .PeriodLabel = "February 2026",
                .TotalSales = 90000,
                .OutputVat = 10800,
                .TotalPurchases = 40000,
                .InputVat = 4800,
                .NetVatPayable = 6000
            }
        }

        Dim interpretation = "The VAT payable is stable."

        ' Act
        Dim result = exporter.ExportPdf(summary, trailing, interpretation)

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
