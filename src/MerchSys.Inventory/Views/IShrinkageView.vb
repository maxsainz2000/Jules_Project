Imports System
Imports System.Collections.Generic
Imports MerchSys.Inventory.Presenters

Namespace Views

    Public Interface IShrinkageView
        Property Presenter As Object

        Property SummaryTotalItemsLost As Integer
        Property SummaryTotalValueLost As Decimal

        Property FilterStartDate As DateTime?
        Property FilterEndDate As DateTime?
        Property FilterReason As String

        Property ShrinkageHistory As List(Of ShrinkageRowItem)
        Property AvailableProducts As List(Of ShrinkageProductItem)
        Property AvailableBatches As List(Of ShrinkageBatchItem)

        Event LoadView As EventHandler
        Event FilterChanged As EventHandler
        Event RecordShrinkageRequested As EventHandler(Of ShrinkageRecordRequest)

    End Interface

End Namespace
