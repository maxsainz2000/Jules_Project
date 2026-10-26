Imports System.Threading

Namespace Services

    Public Class VatReliefSummary
        Public Property Year As Integer
        Public Property Month As Integer
        Public Property PeriodLabel As String

        Public Property VatableSales As Decimal
        Public Property VatExemptSales As Decimal
        Public Property ZeroRatedSales As Decimal
        Public Property TotalSales As Decimal
        Public Property OutputVat As Decimal

        Public Property VatablePurchases As Decimal
        Public Property VatExemptPurchases As Decimal
        Public Property ZeroRatedPurchases As Decimal
        Public Property TotalPurchases As Decimal
        Public Property InputVat As Decimal

        Public Property NetVatPayable As Decimal
    End Class

    Public Interface IVatReliefReportService
        Function GetMonthlySummaryAsync(year As Integer, month As Integer, Optional cancellationToken As CancellationToken = Nothing) As Task(Of VatReliefSummary)
        Function GetTrailingMonthsAsync(endYear As Integer, endMonth As Integer, Optional count As Integer = 12, Optional cancellationToken As CancellationToken = Nothing) As Task(Of List(Of VatReliefSummary))
    End Interface

End Namespace
