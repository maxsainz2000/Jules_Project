Imports MerchSys.Inventory.Services

Namespace Views

    Public Interface IStockDashboardView
        ' Summary Cards
        Property TotalStockValue As Decimal
        Property LowStockCount As Integer
        Property ExpiredBatchCount As Integer
        Property CriticalRiskCount As Integer
        Property TotalProducts As Integer

        ' Filters
        Property FilterText As String
        Property CategoryFilter As String
        Property StatusFilter As String

        ' Methods
        Sub SetProductRows(rows As IEnumerable(Of Presenters.ProductRowItem))
        Sub SetBatchRows(rows As IEnumerable(Of StockBatchSummaryDto))

        ' Events
        Event FilterChanged As EventHandler
        Event ProductSelected As EventHandler(Of Integer)

    End Interface

End Namespace
