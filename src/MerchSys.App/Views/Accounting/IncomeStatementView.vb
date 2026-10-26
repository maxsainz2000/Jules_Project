Imports MerchSys.Accounting.Presenters
Imports MerchSys.Accounting.Views
Imports MerchSys.Accounting.Services
Imports System.ComponentModel

Namespace Views.Accounting

    Partial Public Class IncomeStatementView
        Inherits UserControl
        Implements IIncomeStatementView

        Private _presenter As IncomeStatementPresenter
        Private _isInitializing As Boolean = True

        Public Sub New(presenter As IncomeStatementPresenter)
            InitializeComponent()
            _presenter = presenter
            _presenter.View = Me
        End Sub

        Private Sub IncomeStatementView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            If Not DesignMode Then
                RaiseEvent ViewLoaded(Me, EventArgs.Empty)
                _isInitializing = False
                UpdateSelectorVisibility()
            End If
        End Sub

        Public Event PeriodChanged As EventHandler Implements IIncomeStatementView.PeriodChanged
        Public Event RefreshRequested As EventHandler Implements IIncomeStatementView.RefreshRequested
        Public Event ExportPdfRequested As EventHandler(Of String) Implements IIncomeStatementView.ExportPdfRequested
        Public Event ViewLoaded As EventHandler Implements IIncomeStatementView.ViewLoaded

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property PeriodType As IncomeStatementPeriodType Implements IIncomeStatementView.PeriodType
            Get
                If rbAnnual.Checked Then Return IncomeStatementPeriodType.Annual
                If rbQuarterly.Checked Then Return IncomeStatementPeriodType.Quarterly
                Return IncomeStatementPeriodType.Monthly
            End Get
            Set(value As IncomeStatementPeriodType)
                Select Case value
                    Case IncomeStatementPeriodType.Monthly
                        rbMonthly.Checked = True
                    Case IncomeStatementPeriodType.Quarterly
                        rbQuarterly.Checked = True
                    Case IncomeStatementPeriodType.Annual
                        rbAnnual.Checked = True
                End Select
                UpdateSelectorVisibility()
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SelectedYear As Integer Implements IIncomeStatementView.SelectedYear
            Get
                If cboYear.SelectedItem IsNot Nothing Then
                    Return CInt(cboYear.SelectedItem)
                End If
                Return DateTime.Now.Year
            End Get
            Set(value As Integer)
                cboYear.SelectedItem = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SelectedMonth As Integer Implements IIncomeStatementView.SelectedMonth
            Get
                If cboMonth.SelectedItem IsNot Nothing Then
                    Return CInt(cboMonth.SelectedItem)
                End If
                Return DateTime.Now.Month
            End Get
            Set(value As Integer)
                cboMonth.SelectedItem = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SelectedQuarter As Integer Implements IIncomeStatementView.SelectedQuarter
            Get
                If cboQuarter.SelectedItem IsNot Nothing Then
                    Return CInt(cboQuarter.SelectedItem)
                End If
                Return (DateTime.Now.Month - 1) \ 3 + 1
            End Get
            Set(value As Integer)
                cboQuarter.SelectedItem = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property PeriodLabel As String Implements IIncomeStatementView.PeriodLabel
            Get
                Return lblTitle.Text
            End Get
            Set(value As String)
                SafeInvoke(Sub() lblTitle.Text = $"Income Statement (P&&L) - {value}")
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property NetSalesDisplay As String Implements IIncomeStatementView.NetSalesDisplay
            Get
                Return lblNetSales.Text
            End Get
            Set(value As String)
                SafeInvoke(Sub() lblNetSales.Text = $"Net Sales: {value}")
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property CostOfGoodsSoldDisplay As String Implements IIncomeStatementView.CostOfGoodsSoldDisplay
            Get
                Return lblCostOfGoodsSold.Text
            End Get
            Set(value As String)
                SafeInvoke(Sub() lblCostOfGoodsSold.Text = $"COGS: -{value}")
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property GrossProfitDisplay As String Implements IIncomeStatementView.GrossProfitDisplay
            Get
                Return lblGrossProfit.Text
            End Get
            Set(value As String)
                SafeInvoke(Sub() lblGrossProfit.Text = $"Gross Profit: {value}")
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property GrossMarginPercentDisplay As String Implements IIncomeStatementView.GrossMarginPercentDisplay
            Get
                Return lblGrossMarginPercent.Text
            End Get
            Set(value As String)
                SafeInvoke(Sub() lblGrossMarginPercent.Text = $"Gross Margin: {value}")
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property OperatingExpensesDisplay As String Implements IIncomeStatementView.OperatingExpensesDisplay
            Get
                Return lblOperatingExpenses.Text
            End Get
            Set(value As String)
                SafeInvoke(Sub() lblOperatingExpenses.Text = $"Operating Expenses: -{value}")
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property NetIncomeDisplay As String Implements IIncomeStatementView.NetIncomeDisplay
            Get
                Return lblNetIncome.Text
            End Get
            Set(value As String)
                SafeInvoke(Sub() lblNetIncome.Text = $"Net Income: {value}")
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property NetMarginPercentDisplay As String Implements IIncomeStatementView.NetMarginPercentDisplay
            Get
                Return lblNetMarginPercent.Text
            End Get
            Set(value As String)
                SafeInvoke(Sub() lblNetMarginPercent.Text = $"Net Margin: {value}")
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property WhatThisMeansText As String Implements IIncomeStatementView.WhatThisMeansText
            Get
                Return lblWhatThisMeans.Text
            End Get
            Set(value As String)
                SafeInvoke(Sub() lblWhatThisMeans.Text = value)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property IsLoading As Boolean Implements IIncomeStatementView.IsLoading
            Get
                Return lblLoading.Visible
            End Get
            Set(value As Boolean)
                SafeInvoke(Sub()
                               lblLoading.Visible = value
                               btnRefresh.Enabled = Not value
                               cboYear.Enabled = Not value
                               cboMonth.Enabled = Not value
                               cboQuarter.Enabled = Not value
                           End Sub)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ErrorMessage As String Implements IIncomeStatementView.ErrorMessage
            Get
                Return String.Empty
            End Get
            Set(value As String)
                ' Handled by presenter notification service
            End Set
        End Property

        Public Sub SetAvailableYears(years As IEnumerable(Of Integer)) Implements IIncomeStatementView.SetAvailableYears
            SafeInvoke(Sub()
                           cboYear.DataSource = years.ToList()
                       End Sub)
        End Sub

        Public Sub SetAvailableMonths(months As IEnumerable(Of Integer)) Implements IIncomeStatementView.SetAvailableMonths
            SafeInvoke(Sub()
                           cboMonth.DataSource = months.ToList()
                       End Sub)
        End Sub

        Public Sub SetAvailableQuarters(quarters As IEnumerable(Of Integer)) Implements IIncomeStatementView.SetAvailableQuarters
            SafeInvoke(Sub()
                           cboQuarter.DataSource = quarters.ToList()
                       End Sub)
        End Sub

        Public Sub SetProductMargins(margins As IEnumerable(Of ProductMarginDto)) Implements IIncomeStatementView.SetProductMargins
            SafeInvoke(Sub()
                           dgvProductMargins.Rows.Clear()
                           For Each m In margins
                               dgvProductMargins.Rows.Add(
                                   m.ProductName,
                                   m.QuantitySold.ToString("N0"),
                                   $"₱{m.Revenue:N2}",
                                   $"₱{m.Cogs:N2}",
                                   $"₱{m.GrossProfit:N2}",
                                   $"{m.GrossMarginPercent:N1}%"
                               )
                           Next
                       End Sub)
        End Sub

        Public Sub SetOperatingExpensesBreakdown(breakdown As IDictionary(Of String, Decimal)) Implements IIncomeStatementView.SetOperatingExpensesBreakdown
            SafeInvoke(Sub()
                           dgvOperatingExpenses.Rows.Clear()
                           For Each kvp In breakdown
                               dgvOperatingExpenses.Rows.Add(
                                   kvp.Key,
                                   $"₱{kvp.Value:N2}"
                               )
                           Next
                       End Sub)
        End Sub

        Private Sub RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles rbMonthly.CheckedChanged, rbQuarterly.CheckedChanged, rbAnnual.CheckedChanged
            If _isInitializing Then Return
            UpdateSelectorVisibility()
            RaiseEvent PeriodChanged(Me, EventArgs.Empty)
        End Sub

        Private Sub ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboYear.SelectedIndexChanged, cboMonth.SelectedIndexChanged, cboQuarter.SelectedIndexChanged
            If _isInitializing Then Return
            RaiseEvent PeriodChanged(Me, EventArgs.Empty)
        End Sub

        Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
            RaiseEvent RefreshRequested(Me, EventArgs.Empty)
        End Sub

        Private Sub UpdateSelectorVisibility()
            cboMonth.Visible = rbMonthly.Checked
            cboQuarter.Visible = rbQuarterly.Checked
        End Sub

        Private Sub SafeInvoke(action As Action)
            If InvokeRequired Then
                Invoke(action)
            Else
                action()
            End If
        End Sub


        Private Sub btnExportPdf_Click(sender As Object, e As EventArgs) Handles btnExportPdf.Click
            Using dialog As New SaveFileDialog()
                dialog.Filter = "PDF files (*.pdf)|*.pdf"
                dialog.FileName = $"IncomeStatement_{DateTime.Now:yyyyMMdd}.pdf"
                If dialog.ShowDialog() = DialogResult.OK Then
                    RaiseEvent ExportPdfRequested(Me, dialog.FileName)
                End If
            End Using
        End Sub
    End Class

End Namespace
