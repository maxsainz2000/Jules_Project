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
