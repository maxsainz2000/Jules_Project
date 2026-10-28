Imports System.Windows.Forms
Imports System.IO
Imports MerchSys.Accounting.Entities
Imports MerchSys.Accounting.Enums
Imports MerchSys.Accounting.Views
Imports MerchSys.Accounting.Presenters
Imports System.ComponentModel

Namespace Views.Accounting
    Partial Public Class VatReturnView
        Inherits UserControl
        Implements IVatReturnView

        Private ReadOnly _presenter As VatReturnPresenter

        Public Sub New()
            InitializeComponent()
        End Sub

        Public Sub New(presenter As VatReturnPresenter)
            InitializeComponent()
            _presenter = presenter
            If _presenter IsNot Nothing Then
                _presenter.View = Me
            End If

            AddHandler Me.Load, Sub(s, e) RaiseEvent ViewLoaded(Me, EventArgs.Empty)
        End Sub

        ' IVatReturnView implementation
        
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SelectedYear As Integer Implements IVatReturnView.SelectedYear
            Get
                Return If(cmbYear.SelectedItem IsNot Nothing, CInt(cmbYear.SelectedItem), DateTime.UtcNow.Year)
            End Get
            Set(value As Integer)
                If cmbYear.Items.Contains(value) Then cmbYear.SelectedItem = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SelectedPeriod As Integer Implements IVatReturnView.SelectedPeriod
            Get
                Return If(cmbPeriod.SelectedItem IsNot Nothing, CInt(cmbPeriod.SelectedItem), 1)
            End Get
            Set(value As Integer)
                If cmbPeriod.Items.Contains(value) Then cmbPeriod.SelectedItem = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SelectedFormType As VatReturnFormType Implements IVatReturnView.SelectedFormType
            Get
                If rb2550M.Checked Then Return VatReturnFormType.Form2550M
                If rb2550Q.Checked Then Return VatReturnFormType.Form2550Q
                Return VatReturnFormType.Form2551Q
            End Get
            Set(value As VatReturnFormType)
                rb2550M.Checked = (value = VatReturnFormType.Form2550M)
                rb2550Q.Checked = (value = VatReturnFormType.Form2550Q)
                rb2551Q.Checked = (value = VatReturnFormType.Form2551Q)
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property TotalSalesDisplay As String Implements IVatReturnView.TotalSalesDisplay
            Get
                Return lblTotalSales.Text
            End Get
            Set(value As String)
                lblTotalSales.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property TotalPurchasesDisplay As String Implements IVatReturnView.TotalPurchasesDisplay
            Get
                Return lblTotalPurchases.Text
            End Get
            Set(value As String)
                lblTotalPurchases.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property OutputVatDisplay As String Implements IVatReturnView.OutputVatDisplay
            Get
                Return lblOutputVat.Text
            End Get
            Set(value As String)
                lblOutputVat.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property InputVatDisplay As String Implements IVatReturnView.InputVatDisplay
            Get
                Return lblInputVat.Text
            End Get
            Set(value As String)
                lblInputVat.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property NetVatPayableDisplay As String Implements IVatReturnView.NetVatPayableDisplay
            Get
                Return lblNetVatPayable.Text
            End Get
            Set(value As String)
                lblNetVatPayable.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property FilingStatusText As String Implements IVatReturnView.FilingStatusText
            Get
                Return lblFilingStatus.Text
            End Get
            Set(value As String)
                lblFilingStatus.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property WhatThisMeansText As String Implements IVatReturnView.WhatThisMeansText
            Get
                Return txtWhatThisMeans.Text
            End Get
            Set(value As String)
                txtWhatThisMeans.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property CanFile As Boolean Implements IVatReturnView.CanFile
            Get
                Return btnFile.Enabled
            End Get
            Set(value As Boolean)
                btnFile.Enabled = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property CanAmend As Boolean Implements IVatReturnView.CanAmend
            Get
                Return btnAmend.Enabled
            End Get
            Set(value As Boolean)
                btnAmend.Enabled = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property CanExport As Boolean Implements IVatReturnView.CanExport
            Get
                Return btnExportCsv.Enabled AndAlso btnExportPdf.Enabled
            End Get
            Set(value As Boolean)
                btnExportCsv.Enabled = value
                btnExportPdf.Enabled = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property IsLoading As Boolean Implements IVatReturnView.IsLoading
            Get
                Return Not Me.Enabled
            End Get
            Set(value As Boolean)
                Me.Enabled = Not value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ErrorMessage As String Implements IVatReturnView.ErrorMessage
            Get
                Return pnlErrorState.Message
            End Get
            Set(value As String)
                If Me.InvokeRequired Then
                    Me.Invoke(Sub() ErrorMessage = value)
                Else
                    If String.IsNullOrWhiteSpace(value) Then
                        pnlErrorState.Visible = False
                    Else
                        pnlErrorState.Message = value
                        pnlErrorState.Visible = True
                        pnlErrorState.BringToFront()
                    End If
                End If
            End Set
        End Property

        Public Sub SetAvailableYears(years As IEnumerable(Of Integer)) Implements IVatReturnView.SetAvailableYears
            cmbYear.DataSource = years.ToList()
        End Sub

        Public Sub SetAvailablePeriods(periods As IEnumerable(Of Integer)) Implements IVatReturnView.SetAvailablePeriods
            cmbPeriod.DataSource = periods.ToList()
        End Sub

        Public Sub SetVatReturnLines(lines As IEnumerable(Of VatReturnLine)) Implements IVatReturnView.SetVatReturnLines
            dgvLines.DataSource = lines.ToList()
        End Sub

        Public Sub SetVatReturnHistory(returns As IEnumerable(Of VatReturn)) Implements IVatReturnView.SetVatReturnHistory
            dgvHistory.DataSource = returns.ToList()
        End Sub

        Public Event PeriodChanged As EventHandler Implements IVatReturnView.PeriodChanged
        Public Event FormTypeChanged As EventHandler Implements IVatReturnView.FormTypeChanged
        Public Event GenerateRequested As EventHandler Implements IVatReturnView.GenerateRequested
        Public Event FileRequested As EventHandler Implements IVatReturnView.FileRequested
        Public Event AmendRequested As EventHandler Implements IVatReturnView.AmendRequested
        Public Event ExportCsvRequested As EventHandler Implements IVatReturnView.ExportCsvRequested
        Public Event ExportPdfRequested As EventHandler Implements IVatReturnView.ExportPdfRequested
        Public Event ReturnSelected As EventHandler(Of Integer) Implements IVatReturnView.ReturnSelected
        Public Event ViewLoaded As EventHandler Implements IVatReturnView.ViewLoaded
        Public Event ExportReady As EventHandler(Of ExportReadyEventArgs) Implements IVatReturnView.ExportReady
        
        Public Sub PromptToSaveExport(fileName As String, fileBytes As Byte()) Implements IVatReturnView.PromptToSaveExport
            Using sfd As New SaveFileDialog()
                sfd.FileName = fileName
                sfd.Filter = "All Files (*.*)|*.*"
                If fileName.EndsWith(".csv") Then
                    sfd.Filter = "CSV Files (*.csv)|*.csv"
                ElseIf fileName.EndsWith(".pdf") Then
                    sfd.Filter = "PDF Files (*.pdf)|*.pdf"
                End If
                
                If sfd.ShowDialog() = DialogResult.OK Then
                    File.WriteAllBytes(sfd.FileName, fileBytes)
                    MessageBox.Show("Export saved successfully.", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Using
        End Sub

        ' Event handlers for UI controls
        Private Sub cmbPeriod_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbPeriod.SelectedIndexChanged
            RaiseEvent PeriodChanged(Me, EventArgs.Empty)
        End Sub

        Private Sub rbFormType_CheckedChanged(sender As Object, e As EventArgs) Handles rb2550M.CheckedChanged, rb2550Q.CheckedChanged, rb2551Q.CheckedChanged
            Dim rb = TryCast(sender, RadioButton)
            If rb IsNot Nothing AndAlso rb.Checked Then
                RaiseEvent FormTypeChanged(Me, EventArgs.Empty)
            End If
        End Sub

        Private Sub btnGenerate_Click(sender As Object, e As EventArgs) Handles btnGenerate.Click
            RaiseEvent GenerateRequested(Me, EventArgs.Empty)
        End Sub

        Private Sub btnFile_Click(sender As Object, e As EventArgs) Handles btnFile.Click
            RaiseEvent FileRequested(Me, EventArgs.Empty)
        End Sub

        Private Sub btnAmend_Click(sender As Object, e As EventArgs) Handles btnAmend.Click
            RaiseEvent AmendRequested(Me, EventArgs.Empty)
        End Sub

        Private Sub btnExportCsv_Click(sender As Object, e As EventArgs) Handles btnExportCsv.Click
            RaiseEvent ExportCsvRequested(Me, EventArgs.Empty)
        End Sub

        Private Sub btnExportPdf_Click(sender As Object, e As EventArgs) Handles btnExportPdf.Click
            RaiseEvent ExportPdfRequested(Me, EventArgs.Empty)
        End Sub

        Private Sub dgvHistory_SelectionChanged(sender As Object, e As EventArgs) Handles dgvHistory.SelectionChanged
            If dgvHistory.SelectedRows.Count > 0 Then
                Dim returnItem = TryCast(dgvHistory.SelectedRows(0).DataBoundItem, VatReturn)
                If returnItem IsNot Nothing Then
                    RaiseEvent ReturnSelected(Me, returnItem.Id)
                End If
            End If
        End Sub
        
    End Class
End Namespace
