Namespace Services.Insights

    Public Class VatPayableInsightProvider
        Implements IFinancialInsightProvider

        Public Function GenerateInsight(overview As FinancialOverviewDto) As String Implements IFinancialInsightProvider.GenerateInsight
            If overview Is Nothing OrElse Not overview.IsVatRegistered Then
                Return String.Empty
            End If

            If overview.VatPayableSeverity = KpiSeverity.Critical Then
                Return $"⚠️ CRITICAL: VAT payable of ₱{overview.VatPayable:N2} is due on {overview.VatFilingDueDate:MMM dd, yyyy}. Ensure immediate payment to avoid penalties."
            ElseIf overview.VatPayableSeverity = KpiSeverity.Warning Then
                Return $"⚠️ Warning: VAT payable of ₱{overview.VatPayable:N2} is approaching due date on {overview.VatFilingDueDate:MMM dd, yyyy}."
            Else
                Return $"Your upcoming VAT obligation is ₱{overview.VatPayable:N2}, due on {overview.VatFilingDueDate:MMM dd, yyyy}."
            End If
        End Function
    End Class

End Namespace
