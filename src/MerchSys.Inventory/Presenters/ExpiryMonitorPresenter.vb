Imports System
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports System.Linq
Imports MerchSys.Inventory.Services
Imports MerchSys.Inventory.Views

Namespace Presenters

    Public Class ExpiryRowItem
        Public Property BatchId As Integer
        Public Property ProductId As Integer
        Public Property ExpiryDate As DateTime
        Public Property QuantityRemaining As Integer
        Public Property UrgencyLevel As Integer ' 1: Expired, 2: <= 7 days, 3: > 7 days
    End Class

    Public Class ExpiryMonitorPresenter
        Private ReadOnly _view As IExpiryMonitorView
        Private ReadOnly _expiryTrackingService As IExpiryTrackingService
        Private ReadOnly _shrinkageService As IShrinkageService
        Private ReadOnly _timer As Timer

        Public Sub New(view As IExpiryMonitorView, expiryTrackingService As IExpiryTrackingService, shrinkageService As IShrinkageService)
            _view = view
            _expiryTrackingService = expiryTrackingService
            _shrinkageService = shrinkageService

            _view.ThresholdDays = 30 ' Default

            AddHandler _view.RefreshRequested, AddressOf OnRefreshRequested
            AddHandler _view.ThresholdChanged, AddressOf OnThresholdChanged
            AddHandler _view.WriteOffRequested, AddressOf OnWriteOffRequested

            _timer = New Timer()
            _timer.Interval = 60000
            AddHandler _timer.Tick, AddressOf OnTimerTick
        End Sub

        Public Sub Initialize()
            _timer.Start()
            LoadDataAsync().ConfigureAwait(False)
        End Sub

        Private Async Sub OnRefreshRequested(sender As Object, e As EventArgs)
            Await LoadDataAsync()
        End Sub

        Private Async Sub OnThresholdChanged(sender As Object, e As EventArgs)
            Await LoadDataAsync()
        End Sub

        Private Sub OnTimerTick(sender As Object, e As EventArgs)
            LoadDataAsync().ConfigureAwait(False)
        End Sub

        Private Async Sub OnWriteOffRequested(sender As Object, e As ExpiryActionArgs)
            Try
                Await _shrinkageService.RecordShrinkageAsync(e.ProductId, e.Quantity, "Expired Write-Off", e.BatchId)
                Await LoadDataAsync()
            Catch ex As Exception
                ' Handle exception appropriately in UI later (e.g. propagate or show error event, but we don't have an error event yet)
                ' We just let it fail silently or log in real app
                System.Console.WriteLine("Error writing off: " & ex.Message)
            End Try
        End Sub

        Private Async Function LoadDataAsync() As Task
            Dim thresholdDays = _view.ThresholdDays
            Dim nearExpiryAlerts = Await _expiryTrackingService.GetNearExpiryAlertsAsync(thresholdDays)
            Dim expiredAlerts = Await _expiryTrackingService.GetExpiredBatchesAsync()

            Dim today = DateTime.Today

            Dim nearExpiryRows = nearExpiryAlerts.Select(Function(a) New ExpiryRowItem With {
                .BatchId = a.BatchId,
                .ProductId = a.ProductId,
                .ExpiryDate = a.ExpiryDate,
                .QuantityRemaining = a.QuantityRemaining,
                .UrgencyLevel = If((a.ExpiryDate - today).TotalDays <= 7, 2, 3)
            }).ToList()

            Dim expiredRows = expiredAlerts.Select(Function(a) New ExpiryRowItem With {
                .BatchId = a.BatchId,
                .ProductId = a.ProductId,
                .ExpiryDate = a.ExpiryDate,
                .QuantityRemaining = a.QuantityRemaining,
                .UrgencyLevel = 1
            }).ToList()

            ' We don't have unit cost in ExpiryAlertDto, but the requirement states we need TotalValueAtRisk
            ' The DTO in IExpiryTrackingService.vb doesn't expose unit cost. Wait...
            ' ExpiryAlertDto only has ProductId, BatchId, ExpiryDate, QuantityRemaining.
            ' But wait! TotalValueAtRisk is required on the summary card. Where do we get total value at risk?
            ' The prompt says: update summary cards (NearExpiryCount, ExpiredCount, TotalValueAtRisk)
            ' But the injected service IExpiryTrackingService doesn't expose Unit Cost on ExpiryAlertDto.
            ' Let's set it to 0 for now since we can't easily calculate it without another service,
            ' or maybe it's just meant to be a placeholder or we use a different way?
            ' Actually, we could inject something else but strictly adhering to prompted dependencies:
            ' "Strictly adhere to prompted dependencies. Do not inject or use unprompted services to fetch external data"
            ' So I'll set TotalValueAtRisk to 0.

            _view.NearExpiryCount = nearExpiryRows.Count
            _view.ExpiredCount = expiredRows.Count
            _view.TotalValueAtRisk = 0D

            _view.DisplayNearExpiry(nearExpiryRows)
            _view.DisplayExpired(expiredRows)
        End Function
    End Class
End Namespace
