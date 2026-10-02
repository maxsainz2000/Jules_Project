Imports System.Collections.Generic
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports MerchSys.Purchasing.Entities
Imports MerchSys.SharedKernel.Enums

Namespace Views.Dialogs
    Public Class PurchaseOrderEditorDialog
        Inherits Form
        Implements IPurchaseOrderEditorView

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property OnSaveDraft As Func(Of Task) Implements IPurchaseOrderEditorView.OnSaveDraft
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property OnSubmit As Func(Of Task) Implements IPurchaseOrderEditorView.OnSubmit
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property OnAddLine As Func(Of String, Integer, Decimal, Decimal, Task) Implements IPurchaseOrderEditorView.OnAddLine
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property OnRemoveLine As Func(Of Integer, Task) Implements IPurchaseOrderEditorView.OnRemoveLine
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property OnLoadData As Func(Of Task) Implements IPurchaseOrderEditorView.OnLoadData
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property OnVendorChanged As Func(Of Integer, Task) Implements IPurchaseOrderEditorView.OnVendorChanged

        Public Sub New()
            InitializeComponent()
            
            dgvLines.AutoGenerateColumns = False
            dgvLines.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "Id", .HeaderText = "ID", .Visible = False })

            Dim colProduct = New DataGridViewComboBoxColumn() With { .DataPropertyName = "ProductName", .HeaderText = "Product", .Width = 200, .Name = "ProductNameColumn", .DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing }
            dgvLines.Columns.Add(colProduct)

            dgvLines.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "Quantity", .HeaderText = "Qty", .Width = 80 })
            dgvLines.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "UnitCost", .HeaderText = "Unit Cost", .DefaultCellStyle = New DataGridViewCellStyle With { .Format = "₱0.00" } })
            dgvLines.Columns.Add(New DataGridViewTextBoxColumn With { .DataPropertyName = "LineTotal", .HeaderText = "Total", .DefaultCellStyle = New DataGridViewCellStyle With { .Format = "₱0.00" } })

            AddHandler btnSaveDraft.Click, Async Sub(s, e)
                                               If OnSaveDraft IsNot Nothing Then Await OnSaveDraft.Invoke()
                                           End Sub
            AddHandler btnSubmit.Click, Async Sub(s, e)
                                            If OnSubmit IsNot Nothing Then Await OnSubmit.Invoke()
                                        End Sub
            AddHandler btnAddLine.Click, Async Sub(s, e) Await TriggerAddLine()
            AddHandler btnRemoveLine.Click, Async Sub(s, e) Await TriggerRemoveLine()
            AddHandler txtUnitCost.TextChanged, AddressOf CalculateLineTotal
            AddHandler txtQuantity.TextChanged, AddressOf CalculateLineTotal

            AddHandler cmbVendor.SelectedIndexChanged, Async Sub(s, e)
                                                           If OnVendorChanged IsNot Nothing AndAlso cmbVendor.SelectedItem IsNot Nothing Then
                                                               Dim vendor = DirectCast(cmbVendor.SelectedItem, Vendor)
                                                               Await OnVendorChanged.Invoke(vendor.Id)
                                                           End If
                                                       End Sub

            AddHandler cmbVendorProduct.SelectedIndexChanged, AddressOf OnProductSelectionChanged
        End Sub

        Private Sub OnProductSelectionChanged(sender As Object, e As EventArgs)
            If cmbVendorProduct.SelectedItem IsNot Nothing Then
                Dim vp = DirectCast(cmbVendorProduct.SelectedItem, Services.VendorProductDto)
                txtUnitCost.Text = vp.UnitCost.ToString("0.00")
            End If
        End Sub
        
        Private Sub CalculateLineTotal(sender As Object, e As EventArgs)
            Dim cost As Decimal
            Dim qty As Integer
            If Decimal.TryParse(txtUnitCost.Text, cost) AndAlso Integer.TryParse(txtQuantity.Text, qty) Then
                txtLineTotal.Text = (cost * qty).ToString("0.00")
            Else
                txtLineTotal.Text = ""
            End If
        End Sub

        Protected Overrides Async Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            If Not DesignMode AndAlso OnLoadData IsNot Nothing Then
                Await OnLoadData.Invoke()
            End If
        End Sub

        Private Async Function TriggerAddLine() As Task
            If OnAddLine IsNot Nothing Then
                Dim cost As Decimal
                Dim total As Decimal
                Dim qty As Integer

                If cmbVendorProduct.SelectedItem Is Nothing Then
                    ShowError("Please select a product from the vendor catalog.")
                    Return
                End If

                Dim productName = DirectCast(cmbVendorProduct.SelectedItem, Services.VendorProductDto).ProductName
                If String.IsNullOrWhiteSpace(productName) Then
                    ShowError("Selected product has an invalid name.")
                    Return
                End If
                If Not Integer.TryParse(txtQuantity.Text, qty) OrElse qty <= 0 Then
                    ShowError("Please enter a valid quantity.")
                    Return
                End If
                If Not Decimal.TryParse(txtUnitCost.Text, cost) OrElse cost <= 0 Then
                    ShowError("Please enter a valid unit cost.")
                    Return
                End If
                If Not Decimal.TryParse(txtLineTotal.Text, total) OrElse total <= 0 Then
                    ShowError("Please enter a valid line total.")
                    Return
                End If
                
                Await OnAddLine.Invoke(productName, qty, cost, total)
                cmbVendorProduct.SelectedIndex = -1
                txtQuantity.Clear()
                txtUnitCost.Clear()
                txtLineTotal.Clear()
            End If
        End Function

        Private Async Function TriggerRemoveLine() As Task
            If OnRemoveLine IsNot Nothing AndAlso dgvLines.SelectedRows.Count > 0 Then
                Dim id = CInt(dgvLines.SelectedRows(0).Cells(0).Value)
                Await OnRemoveLine.Invoke(id)
            End If
        End Function

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property SelectedVendorId As Integer Implements IPurchaseOrderEditorView.SelectedVendorId
            Get
                If cmbVendor.SelectedItem IsNot Nothing Then
                    Return DirectCast(cmbVendor.SelectedItem, Vendor).Id
                End If
                Return 0
            End Get
            Set(value As Integer)
                For Each item As Vendor In cmbVendor.Items
                    If item.Id = value Then
                        cmbVendor.SelectedItem = item
                        Exit For
                    End If
                Next
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property Notes As String Implements IPurchaseOrderEditorView.Notes
            Get
                Return txtNotes.Text
            End Get
            Set(value As String)
                txtNotes.Text = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property ExpectedDeliveryDate As DateTime? Implements IPurchaseOrderEditorView.ExpectedDeliveryDate
            Get
                Return If(dtpDeliveryDate.Checked, dtpDeliveryDate.Value, CType(Nothing, DateTime?))
            End Get
            Set(value As DateTime?)
                If value.HasValue Then
                    dtpDeliveryDate.Checked = True
                    dtpDeliveryDate.Value = value.Value
                Else
                    dtpDeliveryDate.Checked = False
                End If
            End Set
        End Property
        
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property POStatus As PurchaseOrderStatus Implements IPurchaseOrderEditorView.POStatus
            Get
                ' Not readable from UI directly as a property setting back
                Return PurchaseOrderStatus.Draft
            End Get
            Set(value As PurchaseOrderStatus)
                lblStatus.Text = $"Status: {value.ToString()}"
            End Set
        End Property

        Public Sub BindVendors(vendors As List(Of Vendor)) Implements IPurchaseOrderEditorView.BindVendors
            cmbVendor.DataSource = vendors
            cmbVendor.DisplayMember = "Name"
            cmbVendor.ValueMember = "Id"
        End Sub

        Public Sub BindVendorProducts(products As List(Of Services.VendorProductDto)) Implements IPurchaseOrderEditorView.BindVendorProducts
            cmbVendorProduct.DataSource = products
            cmbVendorProduct.DisplayMember = "ProductName"
            cmbVendorProduct.ValueMember = "ProductId"
            cmbVendorProduct.SelectedIndex = -1

            If dgvLines.Columns.Contains("ProductNameColumn") Then
                Dim colProduct = DirectCast(dgvLines.Columns("ProductNameColumn"), DataGridViewComboBoxColumn)
                colProduct.DataSource = products
                colProduct.DisplayMember = "ProductName"
                colProduct.ValueMember = "ProductName"
            End If
        End Sub

        Public Sub BindLines(lines As List(Of PurchaseOrderLine)) Implements IPurchaseOrderEditorView.BindLines
            dgvLines.DataSource = Nothing
            dgvLines.DataSource = lines
        End Sub

        Public Sub UpdateTotal(total As Decimal) Implements IPurchaseOrderEditorView.UpdateTotal
            lblTotal.Text = $"Total: ₱{total:N2}"
        End Sub

        Public Sub SetReadOnly(isReadOnly As Boolean) Implements IPurchaseOrderEditorView.SetReadOnly
            cmbVendor.Enabled = Not isReadOnly
            txtNotes.ReadOnly = isReadOnly
            dtpDeliveryDate.Enabled = Not isReadOnly
            cmbVendorProduct.Enabled = Not isReadOnly
            txtQuantity.Enabled = Not isReadOnly
            txtUnitCost.Enabled = Not isReadOnly
            txtLineTotal.Enabled = Not isReadOnly
            btnAddLine.Enabled = Not isReadOnly
            btnRemoveLine.Enabled = Not isReadOnly
            btnSaveDraft.Enabled = Not isReadOnly
            btnSubmit.Enabled = Not isReadOnly
        End Sub

        Public Sub ShowError(message As String) Implements IPurchaseOrderEditorView.ShowError
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Sub

        Public Sub CloseDialog() Implements IPurchaseOrderEditorView.CloseDialog
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Sub
        
        Public Shadows Function ShowDialog() As DialogResult Implements IPurchaseOrderEditorView.ShowDialog
            Return MyBase.ShowDialog()
        End Function
    End Class
End Namespace
