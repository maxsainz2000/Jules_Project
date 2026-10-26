Namespace Services

    Public Interface IWhatThisMeansService
        Function GenerateOverviewInterpretation(overview As FinancialOverviewDto) As String
        Function GenerateIncomeStatementInterpretation(statement As IncomeStatementDto, Optional previousMargin As Decimal? = Nothing) As String
        Function GenerateSalesSummaryInterpretation(summary As AccountingSalesSummaryDto) As String
        Function GenerateMarginAlert(currentMargin As Decimal, previousMargin As Decimal, Optional thresholdPercent As Decimal = 3.0D) As String
        Function GenerateVatReliefInterpretation(summary As VatReliefSummary) As String
    End Interface

End Namespace