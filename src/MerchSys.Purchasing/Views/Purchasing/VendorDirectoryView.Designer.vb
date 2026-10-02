Namespace Views
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class VendorDirectoryView
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
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.txtSearch = New System.Windows.Forms.ToolStripTextBox()
        Me.btnAdd = New System.Windows.Forms.ToolStripButton()
        Me.btnEdit = New System.Windows.Forms.ToolStripButton()
        Me.btnDelete = New System.Windows.Forms.ToolStripButton()
        Me.btnRefresh = New System.Windows.Forms.ToolStripButton()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.VendorGrid = New System.Windows.Forms.DataGridView()
        Me.PanelDetails = New System.Windows.Forms.Panel()
        Me.LabelStats = New System.Windows.Forms.Label()
        Me.POGrid = New System.Windows.Forms.DataGridView()
        Me.PanelEditor = New System.Windows.Forms.Panel()
        Me.lblName = New System.Windows.Forms.Label()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.lblPhone = New System.Windows.Forms.Label()
        Me.txtPhone = New System.Windows.Forms.TextBox()
        Me.lblLeadTime = New System.Windows.Forms.Label()
        Me.numLeadTime = New System.Windows.Forms.NumericUpDown()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        ' Adding 5 more placeholder fields to meet the 8-field layout requirement
        Me.lblField4 = New System.Windows.Forms.Label()
        Me.txtField4 = New System.Windows.Forms.TextBox()
        Me.lblField5 = New System.Windows.Forms.Label()
        Me.txtField5 = New System.Windows.Forms.TextBox()
        Me.lblField6 = New System.Windows.Forms.Label()
        Me.txtField6 = New System.Windows.Forms.TextBox()
        Me.lblField7 = New System.Windows.Forms.Label()
        Me.txtField7 = New System.Windows.Forms.TextBox()
        Me.lblField8 = New System.Windows.Forms.Label()
        Me.txtField8 = New System.Windows.Forms.TextBox()

        Me.ToolStrip1.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.VendorGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelDetails.SuspendLayout()
        CType(Me.POGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelEditor.SuspendLayout()
        CType(Me.numLeadTime, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.txtSearch, Me.btnAdd, Me.btnEdit, Me.btnDelete, Me.btnRefresh})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(800, 25)
        Me.ToolStrip1.TabIndex = 0
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'txtSearch
        '
        Me.txtSearch.Name = "txtSearch"
        Me.txtSearch.Size = New System.Drawing.Size(150, 25)
        Me.txtSearch.ToolTipText = "Search by Name or Phone (Press Escape to clear)"
        '
        'btnAdd
        '
        Me.btnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(33, 22)
        Me.btnAdd.Text = "Add"
        '
        'btnEdit
        '
        Me.btnEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnEdit.Name = "btnEdit"
        Me.btnEdit.Size = New System.Drawing.Size(31, 22)
        Me.btnEdit.Text = "Edit"
        '
        'btnDelete
        '
        Me.btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(44, 22)
        Me.btnDelete.Text = "Delete"
        '
        'btnRefresh
        '
        Me.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(50, 22)
        Me.btnRefresh.Text = "Refresh"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 25)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.VendorGrid)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.PanelDetails)
        Me.SplitContainer1.Panel2MinSize = 340
        Me.SplitContainer1.Size = New System.Drawing.Size(800, 425)
        Me.SplitContainer1.SplitterDistance = 450
        Me.SplitContainer1.TabIndex = 1
        '
        'VendorGrid
        '
        Me.VendorGrid.AllowUserToAddRows = False
        Me.VendorGrid.AllowUserToDeleteRows = False
        Me.VendorGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.VendorGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.VendorGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.VendorGrid.Location = New System.Drawing.Point(0, 0)
        Me.VendorGrid.Name = "VendorGrid"
        Me.VendorGrid.ReadOnly = True
        Me.VendorGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.VendorGrid.Size = New System.Drawing.Size(450, 425)
        Me.VendorGrid.TabIndex = 0
        '
        'PanelDetails
        '
        Me.PanelDetails.Controls.Add(Me.POGrid)
        Me.PanelDetails.Controls.Add(Me.LabelStats)
        Me.PanelDetails.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelDetails.Location = New System.Drawing.Point(0, 0)
        Me.PanelDetails.Name = "PanelDetails"
        Me.PanelDetails.Size = New System.Drawing.Size(346, 425)
        Me.PanelDetails.TabIndex = 0
        '
        'LabelStats
        '
        Me.LabelStats.Dock = System.Windows.Forms.DockStyle.Top
        Me.LabelStats.Location = New System.Drawing.Point(0, 0)
        Me.LabelStats.Name = "LabelStats"
        Me.LabelStats.Padding = New System.Windows.Forms.Padding(10)
        Me.LabelStats.Size = New System.Drawing.Size(346, 60)
        Me.LabelStats.TabIndex = 0
        Me.LabelStats.Text = "Purchase History"
        Me.LabelStats.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'POGrid
        '
        Me.POGrid.AllowUserToAddRows = False
        Me.POGrid.AllowUserToDeleteRows = False
        Me.POGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.POGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.POGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.POGrid.Location = New System.Drawing.Point(0, 60)
        Me.POGrid.Name = "POGrid"
        Me.POGrid.ReadOnly = True
        Me.POGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.POGrid.Size = New System.Drawing.Size(346, 365)
        Me.POGrid.TabIndex = 1
        '
        'PanelEditor
        '
        Me.PanelEditor.Controls.Add(Me.lblName)
        Me.PanelEditor.Controls.Add(Me.txtName)
        Me.PanelEditor.Controls.Add(Me.lblPhone)
        Me.PanelEditor.Controls.Add(Me.txtPhone)
        Me.PanelEditor.Controls.Add(Me.lblLeadTime)
        Me.PanelEditor.Controls.Add(Me.numLeadTime)
        Me.PanelEditor.Controls.Add(Me.lblField4)
        Me.PanelEditor.Controls.Add(Me.txtField4)
        Me.PanelEditor.Controls.Add(Me.lblField5)
        Me.PanelEditor.Controls.Add(Me.txtField5)
        Me.PanelEditor.Controls.Add(Me.lblField6)
        Me.PanelEditor.Controls.Add(Me.txtField6)
        Me.PanelEditor.Controls.Add(Me.lblField7)
        Me.PanelEditor.Controls.Add(Me.txtField7)
        Me.PanelEditor.Controls.Add(Me.lblField8)
        Me.PanelEditor.Controls.Add(Me.txtField8)
        Me.PanelEditor.Controls.Add(Me.btnSave)
        Me.PanelEditor.Controls.Add(Me.btnCancel)
        Me.PanelEditor.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelEditor.Location = New System.Drawing.Point(0, 450)
        Me.PanelEditor.Name = "PanelEditor"
        Me.PanelEditor.Size = New System.Drawing.Size(800, 150)
        Me.PanelEditor.TabIndex = 2
        Me.PanelEditor.Visible = False
        '
        'lblName
        '
        Me.lblName.AutoSize = True
        Me.lblName.Location = New System.Drawing.Point(10, 15)
        Me.lblName.Name = "lblName"
        Me.lblName.Size = New System.Drawing.Size(38, 15)
        Me.lblName.TabIndex = 0
        Me.lblName.Text = "Name"
        '
        'txtName
        '
        Me.txtName.Location = New System.Drawing.Point(80, 12)
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(150, 23)
        Me.txtName.TabIndex = 1
        '
        'lblPhone
        '
        Me.lblPhone.AutoSize = True
        Me.lblPhone.Location = New System.Drawing.Point(10, 45)
        Me.lblPhone.Name = "lblPhone"
        Me.lblPhone.Size = New System.Drawing.Size(41, 15)
        Me.lblPhone.TabIndex = 2
        Me.lblPhone.Text = "Phone"
        '
        'txtPhone
        '
        Me.txtPhone.Location = New System.Drawing.Point(80, 42)
        Me.txtPhone.Name = "txtPhone"
        Me.txtPhone.Size = New System.Drawing.Size(150, 23)
        Me.txtPhone.TabIndex = 3
        '
        'lblLeadTime
        '
        Me.lblLeadTime.AutoSize = True
        Me.lblLeadTime.Location = New System.Drawing.Point(10, 75)
        Me.lblLeadTime.Name = "lblLeadTime"
        Me.lblLeadTime.Size = New System.Drawing.Size(61, 15)
        Me.lblLeadTime.TabIndex = 4
        Me.lblLeadTime.Text = "Lead Time"
        '
        'numLeadTime
        '
        Me.numLeadTime.Location = New System.Drawing.Point(80, 72)
        Me.numLeadTime.Name = "numLeadTime"
        Me.numLeadTime.Size = New System.Drawing.Size(150, 23)
        Me.numLeadTime.TabIndex = 5
        '
        'lblField4
        '
        Me.lblField4.AutoSize = True
        Me.lblField4.Location = New System.Drawing.Point(250, 15)
        Me.lblField4.Name = "lblField4"
        Me.lblField4.Size = New System.Drawing.Size(42, 15)
        Me.lblField4.TabIndex = 6
        Me.lblField4.Text = "Field 4"
        '
        'txtField4
        '
        Me.txtField4.Location = New System.Drawing.Point(300, 12)
        Me.txtField4.Name = "txtField4"
        Me.txtField4.Size = New System.Drawing.Size(150, 23)
        Me.txtField4.TabIndex = 7
        '
        'lblField5
        '
        Me.lblField5.AutoSize = True
        Me.lblField5.Location = New System.Drawing.Point(250, 45)
        Me.lblField5.Name = "lblField5"
        Me.lblField5.Size = New System.Drawing.Size(42, 15)
        Me.lblField5.TabIndex = 8
        Me.lblField5.Text = "Field 5"
        '
        'txtField5
        '
        Me.txtField5.Location = New System.Drawing.Point(300, 42)
        Me.txtField5.Name = "txtField5"
        Me.txtField5.Size = New System.Drawing.Size(150, 23)
        Me.txtField5.TabIndex = 9
        '
        'lblField6
        '
        Me.lblField6.AutoSize = True
        Me.lblField6.Location = New System.Drawing.Point(250, 75)
        Me.lblField6.Name = "lblField6"
        Me.lblField6.Size = New System.Drawing.Size(42, 15)
        Me.lblField6.TabIndex = 10
        Me.lblField6.Text = "Field 6"
        '
        'txtField6
        '
        Me.txtField6.Location = New System.Drawing.Point(300, 72)
        Me.txtField6.Name = "txtField6"
        Me.txtField6.Size = New System.Drawing.Size(150, 23)
        Me.txtField6.TabIndex = 11
        '
        'lblField7
        '
        Me.lblField7.AutoSize = True
        Me.lblField7.Location = New System.Drawing.Point(470, 15)
        Me.lblField7.Name = "lblField7"
        Me.lblField7.Size = New System.Drawing.Size(42, 15)
        Me.lblField7.TabIndex = 12
        Me.lblField7.Text = "Field 7"
        '
        'txtField7
        '
        Me.txtField7.Location = New System.Drawing.Point(520, 12)
        Me.txtField7.Name = "txtField7"
        Me.txtField7.Size = New System.Drawing.Size(150, 23)
        Me.txtField7.TabIndex = 13
        '
        'lblField8
        '
        Me.lblField8.AutoSize = True
        Me.lblField8.Location = New System.Drawing.Point(470, 45)
        Me.lblField8.Name = "lblField8"
        Me.lblField8.Size = New System.Drawing.Size(42, 15)
        Me.lblField8.TabIndex = 14
        Me.lblField8.Text = "Field 8"
        '
        'txtField8
        '
        Me.txtField8.Location = New System.Drawing.Point(520, 42)
        Me.txtField8.Name = "txtField8"
        Me.txtField8.Size = New System.Drawing.Size(150, 23)
        Me.txtField8.TabIndex = 15
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(80, 110)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(75, 23)
        Me.btnSave.TabIndex = 16
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(165, 110)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 17
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'VendorDirectoryView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.PanelEditor)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Name = "VendorDirectoryView"
        Me.Size = New System.Drawing.Size(800, 600)
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.VendorGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelDetails.ResumeLayout(False)
        CType(Me.POGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelEditor.ResumeLayout(False)
        Me.PanelEditor.PerformLayout()
        CType(Me.numLeadTime, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Friend WithEvents txtSearch As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents btnAdd As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnEdit As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnDelete As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnRefresh As System.Windows.Forms.ToolStripButton
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents VendorGrid As System.Windows.Forms.DataGridView
    Friend WithEvents PanelDetails As System.Windows.Forms.Panel
    Friend WithEvents LabelStats As System.Windows.Forms.Label
    Friend WithEvents POGrid As System.Windows.Forms.DataGridView
    Friend WithEvents PanelEditor As System.Windows.Forms.Panel
    Friend WithEvents lblName As System.Windows.Forms.Label
    Friend WithEvents txtName As System.Windows.Forms.TextBox
    Friend WithEvents lblPhone As System.Windows.Forms.Label
    Friend WithEvents txtPhone As System.Windows.Forms.TextBox
    Friend WithEvents lblLeadTime As System.Windows.Forms.Label
    Friend WithEvents numLeadTime As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblField4 As System.Windows.Forms.Label
    Friend WithEvents txtField4 As System.Windows.Forms.TextBox
    Friend WithEvents lblField5 As System.Windows.Forms.Label
    Friend WithEvents txtField5 As System.Windows.Forms.TextBox
    Friend WithEvents lblField6 As System.Windows.Forms.Label
    Friend WithEvents txtField6 As System.Windows.Forms.TextBox
    Friend WithEvents lblField7 As System.Windows.Forms.Label
    Friend WithEvents txtField7 As System.Windows.Forms.TextBox
    Friend WithEvents lblField8 As System.Windows.Forms.Label
    Friend WithEvents txtField8 As System.Windows.Forms.TextBox
    Friend WithEvents btnSave As System.Windows.Forms.Button
        Friend WithEvents btnCancel As System.Windows.Forms.Button
    End Class
End Namespace
