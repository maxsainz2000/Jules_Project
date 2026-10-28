Imports System.Drawing
Imports System.Windows.Forms
Imports System.ComponentModel
Imports MerchSys.Accounting.Views
Imports MerchSys.Accounting.Presenters
Imports MerchSys.Accounting.Services
Imports System.Globalization

Namespace Views.Accounting

    Public Partial Class VatReliefReportView
        Implements IVatReliefReportView

        Private ReadOnly _presenter As VatReliefReportPresenter
        Private _isInitializing As Boolean = True

        Public Sub New()
            InitializeComponent()
        End Sub

        Public Sub New(presenter As VatReliefReportPresenter)
            InitializeComponent()

            _presenter = presenter
            If _presenter IsNot Nothing Then
                _presenter.View = Me
            End If

            ConfigureGrid()
            AddHandler pnlErrorState.RetryClicked, Sub(sender, e) RaiseEvent RefreshRequested(Me, EventArgs.Empty)
        End Sub

        Private Sub ConfigureGrid()
            dgvTrailingMonths.AutoGenerateColumns = False
            dgvTrailingMonths.Columns.Clear()

            dgvTrailingMonths.Columns.Add(New DataGridViewTextBoxColumn() With {
                .Name = "PeriodLabel",
                .DataPropertyName = "PeriodLabel",
                .HeaderText = "Period"
            })
            dgvTrailingMonths.Columns.Add(New DataGridViewTextBoxColumn() With {
                .Name = "TotalSales",
                .DataPropertyName = "TotalSales",
                .HeaderText = "Total Sales",
                .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "C2"}
            })
            dgvTrailingMonths.Columns.Add(New DataGridViewTextBoxColumn() With {
                .Name = "OutputVat",
                .DataPropertyName = "OutputVat",
                .HeaderText = "Output VAT",
                .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "C2"}
            })
            dgvTrailingMonths.Columns.Add(New DataGridViewTextBoxColumn() With {
                .Name = "TotalPurchases",
                .DataPropertyName = "TotalPurchases",
                .HeaderText = "Total Purchases",
                .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "C2"}
            })
            dgvTrailingMonths.Columns.Add(New DataGridViewTextBoxColumn() With {
                .Name = "InputVat",
                .DataPropertyName = "InputVat",
                .HeaderText = "Input VAT",
                .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "C2"}
            })
            dgvTrailingMonths.Columns.Add(New DataGridViewTextBoxColumn() With {
                .Name = "NetVatPayable",
                .DataPropertyName = "NetVatPayable",
                .HeaderText = "Net VAT Payable",
                .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "C2"}
            })
        End Sub

        Private Sub VatReliefReportView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            RaiseEvent ViewLoaded(Me, EventArgs.Empty)
            _isInitializing = False
        End Sub

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SelectedYear As Integer Implements IVatReliefReportView.SelectedYear
            Get
                If cboYear.SelectedItem IsNot Nothing Then
                    Return Convert.ToInt32(cboYear.SelectedItem)
                End If
                Return Date.Now.Year
            End Get
            Set(value As Integer)
                cboYear.SelectedItem = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SelectedMonth As Integer Implements IVatReliefReportView.SelectedMonth
            Get
                If cboMonth.SelectedItem IsNot Nothing Then
                    Return DirectCast(cboMonth.SelectedItem, MonthItem).Value
                End If
                Return Date.Now.Month
            End Get
            Set(value As Integer)
                For Each item As MonthItem In cboMonth.Items
                    If item.Value = value Then
                        cboMonth.SelectedItem = item
                        Exit For
                    End If
                Next
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property NetVatPayableText As String Implements IVatReliefReportView.NetVatPayableText
            Get
                Return lblNetVatPayable.Text
            End Get
            Set(value As String)
                lblNetVatPayable.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property NetVatPayableColor As Color Implements IVatReliefReportView.NetVatPayableColor
            Get
                Return lblNetVatPayable.ForeColor
            End Get
            Set(value As Color)
                lblNetVatPayable.ForeColor = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property WhatThisMeansText As String Implements IVatReliefReportView.WhatThisMeansText
            Get
                Return lblWhatThisMeansText.Text
            End Get
            Set(value As String)
                lblWhatThisMeansText.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property IsLoading As Boolean Implements IVatReliefReportView.IsLoading
            Get
                Return Not Me.Enabled
            End Get
            Set(value As Boolean)
                Me.Enabled = Not value
                If value Then pnlErrorState.Visible = False
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ErrorMessage As String Implements IVatReliefReportView.ErrorMessage
            Get
                Return pnlErrorState.Message
            End Get
            Set(value As String)
                pnlErrorState.Message = value
                pnlErrorState.Visible = Not String.IsNullOrEmpty(value)
                If pnlErrorState.Visible Then pnlErrorState.BringToFront()
            End Set
        End Property

        Public Sub SetAvailableYears(years As IEnumerable(Of Integer)) Implements IVatReliefReportView.SetAvailableYears
            cboYear.DataSource = years.ToList()
        End Sub

        Public Sub SetAvailableMonths(months As IEnumerable(Of Integer)) Implements IVatReliefReportView.SetAvailableMonths
            Dim monthItems = months.Select(Function(m) New MonthItem With {
                .Value = m,
                .DisplayName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m)
            }).ToList()

            cboMonth.DisplayMember = "DisplayName"
            cboMonth.ValueMember = "Value"
            cboMonth.DataSource = monthItems
        End Sub

        Public Sub DisplaySummary(summary As VatReliefSummary) Implements IVatReliefReportView.DisplaySummary
            If summary Is Nothing Then Return

            lblVatableSales.Text = $"Vatable Sales: {summary.VatableSales:C2}"
            lblVatExemptSales.Text = $"VAT-Exempt Sales: {summary.VatExemptSales:C2}"
            lblZeroRatedSales.Text = $"Zero-Rated Sales: {summary.ZeroRatedSales:C2}"
            lblTotalSales.Text = $"Total Sales: {summary.TotalSales:C2}"
            lblOutputVat.Text = $"Output VAT: {summary.OutputVat:C2}"

            lblVatablePurchases.Text = $"Vatable Purchases: {summary.VatablePurchases:C2}"
            lblVatExemptPurchases.Text = $"VAT-Exempt Purchases: {summary.VatExemptPurchases:C2}"
            lblZeroRatedPurchases.Text = $"Zero-Rated Purchases: {summary.ZeroRatedPurchases:C2}"
            lblTotalPurchases.Text = $"Total Purchases: {summary.TotalPurchases:C2}"
            lblInputVat.Text = $"Input VAT: {summary.InputVat:C2}"
        End Sub

        Public Sub DisplayTrailingMonths(summaries As IEnumerable(Of VatReliefSummary)) Implements IVatReliefReportView.DisplayTrailingMonths
            dgvTrailingMonths.DataSource = summaries.ToList()
        End Sub

        Public Event ViewLoaded As EventHandler Implements IVatReliefReportView.ViewLoaded
        Public Event PeriodChanged As EventHandler Implements IVatReliefReportView.PeriodChanged
        Public Event RefreshRequested As EventHandler Implements IVatReliefReportView.RefreshRequested
        Public Event ExportPdfRequested As EventHandler(Of String) Implements IVatReliefReportView.ExportPdfRequested

        Private Sub cboYear_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboYear.SelectedIndexChanged
            If Not _isInitializing Then
                RaiseEvent PeriodChanged(Me, EventArgs.Empty)
            End If
        End Sub

        Private Sub cboMonth_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboMonth.SelectedIndexChanged
            If Not _isInitializing Then
                RaiseEvent PeriodChanged(Me, EventArgs.Empty)
            End If
        End Sub

        Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
            RaiseEvent RefreshRequested(Me, EventArgs.Empty)
        End Sub

        Private Sub btnExportPdf_Click(sender As Object, e As EventArgs) Handles btnExportPdf.Click
            Using dialog As New SaveFileDialog()
                dialog.Filter = "PDF files (*.pdf)|*.pdf"
                dialog.FileName = $"VatReliefReport_{DateTime.Now:yyyyMMdd}.pdf"

                If dialog.ShowDialog() = DialogResult.OK Then
                    RaiseEvent ExportPdfRequested(Me, dialog.FileName)
                End If
            End Using
        End Sub

        Private Class MonthItem
            Public Property Value As Integer
            Public Property DisplayName As String
        End Class

    End Class

End Namespace
