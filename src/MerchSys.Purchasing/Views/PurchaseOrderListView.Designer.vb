Namespace Views
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class PurchaseOrderListView
        Inherits System.Windows.Forms.UserControl

        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        Private components As System.ComponentModel.IContainer
        Friend WithEvents dgvPurchaseOrders As System.Windows.Forms.DataGridView
        Friend WithEvents txtSearch As System.Windows.Forms.TextBox
        Friend WithEvents cmbStatus As System.Windows.Forms.ComboBox
        Friend WithEvents btnSearch As System.Windows.Forms.Button
        Friend WithEvents btnNew As System.Windows.Forms.Button
        Friend WithEvents btnEdit As System.Windows.Forms.Button
        Friend WithEvents btnSubmit As System.Windows.Forms.Button
        Friend WithEvents btnDelete As System.Windows.Forms.Button

        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.dgvPurchaseOrders = New System.Windows.Forms.DataGridView()
            Me.txtSearch = New System.Windows.Forms.TextBox()
            Me.cmbStatus = New System.Windows.Forms.ComboBox()
            Me.btnSearch = New System.Windows.Forms.Button()
            Me.btnNew = New System.Windows.Forms.Button()
            Me.btnEdit = New System.Windows.Forms.Button()
            Me.btnSubmit = New System.Windows.Forms.Button()
            Me.btnDelete = New System.Windows.Forms.Button()
            CType(Me.dgvPurchaseOrders, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'dgvPurchaseOrders
            '
            Me.dgvPurchaseOrders.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) Or System.Windows.Forms.AnchorStyles.Left) Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.dgvPurchaseOrders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvPurchaseOrders.Location = New System.Drawing.Point(12, 41)
            Me.dgvPurchaseOrders.Name = "dgvPurchaseOrders"
            Me.dgvPurchaseOrders.Size = New System.Drawing.Size(776, 397)
            Me.dgvPurchaseOrders.TabIndex = 0
            Me.dgvPurchaseOrders.AllowUserToAddRows = False
            Me.dgvPurchaseOrders.AllowUserToDeleteRows = False
            Me.dgvPurchaseOrders.ReadOnly = True
            Me.dgvPurchaseOrders.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
            '
            'txtSearch
            '
            Me.txtSearch.Location = New System.Drawing.Point(12, 12)
            Me.txtSearch.Name = "txtSearch"
            Me.txtSearch.Size = New System.Drawing.Size(150, 20)
            Me.txtSearch.TabIndex = 1
            Me.txtSearch.PlaceholderText = "Search by Order #..."
            '
            'cmbStatus
            '
            Me.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cmbStatus.FormattingEnabled = True
            Me.cmbStatus.Location = New System.Drawing.Point(168, 12)
            Me.cmbStatus.Name = "cmbStatus"
            Me.cmbStatus.Size = New System.Drawing.Size(121, 21)
            Me.cmbStatus.TabIndex = 2
            '
            'btnSearch
            '
            Me.btnSearch.Location = New System.Drawing.Point(295, 10)
            Me.btnSearch.Name = "btnSearch"
            Me.btnSearch.Size = New System.Drawing.Size(75, 23)
            Me.btnSearch.TabIndex = 3
            Me.btnSearch.Text = "Search"
            Me.btnSearch.UseVisualStyleBackColor = True
            '
            'btnNew
            '
            Me.btnNew.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnNew.Location = New System.Drawing.Point(470, 10)
            Me.btnNew.Name = "btnNew"
            Me.btnNew.Size = New System.Drawing.Size(75, 23)
            Me.btnNew.TabIndex = 4
            Me.btnNew.Text = "New PO"
            Me.btnNew.UseVisualStyleBackColor = True
            '
            'btnEdit
            '
            Me.btnEdit.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnEdit.Location = New System.Drawing.Point(551, 10)
            Me.btnEdit.Name = "btnEdit"
            Me.btnEdit.Size = New System.Drawing.Size(75, 23)
            Me.btnEdit.TabIndex = 5
            Me.btnEdit.Text = "Edit"
            Me.btnEdit.UseVisualStyleBackColor = True
            '
            'btnSubmit
            '
            Me.btnSubmit.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnSubmit.Location = New System.Drawing.Point(632, 10)
            Me.btnSubmit.Name = "btnSubmit"
            Me.btnSubmit.Size = New System.Drawing.Size(75, 23)
            Me.btnSubmit.TabIndex = 6
            Me.btnSubmit.Text = "Submit"
            Me.btnSubmit.UseVisualStyleBackColor = True
            '
            'btnDelete
            '
            Me.btnDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnDelete.Location = New System.Drawing.Point(713, 10)
            Me.btnDelete.Name = "btnDelete"
            Me.btnDelete.Size = New System.Drawing.Size(75, 23)
            Me.btnDelete.TabIndex = 7
            Me.btnDelete.Text = "Delete"
            Me.btnDelete.UseVisualStyleBackColor = True
            '
            'PurchaseOrderListView
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.btnDelete)
            Me.Controls.Add(Me.btnSubmit)
            Me.Controls.Add(Me.btnEdit)
            Me.Controls.Add(Me.btnNew)
            Me.Controls.Add(Me.btnSearch)
            Me.Controls.Add(Me.cmbStatus)
            Me.Controls.Add(Me.txtSearch)
            Me.Controls.Add(Me.dgvPurchaseOrders)
            Me.Name = "PurchaseOrderListView"
            Me.Size = New System.Drawing.Size(800, 450)
            CType(Me.dgvPurchaseOrders, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()
        End Sub
    End Class
End Namespace
