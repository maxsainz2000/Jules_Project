Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities

Namespace Handlers

    ''' <summary>
    ''' Finds the <see cref="RevenueRecord"/> for a (sale, product) pair, creating it when absent.
    '''
    ''' Shared by <see cref="SaleCompletedAccountingHandler"/> and
    ''' <see cref="StockDeductedAccountingHandler"/>. Both write to the same row, but from different
    ''' events, and MediatR does not order handlers — so either may run first. Upserting on the same
    ''' natural key is what makes the pair order-independent; a blind Add in either one produced a
    ''' duplicate row (and double-counted revenue) whenever it ran second.
    ''' </summary>
    Friend Module RevenueRecordUpsert

        Friend Async Function FindOrCreateAsync(dbContext As AccountingDbContext, saleId As Integer, productId As Integer, cancellationToken As CancellationToken) As Task(Of RevenueRecord)
            Dim record = Await dbContext.RevenueRecords.FirstOrDefaultAsync(
                Function(r) r.SourceTransactionId = saleId AndAlso r.ProductId = productId,
                cancellationToken)

            If record Is Nothing Then
                record = New RevenueRecord With {
                    .SourceTransactionId = saleId,
                    .ProductId = productId
                }
                dbContext.RevenueRecords.Add(record)
            End If

            Return record
        End Function

    End Module

End Namespace
