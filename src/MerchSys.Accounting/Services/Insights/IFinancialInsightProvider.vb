Namespace Services.Insights
    Public Interface IFinancialInsightProvider
        Function GenerateInsight(overview As FinancialOverviewDto) As String
    End Interface
End Namespace
