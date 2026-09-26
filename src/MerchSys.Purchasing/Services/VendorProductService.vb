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
