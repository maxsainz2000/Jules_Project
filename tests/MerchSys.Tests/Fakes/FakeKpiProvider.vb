Imports System.Threading
Imports MerchSys.Accounting.Services

Public Class FakeKpiProvider
    Implements IKpiProvider

    Public Property OutputVat As Decimal
    Public Property InputVat As Decimal
    Public Property DueDate As DateTime?

    Public Function GetKpiAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of KpiValue) Implements IKpiProvider.GetKpiAsync
        Dim amount = OutputVat - InputVat
        Return Task.FromResult(New KpiValue With {
            .Key = "VatPayable",
            .Amount = amount,
            .DueDate = DueDate,
            .Severity = KpiSeverity.Info
        })
    End Function
End Class
