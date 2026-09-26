Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Purchasing.Data
Imports MerchSys.Purchasing.Entities
Imports System.Threading.Tasks

Namespace Services
    Public Class PurchaseOrderService
        Private ReadOnly _dbContext As PurchasingDbContext

        Public Sub New(dbContext As PurchasingDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function GetHistoryAsync(request As MerchSys.SharedKernel.Paging.PageRequest) As Task(Of MerchSys.SharedKernel.Paging.PagedResult(Of PurchaseOrder))
            Dim result As New MerchSys.SharedKernel.Paging.PagedResult(Of PurchaseOrder)()
            Dim conn = DirectCast(_dbContext.Database.GetDbConnection(), MySqlConnector.MySqlConnection)
            Await _dbContext.Database.OpenConnectionAsync()

            Using cmd = conn.CreateCommand()
                cmd.CommandText = "SELECT Id, OrderNumber, CreatedAt, CreatedBy, ModifiedAt, ModifiedBy, IsDeleted FROM PurchaseOrders WHERE @IsFirstPage = 1 OR (CreatedAt < @CursorDate OR (CreatedAt = @CursorDate AND Id < @CursorId)) ORDER BY CreatedAt DESC, Id DESC LIMIT @PageSizePlusOne"

                cmd.Parameters.Add(New MySqlConnector.MySqlParameter("@IsFirstPage", request.IsFirstPage))
                cmd.Parameters.Add(New MySqlConnector.MySqlParameter("@CursorDate", If(request.CursorDate, CObj(DBNull.Value))))
                cmd.Parameters.Add(New MySqlConnector.MySqlParameter("@CursorId", If(request.CursorId, CObj(DBNull.Value))))
                cmd.Parameters.Add(New MySqlConnector.MySqlParameter("@PageSizePlusOne", request.PageSize + 1))

                Using reader = Await cmd.ExecuteReaderAsync()
                    While Await reader.ReadAsync()
                        Dim po As New PurchaseOrder With {
                            .Id = reader.GetInt32(0),
                            .OrderNumber = If(reader.IsDBNull(1), Nothing, reader.GetString(1)),
                            .CreatedAt = reader.GetDateTime(2),
                            .CreatedBy = If(reader.IsDBNull(3), Nothing, reader.GetString(3)),
                            .ModifiedAt = If(reader.IsDBNull(4), CType(Nothing, DateTime?), reader.GetDateTime(4)),
                            .ModifiedBy = If(reader.IsDBNull(5), Nothing, reader.GetString(5)),
                            .IsDeleted = reader.GetBoolean(6)
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

        Public Async Function CreateDraftAsync() As Task(Of PurchaseOrder)
            ' Find the highest order number including soft-deleted items so we don't duplicate sequence numbers.
            ' Policy A: Sequence skips deleted (OrderNumber is deterministic auto-generated).
            ' Note: Keep pre-existing hotfix... (There wasn't one found, but we maintain deterministic increment).
            Dim maxOrderStr = Await _dbContext.Set(Of PurchaseOrder)().
                IgnoreQueryFilters().
                OrderByDescending(Function(p) p.OrderNumber).
                Select(Function(p) p.OrderNumber).
                FirstOrDefaultAsync()

            Dim maxOrderNumber As Long = 0
            If Not String.IsNullOrEmpty(maxOrderStr) Then
                Long.TryParse(maxOrderStr, maxOrderNumber)
            End If

            Dim newOrderNumber As Long = maxOrderNumber + 1

            Dim newPo As New PurchaseOrder With {
                .OrderNumber = newOrderNumber.ToString()
            }

            _dbContext.Set(Of PurchaseOrder)().Add(newPo)
            Await _dbContext.SaveChangesAsync()

            Return newPo
        End Function
    End Class
End Namespace
