Imports MerchSys.POS.Entities
Imports MerchSys.SharedKernel.Enums

Namespace Services
    Public Class VatBreakdown
        Public Property VatableAmount As Decimal
        Public Property VatExemptAmount As Decimal
        Public Property ZeroRatedAmount As Decimal
        Public Property OutputVat As Decimal
    End Class

    Public Class TransactionVatTotals
        Public Property VatableSales As Decimal
        Public Property VatExemptSales As Decimal
        Public Property ZeroRatedSales As Decimal
        Public Property TotalOutputVat As Decimal
    End Class

    Public Interface IVatCalculator
        Function Decompose(grossAmount As Decimal, treatment As VatTreatment, vatRate As Decimal, isVatRegistered As Boolean) As VatBreakdown
        Function CalculateLine(line As SalesTransactionLine, vatRate As Decimal, isVatRegistered As Boolean) As VatBreakdown
        Function AggregateTransaction(transaction As SalesTransaction, vatRate As Decimal, isVatRegistered As Boolean) As TransactionVatTotals
    End Interface
End Namespace
