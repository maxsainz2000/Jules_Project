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
        Me.SearchTextBox = New System.Windows.Forms.ToolStripTextBox()
        Me.RefreshButton = New System.Windows.Forms.ToolStripButton()
        Me.AddButton = New System.Windows.Forms.ToolStripButton()
        Me.EditButton = New System.Windows.Forms.ToolStripButton()
        Me.DeleteButton = New System.Windows.Forms.ToolStripButton()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.VendorGrid = New System.Windows.Forms.DataGridView()
        Me.DetailPanel = New System.Windows.Forms.Panel()
        Me.POGrid = New System.Windows.Forms.DataGridView()
        Me.StatsLabel = New System.Windows.Forms.Label()
        Me.DetailTitleLabel = New System.Windows.Forms.Label()
        Me.EditorPanel = New System.Windows.Forms.Panel()
        Me.CancelEditButton = New System.Windows.Forms.Button()
        Me.SaveEditButton = New System.Windows.Forms.Button()
        Me.LeadTimeTextBox = New System.Windows.Forms.TextBox()
        Me.LeadTimeLabel = New System.Windows.Forms.Label()
        Me.PhoneTextBox = New System.Windows.Forms.TextBox()
        Me.PhoneLabel = New System.Windows.Forms.Label()
        Me.EmailTextBox = New System.Windows.Forms.TextBox()
        Me.EmailLabel = New System.Windows.Forms.Label()
        Me.ContactTextBox = New System.Windows.Forms.TextBox()
        Me.ContactLabel = New System.Windows.Forms.Label()
        Me.AddressTextBox = New System.Windows.Forms.TextBox()
        Me.AddressLabel = New System.Windows.Forms.Label()
        Me.TaxIdTextBox = New System.Windows.Forms.TextBox()
        Me.TaxIdLabel = New System.Windows.Forms.Label()
        Me.NotesTextBox = New System.Windows.Forms.TextBox()
        Me.NotesLabel = New System.Windows.Forms.Label()
        Me.NameTextBox = New System.Windows.Forms.TextBox()
        Me.NameLabel = New System.Windows.Forms.Label()
        Me.EditorTitleLabel = New System.Windows.Forms.Label()
        Me.ToolStrip1.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.VendorGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.DetailPanel.SuspendLayout()
        CType(Me.POGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.EditorPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SearchTextBox, Me.RefreshButton, Me.AddButton, Me.EditButton, Me.DeleteButton})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(800, 25)
        Me.ToolStrip1.TabIndex = 0
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'SearchTextBox
        '
        Me.SearchTextBox.Name = "SearchTextBox"
        Me.SearchTextBox.Size = New System.Drawing.Size(200, 25)
        Me.SearchTextBox.ToolTipText = "Search vendors... (Press Escape to clear)"
        '
        'RefreshButton
        '
        Me.RefreshButton.Name = "RefreshButton"
        Me.RefreshButton.Size = New System.Drawing.Size(50, 22)
        Me.RefreshButton.Text = "Refresh"
        '
        'AddButton
        '
        Me.AddButton.Name = "AddButton"
        Me.AddButton.Size = New System.Drawing.Size(33, 22)
        Me.AddButton.Text = "Add"
        '
        'EditButton
        '
        Me.EditButton.Name = "EditButton"
        Me.EditButton.Size = New System.Drawing.Size(31, 22)
        Me.EditButton.Text = "Edit"
        '
        'DeleteButton
        '
        Me.DeleteButton.Name = "DeleteButton"
        Me.DeleteButton.Size = New System.Drawing.Size(44, 22)
        Me.DeleteButton.Text = "Delete"
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
        Me.SplitContainer1.Panel2.Controls.Add(Me.DetailPanel)
        Me.SplitContainer1.Size = New System.Drawing.Size(800, 425)
        Me.SplitContainer1.SplitterDistance = 450
        Me.SplitContainer1.TabIndex = 1
        '
        'VendorGrid
        '
        Me.VendorGrid.AllowUserToAddRows = False
        Me.VendorGrid.AllowUserToDeleteRows = False
        Me.VendorGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.VendorGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.VendorGrid.Location = New System.Drawing.Point(0, 0)
        Me.VendorGrid.MultiSelect = False
        Me.VendorGrid.Name = "VendorGrid"
        Me.VendorGrid.ReadOnly = True
        Me.VendorGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.VendorGrid.Size = New System.Drawing.Size(450, 425)
        Me.VendorGrid.TabIndex = 0
        '
        'DetailPanel
        '
        Me.DetailPanel.Controls.Add(Me.POGrid)
        Me.DetailPanel.Controls.Add(Me.StatsLabel)
        Me.DetailPanel.Controls.Add(Me.DetailTitleLabel)
        Me.DetailPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DetailPanel.Location = New System.Drawing.Point(0, 0)
        Me.DetailPanel.Name = "DetailPanel"
        Me.DetailPanel.Padding = New System.Windows.Forms.Padding(10)
        Me.DetailPanel.Size = New System.Drawing.Size(346, 425)
        Me.DetailPanel.TabIndex = 0
        '
        'POGrid
        '
        Me.POGrid.AllowUserToAddRows = False
        Me.POGrid.AllowUserToDeleteRows = False
        Me.POGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.POGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.POGrid.Location = New System.Drawing.Point(10, 80)
        Me.POGrid.Name = "POGrid"
        Me.POGrid.ReadOnly = True
        Me.POGrid.Size = New System.Drawing.Size(326, 335)
        Me.POGrid.TabIndex = 2
        '
        'StatsLabel
        '
        Me.StatsLabel.Dock = System.Windows.Forms.DockStyle.Top
        Me.StatsLabel.Location = New System.Drawing.Point(10, 30)
        Me.StatsLabel.Name = "StatsLabel"
        Me.StatsLabel.Size = New System.Drawing.Size(326, 50)
        Me.StatsLabel.TabIndex = 1
        Me.StatsLabel.Text = "Stats..."
        '
        'DetailTitleLabel
        '
        Me.DetailTitleLabel.Dock = System.Windows.Forms.DockStyle.Top
        Me.DetailTitleLabel.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me.DetailTitleLabel.Location = New System.Drawing.Point(10, 10)
        Me.DetailTitleLabel.Name = "DetailTitleLabel"
        Me.DetailTitleLabel.Size = New System.Drawing.Size(326, 20)
        Me.DetailTitleLabel.TabIndex = 0
        Me.DetailTitleLabel.Text = "Vendor Details"
        '
        'EditorPanel
        '
        Me.EditorPanel.Controls.Add(Me.CancelEditButton)
        Me.EditorPanel.Controls.Add(Me.SaveEditButton)
        Me.EditorPanel.Controls.Add(Me.NameLabel)
        Me.EditorPanel.Controls.Add(Me.NameTextBox)
        Me.EditorPanel.Controls.Add(Me.PhoneLabel)
        Me.EditorPanel.Controls.Add(Me.PhoneTextBox)
        Me.EditorPanel.Controls.Add(Me.EmailLabel)
        Me.EditorPanel.Controls.Add(Me.EmailTextBox)
        Me.EditorPanel.Controls.Add(Me.ContactLabel)
        Me.EditorPanel.Controls.Add(Me.ContactTextBox)
        Me.EditorPanel.Controls.Add(Me.AddressLabel)
        Me.EditorPanel.Controls.Add(Me.AddressTextBox)
        Me.EditorPanel.Controls.Add(Me.TaxIdLabel)
        Me.EditorPanel.Controls.Add(Me.TaxIdTextBox)
        Me.EditorPanel.Controls.Add(Me.LeadTimeLabel)
        Me.EditorPanel.Controls.Add(Me.LeadTimeTextBox)
        Me.EditorPanel.Controls.Add(Me.NotesLabel)
        Me.EditorPanel.Controls.Add(Me.NotesTextBox)
        Me.EditorPanel.Controls.Add(Me.EditorTitleLabel)
        Me.EditorPanel.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.EditorPanel.Location = New System.Drawing.Point(0, 450)
        Me.EditorPanel.Name = "EditorPanel"
        Me.EditorPanel.Size = New System.Drawing.Size(800, 200)
        Me.EditorPanel.TabIndex = 2
        Me.EditorPanel.Visible = False
        '
        'EditorTitleLabel
        '
        Me.EditorTitleLabel.AutoSize = True
        Me.EditorTitleLabel.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me.EditorTitleLabel.Location = New System.Drawing.Point(14, 14)
        Me.EditorTitleLabel.Name = "EditorTitleLabel"
        Me.EditorTitleLabel.Size = New System.Drawing.Size(42, 15)
        Me.EditorTitleLabel.TabIndex = 0
        Me.EditorTitleLabel.Text = "Editor"
        '
        'NameLabel
        '
        Me.NameLabel.AutoSize = True
        Me.NameLabel.Location = New System.Drawing.Point(14, 41)
        Me.NameLabel.Name = "NameLabel"
        Me.NameLabel.Size = New System.Drawing.Size(42, 15)
        Me.NameLabel.TabIndex = 1
        Me.NameLabel.Text = "Name:"
        '
        'NameTextBox
        '
        Me.NameTextBox.Location = New System.Drawing.Point(100, 38)
        Me.NameTextBox.Name = "NameTextBox"
        Me.NameTextBox.Size = New System.Drawing.Size(200, 23)
        Me.NameTextBox.TabIndex = 2
        '
        'PhoneLabel
        '
        Me.PhoneLabel.AutoSize = True
        Me.PhoneLabel.Location = New System.Drawing.Point(14, 70)
        Me.PhoneLabel.Name = "PhoneLabel"
        Me.PhoneLabel.Size = New System.Drawing.Size(44, 15)
        Me.PhoneLabel.TabIndex = 3
        Me.PhoneLabel.Text = "Phone:"
        '
        'PhoneTextBox
        '
        Me.PhoneTextBox.Location = New System.Drawing.Point(100, 67)
        Me.PhoneTextBox.Name = "PhoneTextBox"
        Me.PhoneTextBox.Size = New System.Drawing.Size(200, 23)
        Me.PhoneTextBox.TabIndex = 4
        '
        'EmailLabel
        '
        Me.EmailLabel.AutoSize = True
        Me.EmailLabel.Location = New System.Drawing.Point(14, 99)
        Me.EmailLabel.Name = "EmailLabel"
        Me.EmailLabel.Size = New System.Drawing.Size(39, 15)
        Me.EmailLabel.TabIndex = 5
        Me.EmailLabel.Text = "Email:"
        '
        'EmailTextBox
        '
        Me.EmailTextBox.Location = New System.Drawing.Point(100, 96)
        Me.EmailTextBox.Name = "EmailTextBox"
        Me.EmailTextBox.Size = New System.Drawing.Size(200, 23)
        Me.EmailTextBox.TabIndex = 6
        '
        'ContactLabel
        '
        Me.ContactLabel.AutoSize = True
        Me.ContactLabel.Location = New System.Drawing.Point(14, 128)
        Me.ContactLabel.Name = "ContactLabel"
        Me.ContactLabel.Size = New System.Drawing.Size(51, 15)
        Me.ContactLabel.TabIndex = 7
        Me.ContactLabel.Text = "Contact:"
        '
        'ContactTextBox
        '
        Me.ContactTextBox.Location = New System.Drawing.Point(100, 125)
        Me.ContactTextBox.Name = "ContactTextBox"
        Me.ContactTextBox.Size = New System.Drawing.Size(200, 23)
        Me.ContactTextBox.TabIndex = 8
        '
        'AddressLabel
        '
        Me.AddressLabel.AutoSize = True
        Me.AddressLabel.Location = New System.Drawing.Point(320, 41)
        Me.AddressLabel.Name = "AddressLabel"
        Me.AddressLabel.Size = New System.Drawing.Size(52, 15)
        Me.AddressLabel.TabIndex = 9
        Me.AddressLabel.Text = "Address:"
        '
        'AddressTextBox
        '
        Me.AddressTextBox.Location = New System.Drawing.Point(380, 38)
        Me.AddressTextBox.Name = "AddressTextBox"
        Me.AddressTextBox.Size = New System.Drawing.Size(200, 23)
        Me.AddressTextBox.TabIndex = 10
        '
        'TaxIdLabel
        '
        Me.TaxIdLabel.AutoSize = True
        Me.TaxIdLabel.Location = New System.Drawing.Point(320, 70)
        Me.TaxIdLabel.Name = "TaxIdLabel"
        Me.TaxIdLabel.Size = New System.Drawing.Size(39, 15)
        Me.TaxIdLabel.TabIndex = 11
        Me.TaxIdLabel.Text = "TaxId:"
        '
        'TaxIdTextBox
        '
        Me.TaxIdTextBox.Location = New System.Drawing.Point(380, 67)
        Me.TaxIdTextBox.Name = "TaxIdTextBox"
        Me.TaxIdTextBox.Size = New System.Drawing.Size(200, 23)
        Me.TaxIdTextBox.TabIndex = 12
        '
        'LeadTimeLabel
        '
        Me.LeadTimeLabel.AutoSize = True
        Me.LeadTimeLabel.Location = New System.Drawing.Point(320, 99)
        Me.LeadTimeLabel.Name = "LeadTimeLabel"
        Me.LeadTimeLabel.Size = New System.Drawing.Size(63, 15)
        Me.LeadTimeLabel.TabIndex = 13
        Me.LeadTimeLabel.Text = "Lead Time:"
        '
        'LeadTimeTextBox
        '
        Me.LeadTimeTextBox.Location = New System.Drawing.Point(380, 96)
        Me.LeadTimeTextBox.Name = "LeadTimeTextBox"
        Me.LeadTimeTextBox.Size = New System.Drawing.Size(100, 23)
        Me.LeadTimeTextBox.TabIndex = 14
        '
        'NotesLabel
        '
        Me.NotesLabel.AutoSize = True
        Me.NotesLabel.Location = New System.Drawing.Point(320, 128)
        Me.NotesLabel.Name = "NotesLabel"
        Me.NotesLabel.Size = New System.Drawing.Size(41, 15)
        Me.NotesLabel.TabIndex = 15
        Me.NotesLabel.Text = "Notes:"
        '
        'NotesTextBox
        '
        Me.NotesTextBox.Location = New System.Drawing.Point(380, 125)
        Me.NotesTextBox.Name = "NotesTextBox"
        Me.NotesTextBox.Size = New System.Drawing.Size(200, 23)
        Me.NotesTextBox.TabIndex = 16
        '
        'SaveEditButton
        '
        Me.SaveEditButton.Location = New System.Drawing.Point(14, 160)
        Me.SaveEditButton.Name = "SaveEditButton"
        Me.SaveEditButton.Size = New System.Drawing.Size(75, 23)
        Me.SaveEditButton.TabIndex = 17
        Me.SaveEditButton.Text = "Save"
        Me.SaveEditButton.UseVisualStyleBackColor = True
        '
        'CancelEditButton
        '
        Me.CancelEditButton.Location = New System.Drawing.Point(95, 160)
        Me.CancelEditButton.Name = "CancelEditButton"
        Me.CancelEditButton.Size = New System.Drawing.Size(75, 23)
        Me.CancelEditButton.TabIndex = 18
        Me.CancelEditButton.Text = "Cancel"
        Me.CancelEditButton.UseVisualStyleBackColor = True
        '
        'VendorDirectoryView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.EditorPanel)
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
        Me.DetailPanel.ResumeLayout(False)
        CType(Me.POGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.EditorPanel.ResumeLayout(False)
        Me.EditorPanel.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Friend WithEvents SearchTextBox As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents RefreshButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents AddButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents EditButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents DeleteButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents VendorGrid As System.Windows.Forms.DataGridView
    Friend WithEvents DetailPanel As System.Windows.Forms.Panel
    Friend WithEvents POGrid As System.Windows.Forms.DataGridView
    Friend WithEvents StatsLabel As System.Windows.Forms.Label
    Friend WithEvents DetailTitleLabel As System.Windows.Forms.Label
    Friend WithEvents EditorPanel As System.Windows.Forms.Panel
    Friend WithEvents CancelEditButton As System.Windows.Forms.Button
    Friend WithEvents SaveEditButton As System.Windows.Forms.Button
    Friend WithEvents LeadTimeTextBox As System.Windows.Forms.TextBox
    Friend WithEvents LeadTimeLabel As System.Windows.Forms.Label
    Friend WithEvents PhoneTextBox As System.Windows.Forms.TextBox
    Friend WithEvents PhoneLabel As System.Windows.Forms.Label
    Friend WithEvents EmailTextBox As System.Windows.Forms.TextBox
    Friend WithEvents EmailLabel As System.Windows.Forms.Label
    Friend WithEvents ContactTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ContactLabel As System.Windows.Forms.Label
    Friend WithEvents AddressTextBox As System.Windows.Forms.TextBox
    Friend WithEvents AddressLabel As System.Windows.Forms.Label
    Friend WithEvents TaxIdTextBox As System.Windows.Forms.TextBox
    Friend WithEvents TaxIdLabel As System.Windows.Forms.Label
    Friend WithEvents NotesTextBox As System.Windows.Forms.TextBox
    Friend WithEvents NotesLabel As System.Windows.Forms.Label
    Friend WithEvents NameTextBox As System.Windows.Forms.TextBox
    Friend WithEvents NameLabel As System.Windows.Forms.Label
    Friend WithEvents EditorTitleLabel As System.Windows.Forms.Label

End Class
End Namespace
