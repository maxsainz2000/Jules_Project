Imports System.Threading
Imports System.Threading.Tasks
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Accounting.Data
Imports MerchSys.SharedKernel.Queries

Namespace Handlers

    ''' <summary>
    ''' Reports the recorded unit cost for each line of a sale.
    '''
    ''' Accounting holds the only authoritative figure: <c>RevenueRecord.UnitCost</c> is written from the
    ''' cost of the batches the sale actually consumed, at the moment they were deducted. Inventory asks
    ''' for it when valuing returned goods, because nothing else can supply it — the sale line has no
    ''' cost field, and current stock reflects different batches.
    ''' </summary>
    Public Class GetSaleLineCostsQueryHandler
        Implements IRequestHandler(Of GetSaleLineCostsQuery, GetSaleLineCostsResult)

        Private ReadOnly _dbContext As AccountingDbContext

        Public Sub New(dbContext As AccountingDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function Handle(request As GetSaleLineCostsQuery, cancellationToken As CancellationToken) As Task(Of GetSaleLineCostsResult) Implements IRequestHandler(Of GetSaleLineCostsQuery, GetSaleLineCostsResult).Handle
            Dim records = Await _dbContext.RevenueRecords.
                Where(Function(r) r.SourceTransactionId = request.SaleId).
                Select(Function(r) New With {r.ProductId, r.UnitCost}).
                ToListAsync(cancellationToken)

            Dim result As New GetSaleLineCostsResult()
            For Each record In records
                result.Costs.Add(New SaleLineCost With {
                    .ProductId = record.ProductId,
                    .UnitCost = record.UnitCost
                })
            Next

            Return result
        End Function

    End Class

End Namespace
