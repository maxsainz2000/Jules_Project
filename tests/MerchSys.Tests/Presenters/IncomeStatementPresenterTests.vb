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
Public Class IncomeStatementPresenterTests

    <Fact>
    Public Async Function ExportPdfRequested_GeneratesPdfAndShowsNotification() As Task
        ' Arrange
        Dim mockService = New Mock(Of IIncomeStatementService)()
        Dim mockWhatThisMeans = New Mock(Of IWhatThisMeansService)()
        Dim mockNotification = New Mock(Of INotificationService)()
        Dim mockExporter = New Mock(Of IIncomeStatementPdfExporter)()
        Dim mockView = New Mock(Of IIncomeStatementView)()

        mockView.SetupAllProperties()

        Dim presenter As New IncomeStatementPresenter(mockService.Object, mockWhatThisMeans.Object, mockNotification.Object, mockExporter.Object)
        presenter.View = mockView.Object

        Dim statement = New IncomeStatementDto With {.GrossMarginPercent = 50D}
        mockService.Setup(Function(s) s.GenerateMonthlyAsync(It.IsAny(Of Integer)(), It.IsAny(Of Integer)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(statement)
        mockService.Setup(Function(s) s.GetPerProductMarginsAsync(It.IsAny(Of DateTime)(), It.IsAny(Of DateTime)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(New List(Of ProductMarginDto)())

        mockExporter.Setup(Function(e) e.ExportPdf(It.IsAny(Of IncomeStatementDto)(), It.IsAny(Of IEnumerable(Of ProductMarginDto))(), It.IsAny(Of String)())).Returns(New Byte() {1, 2, 3})

        mockView.Setup(Function(v) v.PeriodType).Returns(IncomeStatementPeriodType.Monthly)
        mockView.Setup(Function(v) v.SelectedYear).Returns(2026)
        mockView.Setup(Function(v) v.SelectedMonth).Returns(3)

        Dim testFilePath = "test_income_statement.pdf"
        If System.IO.File.Exists(testFilePath) Then
            System.IO.File.Delete(testFilePath)
        End If

        ' Act
        mockView.Raise(Sub(v) AddHandler v.ExportPdfRequested, Nothing, testFilePath)
        Await Task.Delay(200)

        ' Assert
        mockExporter.Verify(Function(e) e.ExportPdf(It.IsAny(Of IncomeStatementDto)(), It.IsAny(Of IEnumerable(Of ProductMarginDto))(), It.IsAny(Of String)()), Times.Once())
        mockNotification.Verify(Sub(n) n.ShowInfo("Income Statement PDF exported successfully."), Times.Once())

        Assert.True(System.IO.File.Exists(testFilePath))

        ' Cleanup
        If System.IO.File.Exists(testFilePath) Then
            System.IO.File.Delete(testFilePath)
        End If
    End Function

End Class
End Namespace
