Imports System
Imports System.Collections.Generic
Imports MerchSys.Inventory.Presenters

Namespace Views

    Public Interface IProductManagementView

        Event LoadView As EventHandler
        Event AddProductRequested As EventHandler
        Event EditProductRequested As EventHandler(Of ProductManagementRowItem)
        Event SaveProductRequested As EventHandler(Of ProductManagementRowItem)
        Event DeleteProductRequested As EventHandler(Of ProductManagementRowItem)
        Event AddCategoryRequested As EventHandler
        Event EditCategoryRequested As EventHandler(Of CategoryManagementItem)
        Event SaveCategoryRequested As EventHandler(Of CategoryManagementItem)
        Event DeleteCategoryRequested As EventHandler(Of CategoryManagementItem)

        Property Products As IReadOnlyList(Of ProductManagementRowItem)
        Property Categories As IReadOnlyList(Of CategoryManagementItem)
        Property SelectedProduct As ProductManagementRowItem
        Property SelectedCategory As CategoryManagementItem

        Property Presenter As ProductManagementPresenter

    End Interface

End Namespace
