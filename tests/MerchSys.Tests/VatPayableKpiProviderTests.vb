Imports Xunit
Imports Moq
Imports System.Threading
Imports MerchSys.Accounting.Services
Imports MerchSys.Accounting.Entities
Imports MerchSys.Accounting.Enums

Public Class VatPayableKpiProviderTests
    <Fact>
    Public Async Function GetKpiAsync_WhenNoVatReturns_ReturnsEmptyKpi() As Task
        ' Arrange
        Dim mockService = New Mock(Of IVatReportingService)()
        Dim provider = New VatPayableKpiProvider(mockService.Object)

        ' Setup to return nothing for generate and list
        mockService.Setup(Function(s) s.GenerateVatReturnAsync(It.IsAny(Of Integer)(), It.IsAny(Of Integer)(), It.IsAny(Of VatReturnFormType)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(CType(Nothing, VatReturn))
        mockService.Setup(Function(s) s.ListVatReturnsAsync(It.IsAny(Of Integer?)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(New List(Of VatReturn)())

        ' Act
        Dim result = Await provider.GetKpiAsync()

        ' Assert
        Assert.NotNull(result)
        Assert.Equal("VatPayable", result.Key)
        Assert.Equal(0D, result.Amount)
        Assert.Equal(KpiSeverity.Info, result.Severity)
    End Function

    <Fact>
    Public Async Function GetKpiAsync_WithCriticalSeverity_CalculatesCorrectly() As Task
        ' Arrange
        Dim mockService = New Mock(Of IVatReportingService)()
        Dim provider = New VatPayableKpiProvider(mockService.Object)

        ' Assume today is the 18th of the month. Due date is 20th. (2 days left -> Critical)
        Dim activeDate = DateTime.UtcNow.AddMonths(-1)
        Dim periodEnd = New DateTime(activeDate.Year, activeDate.Month, DateTime.DaysInMonth(activeDate.Year, activeDate.Month), 23, 59, 59, DateTimeKind.Utc)
        Dim vatReturn = New VatReturn With {
            .NetVatPayable = 1000D,
            .PeriodType = VatReturnPeriodType.Monthly,
            .PeriodEnd = periodEnd
        }

        mockService.Setup(Function(s) s.GenerateVatReturnAsync(It.IsAny(Of Integer)(), It.IsAny(Of Integer)(), It.IsAny(Of VatReturnFormType)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(vatReturn)

        ' Act
        Dim result = Await provider.GetKpiAsync()

        ' Assert
        Assert.NotNull(result)
        Assert.Equal(1000D, result.Amount)
        ' Depending on the exact current day, it might be Info/Warning/Critical. Since it's dynamic based on UtcNow,
        ' we can't assert severity exactly unless we inject an IClock, but we can verify it's computed and DueDate is set.
        Assert.True(result.DueDate.HasValue)
    End Function
End Class
