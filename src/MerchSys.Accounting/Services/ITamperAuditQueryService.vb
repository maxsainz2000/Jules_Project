Imports System.Threading
Imports System.Threading.Tasks
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities

Namespace Services

    Public Interface ITamperAuditQueryService
        Function GetIncidentsAsync(Optional limit As Integer = 50, Optional cancellationToken As CancellationToken = Nothing) As Task(Of List(Of TamperAuditEntry))
        Function CountByKindAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of Dictionary(Of String, Integer))
    End Interface

    Public Class TamperAuditQueryService
        Implements ITamperAuditQueryService

        Private ReadOnly _dbContext As AccountingDbContext

        Public Sub New(dbContext As AccountingDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function GetIncidentsAsync(Optional limit As Integer = 50, Optional cancellationToken As CancellationToken = Nothing) As Task(Of List(Of TamperAuditEntry)) Implements ITamperAuditQueryService.GetIncidentsAsync
            Return Await _dbContext.TamperAuditEntries.
                OrderByDescending(Function(x) x.DetectedAt).
                Take(limit).
                ToListAsync(cancellationToken)
        End Function

        Public Async Function CountByKindAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of Dictionary(Of String, Integer)) Implements ITamperAuditQueryService.CountByKindAsync
            Dim groupings = Await _dbContext.TamperAuditEntries.
                GroupBy(Function(x) x.TamperKind).
                Select(Function(g) New With { .Kind = g.Key, .Count = g.Count() }).
                ToListAsync(cancellationToken)

            Return groupings.ToDictionary(Function(k) k.Kind, Function(v) v.Count)
        End Function

    End Class

End Namespace
