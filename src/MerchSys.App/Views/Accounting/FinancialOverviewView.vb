Imports System.Windows.Forms
Imports System.ComponentModel
Imports MerchSys.Accounting.Presenters
Imports MerchSys.Accounting.Services
Imports MerchSys.Accounting.Views

Namespace Views.Accounting

    Partial Public Class FinancialOverviewView
        Inherits UserControl
        Implements IFinancialOverviewView

        Private ReadOnly _presenter As FinancialOverviewPresenter

        ' Designer constructor
        Public Sub New()
            InitializeComponent()
            SetupDataGridViews()
            lblWhatThisMeansIcon.Image = Services.Theming.IconHelper.GetIcon(Services.Theming.AppIcon.Lightbulb, Drawing.Color.Goldenrod, 24)
            lblWhatThisMeansIcon.ImageAlign = Drawing.ContentAlignment.MiddleCenter
        AddHandler pnlErrorState.RetryClicked, Sub(s, ev) RaiseEvent RefreshRequested(Me, EventArgs.Empty)
        End Sub

        ' DI constructor
        Public Sub New(presenter As FinancialOverviewPresenter)
            InitializeComponent()
            _presenter = presenter
            _presenter.View = Me
            AddHandler _presenter.NavigateToVatReturnRequested, AddressOf OnNavigateToVatReturnRequested
            SetupDataGridViews()
            lblWhatThisMeansIcon.Image = Services.Theming.IconHelper.GetIcon(Services.Theming.AppIcon.Lightbulb, Drawing.Color.Goldenrod, 24)
            lblWhatThisMeansIcon.ImageAlign = Drawing.ContentAlignment.MiddleCenter
        AddHandler pnlErrorState.RetryClicked, Sub(s, ev) RaiseEvent RefreshRequested(Me, EventArgs.Empty)
        End Sub

        Private Sub SetupDataGridViews()
            ' Setup Top Products
            dgvTopProducts.AutoGenerateColumns = False
            dgvTopProducts.Columns.Clear()
            dgvTopProducts.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "ProductName", .HeaderText = "Product Name", .Width = 150})
            dgvTopProducts.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "QuantitySold", .HeaderText = "Units Sold", .Width = 80, .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "N0"}})
            dgvTopProducts.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "TotalRevenue", .HeaderText = "Total Revenue", .Width = 100, .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "N2"}})
            dgvTopProducts.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "TotalGrossProfit", .HeaderText = "Gross Profit", .Width = 100, .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "N2"}})

            ' Setup Monthly Trends
            dgvMonthlyTrends.AutoGenerateColumns = False
            dgvMonthlyTrends.Columns.Clear()
            dgvMonthlyTrends.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "MonthLabel", .HeaderText = "Month", .Width = 100})
            dgvMonthlyTrends.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "Revenue", .HeaderText = "Revenue", .Width = 100, .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "N2"}})
            dgvMonthlyTrends.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "Cogs", .HeaderText = "COGS", .Width = 100, .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "N2"}})
            dgvMonthlyTrends.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "GrossProfit", .HeaderText = "Gross Profit", .Width = 100, .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "N2"}})
            dgvMonthlyTrends.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "GrossMarginPercent", .HeaderText = "Margin %", .Width = 80, .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "N1"}})
        End Sub

        Public Event RefreshRequested As EventHandler Implements IFinancialOverviewView.RefreshRequested
        Public Event ViewLoaded As EventHandler Implements IFinancialOverviewView.ViewLoaded
        Public Event VatTileClicked As EventHandler Implements IFinancialOverviewView.VatTileClicked

        Private Sub FinancialOverviewView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            RaiseEvent ViewLoaded(Me, EventArgs.Empty)
        End Sub

        Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
            RaiseEvent RefreshRequested(Me, EventArgs.Empty)
        End Sub

        ''' <summary>
        ''' Initiates the WinForms MVP navigation flow by raising an event to the Presenter.
        ''' (Part of the ACC-16 VAT Payable integration flow).
        ''' </summary>
        Private Sub pnlVatTile_Click(sender As Object, e As EventArgs) Handles pnlVatTile.Click, lblVatPayable.Click, lblVatDueDate.Click
            RaiseEvent VatTileClicked(Me, EventArgs.Empty)
        End Sub

        ''' <summary>
        ''' Completes the WinForms MVP navigation flow. Invoked by the Presenter if role authorization passes.
        ''' Traverses to the main shell to execute module navigation.
        ''' </summary>
        Public Sub OnNavigateToVatReturnRequested(sender As Object, e As EventArgs)
            Dim shell = Application.OpenForms.OfType(Of MainWindow)().FirstOrDefault()
            If shell IsNot Nothing AndAlso shell.Presenter IsNot Nothing Then
                Dim targetItem = shell.Presenter.NavigationGroups.
                    SelectMany(Function(g) g.Items).
                    FirstOrDefault(Function(i) i.Name = "VAT Returns")
                If targetItem IsNot Nothing Then
                    shell.Presenter.NavigateTo(targetItem)
                End If
            End If
        End Sub

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property TodayRevenueDisplay As String Implements IFinancialOverviewView.TodayRevenueDisplay
            Get
                Return lblTodayRevenue.Text
            End Get
            Set(value As String)
                lblTodayRevenue.Text = $"Today: {value}"
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property MtdRevenueDisplay As String Implements IFinancialOverviewView.MtdRevenueDisplay
            Get
                Return lblMtdRevenue.Text
            End Get
            Set(value As String)
                lblMtdRevenue.Text = $"MTD: {value}"
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property YtdRevenueDisplay As String Implements IFinancialOverviewView.YtdRevenueDisplay
            Get
                Return lblYtdRevenue.Text
            End Get
            Set(value As String)
                lblYtdRevenue.Text = $"YTD: {value}"
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property TotalARDisplay As String Implements IFinancialOverviewView.TotalARDisplay
            Get
                Return lblTotalAR.Text
            End Get
            Set(value As String)
                lblTotalAR.Text = $"AR: {value}"
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property TotalAPDisplay As String Implements IFinancialOverviewView.TotalAPDisplay
            Get
                Return lblTotalAP.Text
            End Get
            Set(value As String)
                lblTotalAP.Text = $"AP: {value}"
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property InventoryValueDisplay As String Implements IFinancialOverviewView.InventoryValueDisplay
            Get
                Return lblInventoryValue.Text
            End Get
            Set(value As String)
                lblInventoryValue.Text = $"Inv Value: {value}"
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property GrossMarginTodayDisplay As String Implements IFinancialOverviewView.GrossMarginTodayDisplay
            Get
                Return lblGrossMarginToday.Text
            End Get
            Set(value As String)
                lblGrossMarginToday.Text = $"Margin: {value}"
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property WhatThisMeansText As String Implements IFinancialOverviewView.WhatThisMeansText
            Get
                Return lblWhatThisMeans.Text
            End Get
            Set(value As String)
                lblWhatThisMeans.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property OverdueARCount As Integer Implements IFinancialOverviewView.OverdueARCount
            Get
                Return 0 ' View only
            End Get
            Set(value As Integer)
                lblOverdueARCount.Text = If(value > 0, $"{value} Overdue AR", String.Empty)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property OverdueAPCount As Integer Implements IFinancialOverviewView.OverdueAPCount
            Get
                Return 0 ' View only
            End Get
            Set(value As Integer)
                lblOverdueAPCount.Text = If(value > 0, $"{value} Overdue AP", String.Empty)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property LowStockAlertCount As Integer Implements IFinancialOverviewView.LowStockAlertCount
            Get
                Return 0 ' View only
            End Get
            Set(value As Integer)
                lblLowStockAlertCount.Text = If(value > 0, $"{value} Low Stock Items", String.Empty)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
                Public Property IsLoading As Boolean Implements IFinancialOverviewView.IsLoading
            Get
                Return lblLoading.Visible
            End Get
            Set(value As Boolean)
                SafeInvoke(Sub()
                               lblLoading.Visible = value
                               btnRefresh.Enabled = Not value
                               If value Then pnlErrorState.Visible = False
                           End Sub)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
                Public Property ErrorMessage As String Implements IFinancialOverviewView.ErrorMessage
            Get
                Return "" ' View only
            End Get
            Set(value As String)
                SafeInvoke(Sub()
                               If String.IsNullOrWhiteSpace(value) Then
                                   pnlErrorState.Visible = False
                               Else
                                   pnlErrorState.Message = value
                                   pnlErrorState.Visible = True
                                   pnlErrorState.BringToFront()
                               End If
                           End Sub)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property LastRefreshedText As String Implements IFinancialOverviewView.LastRefreshedText
            Get
                Return lblLastRefreshed.Text
            End Get
            Set(value As String)
                lblLastRefreshed.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property VatPayableDisplay As String Implements IFinancialOverviewView.VatPayableDisplay
            Get
                Return lblVatPayable.Text
            End Get
            Set(value As String)
                lblVatPayable.Text = If(value = "N/A", value, $"VAT Payable: {value}")
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property VatDueDateDisplay As String Implements IFinancialOverviewView.VatDueDateDisplay
            Get
                Return lblVatDueDate.Text
            End Get
            Set(value As String)
                lblVatDueDate.Text = If(String.IsNullOrEmpty(value), "", $"Due: {value}")
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property VatSeverity As KpiSeverity Implements IFinancialOverviewView.VatSeverity
            Get
                If pnlVatTile.BackColor = System.Drawing.Color.LightCoral Then Return KpiSeverity.Critical
                If pnlVatTile.BackColor = System.Drawing.Color.LightYellow Then Return KpiSeverity.Warning
                Return KpiSeverity.Info
            End Get
            Set(value As KpiSeverity)
                Select Case value
                    Case KpiSeverity.Critical
                        pnlVatTile.BackColor = System.Drawing.Color.LightCoral
                    Case KpiSeverity.Warning
                        pnlVatTile.BackColor = System.Drawing.Color.LightYellow
                    Case Else
                        pnlVatTile.BackColor = System.Drawing.Color.LightGray
                End Select
            End Set
        End Property

        Public Sub SetTopSellingProducts(products As IEnumerable(Of TopProductDto)) Implements IFinancialOverviewView.SetTopSellingProducts
            dgvTopProducts.DataSource = products.ToList()
        End Sub

        Public Sub SetMonthlyTrends(trends As IEnumerable(Of MonthlyTrendDto)) Implements IFinancialOverviewView.SetMonthlyTrends
            dgvMonthlyTrends.DataSource = trends.ToList()
        End Sub

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property RevenueDeltaPercent As Decimal? Implements IFinancialOverviewView.RevenueDeltaPercent
            Get
                Return deltaIndicator.Percent
            End Get
            Set(value As Decimal?)
                deltaIndicator.Percent = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property RevenueSparkPoints As List(Of Decimal) Implements IFinancialOverviewView.RevenueSparkPoints
            Get
                Return sparkline.Points
            End Get
            Set(value As List(Of Decimal))
                sparkline.Points = value
            End Set
        End Property

        Private Sub SafeInvoke(action As Action)
            If Me.InvokeRequired Then
                Me.Invoke(action)
            Else
                action()
            End If
        End Sub

    End Class

End Namespace
