Imports MerchSys.Purchasing.Dtos

Namespace Views
    Public Interface IPurchasingDashboardView
        Property OutstandingAp As Decimal
        Property OverdueApCount As Integer
        Property PendingDeliveriesCount As Integer
        Property ReorderSuggestionsCount As Integer
        Property ActiveVendorsCount As Integer
        Property AverageLeadTimeDays As Double
        Property IsLoading As Boolean
        Property ErrorMessage As String

        Sub SetPendingDeliveries(rows As IEnumerable(Of PendingDeliveryRowDto))
        Sub SetTopVendorsBySpend(rows As IEnumerable(Of TopVendorSpendDto))

        Event ViewLoaded As EventHandler
        Event RefreshRequested As EventHandler
        Event NavigateRequested As EventHandler(Of String)
    End Interface
End Namespace