Imports Xunit
Imports MerchSys.Accounting.Services
Imports MerchSys.Accounting.Services.Insights

Public Class VatPayableInsightProviderTests
    <Fact>
    Public Sub GenerateInsight_WhenNotVatRegistered_ReturnsEmpty()
        ' Arrange
        Dim provider = New VatPayableInsightProvider()
        Dim dto = New FinancialOverviewDto With {.IsVatRegistered = False}

        ' Act
        Dim result = provider.GenerateInsight(dto)

        ' Assert
        Assert.Equal(String.Empty, result)
    End Sub

    <Fact>
    Public Sub GenerateInsight_WithCriticalSeverity_ReturnsCriticalMessage()
        ' Arrange
        Dim provider = New VatPayableInsightProvider()
        Dim dto = New FinancialOverviewDto With {
            .IsVatRegistered = True,
            .VatPayableSeverity = KpiSeverity.Critical,
            .VatPayable = 5000D,
            .VatFilingDueDate = New DateTime(2025, 1, 20)
        }

        ' Act
        Dim result = provider.GenerateInsight(dto)

        ' Assert
        Assert.Contains("CRITICAL: VAT payable", result)
        Assert.Contains("5,000.00", result)
    End Sub
End Class
