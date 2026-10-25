Imports System.Collections.Generic
Imports System.Threading
Imports System.Threading.Tasks
Imports MerchSys.Accounting.Entities

Namespace Services

    Public Interface ITamperAuditQueryService
        Function GetIncidentsAsync(Optional limit As Integer = 50, Optional cancellationToken As CancellationToken = Nothing) As Task(Of List(Of TamperAuditEntry))
        Function CountByKindAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of Dictionary(Of String, Integer))
    End Interface

End Namespace
