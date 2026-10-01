Imports System
Imports System.Collections.Generic
Imports MerchSys.Purchasing.Presenters

Namespace Views

    Public Interface IGoodsReceivingView
        Property SelectedPurchaseOrderId As Integer?
        Property CanConfirmReceipt As Boolean

        Sub SetPurchaseOrders(pos As List(Of POSelectorItem))
        Sub SetLineItems(lines As List(Of GRLineItem))
        Sub ShowMessage(message As String, title As String)
        Sub ShowError(message As String)

        Event PurchaseOrderSelected As EventHandler(Of Integer?)
        Event ConfirmReceiptRequested As EventHandler
    End Interface

End Namespace
