Imports Moq
Imports Xunit
Imports MerchSys.Accounting.Presenters
Imports MerchSys.Accounting.Services
Imports MerchSys.Accounting.Views
Imports MerchSys.SharedKernel.Interfaces
Imports System.Threading
Imports System.Drawing

Public Class VatReliefReportTests
    Private ReadOnly _serviceMock As Mock(Of IVatReliefReportService)
    Private ReadOnly _whatThisMeansMock As Mock(Of IWhatThisMeansService)
    Private ReadOnly _notificationMock As Mock(Of INotificationService)
    Private ReadOnly _viewMock As Mock(Of IVatReliefReportView)
    Private ReadOnly _presenter As VatReliefReportPresenter
    Private ReadOnly _exporterMock As Mock(Of IVatReliefPdfExporter)

    Public Sub New()
        _serviceMock = New Mock(Of IVatReliefReportService)()
        _whatThisMeansMock = New Mock(Of IWhatThisMeansService)()
        _notificationMock = New Mock(Of INotificationService)()
        _viewMock = New Mock(Of IVatReliefReportView)()
        _viewMock.SetupAllProperties()
        _exporterMock = New Mock(Of IVatReliefPdfExporter)()

        _presenter = New VatReliefReportPresenter(_serviceMock.Object, _whatThisMeansMock.Object, _notificationMock.Object, _exporterMock.Object)
        _presenter.View = _viewMock.Object
    End Sub

    <Fact>
    Public Sub OnViewLoaded_SetsDefaultPeriodAndLoadsData()
        ' Arrange
        _serviceMock.Setup(Function(s) s.GetMonthlySummaryAsync(It.IsAny(Of Integer)(), It.IsAny(Of Integer)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(New VatReliefSummary())
        _serviceMock.Setup(Function(s) s.GetTrailingMonthsAsync(It.IsAny(Of Integer)(), It.IsAny(Of Integer)(), It.IsAny(Of Integer)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(New List(Of VatReliefSummary)())

        ' Act
        _viewMock.Raise(Sub(v) AddHandler v.ViewLoaded, Nothing, Nothing, EventArgs.Empty)

        ' Assert
        _viewMock.VerifySet(Sub(v) v.SelectedYear = Date.Now.Year)
        _viewMock.VerifySet(Sub(v) v.SelectedMonth = Date.Now.Month)
        _viewMock.Verify(Sub(v) v.SetAvailableYears(It.IsAny(Of IEnumerable(Of Integer))()))
        _viewMock.Verify(Sub(v) v.SetAvailableMonths(It.IsAny(Of IEnumerable(Of Integer))()))
        _serviceMock.Verify(Function(s) s.GetMonthlySummaryAsync(Date.Now.Year, Date.Now.Month, It.IsAny(Of CancellationToken)()), Times.Once())
    End Sub

    <Fact>
    Public Sub LoadData_WithPositiveNetVatPayable_AssignsRedColorAndFormatsText()
        ' Arrange
        Dim summary = New VatReliefSummary With {
            .OutputVat = 1000,
            .InputVat = 200,
            .NetVatPayable = 800
        }
        _serviceMock.Setup(Function(s) s.GetMonthlySummaryAsync(It.IsAny(Of Integer)(), It.IsAny(Of Integer)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(summary)
        _serviceMock.Setup(Function(s) s.GetTrailingMonthsAsync(It.IsAny(Of Integer)(), It.IsAny(Of Integer)(), It.IsAny(Of Integer)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(New List(Of VatReliefSummary)())
        _whatThisMeansMock.Setup(Function(w) w.GenerateVatReliefInterpretation(summary)).Returns("Test Interpretation")

        ' Act
        _viewMock.Raise(Sub(v) AddHandler v.RefreshRequested, Nothing, Nothing, EventArgs.Empty)

        ' Assert
        _viewMock.VerifySet(Sub(v) v.NetVatPayableColor = Color.Firebrick)
        _viewMock.VerifySet(Sub(v) v.NetVatPayableText = summary.NetVatPayable.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("en-PH")))
        _viewMock.VerifySet(Sub(v) v.WhatThisMeansText = "Test Interpretation")
        _viewMock.Verify(Sub(v) v.DisplaySummary(summary), Times.Once())
    End Sub

    <Fact>
    Public Sub LoadData_WithNegativeNetVatPayable_AssignsGreenColorAndCreditText()
        ' Arrange
        Dim summary = New VatReliefSummary With {
            .OutputVat = 200,
            .InputVat = 1000,
            .NetVatPayable = -800
        }
        _serviceMock.Setup(Function(s) s.GetMonthlySummaryAsync(It.IsAny(Of Integer)(), It.IsAny(Of Integer)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(summary)
        _serviceMock.Setup(Function(s) s.GetTrailingMonthsAsync(It.IsAny(Of Integer)(), It.IsAny(Of Integer)(), It.IsAny(Of Integer)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(New List(Of VatReliefSummary)())

        ' Act
        _viewMock.Raise(Sub(v) AddHandler v.RefreshRequested, Nothing, Nothing, EventArgs.Empty)

        ' Assert
        _viewMock.VerifySet(Sub(v) v.NetVatPayableColor = Color.ForestGreen)
        _viewMock.VerifySet(Sub(v) v.NetVatPayableText = Math.Abs(summary.NetVatPayable).ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("en-PH")) & " (Credit)")
    End Sub

    <Fact>
    Public Sub LoadData_WithZeroNetVatPayable_AssignsGrayColor()
        ' Arrange
        Dim summary = New VatReliefSummary With {
            .OutputVat = 500,
            .InputVat = 500,
            .NetVatPayable = 0
        }
        _serviceMock.Setup(Function(s) s.GetMonthlySummaryAsync(It.IsAny(Of Integer)(), It.IsAny(Of Integer)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(summary)
        _serviceMock.Setup(Function(s) s.GetTrailingMonthsAsync(It.IsAny(Of Integer)(), It.IsAny(Of Integer)(), It.IsAny(Of Integer)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(New List(Of VatReliefSummary)())

        ' Act
        _viewMock.Raise(Sub(v) AddHandler v.RefreshRequested, Nothing, Nothing, EventArgs.Empty)

        ' Assert
        _viewMock.VerifySet(Sub(v) v.NetVatPayableColor = Color.Gray)
        _viewMock.VerifySet(Sub(v) v.NetVatPayableText = "₱0.00")
    End Sub

    <Fact>
    Public Sub LoadData_OnServiceException_HandlesGracefullyAndShowsError()
        ' Arrange
        _serviceMock.Setup(Function(s) s.GetMonthlySummaryAsync(It.IsAny(Of Integer)(), It.IsAny(Of Integer)(), It.IsAny(Of CancellationToken)())).ThrowsAsync(New Exception("Database timeout"))

        ' Act
        _viewMock.Raise(Sub(v) AddHandler v.RefreshRequested, Nothing, Nothing, EventArgs.Empty)

        ' Assert
        _viewMock.VerifySet(Sub(v) v.ErrorMessage = "Failed to load VAT relief data: Database timeout")
        _notificationMock.Verify(Sub(n) n.ShowError("Failed to load VAT relief data: Database timeout"), Times.Once())
    End Sub

    <Fact>
    Public Sub PeriodChanged_TriggersDataLoad()
        ' Arrange
        _viewMock.SetupGet(Function(v) v.SelectedYear).Returns(2023)
        _viewMock.SetupGet(Function(v) v.SelectedMonth).Returns(5)
        _serviceMock.Setup(Function(s) s.GetMonthlySummaryAsync(2023, 5, It.IsAny(Of CancellationToken)())).ReturnsAsync(New VatReliefSummary())
        _serviceMock.Setup(Function(s) s.GetTrailingMonthsAsync(It.IsAny(Of Integer)(), It.IsAny(Of Integer)(), It.IsAny(Of Integer)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(New List(Of VatReliefSummary)())

        ' Act
        _viewMock.Raise(Sub(v) AddHandler v.PeriodChanged, Nothing, Nothing, EventArgs.Empty)

        ' Assert
        _serviceMock.Verify(Function(s) s.GetMonthlySummaryAsync(2023, 5, It.IsAny(Of CancellationToken)()), Times.Once())
    End Sub

    <Fact>
    Public Async Function ExportPdfRequested_GeneratesPdfAndShowsNotification() As System.Threading.Tasks.Task
        ' Arrange
        _viewMock.Object.SelectedYear = 2026
        _viewMock.Object.SelectedMonth = 3
        _viewMock.Object.IsLoading = False

        Dim summary = New VatReliefSummary()
        _serviceMock.Setup(Function(s) s.GetMonthlySummaryAsync(2026, 3, It.IsAny(Of CancellationToken)())).ReturnsAsync(summary)

        Dim trailing = New List(Of VatReliefSummary)()
        _serviceMock.Setup(Function(s) s.GetTrailingMonthsAsync(2026, 3, 12, It.IsAny(Of CancellationToken)())).ReturnsAsync(trailing)

        _whatThisMeansMock.Setup(Function(s) s.GenerateVatReliefInterpretation(It.IsAny(Of VatReliefSummary)())).Returns("Interp")

        Dim pdfBytes = New Byte() {&H25, &H50, &H44, &H46}
        _exporterMock.Setup(Function(e) e.ExportPdf(It.IsAny(Of VatReliefSummary)(), It.IsAny(Of List(Of VatReliefSummary))(), It.IsAny(Of String)())).Returns(pdfBytes)

        Dim testFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"test_vat_relief_{Guid.NewGuid()}.pdf")
        Dim tcs As New System.Threading.Tasks.TaskCompletionSource(Of Boolean)()

        _notificationMock.Setup(Sub(n) n.ShowInfo(It.IsAny(Of String)())).Callback(Sub(msg As String) tcs.TrySetResult(True))

        Try
            ' Act
            _viewMock.Raise(Sub(m) AddHandler m.ExportPdfRequested, Nothing, Nothing, testFilePath)

            ' Wait for fire-and-forget handler
            Await System.Threading.Tasks.Task.WhenAny(tcs.Task, System.Threading.Tasks.Task.Delay(2000))

            ' Assert
            _exporterMock.Verify(Function(e) e.ExportPdf(summary, trailing, "Interp"), Times.Once())
            _notificationMock.Verify(Sub(n) n.ShowInfo("VAT Relief Report PDF exported successfully."), Times.Once())
            Assert.True(System.IO.File.Exists(testFilePath))

            Dim savedBytes = System.IO.File.ReadAllBytes(testFilePath)
            Assert.Equal(pdfBytes, savedBytes)
        Finally
            If System.IO.File.Exists(testFilePath) Then
                System.IO.File.Delete(testFilePath)
            End If
        End Try
    End Function

End Class
