Imports MediatR
Imports MerchSys.SharedKernel.Enums

Namespace Events

    ''' <summary>
    ''' Published when goods are received, containing VAT breakdowns for input tax purposes.
    ''' </summary>
    Public Class GoodsReceivedWithVatEvent
        Implements INotification

        Public Property SourcePurchaseOrderId As Integer
        Public Property VatableInput As Decimal
        Public Property VatExemptInput As Decimal
        Public Property ZeroRatedInput As Decimal
        Public Property InputVat As Decimal
        Public Property Items As List(Of GoodsReceivedItemWithVat) = New List(Of GoodsReceivedItemWithVat)()

        Public Class GoodsReceivedItemWithVat
            Public Property ProductId As Integer
            Public Property Treatment As VatTreatment
            Public Property VatClassification As VatTreatment
            Public Property VatableSales As Decimal
            Public Property InputVat As Decimal
            Public Property VatAmount As Decimal
        End Class
    End Class

End Namespace
