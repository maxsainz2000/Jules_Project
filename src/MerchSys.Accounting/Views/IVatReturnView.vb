Imports MerchSys.Accounting.Entities
Imports MerchSys.Accounting.Enums

Namespace Views
    Public Class ExportReadyEventArgs
        Inherits EventArgs
        Public Property FileName As String
        Public Property FileBytes As Byte()
    End Class

    Public Interface IVatReturnView
        Property SelectedYear As Integer
        Property SelectedPeriod As Integer
        Property SelectedFormType As VatReturnFormType
        Property TotalSalesDisplay As String
        Property TotalPurchasesDisplay As String
        Property OutputVatDisplay As String
        Property InputVatDisplay As String
        Property NetVatPayableDisplay As String
        Property FilingStatusText As String
        Property WhatThisMeansText As String
        Property CanFile As Boolean
        Property CanAmend As Boolean
        Property CanExport As Boolean
        Property IsLoading As Boolean
        Property ErrorMessage As String

        Sub SetAvailableYears(years As IEnumerable(Of Integer))
        Sub SetAvailablePeriods(periods As IEnumerable(Of Integer))
        Sub SetVatReturnLines(lines As IEnumerable(Of VatReturnLine))
        Sub SetVatReturnHistory(returns As IEnumerable(Of VatReturn))

        Event PeriodChanged As EventHandler
        Event FormTypeChanged As EventHandler
        Event GenerateRequested As EventHandler
        Event FileRequested As EventHandler
        Event AmendRequested As EventHandler
        Event ExportCsvRequested As EventHandler
        Event ExportPdfRequested As EventHandler
        Event ReturnSelected As EventHandler(Of Integer)
        Event ViewLoaded As EventHandler
        Event ExportReady As EventHandler(Of ExportReadyEventArgs)
        
        Sub PromptToSaveExport(fileName As String, fileBytes As Byte())
    End Interface
End Namespace
