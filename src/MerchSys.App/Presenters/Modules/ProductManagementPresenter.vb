Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Inventory.Data
Imports MerchSys.Inventory.Entities
Imports System.Threading.Tasks
Imports System

Namespace Presenters.Modules
    Public Class ProductManagementPresenter
        Private ReadOnly _dbContext As InventoryDbContext

        Public Sub New(dbContext As InventoryDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function SaveProductAsync(sku As String) As Task(Of Product)
            ' Policy B: Friendly duplicate handling for SKU
            Dim existingProduct = Await _dbContext.Set(Of Product)().
                IgnoreQueryFilters().
                FirstOrDefaultAsync(Function(p) p.SKU = sku)

            If existingProduct IsNot Nothing Then
                If existingProduct.IsDeleted Then
                    Throw New Exception($"A deleted product already exists with the SKU '{sku}'. Please restore it or choose a different SKU.")
                Else
                    Throw New Exception($"A product already exists with the SKU '{sku}'.")
                End If
            End If

            Dim newProduct As New Product With {
                .SKU = sku
            }

            _dbContext.Set(Of Product)().Add(newProduct)
            Await _dbContext.SaveChangesAsync()

            Return newProduct
        End Function

        Public Async Function SaveCategoryAsync(name As String) As Task(Of Category)
            ' Policy B: Friendly duplicate handling for Category Name
            Dim existingCategory = Await _dbContext.Set(Of Category)().
                IgnoreQueryFilters().
                FirstOrDefaultAsync(Function(c) c.Name = name)

            If existingCategory IsNot Nothing Then
                If existingCategory.IsDeleted Then
                    Throw New Exception($"A deleted category already exists with the name '{name}'. Please restore it or choose a different name.")
                Else
                    Throw New Exception($"A category already exists with the name '{name}'.")
                End If
            End If

            Dim newCategory As New Category With {
                .Name = name
            }

            _dbContext.Set(Of Category)().Add(newCategory)
            Await _dbContext.SaveChangesAsync()

            Return newCategory
        End Function
    End Class
End Namespace
