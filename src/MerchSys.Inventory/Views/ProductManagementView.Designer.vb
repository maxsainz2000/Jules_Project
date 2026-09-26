Imports System.Windows.Forms

Namespace Views

    Partial Class ProductManagementView
        ''' <summary>
        ''' Required designer variable.
        ''' </summary>
        Private components As System.ComponentModel.IContainer = Nothing

        ''' <summary>
        ''' Clean up any resources being used.
        ''' </summary>
        ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso (components IsNot Nothing) Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
            Me.tabControl = New System.Windows.Forms.TabControl()
            Me.tabProducts = New System.Windows.Forms.TabPage()
            Me.dgvProducts = New System.Windows.Forms.DataGridView()
            Me.toolStripProducts = New System.Windows.Forms.ToolStrip()
            Me.btnAddProduct = New System.Windows.Forms.ToolStripButton()
            Me.btnEditProduct = New System.Windows.Forms.ToolStripButton()
            Me.btnDeleteProduct = New System.Windows.Forms.ToolStripButton()
            Me.tabCategories = New System.Windows.Forms.TabPage()
            Me.dgvCategories = New System.Windows.Forms.DataGridView()
            Me.toolStripCategories = New System.Windows.Forms.ToolStrip()
            Me.btnAddCategory = New System.Windows.Forms.ToolStripButton()
            Me.btnEditCategory = New System.Windows.Forms.ToolStripButton()
            Me.btnDeleteCategory = New System.Windows.Forms.ToolStripButton()

            Me.tabControl.SuspendLayout()
            Me.tabProducts.SuspendLayout()
            CType(Me.dgvProducts, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.toolStripProducts.SuspendLayout()
            Me.tabCategories.SuspendLayout()
            CType(Me.dgvCategories, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.toolStripCategories.SuspendLayout()
            Me.SuspendLayout()
            '
            ' tabControl
            '
            Me.tabControl.Controls.Add(Me.tabProducts)
            Me.tabControl.Controls.Add(Me.tabCategories)
            Me.tabControl.Dock = System.Windows.Forms.DockStyle.Fill
            Me.tabControl.Location = New System.Drawing.Point(0, 0)
            Me.tabControl.Name = "tabControl"
            Me.tabControl.SelectedIndex = 0
            Me.tabControl.Size = New System.Drawing.Size(800, 600)
            Me.tabControl.TabIndex = 0
            '
            ' tabProducts
            '
            Me.tabProducts.Controls.Add(Me.dgvProducts)
            Me.tabProducts.Controls.Add(Me.toolStripProducts)
            Me.tabProducts.Location = New System.Drawing.Point(4, 24)
            Me.tabProducts.Name = "tabProducts"
            Me.tabProducts.Padding = New System.Windows.Forms.Padding(3)
            Me.tabProducts.Size = New System.Drawing.Size(792, 572)
            Me.tabProducts.TabIndex = 0
            Me.tabProducts.Text = "Products"
            Me.tabProducts.UseVisualStyleBackColor = True
            '
            ' dgvProducts
            '
            Me.dgvProducts.AllowUserToAddRows = False
            Me.dgvProducts.AllowUserToDeleteRows = False
            Me.dgvProducts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvProducts.Dock = System.Windows.Forms.DockStyle.Fill
            Me.dgvProducts.Location = New System.Drawing.Point(3, 28)
            Me.dgvProducts.Name = "dgvProducts"
            Me.dgvProducts.ReadOnly = True
            Me.dgvProducts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
            Me.dgvProducts.Size = New System.Drawing.Size(786, 541)
            Me.dgvProducts.TabIndex = 1
            '
            ' toolStripProducts
            '
            Me.toolStripProducts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnAddProduct, Me.btnEditProduct, Me.btnDeleteProduct})
            Me.toolStripProducts.Location = New System.Drawing.Point(3, 3)
            Me.toolStripProducts.Name = "toolStripProducts"
            Me.toolStripProducts.Size = New System.Drawing.Size(786, 25)
            Me.toolStripProducts.TabIndex = 0
            Me.toolStripProducts.Text = "toolStrip1"
            '
            ' btnAddProduct
            '
            Me.btnAddProduct.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.btnAddProduct.Name = "btnAddProduct"
            Me.btnAddProduct.Size = New System.Drawing.Size(33, 22)
            Me.btnAddProduct.Text = "Add"
            '
            ' btnEditProduct
            '
            Me.btnEditProduct.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.btnEditProduct.Name = "btnEditProduct"
            Me.btnEditProduct.Size = New System.Drawing.Size(31, 22)
            Me.btnEditProduct.Text = "Edit"
            '
            ' btnDeleteProduct
            '
            Me.btnDeleteProduct.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.btnDeleteProduct.Name = "btnDeleteProduct"
            Me.btnDeleteProduct.Size = New System.Drawing.Size(44, 22)
            Me.btnDeleteProduct.Text = "Delete"
            '
            ' tabCategories
            '
            Me.tabCategories.Controls.Add(Me.dgvCategories)
            Me.tabCategories.Controls.Add(Me.toolStripCategories)
            Me.tabCategories.Location = New System.Drawing.Point(4, 24)
            Me.tabCategories.Name = "tabCategories"
            Me.tabCategories.Padding = New System.Windows.Forms.Padding(3)
            Me.tabCategories.Size = New System.Drawing.Size(792, 572)
            Me.tabCategories.TabIndex = 1
            Me.tabCategories.Text = "Categories"
            Me.tabCategories.UseVisualStyleBackColor = True
            '
            ' dgvCategories
            '
            Me.dgvCategories.AllowUserToAddRows = False
            Me.dgvCategories.AllowUserToDeleteRows = False
            Me.dgvCategories.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvCategories.Dock = System.Windows.Forms.DockStyle.Fill
            Me.dgvCategories.Location = New System.Drawing.Point(3, 28)
            Me.dgvCategories.Name = "dgvCategories"
            Me.dgvCategories.ReadOnly = True
            Me.dgvCategories.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
            Me.dgvCategories.Size = New System.Drawing.Size(786, 541)
            Me.dgvCategories.TabIndex = 1
            '
            ' toolStripCategories
            '
            Me.toolStripCategories.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnAddCategory, Me.btnEditCategory, Me.btnDeleteCategory})
            Me.toolStripCategories.Location = New System.Drawing.Point(3, 3)
            Me.toolStripCategories.Name = "toolStripCategories"
            Me.toolStripCategories.Size = New System.Drawing.Size(786, 25)
            Me.toolStripCategories.TabIndex = 0
            Me.toolStripCategories.Text = "toolStrip2"
            '
            ' btnAddCategory
            '
            Me.btnAddCategory.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.btnAddCategory.Name = "btnAddCategory"
            Me.btnAddCategory.Size = New System.Drawing.Size(33, 22)
            Me.btnAddCategory.Text = "Add"
            '
            ' btnEditCategory
            '
            Me.btnEditCategory.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.btnEditCategory.Name = "btnEditCategory"
            Me.btnEditCategory.Size = New System.Drawing.Size(31, 22)
            Me.btnEditCategory.Text = "Edit"
            '
            ' btnDeleteCategory
            '
            Me.btnDeleteCategory.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.btnDeleteCategory.Name = "btnDeleteCategory"
            Me.btnDeleteCategory.Size = New System.Drawing.Size(44, 22)
            Me.btnDeleteCategory.Text = "Delete"
            '
            ' ProductManagementView
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.tabControl)
            Me.Name = "ProductManagementView"
            Me.Size = New System.Drawing.Size(800, 600)

            Me.tabControl.ResumeLayout(False)
            Me.tabProducts.ResumeLayout(False)
            Me.tabProducts.PerformLayout()
            CType(Me.dgvProducts, System.ComponentModel.ISupportInitialize).EndInit()
            Me.toolStripProducts.ResumeLayout(False)
            Me.toolStripProducts.PerformLayout()
            Me.tabCategories.ResumeLayout(False)
            Me.tabCategories.PerformLayout()
            CType(Me.dgvCategories, System.ComponentModel.ISupportInitialize).EndInit()
            Me.toolStripCategories.ResumeLayout(False)
            Me.toolStripCategories.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents tabControl As System.Windows.Forms.TabControl
        Friend WithEvents tabProducts As System.Windows.Forms.TabPage
        Friend WithEvents tabCategories As System.Windows.Forms.TabPage
        Friend WithEvents dgvProducts As System.Windows.Forms.DataGridView
        Friend WithEvents toolStripProducts As System.Windows.Forms.ToolStrip
        Friend WithEvents btnAddProduct As System.Windows.Forms.ToolStripButton
        Friend WithEvents btnEditProduct As System.Windows.Forms.ToolStripButton
        Friend WithEvents btnDeleteProduct As System.Windows.Forms.ToolStripButton
        Friend WithEvents dgvCategories As System.Windows.Forms.DataGridView
        Friend WithEvents toolStripCategories As System.Windows.Forms.ToolStrip
        Friend WithEvents btnAddCategory As System.Windows.Forms.ToolStripButton
        Friend WithEvents btnEditCategory As System.Windows.Forms.ToolStripButton
        Friend WithEvents btnDeleteCategory As System.Windows.Forms.ToolStripButton
    End Class

End Namespace
