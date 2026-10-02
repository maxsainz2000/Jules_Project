Imports System
Imports System.Collections.Generic
Imports MerchSys.Purchasing.Presenters

Namespace Views
    Public Interface IAPLedgerView
        Event Load As EventHandler
        Event FilterChanged As EventHandler
        Event VendorChanged As EventHandler
        Event RecordPaymentClicked As EventHandler

        Property Vendors As IEnumerable(Of VendorSelectorItem)
        Property SelectedVendorId As Integer

        Property IsAllFilterActive As Boolean
        Property IsOutstandingFilterActive As Boolean
        Property IsOverdueFilterActive As Boolean
        Property IsPaidFilterActive As Boolean

        Property LedgerRows As IEnumerable(Of APLedgerRow)
        Property TotalOutstanding As Decimal

        ReadOnly Property SelectedEntryId As Integer
    End Interface
End Namespace
