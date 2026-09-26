Imports System
Imports System.Windows.Forms
Imports MerchSys.Inventory.Presenters

Namespace Views.Dialogs

    Public Partial Class CategoryEditDialog
        Inherits Form

        Private ReadOnly _existingCategory As CategoryManagementItem

        Public Sub New(existingCategory As CategoryManagementItem)
            InitializeComponent()
            _existingCategory = existingCategory
        End Sub

        Public ReadOnly Property CategoryData As CategoryManagementItem
            Get
                Return New CategoryManagementItem With {
                    .Id = If(_existingCategory IsNot Nothing, _existingCategory.Id, 0),
                    .Name = txtName.Text
                }
            End Get
        End Property

        Private Sub CategoryEditDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            If _existingCategory IsNot Nothing Then
                Me.Text = "Edit Category"
                txtName.Text = _existingCategory.Name
            Else
                Me.Text = "Add Category"
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
