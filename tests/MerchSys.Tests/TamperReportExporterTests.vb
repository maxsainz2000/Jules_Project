Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports System.Threading.Tasks
Imports MerchSys.Accounting.Presenters
Imports MerchSys.Accounting.Services
Imports MerchSys.Accounting.Views
Imports MerchSys.SharedKernel.Interfaces
Imports Moq
Imports Xunit

Public Class TamperReportExporterTests

    <Fact>
    Public Async Function TamperReportExporter_Csv_EscapesQuotes_AndWrapsValuesInQuotes() As Task
        Dim exporter As New TamperReportExporter()
        Dim incidents = New List(Of TamperAuditEntryDto) From {
            New TamperAuditEntryDto With {
                .DetectedAt = New DateTime(2023, 1, 1, 12, 0, 0),
                .ReceiptNumber = "R-001",
                .ExpectedHash = "abc",
                .ActualHash = "def",
                .TamperKind = "Some "" quote",
                .Details = "Line1" & vbCrLf & "Line2"
            }
        }

        Dim options = New TamperReportExportOptions()
        Dim bytes = Await exporter.ExportAsync(incidents, TamperReportFormat.Csv, options)
        Dim csvText = Encoding.UTF8.GetString(bytes)

        Assert.Contains("""Some """" quote""", csvText)
        Assert.Contains("""Line1", csvText)
    End Function

    <Fact>
    Public Async Function TamperReportExporter_Pdf_GeneratesBytes() As Task
        Dim exporter As New TamperReportExporter()
        Dim incidents = New List(Of TamperAuditEntryDto) From {
            New TamperAuditEntryDto With {
                .DetectedAt = DateTime.Now,
                .ReceiptNumber = "R-001",
                .ExpectedHash = "abc",
                .ActualHash = "def",
                .TamperKind = "Critical",
                .Details = "Details"
            }
        }

        Dim options = New TamperReportExportOptions With {
            .BusinessName = "Test",
            .Title = "Test Title",
            .FromDate = DateTime.Now,
            .ToDate = DateTime.Now
        }

        Dim bytes = Await exporter.ExportAsync(incidents, TamperReportFormat.Pdf, options)
        Assert.NotNull(bytes)
        Assert.True(bytes.Length > 0)
    End Function

    <Fact>
    Public Async Function Presenter_ExportCsv_CallsExporterAndWritesFile() As Task
        Dim testFilePath = Path.Combine(Path.GetTempPath(), $"test_export_{Guid.NewGuid()}.csv")
        Dim tcs As New TaskCompletionSource(Of Boolean)()

        Try
            Dim mockQuery = New Mock(Of ITamperAuditQueryService)()
            Dim mockExporter = New Mock(Of ITamperReportExporter)()
            Dim mockNotification = New Mock(Of INotificationService)()
            Dim mockView = New Mock(Of ITamperAuditReportView)()

            mockView.SetupProperty(Function(v) v.FromDate)
            mockView.SetupProperty(Function(v) v.ToDate)

            Dim fakeCsvBytes = Encoding.UTF8.GetBytes("fake csv content")
            mockExporter.Setup(Function(e) e.ExportAsync(It.IsAny(Of IEnumerable(Of TamperAuditEntryDto))(), TamperReportFormat.Csv, It.IsAny(Of TamperReportExportOptions)())).ReturnsAsync(fakeCsvBytes)

            mockNotification.Setup(Sub(n) n.ShowInfo(It.IsAny(Of String)())).Callback(Sub() tcs.TrySetResult(True))

            Dim presenter = New TamperAuditReportPresenter(mockQuery.Object, mockExporter.Object, mockNotification.Object)
            presenter.View = mockView.Object

            mockView.Raise(Sub(m) AddHandler m.ExportCsvRequested, Nothing, Nothing, testFilePath)

            Dim completed = Await Task.WhenAny(tcs.Task, Task.Delay(5000))
            Assert.Same(tcs.Task, completed)
            Assert.True(tcs.Task.Result)

            Assert.True(File.Exists(testFilePath))
            Dim content = Await File.ReadAllTextAsync(testFilePath)
            Assert.Equal("fake csv content", content)
        Finally
            If File.Exists(testFilePath) Then
                File.Delete(testFilePath)
            End If
        End Try
    End Function

End Class
