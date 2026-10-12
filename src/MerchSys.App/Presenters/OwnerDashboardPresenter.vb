Imports System.Globalization
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Logging
Imports MerchSys.Accounting.Data
Imports MerchSys.App.Views
Imports MerchSys.Inventory.Data
Imports MerchSys.POS.Data
Imports MerchSys.Purchasing.Data
Imports MerchSys.SharedKernel.Enums

Namespace Presenters

    ''' <summary>
    ''' Owner Dashboard presenter. Aggregates live KPIs from each module's DbContext and
    ''' renders them with a plain-language interpretation. Replaces the previous
    ''' hardcoded mock figures (H1). Owns a 60-second refresh timer and implements
    ''' <see cref="IDisposable"/> so the timer does not leak on close (M9).
    ''' </summary>
    Public Class OwnerDashboardPresenter
        Implements IDisposable

        Private ReadOnly _view As IOwnerDashboardView
        Private ReadOnly _purchasingDb As PurchasingDbContext
        Private ReadOnly _inventoryDb As InventoryDbContext
        Private ReadOnly _posDb As POSDbContext
        Private ReadOnly _accountingDb As AccountingDbContext
        Private ReadOnly _logger As ILogger(Of OwnerDashboardPresenter)
        Private ReadOnly _refreshTimer As Timer

        Private _disposed As Boolean

        Public Sub New(view As IOwnerDashboardView,
                       purchasingDb As PurchasingDbContext,
                       inventoryDb As InventoryDbContext,
                       posDb As POSDbContext,
                       accountingDb As AccountingDbContext,
                       logger As ILogger(Of OwnerDashboardPresenter))
            _view = view
            _purchasingDb = purchasingDb
            _inventoryDb = inventoryDb
            _posDb = posDb
            _accountingDb = accountingDb
            _logger = logger

            _refreshTimer = New Timer()
            _refreshTimer.Interval = 60000 ' 60 seconds

            AddHandler _view.Load, AddressOf OnViewLoad
            AddHandler _refreshTimer.Tick, AddressOf OnRefreshTick
        End Sub

        Private Async Sub OnViewLoad(sender As Object, e As EventArgs)
            Await LoadDashboardDataAsync()
            If Not _disposed Then _refreshTimer.Start()
        End Sub

        Private Async Sub OnRefreshTick(sender As Object, e As EventArgs)
            Await LoadDashboardDataAsync()
        End Sub

        Private Async Function LoadDashboardDataAsync() As Task
            _view.SetLoading(True)
            Try
                Await LoadPurchasingAsync()
                Await LoadInventoryAsync()
                Await LoadSalesAsync()
                Await LoadAccountingAsync()
            Catch ex As Exception
                ' Each card guards its own query; reaching here means something unexpected.
                _logger.LogError(ex, "Unexpected failure while loading owner dashboard KPIs.")
            Finally
                _view.SetLoading(False)
            End Try
        End Function

        Private Async Function LoadPurchasingAsync() As Task
            Try
                Dim vendorCount = Await _purchasingDb.Vendors.CountAsync()
                Dim openPoCount = Await _purchasingDb.PurchaseOrders.CountAsync(
                    Function(po) po.Status <> PurchaseOrderStatus.Closed AndAlso po.Status <> PurchaseOrderStatus.Cancelled)
                Dim overdueAp = Await _purchasingDb.AccountsPayable.
                    Where(Function(ap) Not ap.IsPaid AndAlso ap.DueDate < DateTime.UtcNow).
                    SumAsync(Function(ap) ap.TotalAmount - ap.AmountPaid)

                _view.Kpi1Value = $"{vendorCount} vendors · {openPoCount} open POs"
                _view.Kpi1Interpretation =
                    If(openPoCount = 0,
                       $"All purchase orders are settled. Overdue accounts payable: {FormatMoney(overdueAp)}.",
                       $"{openPoCount} purchase order(s) are still open. Overdue accounts payable: {FormatMoney(overdueAp)}.")
            Catch ex As Exception
                _logger.LogWarning(ex, "Purchasing KPIs unavailable.")
                _view.Kpi1Value = "Unavailable"
                _view.Kpi1Interpretation = "Purchasing figures could not be loaded right now."
            End Try
        End Function

        Private Async Function LoadInventoryAsync() As Task
            Try
                Dim skuCount = Await _inventoryDb.Products.CountAsync()
                Dim stockValue = Await _inventoryDb.StockBatches.SumAsync(Function(b) b.QuantityRemaining * b.UnitCost)
                Dim expiringSoon = Await _inventoryDb.StockBatches.CountAsync(
                    Function(b) b.QuantityRemaining > 0 AndAlso b.ExpiryDate.HasValue AndAlso b.ExpiryDate.Value <= DateTime.UtcNow.AddDays(30))

                _view.Kpi2Value = $"{skuCount} SKUs · {FormatMoney(stockValue)}"
                _view.Kpi2Interpretation =
                    $"{FormatMoney(stockValue)} of stock is on hand across {skuCount} product(s). " &
                    If(expiringSoon = 0, "Nothing is expiring within 30 days.", $"{expiringSoon} batch(es) expire within 30 days.")
            Catch ex As Exception
                _logger.LogWarning(ex, "Inventory KPIs unavailable.")
                _view.Kpi2Value = "Unavailable"
                _view.Kpi2Interpretation = "Inventory figures could not be loaded right now."
            End Try
        End Function

        Private Async Function LoadSalesAsync() As Task
            Try
                Dim todayStart = DateTime.UtcNow.Date
                Dim weekStart = todayStart.AddDays(-6)

                Dim todayRevenue = Await _posDb.SalesTransactions.
                    Where(Function(t) Not t.IsVoid AndAlso t.TransactionDate >= todayStart).
                    SumAsync(Function(t) t.TotalAmount)
                Dim weekRevenue = Await _posDb.SalesTransactions.
                    Where(Function(t) Not t.IsVoid AndAlso t.TransactionDate >= weekStart).
                    SumAsync(Function(t) t.TotalAmount)
                Dim transactionsToday = Await _posDb.SalesTransactions.
                    CountAsync(Function(t) Not t.IsVoid AndAlso t.TransactionDate >= todayStart)

                _view.Kpi3Value = $"{FormatMoney(todayRevenue)} today · {FormatMoney(weekRevenue)} this week"
                _view.Kpi3Interpretation =
                    If(transactionsToday = 0,
                       "No sales recorded today.",
                       $"{transactionsToday} sale(s) recorded today.")
            Catch ex As Exception
                _logger.LogWarning(ex, "Sales KPIs unavailable.")
                _view.Kpi3Value = "Unavailable"
                _view.Kpi3Interpretation = "Sales figures could not be loaded right now."
            End Try
        End Function

        Private Async Function LoadAccountingAsync() As Task
            Try
                Dim now = DateTime.UtcNow
                Dim monthStart = New DateTime(now.Year, now.Month, 1)

                Dim revenue = Await _accountingDb.RevenueRecords.
                    Where(Function(r) r.RecordDate >= monthStart).
                    SumAsync(Function(r) r.TotalRevenue)
                Dim expenses = Await _accountingDb.ExpenseRecords.
                    Where(Function(e) e.ExpenseDate >= monthStart).
                    SumAsync(Function(e) e.Amount)
                Dim netIncome = revenue - expenses

                _view.Kpi4Value = $"{FormatMoney(netIncome)} net income (MTD)"
                _view.Kpi4Interpretation =
                    If(netIncome >= 0,
                       $"The business is profitable this month with {FormatMoney(netIncome)} net income.",
                       $"The business is running at a loss of {FormatMoney(Math.Abs(netIncome))} this month.")
            Catch ex As Exception
                _logger.LogWarning(ex, "Accounting KPIs unavailable.")
                _view.Kpi4Value = "Unavailable"
                _view.Kpi4Interpretation = "Accounting figures could not be loaded right now."
            End Try
        End Function

        Private Shared Function FormatMoney(value As Decimal) As String
            Return "₱" & value.ToString("N2", CultureInfo.InvariantCulture)
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            If _disposed Then Return
            _disposed = True

            _refreshTimer.Stop()
            RemoveHandler _view.Load, AddressOf OnViewLoad
            RemoveHandler _refreshTimer.Tick, AddressOf OnRefreshTick
            _refreshTimer.Dispose()
        End Sub

    End Class
End Namespace
