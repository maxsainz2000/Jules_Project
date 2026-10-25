Imports System.Threading
Imports MerchSys.Accounting.Services
Imports MerchSys.Accounting.Entities

Public Class FakeFinancialOverviewService
    Implements IFinancialOverviewService

    Public Function GetOverviewAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of FinancialOverviewDto) Implements IFinancialOverviewService.GetOverviewAsync
        Return Task.FromResult(New FinancialOverviewDto())
    End Function

    Public Function RefreshSnapshotAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of FinancialSnapshot) Implements IFinancialOverviewService.RefreshSnapshotAsync
        Return Task.FromResult(New FinancialSnapshot())
    End Function
End Class
