Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities

Namespace Services

    Public Class TamperAuditQueryService
        Implements ITamperAuditQueryService

        Private ReadOnly _dbContext As AccountingDbContext

        Public Sub New(dbContext As AccountingDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function GetIncidentsAsync(Optional limit As Integer = 50, Optional cancellationToken As CancellationToken = Nothing) As Task(Of List(Of TamperAuditEntry)) Implements ITamperAuditQueryService.GetIncidentsAsync
            Return Await _dbContext.TamperAuditEntries _
                .OrderByDescending(Function(x) x.DetectedAt) _
                .Take(limit) _
                .ToListAsync(cancellationToken)
        End Function

        Public Async Function CountByKindAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of Dictionary(Of String, Integer)) Implements ITamperAuditQueryService.CountByKindAsync
            Dim rawCounts = Await _dbContext.TamperAuditEntries _
                .GroupBy(Function(x) x.TamperKind) _
                .Select(Function(g) New With { .Kind = g.Key, .Count = g.Count() }) _
                .ToListAsync(cancellationToken)

            Return rawCounts.ToDictionary(Function(x) x.Kind, Function(x) x.Count)
        End Function

    End Class

End Namespace
