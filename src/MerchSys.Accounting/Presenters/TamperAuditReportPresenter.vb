Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports MerchSys.Accounting.Services
Imports MerchSys.Accounting.Views
Imports MerchSys.SharedKernel.Interfaces

Namespace Presenters

    Public Class TamperAuditEntryDto
        Public Property DetectedAt As DateTime
        Public Property ReceiptNumber As String
        Public Property ExpectedHash As String
        Public Property ExpectedHashShort As String
        Public Property ActualHash As String
        Public Property ActualHashShort As String
        Public Property TamperKind As String
        Public Property Details As String
    End Class

    Public Class TamperAuditReportPresenter
        Private _view As ITamperAuditReportView
        Private ReadOnly _queryService As ITamperAuditQueryService
        Private ReadOnly _notificationService As INotificationService
        Private ReadOnly _exporter As ITamperReportExporter
        Private ReadOnly _mediator As MediatR.IMediator

        Private _currentDtos As List(Of TamperAuditEntryDto)

        Public Sub New(queryService As ITamperAuditQueryService, Optional notificationService As INotificationService = Nothing)
            Me.New(queryService, Nothing, notificationService, Nothing)
        End Sub

        Public Sub New(queryService As ITamperAuditQueryService, exporter As ITamperReportExporter, Optional notificationService As INotificationService = Nothing, Optional mediator As MediatR.IMediator = Nothing)
            _queryService = queryService
            _exporter = exporter
            _notificationService = notificationService
            _mediator = mediator
        End Sub

        Public Property View As ITamperAuditReportView
            Get
                Return _view
            End Get
            Set(value As ITamperAuditReportView)
                If _view IsNot Nothing Then
                    RemoveHandler _view.ViewLoaded, AddressOf OnViewLoaded
                    RemoveHandler _view.RefreshRequested, AddressOf OnRefreshRequested
                    RemoveHandler _view.FilterChanged, AddressOf OnFilterChanged
                    RemoveHandler _view.ExportCsvRequested, AddressOf OnExportCsvRequested
                    RemoveHandler _view.ExportPdfRequested, AddressOf OnExportPdfRequested
                End If

                _view = value

                If _view IsNot Nothing Then
                    _view.FromDate = DateTime.Today.AddDays(-30)
                    _view.ToDate = DateTime.Today

                    AddHandler _view.ViewLoaded, AddressOf OnViewLoaded
                    AddHandler _view.RefreshRequested, AddressOf OnRefreshRequested
                    AddHandler _view.FilterChanged, AddressOf OnFilterChanged
                    AddHandler _view.ExportCsvRequested, AddressOf OnExportCsvRequested
                    AddHandler _view.ExportPdfRequested, AddressOf OnExportPdfRequested
                End If
            End Set
        End Property

        Private Async Sub OnViewLoaded(sender As Object, e As EventArgs)
            Await LoadIncidentsAsync()
        End Sub

        Private Async Sub OnRefreshRequested(sender As Object, e As EventArgs)
            Await LoadIncidentsAsync()
        End Sub

        Private Async Sub OnFilterChanged(sender As Object, e As EventArgs)
            Await LoadIncidentsAsync()
        End Sub

        Private Async Function LoadIncidentsAsync() As Task
            _view.IsLoading = True
            _view.ErrorMessage = String.Empty

            Dim incidents As List(Of Entities.TamperAuditEntry) = Nothing
            Dim errorMsg As String = Nothing

            Try
                incidents = Await _queryService.GetIncidentsAsync()
            Catch ex As Exception
                errorMsg = "Failed to load tamper audit incidents."
                If _notificationService IsNot Nothing Then
                    _notificationService.ShowError(errorMsg)
                End If
            End Try

            If errorMsg IsNot Nothing Then
                _view.ErrorMessage = errorMsg
                _view.IsLoading = False
                Return
            End If

            If incidents IsNot Nothing Then
                Dim filtered = incidents.Where(Function(x) x.DetectedAt.Date >= _view.FromDate.Date AndAlso x.DetectedAt.Date <= _view.ToDate.Date).ToList()
                _view.HasNoEntries = (filtered.Count = 0)

                Dim dtos = filtered.Select(Function(x) New TamperAuditEntryDto With {
                    .DetectedAt = x.DetectedAt,
                    .ReceiptNumber = x.ReceiptNumber,
                    .ExpectedHash = x.ExpectedHash,
                    .ExpectedHashShort = TruncateHash(x.ExpectedHash),
                    .ActualHash = x.ActualHash,
                    .ActualHashShort = TruncateHash(x.ActualHash),
                    .TamperKind = x.TamperKind,
                    .Details = $"Detected on {x.MachineName} by {x.WindowsUser}"
                }).ToList()

                _currentDtos = dtos
                _view.SetIncidents(dtos)
            End If

            _view.IsLoading = False
        End Function

        Private Async Sub OnExportCsvRequested(sender As Object, filePath As String)
            If _exporter Is Nothing Then
                _notificationService?.ShowError("Exporter is not configured.")
                Return
            End If

            Dim success = False
            Try
                Dim options = New TamperReportExportOptions With {
                    .BusinessName = "My Business",
                    .Title = "Tamper Audit Report",
                    .FromDate = _view.FromDate,
                    .ToDate = _view.ToDate
                }
                Dim bytes = Await _exporter.ExportAsync(_currentDtos, TamperReportFormat.Csv, options)
                Await System.IO.File.WriteAllBytesAsync(filePath, bytes)
                success = True
            Catch ex As Exception
                _notificationService?.ShowError($"Failed to export CSV: {ex.Message}")
            End Try

            If success Then
                _notificationService?.ShowInfo("CSV exported successfully.")
            End If
        End Sub

        Private Async Sub OnExportPdfRequested(sender As Object, filePath As String)
            If _exporter Is Nothing Then
                _notificationService?.ShowError("Exporter is not configured.")
                Return
            End If

            Dim success = False
            Try
                Dim options = New TamperReportExportOptions With {
                    .BusinessName = "My Business",
                    .Title = "Tamper Audit Report",
                    .FromDate = _view.FromDate,
                    .ToDate = _view.ToDate
                }
                Dim bytes = Await _exporter.ExportAsync(_currentDtos, TamperReportFormat.Pdf, options)
                Await System.IO.File.WriteAllBytesAsync(filePath, bytes)
                success = True
            Catch ex As Exception
                _notificationService?.ShowError($"Failed to export PDF: {ex.Message}")
            End Try

            If success Then
                _notificationService?.ShowInfo("PDF exported successfully.")
            End If
        End Sub

        Private Function TruncateHash(hash As String) As String
            If String.IsNullOrEmpty(hash) Then Return String.Empty
            If hash.Length > 12 Then
                Return hash.Substring(0, 12) & "..."
            End If
            Return hash
        End Function
    End Class

End Namespace
