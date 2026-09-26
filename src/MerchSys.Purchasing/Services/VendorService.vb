Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Purchasing.Data
Imports MerchSys.Purchasing.Entities
Imports System.Threading.Tasks
Imports System

Namespace Services
    Public Class VendorService
        Private ReadOnly _dbContext As PurchasingDbContext

        Public Sub New(dbContext As PurchasingDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function GetHistoryAsync(request As MerchSys.SharedKernel.Paging.PageRequest) As Task(Of MerchSys.SharedKernel.Paging.PagedResult(Of Vendor))
            Dim result As New MerchSys.SharedKernel.Paging.PagedResult(Of Vendor)()
            Dim conn = DirectCast(_dbContext.Database.GetDbConnection(), MySqlConnector.MySqlConnection)
            Await _dbContext.Database.OpenConnectionAsync()

            Using cmd = conn.CreateCommand()
                cmd.CommandText = "SELECT Id, Name, CreatedAt, CreatedBy, ModifiedAt, ModifiedBy, IsDeleted FROM Vendors WHERE @IsFirstPage = 1 OR (CreatedAt < @CursorDate OR (CreatedAt = @CursorDate AND Id < @CursorId)) ORDER BY CreatedAt DESC, Id DESC LIMIT @PageSizePlusOne"

                cmd.Parameters.Add(New MySqlConnector.MySqlParameter("@IsFirstPage", request.IsFirstPage))
                cmd.Parameters.Add(New MySqlConnector.MySqlParameter("@CursorDate", If(request.CursorDate, CObj(DBNull.Value))))
                cmd.Parameters.Add(New MySqlConnector.MySqlParameter("@CursorId", If(request.CursorId, CObj(DBNull.Value))))
                cmd.Parameters.Add(New MySqlConnector.MySqlParameter("@PageSizePlusOne", request.PageSize + 1))

                Using reader = Await cmd.ExecuteReaderAsync()
                    While Await reader.ReadAsync()
                        Dim v As New Vendor With {
                            .Id = reader.GetInt32(0),
                            .Name = If(reader.IsDBNull(1), Nothing, reader.GetString(1)),
                            .CreatedAt = reader.GetDateTime(2),
                            .CreatedBy = If(reader.IsDBNull(3), Nothing, reader.GetString(3)),
                            .ModifiedAt = If(reader.IsDBNull(4), CType(Nothing, DateTime?), reader.GetDateTime(4)),
                            .ModifiedBy = If(reader.IsDBNull(5), Nothing, reader.GetString(5)),
                            .IsDeleted = reader.GetBoolean(6)
                        }
                        result.Items.Add(v)
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

        Public Async Function CreateAsync(name As String) As Task(Of Vendor)
            ' Policy B: Friendly duplicate handling. Check against all (including soft-deleted).
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
                .Name = name
            }

            _dbContext.Set(Of Vendor)().Add(newVendor)
            Await _dbContext.SaveChangesAsync()

            Return newVendor
        End Function

        Public Async Function UpdateAsync(id As Integer, newName As String) As Task(Of Vendor)
            ' Policy B: Friendly duplicate handling
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
            Await _dbContext.SaveChangesAsync()

            Return vendorToUpdate
        End Function
    End Class
End Namespace
