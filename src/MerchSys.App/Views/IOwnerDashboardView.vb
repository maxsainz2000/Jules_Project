Namespace Views
    Public Interface IOwnerDashboardView
        Event Load As EventHandler

        Property Kpi1Value As String
        Property Kpi1Interpretation As String

        Property Kpi2Value As String
        Property Kpi2Interpretation As String

        Property Kpi3Value As String
        Property Kpi3Interpretation As String

        Property Kpi4Value As String
        Property Kpi4Interpretation As String

        Event RefreshRequested As EventHandler

        Property ErrorMessage As String
        Property IsLoading As Boolean

        Property TodayRevenueDelta As Decimal?
        Property WeekRevenueDelta As Decimal?

        Sub SetLoading(loading As Boolean)
    End Interface
End Namespace
