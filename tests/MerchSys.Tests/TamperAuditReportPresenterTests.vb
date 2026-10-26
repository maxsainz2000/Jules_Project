Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks
Imports MerchSys.Accounting.Presenters
Imports MerchSys.Accounting.Services
Imports MerchSys.Accounting.Views
Imports MerchSys.SharedKernel.Interfaces
Imports Moq
Imports Xunit

Public Class TamperAuditReportPresenterTests

    Private ReadOnly _viewMock As Mock(Of ITamperAuditReportView)
    Private ReadOnly _queryServiceMock As Mock(Of ITamperAuditQueryService)
    Private ReadOnly _notificationServiceMock As Mock(Of INotificationService)

    Public Sub New()
        _viewMock = New Mock(Of ITamperAuditReportView)()
        _queryServiceMock = New Mock(Of ITamperAuditQueryService)()
        _notificationServiceMock = New Mock(Of INotificationService)()

        _viewMock.SetupProperty(Function(v) v.FromDate)
        _viewMock.SetupProperty(Function(v) v.ToDate)
        _viewMock.SetupProperty(Function(v) v.HasNoEntries)
        _viewMock.SetupProperty(Function(v) v.IsLoading)
        _viewMock.SetupProperty(Function(v) v.ErrorMessage)
    End Sub

    <Fact>
    Public Sub Constructor_SetsDefaultDateRange()
        Dim presenter = New TamperAuditReportPresenter(_queryServiceMock.Object, _notificationServiceMock.Object)
        presenter.View = _viewMock.Object
        Assert.Equal(DateTime.Today.AddDays(-30), _viewMock.Object.FromDate)
        Assert.Equal(DateTime.Today, _viewMock.Object.ToDate)
    End Sub

    <Fact>
    Public Async Function OnViewLoaded_LoadsIncidentsAndMapsToDtos() As Task
        Dim incidents = New List(Of Accounting.Entities.TamperAuditEntry) From {
            New Accounting.Entities.TamperAuditEntry With {
                .DetectedAt = DateTime.Today,
                .ReceiptNumber = "R-001",
                .ExpectedHash = "123456789012345",
                .ActualHash = "098765432109876",
                .TamperKind = "Critical",
                .MachineName = "POS-1",
                .WindowsUser = "Manager"
            }
        }
        _queryServiceMock.Setup(Function(q) q.GetIncidentsAsync(It.IsAny(Of Integer)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(incidents)

        Dim presenter = New TamperAuditReportPresenter(_queryServiceMock.Object, _notificationServiceMock.Object)
        presenter.View = _viewMock.Object

        _viewMock.Raise(Sub(v) AddHandler v.ViewLoaded, Nothing, Nothing, EventArgs.Empty)
        Await Task.Yield() ' Allow async handler to run

        _viewMock.Verify(Sub(v) v.SetIncidents(It.Is(Of IEnumerable(Of TamperAuditEntryDto))(Function(dtos) dtos.Count() = 1 AndAlso dtos.First().ExpectedHashShort = "123456789012..." AndAlso dtos.First().ActualHashShort = "098765432109..." AndAlso dtos.First().Details.Contains("POS-1") AndAlso dtos.First().Details.Contains("Manager"))), Times.Once)

        Assert.False(_viewMock.Object.HasNoEntries)
    End Function

    <Fact>
    Public Async Function OnFilterChanged_FiltersByDateAndSetsEmptyState() As Task
        Dim incidents = New List(Of Accounting.Entities.TamperAuditEntry) From {
            New Accounting.Entities.TamperAuditEntry With {
                .DetectedAt = DateTime.Today.AddDays(-10),
                .ReceiptNumber = "R-001"
            },
            New Accounting.Entities.TamperAuditEntry With {
                .DetectedAt = DateTime.Today.AddDays(-40),
                .ReceiptNumber = "R-002"
            }
        }
        _queryServiceMock.Setup(Function(q) q.GetIncidentsAsync(It.IsAny(Of Integer)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(incidents)

        Dim presenter = New TamperAuditReportPresenter(_queryServiceMock.Object, _notificationServiceMock.Object)
        presenter.View = _viewMock.Object

        _viewMock.Object.FromDate = DateTime.Today.AddDays(-30)
        _viewMock.Object.ToDate = DateTime.Today

        _viewMock.Raise(Sub(v) AddHandler v.FilterChanged, Nothing, Nothing, EventArgs.Empty)
        Await Task.Yield() ' Allow async handler to run

        _viewMock.Verify(Sub(v) v.SetIncidents(It.Is(Of IEnumerable(Of TamperAuditEntryDto))(Function(dtos) dtos.Count() = 1 AndAlso dtos.First().ReceiptNumber = "R-001")), Times.Once)

        Assert.False(_viewMock.Object.HasNoEntries)
    End Function

    <Fact>
    Public Async Function OnRefreshRequested_WithNoResults_SetsHasNoEntries() As Task
        Dim incidents = New List(Of Accounting.Entities.TamperAuditEntry)()
        _queryServiceMock.Setup(Function(q) q.GetIncidentsAsync(It.IsAny(Of Integer)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(incidents)

        Dim presenter = New TamperAuditReportPresenter(_queryServiceMock.Object, _notificationServiceMock.Object)
        presenter.View = _viewMock.Object

        _viewMock.Raise(Sub(v) AddHandler v.RefreshRequested, Nothing, Nothing, EventArgs.Empty)
        Await Task.Yield() ' Allow async handler to run

        Assert.True(_viewMock.Object.HasNoEntries)
        _viewMock.Verify(Sub(v) v.SetIncidents(It.Is(Of IEnumerable(Of TamperAuditEntryDto))(Function(dtos) Not dtos.Any())), Times.Once)
    End Function

    <Fact>
    Public Async Function OnViewLoaded_QueryFails_ShowsError() As Task
        _queryServiceMock.Setup(Function(q) q.GetIncidentsAsync(It.IsAny(Of Integer)(), It.IsAny(Of CancellationToken)())).ThrowsAsync(New Exception("DB Error"))

        Dim presenter = New TamperAuditReportPresenter(_queryServiceMock.Object, _notificationServiceMock.Object)
        presenter.View = _viewMock.Object

        _viewMock.Raise(Sub(v) AddHandler v.ViewLoaded, Nothing, Nothing, EventArgs.Empty)
        Await Task.Yield() ' Allow async handler to run

        Assert.Equal("Failed to load tamper audit incidents.", _viewMock.Object.ErrorMessage)
        _notificationServiceMock.Verify(Sub(n) n.ShowError("Failed to load tamper audit incidents."), Times.Once)
    End Function

End Class
