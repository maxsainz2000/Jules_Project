Imports MediatR

Namespace Events

    ''' <summary>
    ''' Published when a sale is returned. One event, two consumers — Inventory restocks, Accounting
    ''' reverses the revenue and COGS.
    '''
    ''' It replaces the old <c>StockReturnedEvent</c>, which carried only a product and a quantity and
    ''' therefore could not support either consumer properly: Inventory had no cost to value the
    ''' returned goods with, and Accounting had no idea a return had happened at all, so returned sales
    ''' kept their revenue and COGS on the books.
    '''
    ''' Two quantities are carried per line on purpose:
    ''' <list type="bullet">
    ''' <item><c>QuantityReturned</c> — this return only. Inventory restocks this much.</item>
    ''' <item><c>CumulativeQuantityReturned</c> — this return plus all earlier ones. Accounting uses it
    ''' with <c>OriginalQuantitySold</c> to set the net position <em>absolutely</em> rather than
    ''' subtracting, which makes the reversal idempotent: publishing this event twice leaves the same
    ''' result instead of reversing twice.</item>
    ''' </list>
    ''' </summary>
    Public Class SaleReturnedEvent
        Implements INotification

        Public Property SaleId As Integer
        Public Property SalesReturnId As Integer
        Public Property ReturnedAt As DateTime
        Public Property TotalRefundAmount As Decimal
        Public Property IsRestocked As Boolean
        Public Property Items As List(Of ReturnedItem) = New List(Of ReturnedItem)()

        Public Class ReturnedItem
            Public Property ProductId As Integer
            Public Property QuantityReturned As Integer
            Public Property CumulativeQuantityReturned As Integer
            Public Property OriginalQuantitySold As Integer
            Public Property UnitPrice As Decimal

            ''' <summary>
            ''' The VAT figures the original sale booked for this product, for the original quantity.
            ''' Carried on the event rather than read back from the <c>RevenueRecord</c> because that
            ''' record's own VAT columns are overwritten by each reversal — using them as the base
            ''' would compound on a repeated publication. Accounting assigns
            ''' <c>original × (net ÷ original quantity)</c> so the reversal stays idempotent.
            ''' </summary>
            Public Property OriginalVatableAmount As Decimal
            Public Property OriginalVatExemptAmount As Decimal
            Public Property OriginalZeroRatedAmount As Decimal
            Public Property OriginalOutputVat As Decimal
        End Class

    End Class

End Namespace
