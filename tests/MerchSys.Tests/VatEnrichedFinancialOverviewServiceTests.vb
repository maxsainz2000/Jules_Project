Imports Xunit
Imports Moq
Imports System.Threading
Imports MerchSys.Accounting.Services

Public Class VatEnrichedFinancialOverviewServiceTests
    <Fact>
    Public Async Function GetOverviewAsync_EnrichesDtoWithVatKpi() As Task
        ' Arrange
        Dim mockInner = New Mock(Of IFinancialOverviewService)()
        Dim baseDto = New FinancialOverviewDto With {.TodayRevenue = 500}
        mockInner.Setup(Function(s) s.GetOverviewAsync(It.IsAny(Of CancellationToken)())).ReturnsAsync(baseDto)

        Dim mockKpi = New Mock(Of IKpiProvider)()
        mockKpi.Setup(Function(s) s.GetKpiAsync(It.IsAny(Of CancellationToken)())).ReturnsAsync(New KpiValue With {
            .Key = "VatPayable",
            .Amount = 1500.5D,
            .Label = "VAT Payable",
            .DueDate = New DateTime(2025, 1, 20),
            .Severity = KpiSeverity.Warning
        })

        Dim provider = New VatEnrichedFinancialOverviewService(mockInner.Object, {mockKpi.Object})

        ' Act
        Dim result = Await provider.GetOverviewAsync()

        ' Assert
        Assert.NotNull(result)
        Assert.Equal(500D, result.TodayRevenue)
        Assert.True(result.IsVatRegistered)
        Assert.Equal(1500.5D, result.VatPayable)
        Assert.Equal("VAT Payable", result.VatPayableLabel)
        Assert.Equal(KpiSeverity.Warning, result.VatPayableSeverity)
        Assert.Equal(New DateTime(2025, 1, 20), result.VatFilingDueDate)
    End Function
End Class
