Imports MediatR
Imports MerchSys.SharedKernel.Enums

Namespace Events

    ''' <summary>
    ''' Published when a sale is completed, containing VAT breakdowns at both the header and line-item level.
    ''' </summary>
    Public Class SaleCompletedWithVatEvent
        Implements INotification

        Public Property SaleId As Integer
        Public Property VatableSales As Decimal
        Public Property VatExemptSales As Decimal
        Public Property ZeroRatedSales As Decimal
        Public Property OutputVat As Decimal
        Public Property IsVatRegistered As Boolean
        Public Property Items As List(Of SaleItemWithVat) = New List(Of SaleItemWithVat)()

        Public Class SaleItemWithVat
            Public Property ProductId As Integer
            Public Property Treatment As VatTreatment
            Public Property VatableAmount As Decimal
            Public Property VatExemptAmount As Decimal
            Public Property ZeroRatedAmount As Decimal
            Public Property OutputVat As Decimal
        End Class
    End Class

End Namespace
