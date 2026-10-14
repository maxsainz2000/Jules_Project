Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Purchasing.Data
Imports MerchSys.Purchasing.Entities
Imports System.Threading.Tasks
Imports MerchSys.SharedKernel.Paging
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Data
Imports MerchSys.SharedKernel.Exceptions
Imports MerchSys.Purchasing.Helpers
Imports System

Namespace Services
    Public Class PurchaseOrderService
        Implements IPurchaseOrderService

        Private ReadOnly _dbContext As PurchasingDbContext

        Public Sub New(dbContext As PurchasingDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function GetHistoryAsync(request As PageRequest) As Task(Of PagedResult(Of PurchaseOrder)) Implements IPurchaseOrderService.GetHistoryAsync
            Dim result As New PagedResult(Of PurchaseOrder)()

            ' Plain EF query. The raw SQL this replaced existed only for the "EF Core ToListAsync
            ' returns an empty list" workaround, which was retested against MariaDB on 2026-10-12
            ' and did not reproduce (see AGENTS.md). Soft-deleted rows are now excluded by
            ' BaseDbContext's global query filter, which the raw SQL did not do.
            Dim query = _dbContext.PurchaseOrders.AsQueryable()

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

        Public Async Function CreateDraftAsync(vendorId As Integer, Optional notes As String = Nothing, Optional expectedDeliveryDate As DateTime? = Nothing) As Task(Of PurchaseOrder) Implements IPurchaseOrderService.CreateDraftAsync
            Dim year = DateTime.UtcNow.Year
            Dim prefix = "PO"
            Dim prefixFilter = $"{prefix}-{year.ToString("0000")}-"

            Dim existingOrders = Await _dbContext.Set(Of PurchaseOrder)().
                IgnoreQueryFilters().
                Where(Function(p) p.OrderNumber.StartsWith(prefixFilter)).
                Select(Function(p) p.OrderNumber).
                ToListAsync()

            Dim newOrderNumber = SequentialNumberGenerator.Generate(prefix, year, existingOrders)

            Dim newPo As New PurchaseOrder With {
                .OrderNumber = newOrderNumber,
                .VendorId = vendorId,
                .Status = PurchaseOrderStatus.Draft,
                .Notes = notes,
                .ExpectedDeliveryDate = expectedDeliveryDate,
                .TotalAmount = 0
            }

            _dbContext.Set(Of PurchaseOrder)().Add(newPo)
            Await _dbContext.SaveChangesAsync()

            Return newPo
        End Function

        Public Async Function UpdateDraftAsync(id As Integer, vendorId As Integer, Optional notes As String = Nothing, Optional expectedDeliveryDate As DateTime? = Nothing) As Task(Of PurchaseOrder) Implements IPurchaseOrderService.UpdateDraftAsync
            Dim po = Await _dbContext.Set(Of PurchaseOrder)().FindAsync(id)
            If po Is Nothing Then Throw New InvalidOperationException("Purchase order not found.")
            If po.Status <> PurchaseOrderStatus.Draft Then Throw New InvalidOperationException("Only draft purchase orders can be updated.")

            po.VendorId = vendorId
            If notes IsNot Nothing Then po.Notes = notes
            If expectedDeliveryDate IsNot Nothing Then po.ExpectedDeliveryDate = expectedDeliveryDate

            Await SaveOrReportConflictAsync("updating it")
            Return po
        End Function

        Public Async Function SubmitAsync(id As Integer) As Task Implements IPurchaseOrderService.SubmitAsync
            Dim po = Await _dbContext.Set(Of PurchaseOrder)().FindAsync(id)
            If po Is Nothing Then Throw New InvalidOperationException("Purchase order not found.")
            If po.Status <> PurchaseOrderStatus.Draft Then Throw New InvalidOperationException("Can only submit draft purchase orders.")

            po.Status = PurchaseOrderStatus.Submitted
            Await SaveOrReportConflictAsync("submitting it")
        End Function

        Public Async Function ReceiveAsync(id As Integer) As Task Implements IPurchaseOrderService.ReceiveAsync
            Dim po = Await _dbContext.Set(Of PurchaseOrder)().FindAsync(id)
            If po Is Nothing Then Throw New InvalidOperationException("Purchase order not found.")
            If po.Status <> PurchaseOrderStatus.Submitted Then Throw New InvalidOperationException("Can only receive submitted purchase orders.")

            po.Status = PurchaseOrderStatus.Received
            Await SaveOrReportConflictAsync("receiving it")
        End Function

        Public Async Function VerifyAsync(id As Integer) As Task Implements IPurchaseOrderService.VerifyAsync
            Dim po = Await _dbContext.Set(Of PurchaseOrder)().FindAsync(id)
            If po Is Nothing Then Throw New InvalidOperationException("Purchase order not found.")
            If po.Status <> PurchaseOrderStatus.Received Then Throw New InvalidOperationException("Can only verify received purchase orders.")

            po.Status = PurchaseOrderStatus.Verified
            Await SaveOrReportConflictAsync("verifying it")
        End Function

        Public Async Function CloseAsync(id As Integer) As Task Implements IPurchaseOrderService.CloseAsync
            Dim po = Await _dbContext.Set(Of PurchaseOrder)().FindAsync(id)
            If po Is Nothing Then Throw New InvalidOperationException("Purchase order not found.")
            If po.Status <> PurchaseOrderStatus.Verified Then Throw New InvalidOperationException("Can only close verified purchase orders.")

            po.Status = PurchaseOrderStatus.Closed

            Dim apEntry As New AccountsPayableEntry With {
                .VendorId = po.VendorId,
                .PurchaseOrderId = po.Id,
                .InvoiceDate = DateTime.UtcNow,
                .DueDate = DateTime.UtcNow.AddDays(30),
                .TotalAmount = po.TotalAmount,
                .AmountPaid = 0,
                .IsPaid = False
            }
            _dbContext.Set(Of AccountsPayableEntry)().Add(apEntry)

            ' One SaveChanges, so the status change and the payable entry commit together — the token
            ' guard here is what stops two clients closing the same order and booking the liability
            ' twice.
            Await SaveOrReportConflictAsync("closing it")
        End Function

        Public Async Function AddLineAsync(id As Integer, line As CreatePOLineDto) As Task(Of PurchaseOrderLine) Implements IPurchaseOrderService.AddLineAsync
            Dim po = Await _dbContext.Set(Of PurchaseOrder)().FindAsync(id)
            If po Is Nothing Then Throw New InvalidOperationException("Purchase order not found.")
            If po.Status <> PurchaseOrderStatus.Draft Then Throw New InvalidOperationException("Can only add lines to draft purchase orders.")

            Dim newLine As New PurchaseOrderLine With {
                .PurchaseOrderId = id,
                .ProductId = line.ProductId,
                .ProductName = line.ProductName,
                .Quantity = line.Quantity,
                .UnitCost = line.UnitCost,
                .LineTotal = line.LineTotal
            }

            _dbContext.Set(Of PurchaseOrderLine)().Add(newLine)
            Await _dbContext.SaveChangesAsync()

            Await RecalculateTotalAsync(id)
            Return newLine
        End Function

        Public Async Function RemoveLineAsync(id As Integer, lineId As Integer) As Task Implements IPurchaseOrderService.RemoveLineAsync
            Dim po = Await _dbContext.Set(Of PurchaseOrder)().FindAsync(id)
            If po Is Nothing Then Throw New InvalidOperationException("Purchase order not found.")
            If po.Status <> PurchaseOrderStatus.Draft Then Throw New InvalidOperationException("Can only remove lines from draft purchase orders.")

            Dim line = Await _dbContext.Set(Of PurchaseOrderLine)().FindAsync(lineId)
            If line IsNot Nothing AndAlso line.PurchaseOrderId = id Then
                _dbContext.Set(Of PurchaseOrderLine)().Remove(line)
                Await _dbContext.SaveChangesAsync()
                Await RecalculateTotalAsync(id)
            End If
        End Function

        Private Async Function RecalculateTotalAsync(id As Integer) As Task
            Dim po = Await _dbContext.Set(Of PurchaseOrder)().FindAsync(id)
            If po IsNot Nothing Then
                Dim total = Await _dbContext.Set(Of PurchaseOrderLine)().
                    Where(Function(l) l.PurchaseOrderId = id).
                    SumAsync(Function(l) l.LineTotal)
                po.TotalAmount = total
                Await SaveOrReportConflictAsync("recalculating its total")
            End If
        End Function
    
        ''' <summary>
        ''' Saves and translates a lost race into the operator-facing conflict AGENTS.md requires.
        '''
        ''' The order carries a row-version token, so a write based on a stale read fails here
        ''' instead of silently overwriting the other client's change — which is the whole point of
        ''' the token. Without this translation the operator would see a raw provider error.
        ''' </summary>
        Private Async Function SaveOrReportConflictAsync(operation As String) As Task
            ' Capture the failure rather than awaiting in the Catch block (VB forbids Await there).
            Dim concurrencyFailure As DbUpdateConcurrencyException = Nothing
            Try
                Await _dbContext.SaveChangesAsync()
            Catch ex As DbUpdateConcurrencyException
                concurrencyFailure = ex
            End Try

            If concurrencyFailure IsNot Nothing Then
                Throw New ConcurrentModificationException($"Purchase order was changed by another client while {operation}.")
            End If
        End Function

        Public Async Function GetByIdAsync(id As Integer) As Task(Of PurchaseOrder) Implements IPurchaseOrderService.GetByIdAsync
            Dim items = Await _dbContext.Set(Of PurchaseOrder)().Where(Function(p) p.Id = id).Select(Function(p) New With { .Id = p.Id, .OrderNumber = p.OrderNumber, .VendorId = p.VendorId, .Status = p.Status, .Notes = p.Notes, .ExpectedDeliveryDate = p.ExpectedDeliveryDate, .TotalAmount = p.TotalAmount, .CreatedAt = p.CreatedAt, .CreatedBy = p.CreatedBy, .ModifiedAt = p.ModifiedAt, .ModifiedBy = p.ModifiedBy, .IsDeleted = p.IsDeleted }).ToListAsync()
            If items.Count = 0 Then Return Nothing
            Dim item = items(0)
            Return New PurchaseOrder With { .Id = item.Id, .OrderNumber = item.OrderNumber, .VendorId = item.VendorId, .Status = item.Status, .Notes = item.Notes, .ExpectedDeliveryDate = item.ExpectedDeliveryDate, .TotalAmount = item.TotalAmount, .CreatedAt = item.CreatedAt, .CreatedBy = item.CreatedBy, .ModifiedAt = item.ModifiedAt, .ModifiedBy = item.ModifiedBy, .IsDeleted = item.IsDeleted }
        End Function

        Public Async Function GetLinesAsync(id As Integer) As Task(Of List(Of PurchaseOrderLine)) Implements IPurchaseOrderService.GetLinesAsync
            Dim items = Await _dbContext.Set(Of PurchaseOrderLine)().Where(Function(p) p.PurchaseOrderId = id).Select(Function(p) New With { .Id = p.Id, .PurchaseOrderId = p.PurchaseOrderId, .ProductId = p.ProductId, .ProductName = p.ProductName, .Quantity = p.Quantity, .UnitCost = p.UnitCost, .LineTotal = p.LineTotal, .CreatedAt = p.CreatedAt, .CreatedBy = p.CreatedBy, .RowVersion = p.RowVersion }).ToListAsync()
            Dim ret As New List(Of PurchaseOrderLine)
            For Each item In items
                ret.Add(New PurchaseOrderLine With { .Id = item.Id, .PurchaseOrderId = item.PurchaseOrderId, .ProductId = item.ProductId, .ProductName = item.ProductName, .Quantity = item.Quantity, .UnitCost = item.UnitCost, .LineTotal = item.LineTotal, .CreatedAt = item.CreatedAt, .CreatedBy = item.CreatedBy, .RowVersion = item.RowVersion })
            Next
            Return ret
        End Function

        Public Async Function DeleteAsync(id As Integer) As Task Implements IPurchaseOrderService.DeleteAsync
            Dim po = Await _dbContext.Set(Of PurchaseOrder)().FindAsync(id)
            If po Is Nothing Then Throw New InvalidOperationException("Purchase order not found.")
            If po.Status <> PurchaseOrderStatus.Draft Then Throw New InvalidOperationException("Can only delete draft purchase orders.")
            _dbContext.Set(Of PurchaseOrder)().Remove(po)
            Await SaveOrReportConflictAsync("deleting it")
        End Function

        Public Async Function SearchAsync(term As String, status As PurchaseOrderStatus?) As Task(Of List(Of PurchaseOrder)) Implements IPurchaseOrderService.SearchAsync
            Dim query = _dbContext.Set(Of PurchaseOrder)().AsQueryable()
            If Not String.IsNullOrWhiteSpace(term) Then
                query = query.Where(Function(p) p.OrderNumber.Contains(term))
            End If
            If status.HasValue Then
                query = query.Where(Function(p) p.Status = status.Value)
            End If
            Dim items = Await query.Select(Function(p) New With { .Id = p.Id, .OrderNumber = p.OrderNumber, .VendorId = p.VendorId, .Status = p.Status, .Notes = p.Notes, .ExpectedDeliveryDate = p.ExpectedDeliveryDate, .TotalAmount = p.TotalAmount, .CreatedAt = p.CreatedAt, .CreatedBy = p.CreatedBy, .ModifiedAt = p.ModifiedAt, .ModifiedBy = p.ModifiedBy, .IsDeleted = p.IsDeleted }).ToListAsync()
            Dim ret As New List(Of PurchaseOrder)
            For Each item In items
                ret.Add(New PurchaseOrder With { .Id = item.Id, .OrderNumber = item.OrderNumber, .VendorId = item.VendorId, .Status = item.Status, .Notes = item.Notes, .ExpectedDeliveryDate = item.ExpectedDeliveryDate, .TotalAmount = item.TotalAmount, .CreatedAt = item.CreatedAt, .CreatedBy = item.CreatedBy, .ModifiedAt = item.ModifiedAt, .ModifiedBy = item.ModifiedBy, .IsDeleted = item.IsDeleted })
            Next
            Return ret
        End Function
End Class
End Namespace
