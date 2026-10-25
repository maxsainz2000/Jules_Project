Imports System.Threading
Imports MerchSys.Accounting.Entities

Namespace Services

    Public Class VatEnrichedFinancialOverviewService
        Implements IFinancialOverviewService

        Private ReadOnly _inner As IFinancialOverviewService
        Private ReadOnly _kpiProviders As IEnumerable(Of IKpiProvider)

        Public Sub New(inner As IFinancialOverviewService, kpiProviders As IEnumerable(Of IKpiProvider))
            _inner = inner
            _kpiProviders = kpiProviders
        End Sub

        Public Async Function GetOverviewAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of FinancialOverviewDto) Implements IFinancialOverviewService.GetOverviewAsync
            Dim dto = Await _inner.GetOverviewAsync(cancellationToken)

            If _kpiProviders IsNot Nothing Then
                For Each provider In _kpiProviders
                    Dim kpi = Await provider.GetKpiAsync(cancellationToken)
                    If kpi IsNot Nothing AndAlso kpi.Key = "VatPayable" Then
                        dto.VatPayable = kpi.Amount
                        dto.VatPayableLabel = kpi.Label
                        dto.VatFilingDueDate = kpi.DueDate
                        dto.VatPayableSeverity = kpi.Severity
                        dto.IsVatRegistered = True ' Implicit if we are fetching VAT
                        ' Could add specific period desc if provided, using a generic description for now based on due date
                        If kpi.DueDate.HasValue Then
                            dto.VatPeriodDescription = $"Due by {kpi.DueDate.Value:MMM dd, yyyy}"
                        Else
                            dto.VatPeriodDescription = "N/A"
                        End If
                    End If
                Next
            End If

            Return dto
        End Function

        Public Function RefreshSnapshotAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of FinancialSnapshot) Implements IFinancialOverviewService.RefreshSnapshotAsync
            Return _inner.RefreshSnapshotAsync(cancellationToken)
        End Function
    End Class

End Namespace
