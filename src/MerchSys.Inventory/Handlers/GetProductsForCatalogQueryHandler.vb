Imports System.Threading
Imports System.Threading.Tasks
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Inventory.Data
Imports MerchSys.SharedKernel.Queries

Namespace Handlers

    Public Class GetProductsForCatalogQueryHandler
        Implements IRequestHandler(Of GetProductsForCatalogQuery, List(Of CatalogProductDto))

        Private ReadOnly _dbContext As InventoryDbContext

        Public Sub New(dbContext As InventoryDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function Handle(request As GetProductsForCatalogQuery, cancellationToken As CancellationToken) As Task(Of List(Of CatalogProductDto)) Implements IRequestHandler(Of GetProductsForCatalogQuery, List(Of CatalogProductDto)).Handle
            ' Plain EF query. The raw reader this replaced existed only for the "EF Core ToListAsync
            ' returns an empty list" workaround, retested against MariaDB on 2026-10-12 and not
            ' reproducible (see AGENTS.md). Soft-deleted rows are excluded by the global query filter.
            Return Await _dbContext.Products.
                Select(Function(p) New CatalogProductDto With {
                    .Id = p.Id,
                    .Name = p.Name,
                    .RetailPrice = p.RetailPrice
                }).
                ToListAsync(cancellationToken)
        End Function

    End Class

End Namespace
