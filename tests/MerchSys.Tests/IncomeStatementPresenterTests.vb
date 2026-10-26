Imports System.IO
Imports System.Threading
Imports System.Threading.Tasks
Imports MerchSys.Accounting.Presenters
Imports MerchSys.Accounting.Services
Imports MerchSys.Accounting.Views
Imports MerchSys.SharedKernel.Interfaces
Imports Moq
Imports Xunit

Public Class IncomeStatementPresenterTests
    Private ReadOnly _mockService As Mock(Of IIncomeStatementService)
    Private ReadOnly _mockWhatThisMeans As Mock(Of IWhatThisMeansService)
    Private ReadOnly _mockNotification As Mock(Of INotificationService)
    Private ReadOnly _mockExporter As Mock(Of IIncomeStatementPdfExporter)
    Private ReadOnly _mockView As Mock(Of IIncomeStatementView)
    Private ReadOnly _presenter As IncomeStatementPresenter

    Public Sub New()
        _mockService = New Mock(Of IIncomeStatementService)()
        _mockWhatThisMeans = New Mock(Of IWhatThisMeansService)()
        _mockNotification = New Mock(Of INotificationService)()
        _mockExporter = New Mock(Of IIncomeStatementPdfExporter)()
        _mockView = New Mock(Of IIncomeStatementView)()
        _mockView.SetupAllProperties()

        _presenter = New IncomeStatementPresenter(
            _mockService.Object,
            _mockWhatThisMeans.Object,
            _mockNotification.Object,
            _mockExporter.Object)

        _presenter.View = _mockView.Object
    End Sub

    <Fact>
    Public Async Function ExportPdfRequested_GeneratesPdfAndShowsNotification() As Task
        ' Arrange
        _mockView.Object.PeriodType = IncomeStatementPeriodType.Monthly
        _mockView.Object.SelectedYear = 2026
        _mockView.Object.SelectedMonth = 3
        _mockView.Object.IsLoading = False

        Dim statement = New IncomeStatementDto With {
            .StartDate = New DateTime(2026, 3, 1),
            .EndDate = New DateTime(2026, 3, 31)
        }

        _mockService.Setup(Function(s) s.GenerateMonthlyAsync(2026, 3, It.IsAny(Of CancellationToken)())).ReturnsAsync(statement)
        _mockService.Setup(Function(s) s.GenerateMonthlyAsync(2026, 2, It.IsAny(Of CancellationToken)())).ReturnsAsync(New IncomeStatementDto())

        Dim margins = New List(Of ProductMarginDto)()
        _mockService.Setup(Function(s) s.GetPerProductMarginsAsync(It.IsAny(Of DateTime)(), It.IsAny(Of DateTime)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(margins)

        _mockWhatThisMeans.Setup(Function(s) s.GenerateIncomeStatementInterpretation(It.IsAny(Of IncomeStatementDto)(), It.IsAny(Of Decimal?))).Returns("Interp")

        Dim pdfBytes = New Byte() {&H25, &H50, &H44, &H46}
        _mockExporter.Setup(Function(e) e.ExportPdf(It.IsAny(Of IncomeStatementDto)(), It.IsAny(Of IEnumerable(Of ProductMarginDto))(), It.IsAny(Of String)())).Returns(pdfBytes)

        Dim testFilePath = Path.Combine(Path.GetTempPath(), $"test_income_stmt_{Guid.NewGuid()}.pdf")
        Dim tcs As New TaskCompletionSource(Of Boolean)()

        _mockNotification.Setup(Sub(n) n.ShowInfo(It.IsAny(Of String)())).Callback(Sub(msg As String) tcs.TrySetResult(True))

        Try
            ' Act
            _mockView.Raise(Sub(m) AddHandler m.ExportPdfRequested, Nothing, Nothing, testFilePath)

            ' Wait for fire-and-forget handler
            Await Task.WhenAny(tcs.Task, Task.Delay(2000))

            ' Assert
            _mockExporter.Verify(Function(e) e.ExportPdf(statement, margins, "Interp"), Times.Once())
            _mockNotification.Verify(Sub(n) n.ShowInfo("Income Statement PDF exported successfully."), Times.Once())
            Assert.True(File.Exists(testFilePath))

            Dim savedBytes = File.ReadAllBytes(testFilePath)
            Assert.Equal(pdfBytes, savedBytes)
        Finally
            If File.Exists(testFilePath) Then
                File.Delete(testFilePath)
            End If
        End Try
    End Function

End Class
