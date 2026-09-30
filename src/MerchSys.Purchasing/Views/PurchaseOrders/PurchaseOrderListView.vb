Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Windows.Forms
Imports MerchSys.Purchasing.Entities

Namespace Views.PurchaseOrders
    Public Class PurchaseOrderListView
        Implements IPurchaseOrderListView

        Private _orders As List(Of PurchaseOrder)
        Private _isOwnerRole As Boolean

        Private _presenter As Presenters.PurchaseOrders.PurchaseOrderListPresenter

        Public Sub New()
            InitializeComponent()
            dgvOrders.AutoGenerateColumns = False
            dgvOrders.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "OrderNumber", .HeaderText = "Order #", .Width = 100})
            dgvOrders.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "VendorId", .HeaderText = "Vendor ID", .Width = 80})
            dgvOrders.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "Status", .HeaderText = "Status", .Width = 100})
            dgvOrders.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "ExpectedDeliveryDate", .HeaderText = "Exp. Delivery", .Width = 100})
            dgvOrders.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "TotalAmount", .HeaderText = "Total Amount", .Width = 100, .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "₱0.00"}})
        End Sub

        Public Sub SetPresenter(presenter As Presenters.PurchaseOrders.PurchaseOrderListPresenter)
            _presenter = presenter
        End Sub

        Public Sub SetStatusOptions(options As List(Of String)) Implements IPurchaseOrderListView.SetStatusOptions
            cmbStatusFilter.DataSource = options
        End Sub

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property Orders As List(Of PurchaseOrder) Implements IPurchaseOrderListView.Orders
            Get
                Return _orders
            End Get
            Set(value As List(Of PurchaseOrder))
                _orders = value
                dgvOrders.DataSource = Nothing
                dgvOrders.DataSource = _orders
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SelectedOrder As PurchaseOrder Implements IPurchaseOrderListView.SelectedOrder
            Get
                If dgvOrders.SelectedRows.Count > 0 Then
                    Return CType(dgvOrders.SelectedRows(0).DataBoundItem, PurchaseOrder)
                End If
                Return Nothing
            End Get
            Set(value As PurchaseOrder)
                If value Is Nothing Then
                    dgvOrders.ClearSelection()
                    Return
                End If
                For Each row As DataGridViewRow In dgvOrders.Rows
                    If CType(row.DataBoundItem, PurchaseOrder).Id = value.Id Then
                        row.Selected = True
                        Return
                    End If
                Next
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SearchTerm As String Implements IPurchaseOrderListView.SearchTerm
            Get
                Return txtSearch.Text
            End Get
            Set(value As String)
                txtSearch.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property StatusFilter As String Implements IPurchaseOrderListView.StatusFilter
            Get
                Return If(cmbStatusFilter.SelectedItem?.ToString(), "")
            End Get
            Set(value As String)
                cmbStatusFilter.SelectedItem = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property IsOwnerRole As Boolean Implements IPurchaseOrderListView.IsOwnerRole
            Get
                Return _isOwnerRole
            End Get
            Set(value As Boolean)
                _isOwnerRole = value
                btnNew.Visible = Not _isOwnerRole
                btnEdit.Visible = Not _isOwnerRole
                btnSubmit.Visible = Not _isOwnerRole
                btnDelete.Visible = Not _isOwnerRole
            End Set
        End Property

        Private Async Sub PurchaseOrderListView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            If _presenter IsNot Nothing Then
                Await _presenter.OnLoadAsync()
            End If
        End Sub

        Private Async Sub btnNew_Click(sender As Object, e As EventArgs) Handles btnNew.Click
            If _presenter IsNot Nothing Then Await _presenter.OnNewClickedAsync()
        End Sub

        Private Async Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
            If _presenter IsNot Nothing Then Await _presenter.OnEditClickedAsync()
        End Sub

        Private Async Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
            If _presenter IsNot Nothing Then Await _presenter.OnSubmitClickedAsync()
        End Sub

        Private Async Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
            If _presenter IsNot Nothing Then Await _presenter.OnDeleteClickedAsync()
        End Sub

        Private Async Sub cmbStatusFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbStatusFilter.SelectedIndexChanged
            If _presenter IsNot Nothing Then Await _presenter.OnFilterChangedAsync()
        End Sub

        Private Async Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
            If _presenter IsNot Nothing Then Await _presenter.OnFilterChangedAsync()
        End Sub
    End Class
End Namespace