Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms
Imports System.ComponentModel
Imports MerchSys.Inventory.Presenters

Namespace Views

    Public Partial Class ProductManagementView
        Inherits UserControl
        Implements IProductManagementView

        Public Event LoadView As EventHandler Implements IProductManagementView.LoadView
        Public Event AddProductRequested As EventHandler Implements IProductManagementView.AddProductRequested
        Public Event EditProductRequested As EventHandler(Of ProductManagementRowItem) Implements IProductManagementView.EditProductRequested
        Public Event SaveProductRequested As EventHandler(Of ProductManagementRowItem) Implements IProductManagementView.SaveProductRequested
        Public Event DeleteProductRequested As EventHandler(Of ProductManagementRowItem) Implements IProductManagementView.DeleteProductRequested
        Public Event AddCategoryRequested As EventHandler Implements IProductManagementView.AddCategoryRequested
        Public Event EditCategoryRequested As EventHandler(Of CategoryManagementItem) Implements IProductManagementView.EditCategoryRequested
        Public Event SaveCategoryRequested As EventHandler(Of CategoryManagementItem) Implements IProductManagementView.SaveCategoryRequested
        Public Event DeleteCategoryRequested As EventHandler(Of CategoryManagementItem) Implements IProductManagementView.DeleteCategoryRequested

        Private _products As IReadOnlyList(Of ProductManagementRowItem) = New List(Of ProductManagementRowItem)()
        Private _categories As IReadOnlyList(Of CategoryManagementItem) = New List(Of CategoryManagementItem)()

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property Presenter As ProductManagementPresenter Implements IProductManagementView.Presenter

        Public Sub New()
            InitializeComponent()

            AddHandler Me.Load, Sub(sender, e) RaiseEvent LoadView(Me, EventArgs.Empty)

            AddHandler btnAddProduct.Click, AddressOf btnAddProduct_Click
            AddHandler btnEditProduct.Click, AddressOf btnEditProduct_Click
            AddHandler btnDeleteProduct.Click, AddressOf btnDeleteProduct_Click

            AddHandler btnAddCategory.Click, AddressOf btnAddCategory_Click
            AddHandler btnEditCategory.Click, AddressOf btnEditCategory_Click
            AddHandler btnDeleteCategory.Click, AddressOf btnDeleteCategory_Click
        End Sub

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property Products As IReadOnlyList(Of ProductManagementRowItem) Implements IProductManagementView.Products
            Get
                Return _products
            End Get
            Set(value As IReadOnlyList(Of ProductManagementRowItem))
                _products = value

                If InvokeRequired Then
                    Invoke(Sub() UpdateProductsGrid())
                Else
                    UpdateProductsGrid()
                End If
            End Set
        End Property

        Private Sub UpdateProductsGrid()
            dgvProducts.DataSource = Nothing
            dgvProducts.DataSource = _products
        End Sub

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property Categories As IReadOnlyList(Of CategoryManagementItem) Implements IProductManagementView.Categories
            Get
                Return _categories
            End Get
            Set(value As IReadOnlyList(Of CategoryManagementItem))
                _categories = value

                If InvokeRequired Then
                    Invoke(Sub() UpdateCategoriesGrid())
                Else
                    UpdateCategoriesGrid()
                End If
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SelectedProduct As ProductManagementRowItem Implements IProductManagementView.SelectedProduct
            Get
                Return GetSelectedProduct()
            End Get
            Set(value As ProductManagementRowItem)
                ' Selection set not implemented
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SelectedCategory As CategoryManagementItem Implements IProductManagementView.SelectedCategory
            Get
                Return GetSelectedCategory()
            End Get
            Set(value As CategoryManagementItem)
                ' Selection set not implemented
            End Set
        End Property

        Private Sub UpdateCategoriesGrid()
            dgvCategories.DataSource = Nothing
            dgvCategories.DataSource = _categories
        End Sub

        Private Sub btnAddProduct_Click(sender As Object, e As EventArgs)
            RaiseEvent AddProductRequested(Me, EventArgs.Empty)

            Using dialog = New Dialogs.ProductEditDialog(_categories, Nothing)
                If dialog.ShowDialog(Me) = DialogResult.OK Then
                    RaiseEvent SaveProductRequested(Me, dialog.ProductData)
                End If
            End Using
        End Sub

        Private Sub btnEditProduct_Click(sender As Object, e As EventArgs)
            Dim selectedItem = GetSelectedProduct()
            If selectedItem IsNot Nothing Then
                RaiseEvent EditProductRequested(Me, selectedItem)

                Using dialog = New Dialogs.ProductEditDialog(_categories, selectedItem)
                    If dialog.ShowDialog(Me) = DialogResult.OK Then
                        RaiseEvent SaveProductRequested(Me, dialog.ProductData)
                    End If
                End Using
            End If
        End Sub

        Private Sub btnDeleteProduct_Click(sender As Object, e As EventArgs)
            Dim selectedItem = GetSelectedProduct()
            If selectedItem IsNot Nothing Then
                If MessageBox.Show("Are you sure you want to delete this product?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes Then
                    RaiseEvent DeleteProductRequested(Me, selectedItem)
                End If
            End If
        End Sub

        Private Function GetSelectedProduct() As ProductManagementRowItem
            If dgvProducts.SelectedRows.Count > 0 Then
                Return TryCast(dgvProducts.SelectedRows(0).DataBoundItem, ProductManagementRowItem)
            End If
            Return Nothing
        End Function

        Private Sub btnAddCategory_Click(sender As Object, e As EventArgs)
            RaiseEvent AddCategoryRequested(Me, EventArgs.Empty)

            Using dialog = New Dialogs.CategoryEditDialog(Nothing)
                If dialog.ShowDialog(Me) = DialogResult.OK Then
                    RaiseEvent SaveCategoryRequested(Me, dialog.CategoryData)
                End If
            End Using
        End Sub

        Private Sub btnEditCategory_Click(sender As Object, e As EventArgs)
            Dim selectedItem = GetSelectedCategory()
            If selectedItem IsNot Nothing Then
                RaiseEvent EditCategoryRequested(Me, selectedItem)

                Using dialog = New Dialogs.CategoryEditDialog(selectedItem)
                    If dialog.ShowDialog(Me) = DialogResult.OK Then
                        RaiseEvent SaveCategoryRequested(Me, dialog.CategoryData)
                    End If
                End Using
            End If
        End Sub

        Private Sub btnDeleteCategory_Click(sender As Object, e As EventArgs)
            Dim selectedItem = GetSelectedCategory()
            If selectedItem IsNot Nothing Then
                If MessageBox.Show("Are you sure you want to delete this category?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes Then
                    RaiseEvent DeleteCategoryRequested(Me, selectedItem)
                End If
            End If
        End Sub

        Private Function GetSelectedCategory() As CategoryManagementItem
            If dgvCategories.SelectedRows.Count > 0 Then
                Return TryCast(dgvCategories.SelectedRows(0).DataBoundItem, CategoryManagementItem)
            End If
            Return Nothing
        End Function

    End Class

End Namespace
