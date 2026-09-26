Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Inventory.Data
Imports MerchSys.Inventory.Entities
Imports MerchSys.Inventory.Views
Imports MySqlConnector

Namespace Presenters

    Public Class ProductManagementRowItem
        Public Property Id As Integer
        Public Property CategoryId As Integer
        Public Property CategoryName As String
        Public Property Name As String
        Public Property SKU As String
        Public Property RetailPrice As Decimal
        Public Property Unit As String
        Public Property HasExpiry As Boolean
        Public Property MinimumThreshold As Integer
    End Class

    Public Class CategoryManagementItem
        Public Property Id As Integer
        Public Property Name As String
    End Class

    Public Class ProductManagementPresenter
        Private ReadOnly _view As IProductManagementView
        Private ReadOnly _db As InventoryDbContext

        Public Sub New(view As IProductManagementView, db As InventoryDbContext)
            _view = view
            _db = db

            _view.Presenter = Me

            AddHandler _view.LoadView, Async Sub(sender, e) Await HandleLoadViewAsync()
            AddHandler _view.SaveProductRequested, Async Sub(sender, e) Await SaveProductAsync(e)
            AddHandler _view.DeleteProductRequested, Async Sub(sender, e) Await DeleteProductAsync(e.Id)
            AddHandler _view.SaveCategoryRequested, Async Sub(sender, e) Await SaveCategoryAsync(e)
            AddHandler _view.DeleteCategoryRequested, Async Sub(sender, e) Await DeleteCategoryAsync(e.Id)
        End Sub

        Private Async Function HandleLoadViewAsync() As Task
            Await LoadCategoriesAsync()
            Await LoadProductsAsync()
        End Function

        Private Async Function LoadCategoriesAsync() As Task
            ' Use raw reader for EF Core ToListAsync bug workaround on full entities
            Dim categories = New List(Of CategoryManagementItem)()
            Dim connection = CType(_db.Database.GetDbConnection(), MySqlConnection)
            Dim wasClosed = connection.State = System.Data.ConnectionState.Closed
            If wasClosed Then
                Await connection.OpenAsync()
            End If

            Dim savedEx As Exception = Nothing
            Try
                Using cmd = connection.CreateCommand()
                    cmd.CommandText = "SELECT Id, Name FROM Inv_ProductCategories WHERE IsDeleted = 0"
                    Using reader = Await cmd.ExecuteReaderAsync()
                        While Await reader.ReadAsync()
                            categories.Add(New CategoryManagementItem With {
                                .Id = reader.GetInt32(0),
                                .Name = reader.GetString(1)
                            })
                        End While
                    End Using
                End Using
            Catch ex As Exception
                savedEx = ex
            End Try

            If wasClosed Then
                Await connection.CloseAsync()
            End If

            If savedEx IsNot Nothing Then
                Throw savedEx
            End If

            _view.Categories = categories
        End Function

        Private Async Function LoadProductsAsync() As Task
            ' Project to DTO so EF Core ToListAsync works without bug
            Dim products = Await _db.Products.
                Where(Function(p) Not p.IsDeleted).
                GroupJoin(_db.ProductCategories,
                    Function(p) p.ProductCategoryId,
                    Function(c) c.Id,
                    Function(p, c) New With { .p = p, .c = c }).
                SelectMany(
                    Function(x) x.c.DefaultIfEmpty(),
                    Function(x, category) New ProductManagementRowItem With {
                        .Id = x.p.Id,
                        .CategoryId = x.p.ProductCategoryId,
                        .CategoryName = If(category IsNot Nothing, category.Name, String.Empty),
                        .Name = x.p.Name,
                        .SKU = x.p.SKU,
                        .RetailPrice = x.p.RetailPrice,
                        .Unit = x.p.Unit,
                        .HasExpiry = x.p.HasExpiry,
                        .MinimumThreshold = x.p.MinimumThreshold
                    }).
                ToListAsync()

            _view.Products = products
        End Function

        Public Async Function SaveProductAsync(dto As ProductManagementRowItem) As Task
            Dim entity As Product
            If dto.Id = 0 Then
                entity = New Product()
                _db.Products.Add(entity)
            Else
                entity = Await _db.Products.FindAsync(dto.Id)
                If entity Is Nothing Then
                    Throw New Exception("Product not found")
                End If
            End If

            entity.ProductCategoryId = dto.CategoryId
            entity.Name = dto.Name
            entity.SKU = dto.SKU
            entity.RetailPrice = dto.RetailPrice
            entity.Unit = dto.Unit
            entity.HasExpiry = dto.HasExpiry
            entity.MinimumThreshold = dto.MinimumThreshold

            Await _db.SaveChangesAsync()
            Await LoadProductsAsync()
        End Function

        Public Async Function DeleteProductAsync(id As Integer) As Task
            Dim entity = Await _db.Products.FindAsync(id)
            If entity IsNot Nothing Then
                _db.Products.Remove(entity)
                Await _db.SaveChangesAsync()
                Await LoadProductsAsync()
            End If
        End Function

        Public Async Function SaveCategoryAsync(dto As CategoryManagementItem) As Task
            Dim entity As ProductCategory
            If dto.Id = 0 Then
                entity = New ProductCategory()
                _db.ProductCategories.Add(entity)
            Else
                entity = Await _db.ProductCategories.FindAsync(dto.Id)
                If entity Is Nothing Then
                    Throw New Exception("Category not found")
                End If
            End If

            entity.Name = dto.Name
            Await _db.SaveChangesAsync()
            Await LoadCategoriesAsync()
            Await LoadProductsAsync()
        End Function

        Public Async Function DeleteCategoryAsync(id As Integer) As Task
            Dim entity = Await _db.ProductCategories.FindAsync(id)
            If entity IsNot Nothing Then
                _db.ProductCategories.Remove(entity)
                Await _db.SaveChangesAsync()
                Await LoadCategoriesAsync()
                Await LoadProductsAsync()
            End If
        End Function

    End Class

End Namespace
