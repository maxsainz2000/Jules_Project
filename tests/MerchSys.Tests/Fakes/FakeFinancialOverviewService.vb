Imports System.Threading
Imports MerchSys.Accounting.Services
Imports MerchSys.Accounting.Entities

Namespace Fakes

    Public Class FakeFinancialOverviewService
        Implements IFinancialOverviewService

        Public Async Function GetOverviewAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of FinancialOverviewDto) Implements IFinancialOverviewService.GetOverviewAsync
            Dim dto As New FinancialOverviewDto With {
                .TodayRevenue = 5000,
                .SnapshotDate = DateTime.UtcNow
            }
            Return Await Task.FromResult(dto)
        End Function

        Public Async Function RefreshSnapshotAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of FinancialSnapshot) Implements IFinancialOverviewService.RefreshSnapshotAsync
            Return Await Task.FromResult(New FinancialSnapshot())
        End Function
    End Class

End Namespace
