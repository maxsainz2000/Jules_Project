Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms
Imports MerchSys.Inventory.Presenters

Namespace Views.Dialogs

    Public Partial Class ProductEditDialog
        Inherits Form

        Private ReadOnly _categories As IReadOnlyList(Of CategoryManagementItem)
        Private ReadOnly _existingProduct As ProductManagementRowItem

        Public Sub New(categories As IReadOnlyList(Of CategoryManagementItem), existingProduct As ProductManagementRowItem)
            InitializeComponent()
            _categories = categories
            _existingProduct = existingProduct
        End Sub

        Public ReadOnly Property ProductData As ProductManagementRowItem
            Get
                Return New ProductManagementRowItem With {
                    .Id = If(_existingProduct IsNot Nothing, _existingProduct.Id, 0),
                    .CategoryId = Convert.ToInt32(cboCategory.SelectedValue),
                    .Name = txtName.Text,
                    .SKU = txtSKU.Text,
                    .RetailPrice = nudRetailPrice.Value,
                    .Unit = txtUnit.Text,
                    .HasExpiry = chkHasExpiry.Checked,
                    .MinimumThreshold = Convert.ToInt32(nudMinThreshold.Value)
                }
            End Get
        End Property

        Private Sub ProductEditDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            cboCategory.DataSource = _categories
            cboCategory.DisplayMember = "Name"
            cboCategory.ValueMember = "Id"

            If _existingProduct IsNot Nothing Then
                Me.Text = "Edit Product"
                cboCategory.SelectedValue = _existingProduct.CategoryId
                txtName.Text = _existingProduct.Name
                txtSKU.Text = _existingProduct.SKU
                nudRetailPrice.Value = _existingProduct.RetailPrice
                txtUnit.Text = _existingProduct.Unit
                chkHasExpiry.Checked = _existingProduct.HasExpiry
                nudMinThreshold.Value = _existingProduct.MinimumThreshold
            Else
                Me.Text = "Add Product"
            End If
        End Sub

        Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
            If String.IsNullOrWhiteSpace(txtName.Text) Then
                MessageBox.Show("Name is required.")
                Return
            End If

            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Sub

        Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
        End Sub

    End Class

End Namespace
