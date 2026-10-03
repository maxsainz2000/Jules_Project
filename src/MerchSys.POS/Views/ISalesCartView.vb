Imports System
Imports System.Collections.Generic
Imports MerchSys.POS.Services
Imports MerchSys.POS.Entities
Imports MerchSys.SharedKernel.Queries

Namespace Views

    Public Interface ISalesCartView
        ' Properties
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Property CartLines As List(Of CartLineDto)

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Property Total As Decimal

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Property PaymentMethod As String

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Property CashTendered As Decimal

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Property SelectedCustomer As CreditAccount

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Property SearchTerm As String

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Property SelectedProductId As Integer

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Property UpdateQuantity As Integer

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Property SearchResults As List(Of ProductCatalogItem)

        ' Events
        Event SearchProduct As EventHandler
        Event AddToCart As EventHandler
        Event UpdateQty As EventHandler
        Event ProcessPayment As EventHandler

    End Interface

End Namespace
