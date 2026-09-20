Imports System.Windows.Forms
Imports MerchSys.App.Views

Namespace Presenters
    Public Class OwnerDashboardPresenter
        Private ReadOnly _view As IOwnerDashboardView
        Private WithEvents _refreshTimer As Timer

        Public Sub New(view As IOwnerDashboardView)
            _view = view

            _refreshTimer = New Timer()
            _refreshTimer.Interval = 60000 ' 60 seconds

            AddHandler _view.Load, AddressOf OnViewLoad
            AddHandler _refreshTimer.Tick, AddressOf OnRefreshTick
        End Sub

        Private Async Sub OnViewLoad(sender As Object, e As EventArgs)
            Await LoadDashboardDataAsync()
            _refreshTimer.Start()
        End Sub

        Private Async Sub OnRefreshTick(sender As Object, e As EventArgs)
            Await LoadDashboardDataAsync()
        End Sub

        Private Async Function LoadDashboardDataAsync() As Task
            _view.SetLoading(True)

            Try
                ' Simulate data fetching delay
                Await Task.Delay(1000)

                ' Dummy KPI Data setup
                _view.Kpi1Value = "$1,250,000"
                _view.Kpi1Interpretation = "Gross Revenue is up 5% this month compared to last year."

                _view.Kpi2Value = "$420,000"
                _view.Kpi2Interpretation = "Inventory value reflects current stock ready for sale."

                _view.Kpi3Value = "$830,000"
                _view.Kpi3Interpretation = "Total expenses cover payroll, utilities, and COGS."

                _view.Kpi4Value = "$420,000"
                _view.Kpi4Interpretation = "Net profit margins are holding steady at 33.6%."
            Catch ex As Exception
                ' In a real app we might log this or show an error
            Finally
                _view.SetLoading(False)
            End Try
        End Function
    End Class
End Namespace
