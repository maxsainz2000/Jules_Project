Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows.Forms
Imports MerchSys.Inventory.Presenters

Namespace Views.Dialogs

    Public Class RecordShrinkageDialog
        Inherits Form

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property SelectedProductId As Integer
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property SelectedBatchId As Integer?
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property Reason As String
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property Quantity As Integer

        Private _allBatches As List(Of ShrinkageBatchItem)

        Public Sub New()
            InitializeComponent()
        End Sub

        Public Sub Initialize(products As List(Of ShrinkageProductItem), batches As List(Of ShrinkageBatchItem))
            _allBatches = batches

            cmbProduct.DataSource = products
            cmbProduct.DisplayMember = "Name"
            cmbProduct.ValueMember = "Id"

            UpdateBatchDropdown()
        End Sub

        Private Sub cmbProduct_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbProduct.SelectedIndexChanged
            UpdateBatchDropdown()
        End Sub

        Private Sub UpdateBatchDropdown()
            If cmbProduct.SelectedItem Is Nothing Then
                cmbBatch.DataSource = Nothing
                Return
            End If

            Dim selProduct = CType(cmbProduct.SelectedItem, ShrinkageProductItem)
            Dim prodBatches = _allBatches.Where(Function(b) b.ProductId = selProduct.Id).ToList()

            Dim list = New List(Of ShrinkageBatchItem)
            list.Add(New ShrinkageBatchItem With { .Id = 0, .DisplayText = "Auto (FIFO)" })
            list.AddRange(prodBatches)

            cmbBatch.DataSource = list
            cmbBatch.DisplayMember = "DisplayText"
            cmbBatch.ValueMember = "Id"
        End Sub

        Private Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
            If cmbProduct.SelectedItem Is Nothing Then
                MessageBox.Show("Please select a product.")
                Return
            End If

            If numQuantity.Value <= 0 Then
                MessageBox.Show("Quantity must be greater than zero.")
                Return
            End If

            If String.IsNullOrWhiteSpace(txtReason.Text) Then
                MessageBox.Show("Please provide a reason.")
                Return
            End If

            SelectedProductId = CType(cmbProduct.SelectedItem, ShrinkageProductItem).Id

            Dim selBatch = CType(cmbBatch.SelectedItem, ShrinkageBatchItem)
            If selBatch IsNot Nothing AndAlso selBatch.Id > 0 Then
                SelectedBatchId = selBatch.Id
            Else
                SelectedBatchId = Nothing
            End If

            Reason = txtReason.Text
            Quantity = CInt(numQuantity.Value)

            DialogResult = DialogResult.OK
            Close()
        End Sub

        Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
            DialogResult = DialogResult.Cancel
            Close()
        End Sub

    End Class

End Namespace
