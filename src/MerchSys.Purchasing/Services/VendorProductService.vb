Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Purchasing.Data
Imports MerchSys.Purchasing.Entities
Imports System.Threading.Tasks
Imports System

Namespace Services
    Public Class VendorProductService
        Private ReadOnly _dbContext As PurchasingDbContext

        Public Sub New(dbContext As PurchasingDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function GetHistoryAsync(request As MerchSys.SharedKernel.Paging.PageRequest) As Task(Of MerchSys.SharedKernel.Paging.PagedResult(Of VendorProduct))
            Dim result As New MerchSys.SharedKernel.Paging.PagedResult(Of VendorProduct)()
            Dim conn = DirectCast(_dbContext.Database.GetDbConnection(), MySqlConnector.MySqlConnection)
            Await _dbContext.Database.OpenConnectionAsync()

            Using cmd = conn.CreateCommand()
                cmd.CommandText = "SELECT Id, VendorId, ProductId, UnitCost, Notes, CreatedAt, CreatedBy, ModifiedAt, ModifiedBy, IsDeleted FROM VendorProducts WHERE @IsFirstPage = 1 OR (CreatedAt < @CursorDate OR (CreatedAt = @CursorDate AND Id < @CursorId)) ORDER BY CreatedAt DESC, Id DESC LIMIT @PageSizePlusOne"

                cmd.Parameters.Add(New MySqlConnector.MySqlParameter("@IsFirstPage", request.IsFirstPage))
                cmd.Parameters.Add(New MySqlConnector.MySqlParameter("@CursorDate", If(request.CursorDate, CObj(DBNull.Value))))
                cmd.Parameters.Add(New MySqlConnector.MySqlParameter("@CursorId", If(request.CursorId, CObj(DBNull.Value))))
                cmd.Parameters.Add(New MySqlConnector.MySqlParameter("@PageSizePlusOne", request.PageSize + 1))

                Using reader = Await cmd.ExecuteReaderAsync()
                    While Await reader.ReadAsync()
                        Dim vp As New VendorProduct With {
                            .Id = reader.GetInt32(0),
                            .VendorId = reader.GetInt32(1),
                            .ProductId = reader.GetInt32(2),
                            .UnitCost = reader.GetDecimal(3),
                            .Notes = If(reader.IsDBNull(4), Nothing, reader.GetString(4)),
                            .CreatedAt = reader.GetDateTime(5),
                            .CreatedBy = If(reader.IsDBNull(6), Nothing, reader.GetString(6)),
                            .ModifiedAt = If(reader.IsDBNull(7), CType(Nothing, DateTime?), reader.GetDateTime(7)),
                            .ModifiedBy = If(reader.IsDBNull(8), Nothing, reader.GetString(8)),
                            .IsDeleted = reader.GetBoolean(9)
                        }
                        result.Items.Add(vp)
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

        Public Async Function AddCatalogEntryAsync(vendorId As Integer, productId As Integer, unitCost As Decimal, notes As String) As Task(Of VendorProduct)
            ' Policy B: Friendly duplicate handling & auto-restore
            Dim existingEntry = Await _dbContext.Set(Of VendorProduct)().
                IgnoreQueryFilters().
                FirstOrDefaultAsync(Function(vp) vp.VendorId = vendorId AndAlso vp.ProductId = productId)

            If existingEntry IsNot Nothing Then
                If existingEntry.IsDeleted Then
                    ' Auto-restore the soft-deleted entry
                    existingEntry.IsDeleted = False
                    existingEntry.UnitCost = unitCost
                    existingEntry.Notes = notes

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
                .Notes = notes
            }

            _dbContext.Set(Of VendorProduct)().Add(newEntry)
            Await _dbContext.SaveChangesAsync()

            Return newEntry
        End Function
    End Class
End Namespace
