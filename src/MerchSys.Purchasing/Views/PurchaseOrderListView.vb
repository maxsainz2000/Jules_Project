Imports System.Collections
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports MerchSys.Purchasing.Entities
Imports MerchSys.SharedKernel.Enums

Namespace Views
    Public Class PurchaseOrderListView
        Inherits UserControl
        Implements IPurchaseOrderListView

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property OnSearch As Func(Of String, PurchaseOrderStatus?, Task) Implements IPurchaseOrderListView.OnSearch
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property OnNewPO As Func(Of Task) Implements IPurchaseOrderListView.OnNewPO
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property OnEditPO As Func(Of Integer, Task) Implements IPurchaseOrderListView.OnEditPO
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property OnSubmitPO As Func(Of Integer, Task) Implements IPurchaseOrderListView.OnSubmitPO
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property OnDeletePO As Func(Of Integer, Task) Implements IPurchaseOrderListView.OnDeletePO
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property OnLoadPage As Func(Of Task) Implements IPurchaseOrderListView.OnLoadPage

        Public Sub New()
            InitializeComponent()

            cmbStatus.Items.Add("All")
            For Each status In System.Enum.GetValues(GetType(PurchaseOrderStatus))
                cmbStatus.Items.Add(status.ToString())
            Next
            cmbStatus.SelectedIndex = 0

            dgvPurchaseOrders.AutoGenerateColumns = False
            dgvPurchaseOrders.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "Id", .HeaderText = "ID", .Visible = False })
            dgvPurchaseOrders.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "OrderNumber", .HeaderText = "Order Number" })
            dgvPurchaseOrders.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "VendorName", .HeaderText = "Vendor" })
            dgvPurchaseOrders.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "Status", .HeaderText = "Status" })
            dgvPurchaseOrders.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "ExpectedDeliveryDate", .HeaderText = "Delivery Date" })
            dgvPurchaseOrders.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "TotalAmount", .HeaderText = "Total Amount", .DefaultCellStyle = New DataGridViewCellStyle With { .Format = "₱0.00" } })

            AddHandler btnSearch.Click, Async Sub(s, e) Await TriggerSearch()
            AddHandler btnNew.Click, Async Sub(s, e)
                                         If OnNewPO IsNot Nothing Then Await OnNewPO.Invoke()
                                     End Sub
            AddHandler btnEdit.Click, Async Sub(s, e) Await TriggerEdit()
            AddHandler dgvPurchaseOrders.DoubleClick, Async Sub(s, e) Await TriggerEdit()
            AddHandler btnSubmit.Click, Async Sub(s, e) Await TriggerSubmit()
            AddHandler btnDelete.Click, Async Sub(s, e) Await TriggerDelete()
        End Sub

        Protected Overrides Async Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            If Not DesignMode AndAlso OnLoadPage IsNot Nothing Then
                Await OnLoadPage.Invoke()
            End If
        End Sub

        Private Async Function TriggerSearch() As Task
            If OnSearch IsNot Nothing Then
                Dim term = txtSearch.Text
                Dim statusFilter As PurchaseOrderStatus? = Nothing
                If cmbStatus.SelectedIndex > 0 Then
                    statusFilter = CType(System.Enum.Parse(GetType(PurchaseOrderStatus), cmbStatus.SelectedItem.ToString()), PurchaseOrderStatus)
                End If
                Await OnSearch.Invoke(term, statusFilter)
            End If
        End Function

        Private Async Function TriggerEdit() As Task
            If OnEditPO IsNot Nothing AndAlso dgvPurchaseOrders.SelectedRows.Count > 0 Then
                Dim id = CInt(dgvPurchaseOrders.SelectedRows(0).Cells(0).Value)
                Await OnEditPO.Invoke(id)
            End If
        End Function

        Private Async Function TriggerSubmit() As Task
            If OnSubmitPO IsNot Nothing AndAlso dgvPurchaseOrders.SelectedRows.Count > 0 Then
                Dim id = CInt(dgvPurchaseOrders.SelectedRows(0).Cells(0).Value)
                Await OnSubmitPO.Invoke(id)
            End If
        End Function

        Private Async Function TriggerDelete() As Task
            If OnDeletePO IsNot Nothing AndAlso dgvPurchaseOrders.SelectedRows.Count > 0 Then
                Dim id = CInt(dgvPurchaseOrders.SelectedRows(0).Cells(0).Value)
                Await OnDeletePO.Invoke(id)
            End If
        End Function

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property CanEdit As Boolean Implements IPurchaseOrderListView.CanEdit
            Get
                Return btnNew.Enabled
            End Get
            Set(value As Boolean)
                btnNew.Enabled = value
                btnEdit.Enabled = value
                btnSubmit.Enabled = value
                btnDelete.Enabled = value
            End Set
        End Property

        Public Sub BindData(items As IEnumerable) Implements IPurchaseOrderListView.BindData
            dgvPurchaseOrders.DataSource = items
        End Sub

        Public Sub ShowError(message As String) Implements IPurchaseOrderListView.ShowError
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Sub

        Public Sub ShowMessage(message As String) Implements IPurchaseOrderListView.ShowMessage
            MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Sub

        Public Function ConfirmAction(message As String, title As String) As Boolean Implements IPurchaseOrderListView.ConfirmAction
            Return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes
        End Function
    End Class
End Namespace
