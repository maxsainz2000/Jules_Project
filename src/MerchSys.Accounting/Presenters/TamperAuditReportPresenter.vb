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

        Public Sub New(queryService As ITamperAuditQueryService, Optional notificationService As INotificationService = Nothing)
            _queryService = queryService
            _notificationService = notificationService
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
                End If

                _view = value

                If _view IsNot Nothing Then
                    _view.FromDate = DateTime.Today.AddDays(-30)
                    _view.ToDate = DateTime.Today

                    AddHandler _view.ViewLoaded, AddressOf OnViewLoaded
                    AddHandler _view.RefreshRequested, AddressOf OnRefreshRequested
                    AddHandler _view.FilterChanged, AddressOf OnFilterChanged
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

                _view.SetIncidents(dtos)
            End If

            _view.IsLoading = False
        End Function

        Private Function TruncateHash(hash As String) As String
            If String.IsNullOrEmpty(hash) Then Return String.Empty
            If hash.Length > 12 Then
                Return hash.Substring(0, 12) & "..."
            End If
            Return hash
        End Function
    End Class

End Namespace
