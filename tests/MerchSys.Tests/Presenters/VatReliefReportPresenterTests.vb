Imports System.Threading
Imports System.Threading.Tasks
Imports System.Collections.Generic
Imports MerchSys.Accounting.Presenters
Imports MerchSys.Accounting.Services
Imports MerchSys.Accounting.Views
Imports MerchSys.SharedKernel.Interfaces
Imports Moq
Imports Xunit

Namespace Presenters
Public Class VatReliefReportPresenterTests

    <Fact>
    Public Async Function ExportPdfRequested_GeneratesPdfAndShowsNotification() As Task
        ' Arrange
        Dim mockService = New Mock(Of IVatReliefReportService)()
        Dim mockWhatThisMeans = New Mock(Of IWhatThisMeansService)()
        Dim mockNotification = New Mock(Of INotificationService)()
        Dim mockExporter = New Mock(Of IVatReliefPdfExporter)()
        Dim mockView = New Mock(Of IVatReliefReportView)()

        mockView.SetupAllProperties()

        Dim presenter As New VatReliefReportPresenter(mockService.Object, mockWhatThisMeans.Object, mockNotification.Object, mockExporter.Object)
        presenter.View = mockView.Object

        Dim summary = New VatReliefSummary()
        mockService.Setup(Function(s) s.GetMonthlySummaryAsync(It.IsAny(Of Integer)(), It.IsAny(Of Integer)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(summary)
        mockService.Setup(Function(s) s.GetTrailingMonthsAsync(It.IsAny(Of Integer)(), It.IsAny(Of Integer)(), It.IsAny(Of Integer)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(New List(Of VatReliefSummary)())

        mockExporter.Setup(Function(e) e.ExportPdf(It.IsAny(Of VatReliefSummary)(), It.IsAny(Of IEnumerable(Of VatReliefSummary))(), It.IsAny(Of String)())).Returns(New Byte() {1, 2, 3})

        mockView.Setup(Function(v) v.SelectedYear).Returns(2026)
        mockView.Setup(Function(v) v.SelectedMonth).Returns(3)

        Dim testFilePath = "test_vat_relief.pdf"
        If System.IO.File.Exists(testFilePath) Then
            System.IO.File.Delete(testFilePath)
        End If

        ' Act
        mockView.Raise(Sub(v) AddHandler v.ExportPdfRequested, Nothing, testFilePath)
        Await Task.Delay(200)

        ' Assert
        mockExporter.Verify(Function(e) e.ExportPdf(It.IsAny(Of VatReliefSummary)(), It.IsAny(Of IEnumerable(Of VatReliefSummary))(), It.IsAny(Of String)()), Times.Once())
        mockNotification.Verify(Sub(n) n.ShowInfo("VAT Relief Report PDF exported successfully."), Times.Once())

        Assert.True(System.IO.File.Exists(testFilePath))

        ' Cleanup
        If System.IO.File.Exists(testFilePath) Then
            System.IO.File.Delete(testFilePath)
        End If
    End Function

End Class
End Namespace
