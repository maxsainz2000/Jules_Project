Imports System.Collections.Generic
Imports MerchSys.Purchasing.Entities

Namespace Views.PurchaseOrders
    Public Interface IPurchaseOrderListView
        Property Orders As List(Of PurchaseOrder)
        Property SelectedOrder As PurchaseOrder
        Property SearchTerm As String
        Property StatusFilter As String
        Property IsOwnerRole As Boolean

        Sub SetStatusOptions(options As List(Of String))
    End Interface
End Namespace