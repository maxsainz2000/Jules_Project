Imports System.Windows.Forms
Imports System.ComponentModel
Imports MerchSys.Accounting.Views
Imports MerchSys.Accounting.Presenters
Imports MerchSys.Accounting.Services
Imports System.Drawing

Namespace Views.Accounting

    Partial Public Class SalesSummaryView
        Inherits UserControl
        Implements ISalesSummaryView

        Private ReadOnly _presenter As SalesSummaryPresenter
        Private _isInitializing As Boolean = True

        Public Event PeriodChanged As EventHandler Implements ISalesSummaryView.PeriodChanged
        Public Event RefreshRequested As EventHandler Implements ISalesSummaryView.RefreshRequested
        Public Event ViewLoaded As EventHandler Implements ISalesSummaryView.ViewLoaded

        Public Sub New(presenter As SalesSummaryPresenter)
            InitializeComponent()
            _presenter = presenter
            _presenter.View = Me

            SetupDataGrids()

            AddHandler rbDaily.CheckedChanged, AddressOf Selector_Changed
            AddHandler rbWeekly.CheckedChanged, AddressOf Selector_Changed
            AddHandler rbMonthly.CheckedChanged, AddressOf Selector_Changed
            AddHandler dtpDate.ValueChanged, AddressOf Selector_Changed
            AddHandler cboYear.SelectedIndexChanged, AddressOf Selector_Changed
            AddHandler cboMonth.SelectedIndexChanged, AddressOf Selector_Changed
            AddHandler btnRefresh.Click, AddressOf Refresh_Clicked
        End Sub

        Private Sub SetupDataGrids()
            dgvPaymentBreakdowns.AutoGenerateColumns = False
            dgvPaymentBreakdowns.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "PaymentMethodName", .HeaderText = "Payment Method", .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill })
            dgvPaymentBreakdowns.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "GrossSales", .HeaderText = "Gross Sales", .DefaultCellStyle = New DataGridViewCellStyle With { .Format = "C2" } })
            dgvPaymentBreakdowns.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "TransactionCount", .HeaderText = "Transactions", .DefaultCellStyle = New DataGridViewCellStyle With { .Format = "N0" } })
            dgvPaymentBreakdowns.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "PercentageOfTotal", .HeaderText = "% of Total", .DefaultCellStyle = New DataGridViewCellStyle With { .Format = "N1" } })

            dgvDailyBreakdown.AutoGenerateColumns = False
            dgvDailyBreakdown.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "SaleDate", .HeaderText = "Date", .DefaultCellStyle = New DataGridViewCellStyle With { .Format = "d" } })
            dgvDailyBreakdown.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "GrossSales", .HeaderText = "Gross Sales", .DefaultCellStyle = New DataGridViewCellStyle With { .Format = "C2" } })
            dgvDailyBreakdown.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "Discounts", .HeaderText = "Discounts", .DefaultCellStyle = New DataGridViewCellStyle With { .Format = "C2" } })
            dgvDailyBreakdown.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "Returns", .HeaderText = "Returns", .DefaultCellStyle = New DataGridViewCellStyle With { .Format = "C2" } })
            dgvDailyBreakdown.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "NetSales", .HeaderText = "Net Sales", .DefaultCellStyle = New DataGridViewCellStyle With { .Format = "C2" } })
            dgvDailyBreakdown.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "TransactionCount", .HeaderText = "Transactions", .DefaultCellStyle = New DataGridViewCellStyle With { .Format = "N0" } })

            dgvPaymentTrends.AutoGenerateColumns = False
            dgvPaymentTrends.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "PeriodLabel", .HeaderText = "Month", .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill })
            dgvPaymentTrends.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "CashSales", .HeaderText = "Cash Sales", .DefaultCellStyle = New DataGridViewCellStyle With { .Format = "C2" } })
            dgvPaymentTrends.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "CreditSales", .HeaderText = "Credit Sales", .DefaultCellStyle = New DataGridViewCellStyle With { .Format = "C2" } })
            dgvPaymentTrends.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "OtherSales", .HeaderText = "Other Sales", .DefaultCellStyle = New DataGridViewCellStyle With { .Format = "C2" } })
            dgvPaymentTrends.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "CreditPercentage", .HeaderText = "Credit %", .DefaultCellStyle = New DataGridViewCellStyle With { .Format = "N1" } })
        End Sub

        Private Sub SafeInvoke(action As Action)
            If InvokeRequired Then
                Invoke(action)
            Else
                action()
            End If
        End Sub

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            RaiseEvent ViewLoaded(Me, EventArgs.Empty)
            _isInitializing = False
        End Sub

        Private Sub Selector_Changed(sender As Object, e As EventArgs)
            If _isInitializing Then Return
            RaiseEvent PeriodChanged(Me, EventArgs.Empty)
        End Sub

        Private Sub Refresh_Clicked(sender As Object, e As EventArgs)
            RaiseEvent RefreshRequested(Me, EventArgs.Empty)
        End Sub

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property PeriodType As SalesSummaryPeriodType Implements ISalesSummaryView.PeriodType
            Get
                If rbDaily.Checked Then Return SalesSummaryPeriodType.Daily
                If rbWeekly.Checked Then Return SalesSummaryPeriodType.Weekly
                Return SalesSummaryPeriodType.Monthly
            End Get
            Set(value As SalesSummaryPeriodType)
                SafeInvoke(Sub()
                               Select Case value
                                   Case SalesSummaryPeriodType.Daily
                                       rbDaily.Checked = True
                                   Case SalesSummaryPeriodType.Weekly
                                       rbWeekly.Checked = True
                                   Case SalesSummaryPeriodType.Monthly
                                       rbMonthly.Checked = True
                               End Select
                           End Sub)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SelectedDate As DateTime Implements ISalesSummaryView.SelectedDate
            Get
                Return dtpDate.Value
            End Get
            Set(value As DateTime)
                SafeInvoke(Sub() dtpDate.Value = value)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SelectedYear As Integer Implements ISalesSummaryView.SelectedYear
            Get
                Return If(cboYear.SelectedItem IsNot Nothing, CInt(cboYear.SelectedItem), DateTime.Today.Year)
            End Get
            Set(value As Integer)
                SafeInvoke(Sub() cboYear.SelectedItem = value)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SelectedMonth As Integer Implements ISalesSummaryView.SelectedMonth
            Get
                Return If(cboMonth.SelectedItem IsNot Nothing, CInt(cboMonth.SelectedItem), DateTime.Today.Month)
            End Get
            Set(value As Integer)
                SafeInvoke(Sub() cboMonth.SelectedItem = value)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property PeriodLabel As String Implements ISalesSummaryView.PeriodLabel
            Get
                Return lblTitle.Text
            End Get
            Set(value As String)
                SafeInvoke(Sub() lblTitle.Text = $"Sales Summary - {value}")
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property TotalGrossSalesDisplay As String Implements ISalesSummaryView.TotalGrossSalesDisplay
            Get
                Return lblTotalGrossSales.Text
            End Get
            Set(value As String)
                SafeInvoke(Sub() lblTotalGrossSales.Text = $"Total Gross Sales: {value}")
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property TotalNetSalesDisplay As String Implements ISalesSummaryView.TotalNetSalesDisplay
            Get
                Return lblNetSalesValue.Text
            End Get
            Set(value As String)
                SafeInvoke(Sub() lblNetSalesValue.Text = value)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property TotalReturnsDisplay As String Implements ISalesSummaryView.TotalReturnsDisplay
            Get
                Return lblReturnsValue.Text
            End Get
            Set(value As String)
                SafeInvoke(Sub() lblReturnsValue.Text = value)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property TransactionCountDisplay As String Implements ISalesSummaryView.TransactionCountDisplay
            Get
                Return lblTransactionsValue.Text
            End Get
            Set(value As String)
                SafeInvoke(Sub()
                               lblTransactionsValue.Text = value
                               lblTotalTransactions.Text = $"Total Transactions: {value}"
                           End Sub)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property AvgTransactionValueDisplay As String Implements ISalesSummaryView.AvgTransactionValueDisplay
            Get
                Return lblAvgTransactionValue.Text
            End Get
            Set(value As String)
                SafeInvoke(Sub() lblAvgTransactionValue.Text = value)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property CreditSalesPercentageDisplay As String Implements ISalesSummaryView.CreditSalesPercentageDisplay
            Get
                Return String.Empty
            End Get
            Set(value As String)
                ' We're just returning it, we use it internally if we want
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property HasCreditWarning As Boolean Implements ISalesSummaryView.HasCreditWarning
            Get
                Return lblWarning.Visible
            End Get
            Set(value As Boolean)
                SafeInvoke(Sub()
                               lblWarning.Visible = value
                               If value Then
                                   pnlWhatThisMeans.BackColor = Color.FromArgb(254, 249, 231)
                               Else
                                   pnlWhatThisMeans.BackColor = Color.FromArgb(235, 245, 251)
                               End If
                           End Sub)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property WhatThisMeansText As String Implements ISalesSummaryView.WhatThisMeansText
            Get
                Return lblWhatThisMeans.Text
            End Get
            Set(value As String)
                SafeInvoke(Sub() lblWhatThisMeans.Text = value)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property IsLoading As Boolean Implements ISalesSummaryView.IsLoading
            Get
                Return lblLoading.Visible
            End Get
            Set(value As Boolean)
                SafeInvoke(Sub()
                               lblLoading.Visible = value
                               btnRefresh.Enabled = Not value
                           End Sub)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ErrorMessage As String Implements ISalesSummaryView.ErrorMessage
            Get
                Return String.Empty
            End Get
            Set(value As String)
                ' Handled by notification service
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ShowDatePicker As Boolean Implements ISalesSummaryView.ShowDatePicker
            Get
                Return dtpDate.Visible
            End Get
            Set(value As Boolean)
                SafeInvoke(Sub() dtpDate.Visible = value)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ShowMonthSelectors As Boolean Implements ISalesSummaryView.ShowMonthSelectors
            Get
                Return cboYear.Visible AndAlso cboMonth.Visible
            End Get
            Set(value As Boolean)
                SafeInvoke(Sub()
                               cboYear.Visible = value
                               cboMonth.Visible = value
                           End Sub)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ShowDailyBreakdown As Boolean Implements ISalesSummaryView.ShowDailyBreakdown
            Get
                Return tabControl1.TabPages.Contains(tabPage2)
            End Get
            Set(value As Boolean)
                SafeInvoke(Sub()
                               If value AndAlso Not tabControl1.TabPages.Contains(tabPage2) Then
                                   tabControl1.TabPages.Insert(1, tabPage2)
                               ElseIf Not value AndAlso tabControl1.TabPages.Contains(tabPage2) Then
                                   tabControl1.TabPages.Remove(tabPage2)
                               End If
                           End Sub)
            End Set
        End Property

        Public Sub SetAvailableYears(years As IEnumerable(Of Integer)) Implements ISalesSummaryView.SetAvailableYears
            SafeInvoke(Sub()
                           cboYear.DataSource = years.ToList()
                       End Sub)
        End Sub

        Public Sub SetAvailableMonths(months As IEnumerable(Of Integer)) Implements ISalesSummaryView.SetAvailableMonths
            SafeInvoke(Sub()
                           cboMonth.DataSource = months.ToList()
                       End Sub)
        End Sub

        Public Sub SetPaymentBreakdowns(breakdowns As IEnumerable(Of PaymentBreakdownDto)) Implements ISalesSummaryView.SetPaymentBreakdowns
            SafeInvoke(Sub()
                           dgvPaymentBreakdowns.DataSource = breakdowns.ToList()
                       End Sub)
        End Sub

        Public Sub SetDailyBreakdowns(breakdowns As IEnumerable(Of DailySalesDto)) Implements ISalesSummaryView.SetDailyBreakdowns
            SafeInvoke(Sub()
                           dgvDailyBreakdown.DataSource = breakdowns.ToList()
                       End Sub)
        End Sub

        Public Sub SetPaymentTrends(trends As IEnumerable(Of PaymentTrendDto)) Implements ISalesSummaryView.SetPaymentTrends
            SafeInvoke(Sub()
                           dgvPaymentTrends.DataSource = trends.ToList()
                       End Sub)
        End Sub

    End Class

End Namespace
