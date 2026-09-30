Imports System.Windows.Forms

Namespace Views.PurchaseOrders
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class PurchaseOrderListView
        Inherits UserControl

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
            Me.dgvOrders = New System.Windows.Forms.DataGridView()
            Me.cmbStatusFilter = New System.Windows.Forms.ComboBox()
            Me.txtSearch = New System.Windows.Forms.TextBox()
            Me.btnNew = New System.Windows.Forms.Button()
            Me.btnEdit = New System.Windows.Forms.Button()
            Me.btnSubmit = New System.Windows.Forms.Button()
            Me.btnDelete = New System.Windows.Forms.Button()
            Me.lblStatus = New System.Windows.Forms.Label()
            Me.lblSearch = New System.Windows.Forms.Label()
            CType(Me.dgvOrders, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'dgvOrders
            '
            Me.dgvOrders.AllowUserToAddRows = False
            Me.dgvOrders.AllowUserToDeleteRows = False
            Me.dgvOrders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvOrders.Location = New System.Drawing.Point(17, 56)
            Me.dgvOrders.MultiSelect = False
            Me.dgvOrders.Name = "dgvOrders"
            Me.dgvOrders.ReadOnly = True
            Me.dgvOrders.RowHeadersVisible = False
            Me.dgvOrders.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
            Me.dgvOrders.Size = New System.Drawing.Size(767, 335)
            Me.dgvOrders.TabIndex = 0
            '
            'cmbStatusFilter
            '
            Me.cmbStatusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cmbStatusFilter.FormattingEnabled = True
            Me.cmbStatusFilter.Location = New System.Drawing.Point(62, 17)
            Me.cmbStatusFilter.Name = "cmbStatusFilter"
            Me.cmbStatusFilter.Size = New System.Drawing.Size(121, 23)
            Me.cmbStatusFilter.TabIndex = 1
            '
            'txtSearch
            '
            Me.txtSearch.Location = New System.Drawing.Point(244, 17)
            Me.txtSearch.Name = "txtSearch"
            Me.txtSearch.Size = New System.Drawing.Size(200, 23)
            Me.txtSearch.TabIndex = 2
            '
            'btnNew
            '
            Me.btnNew.Location = New System.Drawing.Point(17, 397)
            Me.btnNew.Name = "btnNew"
            Me.btnNew.Size = New System.Drawing.Size(75, 23)
            Me.btnNew.TabIndex = 3
            Me.btnNew.Text = "New"
            Me.btnNew.UseVisualStyleBackColor = True
            '
            'btnEdit
            '
            Me.btnEdit.Location = New System.Drawing.Point(98, 397)
            Me.btnEdit.Name = "btnEdit"
            Me.btnEdit.Size = New System.Drawing.Size(75, 23)
            Me.btnEdit.TabIndex = 4
            Me.btnEdit.Text = "Edit"
            Me.btnEdit.UseVisualStyleBackColor = True
            '
            'btnSubmit
            '
            Me.btnSubmit.Location = New System.Drawing.Point(179, 397)
            Me.btnSubmit.Name = "btnSubmit"
            Me.btnSubmit.Size = New System.Drawing.Size(75, 23)
            Me.btnSubmit.TabIndex = 5
            Me.btnSubmit.Text = "Submit"
            Me.btnSubmit.UseVisualStyleBackColor = True
            '
            'btnDelete
            '
            Me.btnDelete.Location = New System.Drawing.Point(260, 397)
            Me.btnDelete.Name = "btnDelete"
            Me.btnDelete.Size = New System.Drawing.Size(75, 23)
            Me.btnDelete.TabIndex = 6
            Me.btnDelete.Text = "Delete"
            Me.btnDelete.UseVisualStyleBackColor = True
            '
            'lblStatus
            '
            Me.lblStatus.AutoSize = True
            Me.lblStatus.Location = New System.Drawing.Point(14, 20)
            Me.lblStatus.Name = "lblStatus"
            Me.lblStatus.Size = New System.Drawing.Size(42, 15)
            Me.lblStatus.TabIndex = 7
            Me.lblStatus.Text = "Status:"
            '
            'lblSearch
            '
            Me.lblSearch.AutoSize = True
            Me.lblSearch.Location = New System.Drawing.Point(193, 20)
            Me.lblSearch.Name = "lblSearch"
            Me.lblSearch.Size = New System.Drawing.Size(45, 15)
            Me.lblSearch.TabIndex = 8
            Me.lblSearch.Text = "Search:"
            '
            'PurchaseOrderListView
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.lblSearch)
            Me.Controls.Add(Me.lblStatus)
            Me.Controls.Add(Me.btnDelete)
            Me.Controls.Add(Me.btnSubmit)
            Me.Controls.Add(Me.btnEdit)
            Me.Controls.Add(Me.btnNew)
            Me.Controls.Add(Me.txtSearch)
            Me.Controls.Add(Me.cmbStatusFilter)
            Me.Controls.Add(Me.dgvOrders)
            Me.Name = "PurchaseOrderListView"
            Me.Size = New System.Drawing.Size(800, 439)
            CType(Me.dgvOrders, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents dgvOrders As DataGridView
        Friend WithEvents cmbStatusFilter As ComboBox
        Friend WithEvents txtSearch As TextBox
        Friend WithEvents btnNew As Button
        Friend WithEvents btnEdit As Button
        Friend WithEvents btnSubmit As Button
        Friend WithEvents btnDelete As Button
        Friend WithEvents lblStatus As Label
        Friend WithEvents lblSearch As Label
    End Class
End Namespace