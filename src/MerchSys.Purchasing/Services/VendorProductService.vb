Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Purchasing.Data
Imports MerchSys.Purchasing.Entities
Imports System.Threading.Tasks
Imports System
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Data

Namespace Services
    Public Class VendorProductService
        Implements IVendorProductService

        Private ReadOnly _dbContext As PurchasingDbContext
        Private ReadOnly _sessionService As ISessionService

        Public Sub New(dbContext As PurchasingDbContext, sessionService As ISessionService)
            _dbContext = dbContext
            _sessionService = sessionService
        End Sub

        Private Sub EnforceManagerRole()
            If _sessionService.CurrentUserRole <> UserRole.Manager AndAlso _sessionService.CurrentUserRole <> UserRole.Developer Then
                Throw New UnauthorizedAccessException("Only managers can modify vendor product catalog.")
            End If
        End Sub

        Public Async Function GetHistoryAsync(request As MerchSys.SharedKernel.Paging.PageRequest) As Task(Of MerchSys.SharedKernel.Paging.PagedResult(Of VendorProduct)) Implements IVendorProductService.GetHistoryAsync
            Dim result As New MerchSys.SharedKernel.Paging.PagedResult(Of VendorProduct)()

            ' Plain EF query. The raw SQL this replaced existed only for the "EF Core ToListAsync
            ' returns an empty list" workaround, which was retested against MariaDB on 2026-10-12
            ' and did not reproduce (see AGENTS.md). Soft-deleted rows are now excluded by
            ' BaseDbContext's global query filter, which the raw SQL did not do.
            Dim query = _dbContext.VendorProducts.AsQueryable()

            If Not request.IsFirstPage Then
                If Not request.CursorDate.HasValue OrElse Not request.CursorId.HasValue Then
                    ' No cursor to page from; the raw SQL matched no rows in this case either.
                    result.HasMore = False
                    Return result
                End If

                Dim cursorDate = request.CursorDate.Value
                Dim cursorId = request.CursorId.Value
                query = query.Where(Function(x) x.CreatedAt < cursorDate OrElse
                                             (x.CreatedAt = cursorDate AndAlso x.Id < cursorId))
            End If

            result.Items = Await query.
                OrderByDescending(Function(x) x.CreatedAt).
                ThenByDescending(Function(x) x.Id).
                Take(request.PageSize + 1).
                ToListAsync()

            If result.Items.Count > request.PageSize Then
                result.HasMore = True
                result.Items.RemoveAt(result.Items.Count - 1)
            Else
                result.HasMore = False
            End If

            If result.Items.Count > 0 Then
                Dim lastItem = result.Items(result.Items.Count - 1)
                result.NextCursorDate = lastItem.CreatedAt
                result.NextCursorId = lastItem.Id
            End If

            Return result
        End Function

        Public Async Function AddCatalogEntryAsync(vendorId As Integer, productId As Integer, unitCost As Decimal, notes As String) As Task(Of VendorProduct) Implements IVendorProductService.AddCatalogEntryAsync
            EnforceManagerRole()

            ' Policy B: Friendly duplicate handling & auto-restore
            Dim existingEntry = Await _dbContext.VendorProducts.
                IgnoreQueryFilters().
                FirstOrDefaultAsync(Function(vp) vp.VendorId = vendorId AndAlso vp.ProductId = productId)

            If existingEntry IsNot Nothing Then
                If existingEntry.IsDeleted Then
                    ' Auto-restore the soft-deleted entry
                    existingEntry.IsDeleted = False
                    existingEntry.UnitCost = unitCost
                    existingEntry.Notes = notes
                    existingEntry.ModifiedAt = DateTime.UtcNow
                    existingEntry.ModifiedBy = _sessionService.CurrentUsername

                    Await _dbContext.SaveChangesAsync()
                    Return existingEntry
                Else
                    Throw New Exception("This product is already in the vendor's catalog.")
                End If
            End If

            Dim newEntry As New VendorProduct With {
                .VendorId = vendorId,
                .ProductId = productId,
                .UnitCost = unitCost,
                .Notes = notes,
                .CreatedAt = DateTime.UtcNow,
                .CreatedBy = _sessionService.CurrentUsername
            }

            _dbContext.VendorProducts.Add(newEntry)
            Await _dbContext.SaveChangesAsync()

            Return newEntry
        End Function

        Public Async Function UpdateCatalogEntryAsync(id As Integer, unitCost As Decimal, notes As String) As Task(Of VendorProduct) Implements IVendorProductService.UpdateCatalogEntryAsync
            EnforceManagerRole()

            Dim entry = Await _dbContext.VendorProducts.FindAsync(id)
            If entry Is Nothing Then Throw New Exception("Vendor product not found.")

            entry.UnitCost = unitCost
            entry.Notes = notes
            entry.ModifiedAt = DateTime.UtcNow
            entry.ModifiedBy = _sessionService.CurrentUsername

            Await _dbContext.SaveChangesAsync()
            Return entry
        End Function

        Public Async Function DeleteCatalogEntryAsync(id As Integer) As Task Implements IVendorProductService.DeleteCatalogEntryAsync
            EnforceManagerRole()

            Dim entry = Await _dbContext.VendorProducts.FindAsync(id)
            If entry Is Nothing Then Throw New Exception("Vendor product not found.")

            entry.IsDeleted = True
            entry.ModifiedAt = DateTime.UtcNow
            entry.ModifiedBy = _sessionService.CurrentUsername

            Await _dbContext.SaveChangesAsync()
        End Function
    End Class
End Namespace
