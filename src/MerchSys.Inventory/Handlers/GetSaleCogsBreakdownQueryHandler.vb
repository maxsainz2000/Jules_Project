Imports System.Threading
Imports System.Threading.Tasks
Imports System.Collections.Generic
Imports System.Linq
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.SharedKernel.Queries
Imports MerchSys.Inventory.Data

Namespace Handlers
    Public Class GetSaleCogsBreakdownQueryHandler
        Implements IRequestHandler(Of GetSaleCogsBreakdownQuery, List(Of SaleCogsBreakdownDto))

        Private ReadOnly _dbContext As InventoryDbContext

        Public Sub New(dbContext As InventoryDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function Handle(request As GetSaleCogsBreakdownQuery, cancellationToken As CancellationToken) As Task(Of List(Of SaleCogsBreakdownDto)) Implements IRequestHandler(Of GetSaleCogsBreakdownQuery, List(Of SaleCogsBreakdownDto)).Handle
            Return Await _dbContext.SaleCogsRecords.AsNoTracking().
                Where(Function(r) r.SaleId = request.SaleId).
                Select(Function(r) New SaleCogsBreakdownDto With {
                    .SaleId = r.SaleId,
                    .ProductId = r.ProductId,
                    .BatchId = r.BatchId,
                    .QuantityDeducted = r.QuantityDeducted,
                    .UnitCost = r.UnitCost,
                    .TotalCost = r.TotalCost,
                    .DeductedAt = r.DeductedAt
                }).
                ToListAsync(cancellationToken)
        End Function
    End Class
End Namespace
