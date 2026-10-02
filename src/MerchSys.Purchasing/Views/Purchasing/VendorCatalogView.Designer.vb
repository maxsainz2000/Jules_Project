Imports System.Windows.Forms

Namespace Views.Purchasing
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class VendorCatalogView
        Inherits System.Windows.Forms.UserControl

        'UserControl overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.dgvVendors = New System.Windows.Forms.DataGridView()
            Me.dgvProducts = New System.Windows.Forms.DataGridView()
            Me.btnAdd = New System.Windows.Forms.Button()
            Me.btnEdit = New System.Windows.Forms.Button()
            Me.btnSave = New System.Windows.Forms.Button()
            Me.btnDelete = New System.Windows.Forms.Button()
            Me.numProductId = New System.Windows.Forms.NumericUpDown()
            Me.numUnitCost = New System.Windows.Forms.NumericUpDown()
            Me.txtNotes = New System.Windows.Forms.TextBox()
            Me.lblProductId = New System.Windows.Forms.Label()
            Me.lblUnitCost = New System.Windows.Forms.Label()
            Me.lblNotes = New System.Windows.Forms.Label()
            Me.pnlEditor = New System.Windows.Forms.Panel()
            Me.lblVendors = New System.Windows.Forms.Label()
            Me.lblProducts = New System.Windows.Forms.Label()
            CType(Me.dgvVendors, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.dgvProducts, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.numProductId, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.numUnitCost, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.pnlEditor.SuspendLayout()
            Me.SuspendLayout()
            '
            'dgvVendors
            '
            Me.dgvVendors.AllowUserToAddRows = False
            Me.dgvVendors.AllowUserToDeleteRows = False
            Me.dgvVendors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvVendors.Location = New System.Drawing.Point(12, 29)
            Me.dgvVendors.Name = "dgvVendors"
            Me.dgvVendors.ReadOnly = True
            Me.dgvVendors.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
            Me.dgvVendors.Size = New System.Drawing.Size(250, 400)
            Me.dgvVendors.TabIndex = 0
            Me.dgvVendors.AutoGenerateColumns = False
            '
            'dgvProducts
            '
            Me.dgvProducts.AllowUserToAddRows = False
            Me.dgvProducts.AllowUserToDeleteRows = False
            Me.dgvProducts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvProducts.Location = New System.Drawing.Point(280, 29)
            Me.dgvProducts.Name = "dgvProducts"
            Me.dgvProducts.ReadOnly = True
            Me.dgvProducts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
            Me.dgvProducts.Size = New System.Drawing.Size(400, 250)
            Me.dgvProducts.TabIndex = 1
            Me.dgvProducts.AutoGenerateColumns = False
            '
            'btnAdd
            '
            Me.btnAdd.Location = New System.Drawing.Point(280, 285)
            Me.btnAdd.Name = "btnAdd"
            Me.btnAdd.Size = New System.Drawing.Size(75, 23)
            Me.btnAdd.TabIndex = 2
            Me.btnAdd.Text = "Add"
            Me.btnAdd.UseVisualStyleBackColor = True
            '
            'btnEdit
            '
            Me.btnEdit.Location = New System.Drawing.Point(361, 285)
            Me.btnEdit.Name = "btnEdit"
            Me.btnEdit.Size = New System.Drawing.Size(75, 23)
            Me.btnEdit.TabIndex = 3
            Me.btnEdit.Text = "Edit"
            Me.btnEdit.UseVisualStyleBackColor = True
            '
            'btnDelete
            '
            Me.btnDelete.Location = New System.Drawing.Point(442, 285)
            Me.btnDelete.Name = "btnDelete"
            Me.btnDelete.Size = New System.Drawing.Size(75, 23)
            Me.btnDelete.TabIndex = 4
            Me.btnDelete.Text = "Delete"
            Me.btnDelete.UseVisualStyleBackColor = True
            '
            'pnlEditor
            '
            Me.pnlEditor.Controls.Add(Me.lblNotes)
            Me.pnlEditor.Controls.Add(Me.txtNotes)
            Me.pnlEditor.Controls.Add(Me.lblUnitCost)
            Me.pnlEditor.Controls.Add(Me.numUnitCost)
            Me.pnlEditor.Controls.Add(Me.lblProductId)
            Me.pnlEditor.Controls.Add(Me.numProductId)
            Me.pnlEditor.Controls.Add(Me.btnSave)
            Me.pnlEditor.Location = New System.Drawing.Point(280, 314)
            Me.pnlEditor.Name = "pnlEditor"
            Me.pnlEditor.Size = New System.Drawing.Size(400, 115)
            Me.pnlEditor.TabIndex = 5
            '
            'lblProductId
            '
            Me.lblProductId.AutoSize = True
            Me.lblProductId.Location = New System.Drawing.Point(3, 8)
            Me.lblProductId.Name = "lblProductId"
            Me.lblProductId.Size = New System.Drawing.Size(59, 13)
            Me.lblProductId.TabIndex = 0
            Me.lblProductId.Text = "Product ID"
            '
            'numProductId
            '
            Me.numProductId.Location = New System.Drawing.Point(68, 6)
            Me.numProductId.Maximum = New Decimal(New Integer() {9999999, 0, 0, 0})
            Me.numProductId.Name = "numProductId"
            Me.numProductId.Size = New System.Drawing.Size(120, 20)
            Me.numProductId.TabIndex = 1
            '
            'lblUnitCost
            '
            Me.lblUnitCost.AutoSize = True
            Me.lblUnitCost.Location = New System.Drawing.Point(200, 8)
            Me.lblUnitCost.Name = "lblUnitCost"
            Me.lblUnitCost.Size = New System.Drawing.Size(50, 13)
            Me.lblUnitCost.TabIndex = 2
            Me.lblUnitCost.Text = "Unit Cost"
            '
            'numUnitCost
            '
            Me.numUnitCost.DecimalPlaces = 2
            Me.numUnitCost.Location = New System.Drawing.Point(256, 6)
            Me.numUnitCost.Maximum = New Decimal(New Integer() {9999999, 0, 0, 0})
            Me.numUnitCost.Name = "numUnitCost"
            Me.numUnitCost.Size = New System.Drawing.Size(120, 20)
            Me.numUnitCost.TabIndex = 3
            '
            'lblNotes
            '
            Me.lblNotes.AutoSize = True
            Me.lblNotes.Location = New System.Drawing.Point(3, 34)
            Me.lblNotes.Name = "lblNotes"
            Me.lblNotes.Size = New System.Drawing.Size(35, 13)
            Me.lblNotes.TabIndex = 4
            Me.lblNotes.Text = "Notes"
            '
            'txtNotes
            '
            Me.txtNotes.Location = New System.Drawing.Point(68, 32)
            Me.txtNotes.Multiline = True
            Me.txtNotes.Name = "txtNotes"
            Me.txtNotes.Size = New System.Drawing.Size(308, 48)
            Me.txtNotes.TabIndex = 5
            '
            'btnSave
            '
            Me.btnSave.Location = New System.Drawing.Point(301, 86)
            Me.btnSave.Name = "btnSave"
            Me.btnSave.Size = New System.Drawing.Size(75, 23)
            Me.btnSave.TabIndex = 6
            Me.btnSave.Text = "Save"
            Me.btnSave.UseVisualStyleBackColor = True
            '
            'lblVendors
            '
            Me.lblVendors.AutoSize = True
            Me.lblVendors.Location = New System.Drawing.Point(12, 10)
            Me.lblVendors.Name = "lblVendors"
            Me.lblVendors.Size = New System.Drawing.Size(46, 13)
            Me.lblVendors.TabIndex = 6
            Me.lblVendors.Text = "Vendors"
            '
            'lblProducts
            '
            Me.lblProducts.AutoSize = True
            Me.lblProducts.Location = New System.Drawing.Point(277, 10)
            Me.lblProducts.Name = "lblProducts"
            Me.lblProducts.Size = New System.Drawing.Size(86, 13)
            Me.lblProducts.TabIndex = 7
            Me.lblProducts.Text = "Vendor Products"
            '
            'VendorCatalogView
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.lblProducts)
            Me.Controls.Add(Me.lblVendors)
            Me.Controls.Add(Me.pnlEditor)
            Me.Controls.Add(Me.btnDelete)
            Me.Controls.Add(Me.btnEdit)
            Me.Controls.Add(Me.btnAdd)
            Me.Controls.Add(Me.dgvProducts)
            Me.Controls.Add(Me.dgvVendors)
            Me.Name = "VendorCatalogView"
            Me.Size = New System.Drawing.Size(700, 450)
            CType(Me.dgvVendors, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.dgvProducts, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.numProductId, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.numUnitCost, System.ComponentModel.ISupportInitialize).EndInit()
            Me.pnlEditor.ResumeLayout(False)
            Me.pnlEditor.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()
        End Sub

        Friend WithEvents dgvVendors As System.Windows.Forms.DataGridView
        Friend WithEvents dgvProducts As System.Windows.Forms.DataGridView
        Friend WithEvents btnAdd As System.Windows.Forms.Button
        Friend WithEvents btnEdit As System.Windows.Forms.Button
        Friend WithEvents btnSave As System.Windows.Forms.Button
        Friend WithEvents btnDelete As System.Windows.Forms.Button
        Friend WithEvents pnlEditor As System.Windows.Forms.Panel
        Friend WithEvents numProductId As System.Windows.Forms.NumericUpDown
        Friend WithEvents numUnitCost As System.Windows.Forms.NumericUpDown
        Friend WithEvents txtNotes As System.Windows.Forms.TextBox
        Friend WithEvents lblProductId As System.Windows.Forms.Label
        Friend WithEvents lblUnitCost As System.Windows.Forms.Label
        Friend WithEvents lblNotes As System.Windows.Forms.Label
        Friend WithEvents lblVendors As System.Windows.Forms.Label
        Friend WithEvents lblProducts As System.Windows.Forms.Label
    End Class
End Namespace
