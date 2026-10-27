Imports System.Threading.Tasks
Imports MerchSys.Purchasing.Data
Imports MerchSys.Purchasing.Dtos
Imports MerchSys.Purchasing.Views
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Interfaces
Imports Microsoft.EntityFrameworkCore

Namespace Presenters
    Public Class PurchasingDashboardPresenter
        Implements IDisposable

        Private ReadOnly _dbContext As PurchasingDbContext
        Private ReadOnly _notificationService As INotificationService
        Private _view As IPurchasingDashboardView
        Private _disposed As Boolean

        Public Sub New(dbContext As PurchasingDbContext,
                       notificationService As INotificationService)
            _dbContext = dbContext
            _notificationService = notificationService
        End Sub

        Public Property View As IPurchasingDashboardView
            Get
                Return _view
            End Get
            Set(value As IPurchasingDashboardView)
                If _view IsNot Nothing Then
                    RemoveHandler _view.ViewLoaded, AddressOf OnViewLoaded
                    RemoveHandler _view.RefreshRequested, AddressOf OnRefreshRequested
                    RemoveHandler _view.NavigateRequested, AddressOf OnNavigateRequested
                End If
                _view = value
                If _view IsNot Nothing Then
                    AddHandler _view.ViewLoaded, AddressOf OnViewLoaded
                    AddHandler _view.RefreshRequested, AddressOf OnRefreshRequested
                    AddHandler _view.NavigateRequested, AddressOf OnNavigateRequested
                End If
            End Set
        End Property

        Private Async Sub OnViewLoaded(sender As Object, e As EventArgs)
            Await LoadDashboardDataAsync()
        End Sub

        Private Async Sub OnRefreshRequested(sender As Object, e As EventArgs)
            Await LoadDashboardDataAsync()
        End Sub

        Private Sub OnNavigateRequested(sender As Object, target As String)
            ' Future navigation handled here or in MainWindow
        End Sub

        Private Async Function LoadDashboardDataAsync() As Task
            If _view Is Nothing Then Return

            _view.IsLoading = True
            _view.ErrorMessage = String.Empty

            Try
                ' Outstanding AP
                Dim outstandingAp = Await _dbContext.AccountsPayable.
                    Where(Function(ap) Not ap.IsPaid).
                    SumAsync(Function(ap) ap.TotalAmount - ap.AmountPaid)
                _view.OutstandingAp = outstandingAp

                ' Overdue AP
                Dim today = DateTime.Today
                Dim overdueApCount = Await _dbContext.AccountsPayable.
                    Where(Function(ap) Not ap.IsPaid AndAlso ap.DueDate < today).
                    CountAsync()
                _view.OverdueApCount = overdueApCount

                ' Pending Deliveries Count
                Dim pendingDeliveriesQuery = _dbContext.PurchaseOrders.
                    Where(Function(po) po.Status = PurchaseOrderStatus.Submitted OrElse po.Status = PurchaseOrderStatus.PartialDelivery)

                Dim pendingDeliveriesCount = Await pendingDeliveriesQuery.CountAsync()
                _view.PendingDeliveriesCount = pendingDeliveriesCount

                ' Reorder Suggestions Count
                Dim reorderSuggestionsCount = Await _dbContext.ReorderSuggestions.CountAsync()
                _view.ReorderSuggestionsCount = reorderSuggestionsCount

                ' Active Vendors
                Dim activeVendorsQuery = _dbContext.Vendors.
                    Where(Function(v) Not v.IsDeleted)

                Dim activeVendorsCount = Await activeVendorsQuery.CountAsync()
                _view.ActiveVendorsCount = activeVendorsCount

                ' Average Lead Time
                Dim avgLeadTime As Double = 0
                If activeVendorsCount > 0 Then
                    avgLeadTime = Await activeVendorsQuery.AverageAsync(Function(v) v.LeadTimeDays)
                End If
                _view.AverageLeadTimeDays = avgLeadTime

                ' Top 10 Pending Deliveries Table
                Dim topPending = Await pendingDeliveriesQuery.
                    OrderBy(Function(po) po.ExpectedDeliveryDate).
                    Take(10).
                    Join(_dbContext.Vendors,
                         Function(po) po.VendorId,
                         Function(v) v.Id,
                         Function(po, v) New PendingDeliveryRowDto With {
                             .POId = po.Id,
                             .OrderNumber = po.OrderNumber,
                             .VendorName = v.Name,
                             .OrderDate = po.CreatedAt,
                             .ExpectedDeliveryDate = po.ExpectedDeliveryDate,
                             .Status = po.Status,
                             .TotalAmount = po.TotalAmount
                         }).
                    ToListAsync()
                _view.SetPendingDeliveries(topPending)

                ' Top 5 Vendors by Spend
                Dim topVendors = Await _dbContext.AccountsPayable.
                    Join(_dbContext.Vendors,
                         Function(ap) ap.VendorId,
                         Function(v) v.Id,
                         Function(ap, v) New With { .VendorId = v.Id, .VendorName = v.Name, .Amount = ap.TotalAmount }).
                    GroupBy(Function(x) New With { Key x.VendorId, Key x.VendorName }).
                    Select(Function(g) New TopVendorSpendDto With {
                        .VendorId = g.Key.VendorId,
                        .VendorName = g.Key.VendorName,
                        .TotalSpend = g.Sum(Function(x) x.Amount)
                    }).
                    OrderByDescending(Function(x) x.TotalSpend).
                    Take(5).
                    ToListAsync()
                _view.SetTopVendorsBySpend(topVendors)

            Catch ex As Exception
                _view.ErrorMessage = "Failed to load dashboard data."
                If _notificationService IsNot Nothing Then
                    _notificationService.ShowError("Failed to load purchasing dashboard data: " & ex.Message)
                End If
            Finally
                _view.IsLoading = False
            End Try
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            If Not _disposed Then
                View = Nothing
                _disposed = True
            End If
        End Sub
    End Class
End Namespace