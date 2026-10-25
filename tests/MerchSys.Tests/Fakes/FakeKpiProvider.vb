Imports System.Threading
Imports MerchSys.Accounting.Services

Namespace Fakes

    Public Class FakeKpiProvider
        Implements IKpiProvider

        Public Async Function GetKpiAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of KpiValue) Implements IKpiProvider.GetKpiAsync
            Dim kpi As New KpiValue With {
                .Key = "VatPayable",
                .Label = "VAT Payable",
                .Amount = 9000D, ' 12000 Output VAT - 3000 Input VAT
                .Severity = KpiSeverity.Info,
                .DueDate = DateTime.UtcNow.AddDays(10)
            }
            Return Await Task.FromResult(kpi)
        End Function
    End Class

End Namespace
