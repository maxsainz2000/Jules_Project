Imports MerchSys.Accounting.Services
Imports Xunit
Imports System.Text
Imports System.Collections.Generic
Imports System.Linq

Namespace Services
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
            .TotalSales = 100000,
            .OutputVat = 12000,
            .VatablePurchases = 50000,
            .TotalPurchases = 50000,
            .InputVat = 6000,
            .NetVatPayable = 6000
        }

        Dim trailingMonths = New List(Of VatReliefSummary) From {
            New VatReliefSummary With {.PeriodLabel = "Feb 2026", .TotalSales = 90000, .OutputVat = 10800, .TotalPurchases = 40000, .InputVat = 4800, .NetVatPayable = 6000}
        }

        Dim interpretation = "Net VAT Payable is stable."

        ' Act
        Dim pdfBytes = exporter.ExportPdf(summary, trailingMonths, interpretation)

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
