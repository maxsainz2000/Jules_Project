Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms
Imports MerchSys.Inventory.Presenters

Namespace Views
    Public Interface IProductPriceHistoryView
        Event LoadView As EventHandler

        Property ProductId As Integer
        Property History As IReadOnlyList(Of PriceHistoryRowItem)
        Property Presenter As ProductPriceHistoryPresenter

        Function ShowDialog(owner As IWin32Window) As DialogResult
    End Interface
End Namespace
