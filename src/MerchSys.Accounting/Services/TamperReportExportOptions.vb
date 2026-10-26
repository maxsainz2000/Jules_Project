Imports System
Imports System.Collections.Generic
Imports MerchSys.Accounting.Presenters

Namespace Services

    Public Class TamperReportExportOptions
        Public Property BusinessName As String
        Public Property BusinessAddress As String
        Public Property BusinessTIN As String
        Public Property Title As String
        Public Property FromDate As DateTime
        Public Property ToDate As DateTime
        Public Property Incidents As IEnumerable(Of TamperAuditEntryDto)
    End Class

End Namespace
