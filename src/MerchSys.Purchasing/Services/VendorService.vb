Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Purchasing.Data
Imports MerchSys.Purchasing.Entities
Imports System.Threading.Tasks
Imports System
Imports System.Linq
Imports System.Collections.Generic
Imports MerchSys.SharedKernel.Data

Namespace Services
    Public Class VendorService
        Implements IVendorService

        Private ReadOnly _dbContext As PurchasingDbContext

        Public Sub New(dbContext As PurchasingDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function GetHistoryAsync(request As MerchSys.SharedKernel.Paging.PageRequest) As Task(Of MerchSys.SharedKernel.Paging.PagedResult(Of Vendor)) Implements IVendorService.GetHistoryAsync
            Dim result As New MerchSys.SharedKernel.Paging.PagedResult(Of Vendor)()

            ' Plain EF query. The raw SQL this replaced existed only for the "EF Core ToListAsync
            ' returns an empty list" workaround, which was retested against MariaDB on 2026-10-12
            ' and did not reproduce (see AGENTS.md). Soft-deleted rows are now excluded by
            ' BaseDbContext's global query filter, which the raw SQL did not do.
            Dim query = _dbContext.Vendors.AsQueryable()

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

        Public Async Function CreateAsync(dto As CreateVendorDto) As Task(Of VendorDetailDto) Implements IVendorService.CreateAsync
            ' Policy B: Friendly duplicate handling. Check against all (including soft-deleted).
            Dim name = dto.Name
            Dim existingVendor = Await _dbContext.Set(Of Vendor)().
                IgnoreQueryFilters().
                FirstOrDefaultAsync(Function(v) v.Name = name)

            If existingVendor IsNot Nothing Then
                If existingVendor.IsDeleted Then
                    Throw New Exception($"A deleted vendor already exists with the name '{name}'. Please restore it or choose a different name.")
                Else
                    Throw New Exception($"A vendor already exists with the name '{name}'.")
                End If
            End If

            Dim newVendor As New Vendor With {
                .Name = name,
                .Phone = dto.Phone,
                .LeadTimeDays = dto.LeadTimeDays
            }

            _dbContext.Set(Of Vendor)().Add(newVendor)
            Await _dbContext.SaveChangesAsync()

            Return New VendorDetailDto With {
                .Id = newVendor.Id,
                .Name = newVendor.Name,
                .Phone = newVendor.Phone,
                .LeadTimeDays = newVendor.LeadTimeDays,
                .IsDeleted = newVendor.IsDeleted
            }
        End Function

        Public Async Function UpdateAsync(id As Integer, dto As UpdateVendorDto) As Task(Of VendorDetailDto) Implements IVendorService.UpdateAsync
            ' Policy B: Friendly duplicate handling
            Dim newName = dto.Name
            Dim existingDuplicate = Await _dbContext.Set(Of Vendor)().
                IgnoreQueryFilters().
                FirstOrDefaultAsync(Function(v) v.Name = newName AndAlso v.Id <> id)

            If existingDuplicate IsNot Nothing Then
                If existingDuplicate.IsDeleted Then
                    Throw New Exception($"A deleted vendor already exists with the name '{newName}'. Please restore it or choose a different name.")
                Else
                    Throw New Exception($"A vendor already exists with the name '{newName}'.")
                End If
            End If

            Dim vendorToUpdate = Await _dbContext.Set(Of Vendor)().FindAsync(id)
            If vendorToUpdate Is Nothing Then
                Throw New Exception("Vendor not found.")
            End If

            vendorToUpdate.Name = newName
            vendorToUpdate.Phone = dto.Phone
            vendorToUpdate.LeadTimeDays = dto.LeadTimeDays
            Await _dbContext.SaveChangesAsync()

            Return New VendorDetailDto With {
                .Id = vendorToUpdate.Id,
                .Name = vendorToUpdate.Name,
                .Phone = vendorToUpdate.Phone,
                .LeadTimeDays = vendorToUpdate.LeadTimeDays,
                .IsDeleted = vendorToUpdate.IsDeleted
            }
        End Function

        Public Async Function DeleteAsync(id As Integer) As Task(Of Boolean) Implements IVendorService.DeleteAsync
            Dim vendorToDelete = Await _dbContext.Set(Of Vendor)().FindAsync(id)
            If vendorToDelete Is Nothing Then
                Return False
            End If

            vendorToDelete.IsDeleted = True
            Await _dbContext.SaveChangesAsync()
            Return True
        End Function

        Public Async Function GetByIdAsync(id As Integer) As Task(Of VendorDetailDto) Implements IVendorService.GetByIdAsync
            Dim result = Await _dbContext.Set(Of Vendor)().
                Where(Function(v) v.Id = id).
                Select(Function(v) New VendorDetailDto With {
                    .Id = v.Id,
                    .Name = v.Name,
                    .Phone = v.Phone,
                    .LeadTimeDays = v.LeadTimeDays,
                    .IsDeleted = v.IsDeleted
                }).
                FirstOrDefaultAsync()

            Return result
        End Function

        Public Async Function GetAllAsync() As Task(Of List(Of VendorDetailDto)) Implements IVendorService.GetAllAsync
            ' We project to VendorDetailDto first via Select, to avoid the VB.NET EF Core bug where ToListAsync()
            ' silently returns an empty list on full entity queries.
            Dim results = Await _dbContext.Set(Of Vendor)().
                Select(Function(v) New VendorDetailDto With {
                    .Id = v.Id,
                    .Name = v.Name,
                    .Phone = v.Phone,
                    .LeadTimeDays = v.LeadTimeDays,
                    .IsDeleted = v.IsDeleted
                }).
                ToListAsync()
            Return results
        End Function

        Public Async Function SearchAsync(searchTerm As String) As Task(Of List(Of VendorDetailDto)) Implements IVendorService.SearchAsync
            Dim query = _dbContext.Set(Of Vendor)().AsQueryable()

            If Not String.IsNullOrWhiteSpace(searchTerm) Then
                query = query.Where(Function(v) EF.Functions.Like(v.Name, $"%{searchTerm}%") OrElse EF.Functions.Like(v.Phone, $"%{searchTerm}%"))
            End If

            Dim results = Await query.
                Select(Function(v) New VendorDetailDto With {
                    .Id = v.Id,
                    .Name = v.Name,
                    .Phone = v.Phone,
                    .LeadTimeDays = v.LeadTimeDays,
                    .IsDeleted = v.IsDeleted
                }).
                ToListAsync()
            Return results
        End Function

        Public Async Function GetPurchaseHistoryAsync(vendorId As Integer) As Task(Of VendorPurchaseHistoryDto) Implements IVendorService.GetPurchaseHistoryAsync
            Dim vendorData = Await _dbContext.Set(Of Vendor)().
                Where(Function(v) v.Id = vendorId).
                Select(Function(v) New With {
                    .VendorId = v.Id,
                    .VendorName = v.Name
                }).
                FirstOrDefaultAsync()

            If vendorData Is Nothing Then
                Return Nothing
            End If

            Dim ordersData = Await _dbContext.Set(Of PurchaseOrder)().
                Where(Function(po) po.VendorId = vendorId).
                GroupBy(Function(po) po.VendorId).
                Select(Function(g) New With {
                    .TotalOrders = g.Count(),
                    .TotalAmount = g.Sum(Function(po) po.TotalAmount),
                    .LastOrderDate = g.Max(Function(po) CType(po.CreatedAt, DateTime?))
                }).
                FirstOrDefaultAsync()

            Dim recentOrders = Await _dbContext.Set(Of PurchaseOrder)().
                Where(Function(po) po.VendorId = vendorId).
                OrderByDescending(Function(po) po.CreatedAt).
                Take(10).
                Select(Function(po) New POSummaryRow With {
                    .Id = po.Id,
                    .OrderNumber = po.OrderNumber,
                    .CreatedAt = po.CreatedAt,
                    .TotalAmount = po.TotalAmount,
                    .Status = po.Status
                }).
                ToListAsync()

            Dim historyDto As New VendorPurchaseHistoryDto With {
                .VendorId = vendorData.VendorId,
                .VendorName = vendorData.VendorName,
                .TotalOrders = If(ordersData IsNot Nothing, ordersData.TotalOrders, 0),
                .TotalAmount = If(ordersData IsNot Nothing, ordersData.TotalAmount, 0D),
                .LastOrderDate = If(ordersData IsNot Nothing, ordersData.LastOrderDate, Nothing)
            }

            historyDto.RecentPOs.AddRange(recentOrders)

            Return historyDto
        End Function
    End Class
End Namespace