Imports System.Threading
Imports MerchSys.Accounting.Entities
Imports MerchSys.Accounting.Enums
Imports MerchSys.Accounting.Services
Imports MerchSys.Accounting.Views
Imports MerchSys.SharedKernel.Interfaces

Namespace Presenters
    Public Class VatReturnPresenter
        Private _view As IVatReturnView
        Private ReadOnly _vatReportingService As IVatReportingService
        Private ReadOnly _vatReturnExporter As IVatReturnExporter
        Private ReadOnly _whatThisMeansService As IWhatThisMeansService
        Private ReadOnly _notificationService As INotificationService
        Private ReadOnly _sessionService As ISessionService
        Private _currentVatReturn As VatReturn

        Public Sub New(vatReportingService As IVatReportingService,
                       vatReturnExporter As IVatReturnExporter,
                       whatThisMeansService As IWhatThisMeansService,
                       notificationService As INotificationService,
                       sessionService As ISessionService)
            _vatReportingService = vatReportingService
            _vatReturnExporter = vatReturnExporter
            _whatThisMeansService = whatThisMeansService
            _notificationService = notificationService
            _sessionService = sessionService
        End Sub

        Public Property View As IVatReturnView
            Get
                Return _view
            End Get
            Set(value As IVatReturnView)
                _view = value
                If _view IsNot Nothing Then
                    AddHandler _view.ViewLoaded, AddressOf OnViewLoaded
                    AddHandler _view.GenerateRequested, AddressOf OnGenerateRequested
                    AddHandler _view.FileRequested, AddressOf OnFileRequested
                    AddHandler _view.AmendRequested, AddressOf OnAmendRequested
                    AddHandler _view.ExportCsvRequested, AddressOf OnExportCsvRequested
                    AddHandler _view.ExportPdfRequested, AddressOf OnExportPdfRequested
                    AddHandler _view.FormTypeChanged, AddressOf OnFormTypeChanged
                    AddHandler _view.PeriodChanged, AddressOf OnPeriodChanged
                    AddHandler _view.ReturnSelected, AddressOf OnReturnSelected
                End If
            End Set
        End Property

        Private Async Sub OnViewLoaded(sender As Object, e As EventArgs)
            _view.IsLoading = True
            _view.ErrorMessage = String.Empty

            Try
                ' Setup available years (e.g. 5 years back to current)
                Dim currentYear = DateTime.UtcNow.Year
                Dim years = Enumerable.Range(currentYear - 5, 6).OrderByDescending(Function(y) y).ToList()
                _view.SetAvailableYears(years)
                _view.SelectedYear = currentYear
                
                ' Setup periods based on default form type (Form2550Q -> Quarterly)
                _view.SelectedFormType = VatReturnFormType.Form2550Q
                UpdateAvailablePeriods()
                
                Await LoadHistoryAsync()
            Catch ex As Exception
                _view.ErrorMessage = $"Error loading view: {ex.Message}"
            Finally
                _view.IsLoading = False
            End Try
        End Sub

        Private Sub OnFormTypeChanged(sender As Object, e As EventArgs)
            UpdateAvailablePeriods()
        End Sub
        
        Private Sub OnPeriodChanged(sender As Object, e As EventArgs)
            ' Handle period changes if needed
        End Sub

        Private Sub UpdateAvailablePeriods()
            If _view.SelectedFormType = VatReturnFormType.Form2550M Then
                _view.SetAvailablePeriods(Enumerable.Range(1, 12).ToList())
            Else
                _view.SetAvailablePeriods(Enumerable.Range(1, 4).ToList())
            End If
            _view.SelectedPeriod = 1
        End Sub

        Private Async Function LoadHistoryAsync() As Task
            Dim history = Await _vatReportingService.ListVatReturnsAsync()
            _view.SetVatReturnHistory(history)
        End Function

        Private Async Sub OnGenerateRequested(sender As Object, e As EventArgs)
            _view.IsLoading = True
            _view.ErrorMessage = String.Empty
            Dim exMsg As String = String.Empty
            
            Try
                _currentVatReturn = Await _vatReportingService.GenerateVatReturnAsync(_view.SelectedYear, _view.SelectedPeriod, _view.SelectedFormType)
                UpdateViewWithCurrentReturn()
                Await LoadHistoryAsync()
            Catch ex As Exception
                exMsg = ex.Message
            End Try

            _view.IsLoading = False
            If Not String.IsNullOrEmpty(exMsg) Then
                _view.ErrorMessage = $"Error generating return: {exMsg}"
            End If
        End Sub

        Private Async Sub OnFileRequested(sender As Object, e As EventArgs)
            If _currentVatReturn Is Nothing OrElse _currentVatReturn.Id = 0 Then Return
            
            _view.IsLoading = True
            _view.ErrorMessage = String.Empty
            Dim exMsg As String = String.Empty

            Try
                Dim user = _sessionService.CurrentUserRole.ToString()
                _currentVatReturn = Await _vatReportingService.FileVatReturnAsync(_currentVatReturn.Id, user)
                UpdateViewWithCurrentReturn()
                Await LoadHistoryAsync()
                _notificationService.ShowSuccess("VAT Return filed successfully.")
            Catch ex As Exception
                exMsg = ex.Message
            End Try

            _view.IsLoading = False
            If Not String.IsNullOrEmpty(exMsg) Then
                _view.ErrorMessage = $"Error filing return: {exMsg}"
            End If
        End Sub

        Private Async Sub OnAmendRequested(sender As Object, e As EventArgs)
            If _currentVatReturn Is Nothing OrElse _currentVatReturn.Id = 0 Then Return
            
            _view.IsLoading = True
            _view.ErrorMessage = String.Empty
            Dim exMsg As String = String.Empty

            Try
                _currentVatReturn = Await _vatReportingService.AmendVatReturnAsync(_currentVatReturn.Id)
                UpdateViewWithCurrentReturn()
                Await LoadHistoryAsync()
                _notificationService.ShowSuccess("VAT Return marked for amendment.")
            Catch ex As Exception
                exMsg = ex.Message
            End Try

            _view.IsLoading = False
            If Not String.IsNullOrEmpty(exMsg) Then
                _view.ErrorMessage = $"Error amending return: {exMsg}"
            End If
        End Sub

        Private Sub OnExportCsvRequested(sender As Object, e As EventArgs)
            If _currentVatReturn Is Nothing Then Return
            Try
                Dim bytes = _vatReturnExporter.ExportCsv(_currentVatReturn)
                Dim fileName = $"VAT_Return_{_currentVatReturn.Year}_{_currentVatReturn.Period}_{_currentVatReturn.FormType}.csv"
                If _view IsNot Nothing Then
                    _view.PromptToSaveExport(fileName, bytes)
                End If
            Catch ex As Exception
                _view.ErrorMessage = $"Error exporting CSV: {ex.Message}"
            End Try
        End Sub

        Private Sub OnExportPdfRequested(sender As Object, e As EventArgs)
            If _currentVatReturn Is Nothing Then Return
            Try
                Dim bytes = _vatReturnExporter.ExportPdf(_currentVatReturn)
                Dim fileName = $"VAT_Return_{_currentVatReturn.Year}_{_currentVatReturn.Period}_{_currentVatReturn.FormType}.pdf"
                If _view IsNot Nothing Then
                    _view.PromptToSaveExport(fileName, bytes)
                End If
            Catch ex As Exception
                _view.ErrorMessage = $"Error exporting PDF: {ex.Message}"
            End Try
        End Sub

        Private Async Sub OnReturnSelected(sender As Object, returnId As Integer)
            _view.IsLoading = True
            _view.ErrorMessage = String.Empty
            Dim exMsg As String = String.Empty

            Try
                _currentVatReturn = Await _vatReportingService.GetVatReturnAsync(returnId)
                If _currentVatReturn IsNot Nothing Then
                    UpdateViewWithCurrentReturn()
                End If
            Catch ex As Exception
                exMsg = ex.Message
            End Try

            _view.IsLoading = False
            If Not String.IsNullOrEmpty(exMsg) Then
                _view.ErrorMessage = $"Error loading return details: {exMsg}"
            End If
        End Sub

        Private Sub UpdateViewWithCurrentReturn()
            If _currentVatReturn Is Nothing Then Return

            _view.SelectedYear = _currentVatReturn.Year
            _view.SelectedFormType = _currentVatReturn.FormType
            UpdateAvailablePeriods()
            _view.SelectedPeriod = _currentVatReturn.Period

            _view.TotalSalesDisplay = _currentVatReturn.TotalSales.ToString("C2")
            _view.TotalPurchasesDisplay = _currentVatReturn.TotalPurchases.ToString("C2")
            _view.OutputVatDisplay = _currentVatReturn.OutputVatTotal.ToString("C2")
            _view.InputVatDisplay = _currentVatReturn.InputVatTotal.ToString("C2")
            _view.NetVatPayableDisplay = _currentVatReturn.NetVatPayable.ToString("C2")

            _view.FilingStatusText = _currentVatReturn.FilingStatus.ToString()
            
            _view.SetVatReturnLines(_currentVatReturn.Lines)
            
            _view.CanFile = (_currentVatReturn.FilingStatus = VatFilingStatus.Generated OrElse _currentVatReturn.FilingStatus = VatFilingStatus.Amended)
            _view.CanAmend = (_currentVatReturn.FilingStatus = VatFilingStatus.Filed)
            _view.CanExport = True

            _view.WhatThisMeansText = $"This {_currentVatReturn.FormType} return for {_currentVatReturn.Year}-{_currentVatReturn.Period} shows a Net VAT Payable of {_currentVatReturn.NetVatPayable:C2}. Ensure Output and Input VAT are correct before filing."
        End Sub
    End Class
End Namespace
