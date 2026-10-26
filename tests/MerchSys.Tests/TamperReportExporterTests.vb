Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
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
    Public Async Function ExportCsv_EscapesQuotesAndCommas() As Task
        Dim exporter = New TamperReportExporter()
        Dim options = New TamperReportExportOptions With {
            .Incidents = New List(Of TamperAuditEntryDto) From {
                New TamperAuditEntryDto With {
                    .DetectedAt = New DateTime(2023, 1, 1, 10, 0, 0),
                    .ReceiptNumber = "R,001",
                    .ExpectedHash = "Hash""123",
                    .ActualHash = "Hash" & vbCrLf & "456",
                    .TamperKind = "Critical",
                    .Details = "Some Details"
                }
            }
        }

        Dim bytes = Await exporter.ExportAsync(options, TamperReportFormat.Csv)
        Dim csv = Encoding.UTF8.GetString(bytes)

        Dim lines = csv.Split(New String() {vbCrLf, vbLf}, StringSplitOptions.None)
        Assert.True(lines.Length >= 2)

        Dim dataLine = lines(1)
        Assert.Contains("""R,001""", dataLine)
        Assert.Contains("""Hash""""123""", dataLine)
        Assert.Contains("""Hash" & vbCrLf & "456""", csv)
    End Function

    <Fact>
    Public Async Function Presenter_ExportCsv_CallsExporterAndWritesFile() As Task
        Dim mockView = New Mock(Of ITamperAuditReportView)()
        Dim mockQuery = New Mock(Of ITamperAuditQueryService)()
        Dim mockExporter = New Mock(Of ITamperReportExporter)()
        Dim mockNotification = New Mock(Of INotificationService)()

        Dim testFilePath = Path.Combine(Path.GetTempPath(), $"test_export_{Guid.NewGuid()}.csv")
        Dim fakeBytes = Encoding.UTF8.GetBytes("fake csv content")

        mockExporter.Setup(Function(e) e.ExportAsync(It.IsAny(Of TamperReportExportOptions)(), TamperReportFormat.Csv)).ReturnsAsync(fakeBytes)

        Dim presenter = New TamperAuditReportPresenter(mockQuery.Object, mockExporter.Object, mockNotification.Object)
        presenter.View = mockView.Object

        Try
            mockView.Raise(Sub(m) AddHandler m.ExportCsvRequested, Nothing, Nothing, testFilePath)
            Await Task.Yield() ' Allow async handler to run

            Assert.True(File.Exists(testFilePath))
            Dim content = Await File.ReadAllTextAsync(testFilePath)
            Assert.Equal("fake csv content", content)

            mockExporter.Verify(Function(e) e.ExportAsync(It.IsAny(Of TamperReportExportOptions)(), TamperReportFormat.Csv), Times.Once)
            mockNotification.Verify(Sub(n) n.ShowInfo(It.Is(Of String)(Function(s) s.Contains(testFilePath))), Times.Once)
        Finally
            If File.Exists(testFilePath) Then
                File.Delete(testFilePath)
            End If
        End Try
    End Function

End Class
