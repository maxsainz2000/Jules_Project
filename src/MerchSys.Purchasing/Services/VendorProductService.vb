Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Purchasing.Data
Imports MerchSys.Purchasing.Entities
Imports System.Threading.Tasks
Imports System
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.Purchasing.Dtos

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
            If _sessionService.CurrentUserRole = UserRole.Owner Then
                Throw New UnauthorizedAccessException("Owners cannot modify vendor catalogs.")
            End If
        End Sub

        Public Async Function GetHistoryAsync(request As MerchSys.SharedKernel.Paging.PageRequest) As Task(Of MerchSys.SharedKernel.Paging.PagedResult(Of VendorProduct)) Implements IVendorProductService.GetHistoryAsync
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

        Public Async Function AddCatalogEntryAsync(vendorId As Integer, productId As Integer, unitCost As Decimal, notes As String) As Task(Of VendorProduct) Implements IVendorProductService.AddCatalogEntryAsync
            EnforceManagerRole()
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

        Public Async Function UpdateCatalogEntryAsync(id As Integer, unitCost As Decimal, notes As String) As Task(Of VendorProduct) Implements IVendorProductService.UpdateCatalogEntryAsync
            EnforceManagerRole()
            Dim entry = Await _dbContext.Set(Of VendorProduct)().FindAsync(id)
            If entry Is Nothing Then Throw New InvalidOperationException("Catalog entry not found.")

            entry.UnitCost = unitCost
            entry.Notes = notes

            Await _dbContext.SaveChangesAsync()
            Return entry
        End Function

        Public Async Function DeleteCatalogEntryAsync(id As Integer) As Task Implements IVendorProductService.DeleteCatalogEntryAsync
            EnforceManagerRole()
            Dim entry = Await _dbContext.Set(Of VendorProduct)().FindAsync(id)
            If entry IsNot Nothing Then
                _dbContext.Set(Of VendorProduct)().Remove(entry)
                Await _dbContext.SaveChangesAsync()
            End If
        End Function

        Public Async Function GetCatalogForVendorAsync(vendorId As Integer) As Task(Of System.Collections.Generic.List(Of VendorProductDto)) Implements IVendorProductService.GetCatalogForVendorAsync
            Dim dtos As New System.Collections.Generic.List(Of VendorProductDto)()

            Dim conn = DirectCast(_dbContext.Database.GetDbConnection(), MySqlConnector.MySqlConnection)
            Dim wasClosed = (conn.State = System.Data.ConnectionState.Closed)
            If wasClosed Then Await _dbContext.Database.OpenConnectionAsync()

            Try
                Using cmd = conn.CreateCommand()
                    cmd.CommandText = "
                        SELECT vp.Id, vp.VendorId, vp.ProductId, p.Name, vp.UnitCost, vp.Notes
                        FROM Pur_VendorProducts vp
                        JOIN Inv_Products p ON vp.ProductId = p.Id
                        WHERE vp.VendorId = @VendorId AND vp.IsDeleted = 0
                    "
                    cmd.Parameters.Add(New MySqlConnector.MySqlParameter("@VendorId", vendorId))

                    Using reader = Await cmd.ExecuteReaderAsync()
                        While Await reader.ReadAsync()
                            dtos.Add(New VendorProductDto With {
                                .Id = reader.GetInt32(0),
                                .VendorId = reader.GetInt32(1),
                                .ProductId = reader.GetInt32(2),
                                .ProductName = reader.GetString(3),
                                .UnitCost = reader.GetDecimal(4),
                                .Notes = If(reader.IsDBNull(5), Nothing, reader.GetString(5))
                            })
                        End While
                    End Using
                End Using
            Finally
                If wasClosed Then conn.Close()
            End Try

            Return dtos
        End Function
    End Class
End Namespace
