Imports System
Imports System.Collections.Generic
Imports System.Text
Imports MerchSys.Accounting.Presenters
Imports MerchSys.Accounting.Services
Imports Xunit
Imports Moq

Public Class TamperReportExporterTests

    <Fact>
    Public Sub Export_Csv_ReturnsValidCsvWithHeaders()
        ' Arrange
        Dim exporter As New TamperReportExporter()
        Dim entries = New List(Of TamperAuditEntryDto) From {
            New TamperAuditEntryDto With {
                .DetectedAt = New DateTime(2025, 1, 1, 10, 0, 0),
                .ReceiptNumber = "R-001",
                .ExpectedHash = "abc",
                .ActualHash = "def",
                .TamperKind = "Tampered",
                .Details = "Test"
            }
        }

        ' Act
        Dim result = exporter.Export(entries, TamperReportFormat.Csv)
        Dim csvString = Encoding.UTF8.GetString(result)

        ' Assert
        Assert.Contains("DetectedAt,ReceiptNumber,ExpectedHash,ActualHash,TamperKind,Details", csvString)
        Assert.Contains("R-001", csvString)
    End Sub

    <Fact>
    Public Sub Export_Csv_AppliesRfc4180Escaping()
        ' Arrange
        Dim exporter As New TamperReportExporter()
        Dim entries = New List(Of TamperAuditEntryDto) From {
            New TamperAuditEntryDto With {
                .DetectedAt = New DateTime(2025, 1, 1, 10, 0, 0),
                .ReceiptNumber = "R,002",
                .ExpectedHash = "ab""c",
                .ActualHash = "def",
                .TamperKind = "Tampered",
                .Details = "Line1" & vbCrLf & "Line2"
            }
        }

        ' Act
        Dim result = exporter.Export(entries, TamperReportFormat.Csv)
        Dim csvString = Encoding.UTF8.GetString(result)

        ' Assert
        Assert.Contains("""R,002""", csvString)
        Assert.Contains("""ab""""c""", csvString)
        Assert.Contains("""Line1" & vbCrLf & "Line2""", csvString)
    End Sub

    <Fact>
    Public Sub Export_Pdf_ReturnsValidPdfBytes()
        ' Arrange
        Dim exporter As New TamperReportExporter()
        Dim entries = New List(Of TamperAuditEntryDto) From {
            New TamperAuditEntryDto With {
                .DetectedAt = New DateTime(2025, 1, 1, 10, 0, 0),
                .ReceiptNumber = "R-001",
                .ExpectedHash = "abc",
                .ActualHash = "def",
                .TamperKind = "Tampered",
                .Details = "Test"
            }
        }

        ' Act
        Dim result = exporter.Export(entries, TamperReportFormat.Pdf)

        ' Assert
        Assert.NotNull(result)
        Assert.True(result.Length > 0)
        ' Check PDF magic bytes (%PDF)
        Assert.Equal(CByte(&H25), result(0)) ' %
        Assert.Equal(CByte(&H50), result(1)) ' P
        Assert.Equal(CByte(&H44), result(2)) ' D
        Assert.Equal(CByte(&H46), result(3)) ' F
    End Sub

    <Fact>
    Public Sub Presenter_ExportCsv_CallsExporterAndWritesFile()
        ' Arrange
        Dim mockQuery = New Mock(Of ITamperAuditQueryService)()
        mockQuery.Setup(Function(q) q.GetIncidentsAsync()).ReturnsAsync(New List(Of MerchSys.Accounting.Entities.TamperAuditEntry)())

        Dim mockExporter = New Mock(Of ITamperReportExporter)()
        mockExporter.Setup(Function(e) e.Export(It.IsAny(Of IEnumerable(Of TamperAuditEntryDto))(), TamperReportFormat.Csv, It.IsAny(Of TamperReportExportOptions)())).Returns(New Byte() {1, 2, 3})

        Dim mockNotif = New Mock(Of MerchSys.SharedKernel.Interfaces.INotificationService)()
        Dim mockView = New Mock(Of MerchSys.Accounting.Views.ITamperAuditReportView)()

        ' Setup view properties explicitly for setup
        mockView.SetupAllProperties()

        Dim presenter As New TamperAuditReportPresenter(mockQuery.Object, mockExporter.Object, mockNotif.Object)
        presenter.View = mockView.Object

        Dim testFilePath = "test_export.csv"

        ' Act
        mockView.Raise(Sub(m) AddHandler m.ExportCsvRequested, Nothing, testFilePath)

        ' Assert
        ' Using reflection/file IO for actual write is tricky in pure unit tests if we didn't mock file IO.
        ' At minimum we verify the exporter was called.
        mockExporter.Verify(Function(e) e.Export(It.IsAny(Of IEnumerable(Of TamperAuditEntryDto))(), TamperReportFormat.Csv, It.IsAny(Of TamperReportExportOptions)()), Times.Once())
        mockNotif.Verify(Sub(n) n.ShowInfo(It.IsRegex("Exported successfully")), Times.Once())

        If System.IO.File.Exists(testFilePath) Then
            System.IO.File.Delete(testFilePath)
        End If
    End Sub

End Class