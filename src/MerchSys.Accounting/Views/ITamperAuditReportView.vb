Imports System
Imports System.Collections.Generic
Imports MerchSys.Accounting.Presenters

Namespace Views

    Public Interface ITamperAuditReportView
        Event ViewLoaded As EventHandler
        Event RefreshRequested As EventHandler
        Event FilterChanged As EventHandler

        Property FromDate As DateTime
        Property ToDate As DateTime
        Property HasNoEntries As Boolean
        Property IsLoading As Boolean
        Property ErrorMessage As String

        Sub SetIncidents(entries As IEnumerable(Of TamperAuditEntryDto))
    End Interface

End Namespace
