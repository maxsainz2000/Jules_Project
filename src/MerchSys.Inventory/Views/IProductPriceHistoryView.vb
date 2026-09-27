Imports System.Collections.Generic
Imports System.Windows.Forms
Imports MerchSys.Inventory.Presenters

Namespace Views

    Public Interface IProductPriceHistoryView

        Property PriceHistories As IReadOnlyList(Of PriceHistoryRowItem)
        Function ShowDialog(owner As IWin32Window) As DialogResult

    End Interface

End Namespace
