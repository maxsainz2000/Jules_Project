Imports MerchSys.POS.Entities
Imports MerchSys.SharedKernel.Enums

Namespace Services
    Public Class VatCalculator
        Implements IVatCalculator

        Public Function Decompose(grossAmount As Decimal, treatment As VatTreatment, vatRate As Decimal, isVatRegistered As Boolean) As VatBreakdown Implements IVatCalculator.Decompose
            Dim breakdown As New VatBreakdown()

            If Not isVatRegistered Then
                breakdown.VatExemptAmount = grossAmount
                Return breakdown
            End If

            Select Case treatment
                Case VatTreatment.Vatable
                    breakdown.VatableAmount = Math.Round(grossAmount / (1D + vatRate), 2, MidpointRounding.ToEven)
                    breakdown.OutputVat = grossAmount - breakdown.VatableAmount
                Case VatTreatment.Exempt
                    breakdown.VatExemptAmount = grossAmount
                Case VatTreatment.ZeroRated
                    breakdown.ZeroRatedAmount = grossAmount
            End Select

            Return breakdown
        End Function

        Public Function CalculateLine(line As SalesTransactionLine, vatRate As Decimal, isVatRegistered As Boolean) As VatBreakdown Implements IVatCalculator.CalculateLine
            Return Decompose(line.LineTotal, line.Treatment, vatRate, isVatRegistered)
        End Function

        Public Function AggregateTransaction(transaction As SalesTransaction, vatRate As Decimal, isVatRegistered As Boolean) As TransactionVatTotals Implements IVatCalculator.AggregateTransaction
            Dim totals As New TransactionVatTotals()

            For Each line In transaction.Lines
                Dim lineBreakdown = CalculateLine(line, vatRate, isVatRegistered)
                totals.VatableSales += lineBreakdown.VatableAmount
                totals.VatExemptSales += lineBreakdown.VatExemptAmount
                totals.ZeroRatedSales += lineBreakdown.ZeroRatedAmount
                totals.TotalOutputVat += lineBreakdown.OutputVat
            Next

            Return totals
        End Function
    End Class
End Namespace
