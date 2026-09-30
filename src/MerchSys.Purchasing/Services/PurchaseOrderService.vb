Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Purchasing.Data
Imports MerchSys.Purchasing.Entities
Imports System.Threading.Tasks
Imports MerchSys.SharedKernel.Paging
Imports MerchSys.SharedKernel.Enums
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
            Dim conn = DirectCast(_dbContext.Database.GetDbConnection(), MySqlConnector.MySqlConnection)
            Await _dbContext.Database.OpenConnectionAsync()

            Using cmd = conn.CreateCommand()
                cmd.CommandText = "SELECT Id, OrderNumber, VendorId, Status, Notes, ExpectedDeliveryDate, TotalAmount, CreatedAt, CreatedBy, ModifiedAt, ModifiedBy, IsDeleted FROM Pur_PurchaseOrders WHERE @IsFirstPage = 1 OR (CreatedAt < @CursorDate OR (CreatedAt = @CursorDate AND Id < @CursorId)) ORDER BY CreatedAt DESC, Id DESC LIMIT @PageSizePlusOne"

                cmd.Parameters.Add(New MySqlConnector.MySqlParameter("@IsFirstPage", request.IsFirstPage))
                cmd.Parameters.Add(New MySqlConnector.MySqlParameter("@CursorDate", If(request.CursorDate, CObj(DBNull.Value))))
                cmd.Parameters.Add(New MySqlConnector.MySqlParameter("@CursorId", If(request.CursorId, CObj(DBNull.Value))))
                cmd.Parameters.Add(New MySqlConnector.MySqlParameter("@PageSizePlusOne", request.PageSize + 1))

                Using reader = Await cmd.ExecuteReaderAsync()
                    While Await reader.ReadAsync()
                        Dim po As New PurchaseOrder With {
                            .Id = reader.GetInt32(0),
                            .OrderNumber = If(reader.IsDBNull(1), Nothing, reader.GetString(1)),
                            .VendorId = reader.GetInt32(2),
                            .Status = CType(reader.GetInt32(3), PurchaseOrderStatus),
                            .Notes = If(reader.IsDBNull(4), Nothing, reader.GetString(4)),
                            .ExpectedDeliveryDate = If(reader.IsDBNull(5), CType(Nothing, DateTime?), reader.GetDateTime(5)),
                            .TotalAmount = reader.GetDecimal(6),
                            .CreatedAt = reader.GetDateTime(7),
                            .CreatedBy = If(reader.IsDBNull(8), Nothing, reader.GetString(8)),
                            .ModifiedAt = If(reader.IsDBNull(9), CType(Nothing, DateTime?), reader.GetDateTime(9)),
                            .ModifiedBy = If(reader.IsDBNull(10), Nothing, reader.GetString(10)),
                            .IsDeleted = reader.GetBoolean(11)
                        }
                        result.Items.Add(po)
                    End While
                End Using
            End Using

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

            Await _dbContext.SaveChangesAsync()
            Return po
        End Function

        Public Async Function SubmitAsync(id As Integer) As Task Implements IPurchaseOrderService.SubmitAsync
            Dim po = Await _dbContext.Set(Of PurchaseOrder)().FindAsync(id)
            If po Is Nothing Then Throw New InvalidOperationException("Purchase order not found.")
            If po.Status <> PurchaseOrderStatus.Draft Then Throw New InvalidOperationException("Can only submit draft purchase orders.")

            po.Status = PurchaseOrderStatus.Submitted
            Await _dbContext.SaveChangesAsync()
        End Function

        Public Async Function ReceiveAsync(id As Integer) As Task Implements IPurchaseOrderService.ReceiveAsync
            Dim po = Await _dbContext.Set(Of PurchaseOrder)().FindAsync(id)
            If po Is Nothing Then Throw New InvalidOperationException("Purchase order not found.")
            If po.Status <> PurchaseOrderStatus.Submitted Then Throw New InvalidOperationException("Can only receive submitted purchase orders.")

            po.Status = PurchaseOrderStatus.Received
            Await _dbContext.SaveChangesAsync()
        End Function

        Public Async Function VerifyAsync(id As Integer) As Task Implements IPurchaseOrderService.VerifyAsync
            Dim po = Await _dbContext.Set(Of PurchaseOrder)().FindAsync(id)
            If po Is Nothing Then Throw New InvalidOperationException("Purchase order not found.")
            If po.Status <> PurchaseOrderStatus.Received Then Throw New InvalidOperationException("Can only verify received purchase orders.")

            po.Status = PurchaseOrderStatus.Verified
            Await _dbContext.SaveChangesAsync()
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

            Await _dbContext.SaveChangesAsync()
        End Function

        Public Async Function AddLineAsync(id As Integer, line As CreatePOLineDto) As Task(Of PurchaseOrderLine) Implements IPurchaseOrderService.AddLineAsync
            Dim po = Await _dbContext.Set(Of PurchaseOrder)().FindAsync(id)
            If po Is Nothing Then Throw New InvalidOperationException("Purchase order not found.")
            If po.Status <> PurchaseOrderStatus.Draft Then Throw New InvalidOperationException("Can only add lines to draft purchase orders.")

            Dim newLine As New PurchaseOrderLine With {
                .PurchaseOrderId = id,
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
                Await _dbContext.SaveChangesAsync()
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
            Await _dbContext.SaveChangesAsync()
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
