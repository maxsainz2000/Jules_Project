Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports MerchSys.Accounting.Presenters
Imports MerchSys.Accounting.Views

Namespace Views.Accounting

    Partial Public Class TamperAuditReportView
        Inherits UserControl
        Implements ITamperAuditReportView

        Private ReadOnly _presenter As TamperAuditReportPresenter

        Public Event ViewLoaded As EventHandler Implements ITamperAuditReportView.ViewLoaded
        Public Event RefreshRequested As EventHandler Implements ITamperAuditReportView.RefreshRequested
        Public Event FilterChanged As EventHandler Implements ITamperAuditReportView.FilterChanged
        Public Event ExportCsvRequested As EventHandler(Of String) Implements ITamperAuditReportView.ExportCsvRequested
        Public Event ExportPdfRequested As EventHandler(Of String) Implements ITamperAuditReportView.ExportPdfRequested

        Public Sub New(presenter As TamperAuditReportPresenter)
            InitializeComponent()
            _presenter = presenter
            _presenter.View = Me

            ' Wire up event handlers after InitializeComponent
            AddHandler dtpFrom.ValueChanged, AddressOf OnFilterChanged
            AddHandler dtpTo.ValueChanged, AddressOf OnFilterChanged
            AddHandler btnRefresh.Click, AddressOf OnRefreshRequested
            AddHandler btnExportCsv.Click, AddressOf OnExportCsvClicked
            AddHandler btnExportPdf.Click, AddressOf OnExportPdfClicked
            AddHandler dgvIncidents.CellFormatting, AddressOf dgvIncidents_CellFormatting
        End Sub

        Private Sub OnExportCsvClicked(sender As Object, e As EventArgs)
            Using sfd As New SaveFileDialog()
                sfd.Filter = "CSV Files (*.csv)|*.csv"
                sfd.DefaultExt = "csv"
                sfd.FileName = $"tamper-incidents-{DateTime.Now:yyyyMMdd}.csv"
                If sfd.ShowDialog() = DialogResult.OK Then
                    RaiseEvent ExportCsvRequested(Me, sfd.FileName)
                End If
            End Using
        End Sub

        Private Sub OnExportPdfClicked(sender As Object, e As EventArgs)
            Using sfd As New SaveFileDialog()
                sfd.Filter = "PDF Files (*.pdf)|*.pdf"
                sfd.DefaultExt = "pdf"
                sfd.FileName = $"tamper-incidents-{DateTime.Now:yyyyMMdd}.pdf"
                If sfd.ShowDialog() = DialogResult.OK Then
                    RaiseEvent ExportPdfRequested(Me, sfd.FileName)
                End If
            End Using
        End Sub

        Private Sub TamperAuditReportView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            RaiseEvent ViewLoaded(Me, EventArgs.Empty)
        End Sub

        Private Sub OnFilterChanged(sender As Object, e As EventArgs)
            RaiseEvent FilterChanged(Me, EventArgs.Empty)
        End Sub

        Private Sub OnRefreshRequested(sender As Object, e As EventArgs)
            RaiseEvent RefreshRequested(Me, EventArgs.Empty)
        End Sub

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property FromDate As DateTime Implements ITamperAuditReportView.FromDate
            Get
                Return dtpFrom.Value
            End Get
            Set(value As DateTime)
                ' Prevent event firing during initial set
                RemoveHandler dtpFrom.ValueChanged, AddressOf OnFilterChanged
                dtpFrom.Value = value
                AddHandler dtpFrom.ValueChanged, AddressOf OnFilterChanged
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ToDate As DateTime Implements ITamperAuditReportView.ToDate
            Get
                Return dtpTo.Value
            End Get
            Set(value As DateTime)
                ' Prevent event firing during initial set
                RemoveHandler dtpTo.ValueChanged, AddressOf OnFilterChanged
                dtpTo.Value = value
                AddHandler dtpTo.ValueChanged, AddressOf OnFilterChanged
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property HasNoEntries As Boolean Implements ITamperAuditReportView.HasNoEntries
            Get
                Return lblEmptyState.Visible
            End Get
            Set(value As Boolean)
                lblEmptyState.Visible = value
                dgvIncidents.Visible = Not value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property IsLoading As Boolean Implements ITamperAuditReportView.IsLoading
            Get
                Return Not btnRefresh.Enabled
            End Get
            Set(value As Boolean)
                btnRefresh.Enabled = Not value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ErrorMessage As String Implements ITamperAuditReportView.ErrorMessage
            Get
                Return "" ' Implementation depends on notification service, not directly shown on view
            End Get
            Set(value As String)
                ' No-op, presenter uses notification service for errors
            End Set
        End Property

        Public Sub SetIncidents(entries As IEnumerable(Of TamperAuditEntryDto)) Implements ITamperAuditReportView.SetIncidents
            dgvIncidents.DataSource = Nothing
            dgvIncidents.DataSource = entries
        End Sub

        Private Sub dgvIncidents_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
            If e.RowIndex >= 0 AndAlso e.RowIndex < dgvIncidents.Rows.Count Then
                Dim row = dgvIncidents.Rows(e.RowIndex)
                Dim entry = TryCast(row.DataBoundItem, TamperAuditEntryDto)

                If entry IsNot Nothing Then
                    ' Tooltips for hashes
                    If dgvIncidents.Columns(e.ColumnIndex).DataPropertyName = "ExpectedHashShort" Then
                        Dim cell = row.Cells(e.ColumnIndex)
                        cell.ToolTipText = entry.ExpectedHash
                    ElseIf dgvIncidents.Columns(e.ColumnIndex).DataPropertyName = "ActualHashShort" Then
                        Dim cell = row.Cells(e.ColumnIndex)
                        cell.ToolTipText = entry.ActualHash
                    End If

                    ' Styling for TamperKind
                    If entry.TamperKind = "Critical" OrElse entry.TamperKind = "Tampered" Then
                        row.DefaultCellStyle.ForeColor = Color.Red
                        row.DefaultCellStyle.SelectionForeColor = Color.Red
                    End If
                End If
            End If
        End Sub
    End Class

End Namespace
