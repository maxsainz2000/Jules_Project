Imports MediatR

Namespace Queries

    ''' <summary>
    ''' Asks Accounting what the recorded unit cost was for each line of a sale.
    '''
    ''' Inventory needs this to value returned goods. The cost cannot come from the sale line itself —
    ''' <c>SalesTransactionLine</c> has no cost field, only <c>UnitPrice</c>, which is the selling price
    ''' — and it cannot be recomputed from current stock, because that returns the cost of the batches
    ''' still on hand rather than the ones the sale actually consumed. Accounting holds the authoritative
    ''' figure in <c>RevenueRecord.UnitCost</c>, written when the stock was deducted.
    ''' </summary>
    Public Class GetSaleLineCostsQuery
        Implements IRequest(Of GetSaleLineCostsResult)

        Public Property SaleId As Integer

    End Class

End Namespace
