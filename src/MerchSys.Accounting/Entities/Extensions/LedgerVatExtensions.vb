Namespace Entities
    Partial Public Class RevenueRecord
        Public Property VatableAmount As Decimal
        Public Property VatExemptAmount As Decimal
        Public Property ZeroRatedAmount As Decimal
        Public Property OutputVat As Decimal
        Public Property InputVat As Decimal
        Public Property VatTreatment As String
    End Class

    Partial Public Class ExpenseRecord
        Public Property VatableAmount As Decimal
        Public Property VatExemptAmount As Decimal
        Public Property ZeroRatedAmount As Decimal
        Public Property OutputVat As Decimal
        Public Property InputVat As Decimal
        Public Property VatTreatment As String
    End Class
End Namespace