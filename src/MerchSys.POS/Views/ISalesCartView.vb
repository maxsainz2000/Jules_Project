Imports System.ComponentModel
Imports System.Collections.Generic
Imports MerchSys.POS.Services
Imports System

Namespace Views

    Public Class UpdateQtyEventArgs
        Inherits EventArgs
        Public Property ProductId As Integer
        Public Property NewQuantity As Integer
    End Class

    Public Interface ISalesCartView

        Event SearchProduct As EventHandler(Of String)
        Event AddToCart As EventHandler(Of Integer)
        Event UpdateQty As EventHandler(Of UpdateQtyEventArgs)
        Event ProcessPayment As EventHandler
        Event PreviewReceipt As EventHandler(Of Integer)

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Property CartLines As IEnumerable(Of CartLineDto)

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Property Total As Decimal

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Property PaymentMethod As String

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Property CashTendered As Decimal

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Property SelectedCustomer As Nullable(Of Integer)

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Property ReceiptPreview As String

    End Interface

End Namespace
