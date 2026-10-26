Imports System.Drawing
Imports MerchSys.Accounting.Services

Namespace Views

    Public Interface IVatReliefReportView
        Property SelectedYear As Integer
        Property SelectedMonth As Integer

        Property NetVatPayableText As String
        Property NetVatPayableColor As Color
        Property WhatThisMeansText As String
        Property IsLoading As Boolean
        Property ErrorMessage As String

        Sub SetAvailableYears(years As IEnumerable(Of Integer))
        Sub SetAvailableMonths(months As IEnumerable(Of Integer))
        Sub DisplaySummary(summary As VatReliefSummary)
        Sub DisplayTrailingMonths(summaries As IEnumerable(Of VatReliefSummary))

        Event ViewLoaded As EventHandler
        Event PeriodChanged As EventHandler
        Event RefreshRequested As EventHandler
    End Interface

End Namespace
