Imports System.Threading
Imports MerchSys.Accounting.Entities
Imports MerchSys.Accounting.Enums
Imports MerchSys.Accounting.Exceptions

Namespace Services

    Public Class VatPayableKpiProvider
        Implements IKpiProvider

        Private ReadOnly _vatReportingService As IVatReportingService

        Public Sub New(vatReportingService As IVatReportingService)
            _vatReportingService = vatReportingService
        End Sub

        Public Async Function GetKpiAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of KpiValue) Implements IKpiProvider.GetKpiAsync
            Dim today = DateTime.UtcNow.Date

            Dim currentYear = today.Year
            ' We'll just look for unfiled returns. Actually the requirement says:
            ' "Determines the active period (month/quarter) and calculates net VAT payable via IVatReportingService or AccountingDbContext."
            ' "Handles VatReturnLockedException gracefully (falls back to ListVatReturnsAsync without using Await in Catch — BC36943)."

            Dim returns As List(Of VatReturn) = Nothing
            Dim vatReturn As VatReturn = Nothing
            Dim vatLockedEx As VatReturnLockedException = Nothing

            Try
                ' For a simple implementation, let's try to generate for the previous month.
                ' VAT is usually due 20th/25th of the month FOLLOWING the period.
                Dim activeDate = today.AddMonths(-1)
                vatReturn = Await _vatReportingService.GenerateVatReturnAsync(activeDate.Year, activeDate.Month, VatReturnFormType.Form2550M, cancellationToken)
            Catch ex As VatReturnLockedException
                vatLockedEx = ex
            Catch ex As Exception
                ' Ignore other exceptions, fallback
            End Try

            If vatLockedEx IsNot Nothing OrElse vatReturn Is Nothing Then
                returns = Await _vatReportingService.ListVatReturnsAsync(today.Year, cancellationToken)
                vatReturn = returns.Where(Function(r) r.FilingStatus <> VatFilingStatus.Filed).OrderByDescending(Function(r) r.PeriodEnd).FirstOrDefault()

                If vatReturn Is Nothing Then
                    returns = Await _vatReportingService.ListVatReturnsAsync(today.Year - 1, cancellationToken)
                    vatReturn = returns.Where(Function(r) r.FilingStatus <> VatFilingStatus.Filed).OrderByDescending(Function(r) r.PeriodEnd).FirstOrDefault()
                End If
            End If

            If vatReturn Is Nothing Then
                Return New KpiValue With {
                    .Key = "VatPayable",
                    .Label = "VAT Payable",
                    .Amount = 0,
                    .Severity = KpiSeverity.Info
                }
            End If

            Dim dueDate As DateTime
            If vatReturn.PeriodType = VatReturnPeriodType.Monthly Then
                dueDate = New DateTime(vatReturn.PeriodEnd.Year, vatReturn.PeriodEnd.Month, 20, 23, 59, 59, DateTimeKind.Utc).AddMonths(1)
            Else
                dueDate = New DateTime(vatReturn.PeriodEnd.Year, vatReturn.PeriodEnd.Month, 25, 23, 59, 59, DateTimeKind.Utc).AddMonths(1)
            End If

            Dim severity As KpiSeverity
            Dim daysUntilDue = (dueDate.Date - today).TotalDays

            If daysUntilDue >= 15 Then
                severity = KpiSeverity.Info
            ElseIf daysUntilDue >= 5 Then
                severity = KpiSeverity.Warning
            Else
                severity = KpiSeverity.Critical
            End If

            Return New KpiValue With {
                .Key = "VatPayable",
                .Label = "VAT Payable",
                .Amount = vatReturn.NetVatPayable,
                .DueDate = dueDate,
                .Severity = severity
            }
        End Function
    End Class

End Namespace
