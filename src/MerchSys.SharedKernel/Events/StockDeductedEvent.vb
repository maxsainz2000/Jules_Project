Imports MediatR

Namespace Events

    ''' <summary>
    ''' Published by the Inventory module after a sale's stock has been deducted, carrying the true
    ''' FIFO cost of the units actually consumed.
    '''
    ''' This event exists because the cost is only knowable at deduction time. Nothing in the model
    ''' records which batches a sale consumed, so once the batches have been decremented the
    ''' information is gone and no later query can recover it. Accounting consumes this to book COGS.
    '''
    ''' It is published from Inventory rather than carrying cost on <see cref="SaleCompletedEvent"/>
    ''' because POS raises that event and has no knowledge of batch costs — only Inventory does.
    ''' </summary>
    Public Class StockDeductedEvent
        Implements INotification

        Public Property SaleId As Integer
        Public Property DeductedAt As DateTime
        Public Property Items As List(Of StockDeductedItem) = New List(Of StockDeductedItem)()

        Public Class StockDeductedItem
            Public Property ProductId As Integer
            Public Property QuantityDeducted As Integer
            Public Property TotalCost As Decimal
        End Class

    End Class

End Namespace
