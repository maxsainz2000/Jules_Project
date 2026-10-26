Imports System.Drawing
Imports MerchSys.Accounting.Services
Imports MerchSys.Accounting.Views
Imports MerchSys.SharedKernel.Interfaces
Imports System.Threading

Namespace Presenters

    Public Class VatReliefReportPresenter
        Private ReadOnly _service As IVatReliefReportService
        Private ReadOnly _whatThisMeansService As IWhatThisMeansService
        Private ReadOnly _notificationService As INotificationService
        Private ReadOnly _exporter As IVatReliefPdfExporter
        Private _view As IVatReliefReportView

        Public Sub New(service As IVatReliefReportService, whatThisMeansService As IWhatThisMeansService, notificationService As INotificationService,
            Optional exporter As IVatReliefPdfExporter = Nothing)
            _service = service
            _whatThisMeansService = whatThisMeansService
            _notificationService = notificationService
            _exporter = exporter
        End Sub

        Public Property View As IVatReliefReportView
            Get
                Return _view
            End Get
            Set(value As IVatReliefReportView)
                _view = value
                If _view IsNot Nothing Then
                    AddHandler _view.ViewLoaded, AddressOf OnViewLoaded
                    AddHandler _view.PeriodChanged, AddressOf OnPeriodChanged
                    AddHandler _view.RefreshRequested, AddressOf OnRefreshRequested
                    AddHandler _view.ExportPdfRequested, AddressOf OnExportPdfRequested
                End If
            End Set
        End Property

        Private Async Sub OnViewLoaded(sender As Object, e As EventArgs)
            Dim currentYear = Date.Now.Year
            Dim years = Enumerable.Range(currentYear - 5, 6).OrderByDescending(Function(y) y).ToList()
            Dim months = Enumerable.Range(1, 12).ToList()

            _view.SetAvailableYears(years)
            _view.SetAvailableMonths(months)

            _view.SelectedYear = currentYear
            _view.SelectedMonth = Date.Now.Month

            Await LoadDataAsync()
        End Sub

        Private Async Sub OnPeriodChanged(sender As Object, e As EventArgs)
            Await LoadDataAsync()
        End Sub

        Private Async Sub OnRefreshRequested(sender As Object, e As EventArgs)
            Await LoadDataAsync()
        End Sub

        Private Async Function LoadDataAsync() As Task
            If _view Is Nothing Then Return

            _view.IsLoading = True
            _view.ErrorMessage = String.Empty

            Dim year = _view.SelectedYear
            Dim month = _view.SelectedMonth

            Dim summary As VatReliefSummary = Nothing
            Dim trailing As List(Of VatReliefSummary) = Nothing
            Dim errorMsg As String = Nothing
            Dim interpretation As String = Nothing

            Try
                summary = Await _service.GetMonthlySummaryAsync(year, month)
                trailing = Await _service.GetTrailingMonthsAsync(year, month, 12)
                interpretation = _whatThisMeansService.GenerateVatReliefInterpretation(summary)
            Catch ex As Exception
                errorMsg = "Failed to load VAT relief data: " & ex.Message
            End Try

            If errorMsg IsNot Nothing Then
                _view.IsLoading = False
                _view.ErrorMessage = errorMsg
                _notificationService.ShowError(errorMsg)
                Return
            End If

            _view.DisplaySummary(summary)
            _view.DisplayTrailingMonths(trailing)

            If summary.NetVatPayable > 0 Then
                _view.NetVatPayableText = summary.NetVatPayable.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("en-PH"))
                _view.NetVatPayableColor = Color.Firebrick
            ElseIf summary.NetVatPayable < 0 Then
                _view.NetVatPayableText = Math.Abs(summary.NetVatPayable).ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("en-PH")) & " (Credit)"
                _view.NetVatPayableColor = Color.ForestGreen
            Else
                _view.NetVatPayableText = "₱0.00"
                _view.NetVatPayableColor = Color.Gray
            End If

            _view.WhatThisMeansText = interpretation
            _view.IsLoading = False
        End Function

        Private Async Sub OnExportPdfRequested(sender As Object, filePath As String)
            If _exporter Is Nothing Then
                _notificationService?.ShowError("PDF export is not supported.")
                Return
            End If

            If _view Is Nothing OrElse String.IsNullOrEmpty(filePath) Then Return

            _view.IsLoading = True

            Try
                Dim year = _view.SelectedYear
                Dim month = _view.SelectedMonth
                Dim summary = Await _service.GetMonthlySummaryAsync(year, month)
                Dim trailing = Await _service.GetTrailingMonthsAsync(year, month, 12)
                Dim interpretation = _whatThisMeansService.GenerateVatReliefInterpretation(summary)
                Dim pdfBytes = _exporter.ExportPdf(summary, trailing, interpretation)
                Await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes)
                _notificationService?.ShowInfo("VAT Relief Report PDF exported successfully.")
            Catch ex As Exception
                _notificationService?.ShowError("Failed to export PDF: " & ex.Message)
            Finally
                _view.IsLoading = False
            End Try
        End Sub

    End Class

End Namespace
