Namespace Views.Purchasing
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ReorderSuggestionsView
        Inherits System.Windows.Forms.UserControl

        'UserControl overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
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
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.tabControl = New System.Windows.Forms.TabControl()
            Me.tabSuggestions = New System.Windows.Forms.TabPage()
            Me.dgvSuggestions = New System.Windows.Forms.DataGridView()
            Me.pnlSuggestionsTop = New System.Windows.Forms.Panel()
            Me.lblFilter = New System.Windows.Forms.Label()
            Me.cmbStatusFilter = New System.Windows.Forms.ComboBox()
            Me.btnGenerate = New System.Windows.Forms.Button()
            Me.tabConfiguration = New System.Windows.Forms.TabPage()
            Me.dgvConfigs = New System.Windows.Forms.DataGridView()
            Me.tabControl.SuspendLayout()
            Me.tabSuggestions.SuspendLayout()
            CType(Me.dgvSuggestions, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.pnlSuggestionsTop.SuspendLayout()
            Me.tabConfiguration.SuspendLayout()
            CType(Me.dgvConfigs, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'tabControl
            '
            Me.tabControl.Controls.Add(Me.tabSuggestions)
            Me.tabControl.Controls.Add(Me.tabConfiguration)
            Me.tabControl.Dock = System.Windows.Forms.DockStyle.Fill
            Me.tabControl.Location = New System.Drawing.Point(0, 0)
            Me.tabControl.Name = "tabControl"
            Me.tabControl.SelectedIndex = 0
            Me.tabControl.Size = New System.Drawing.Size(800, 600)
            Me.tabControl.TabIndex = 0
            '
            'tabSuggestions
            '
            Me.tabSuggestions.Controls.Add(Me.dgvSuggestions)
            Me.tabSuggestions.Controls.Add(Me.pnlSuggestionsTop)
            Me.tabSuggestions.Location = New System.Drawing.Point(4, 24)
            Me.tabSuggestions.Name = "tabSuggestions"
            Me.tabSuggestions.Padding = New System.Windows.Forms.Padding(3)
            Me.tabSuggestions.Size = New System.Drawing.Size(792, 572)
            Me.tabSuggestions.TabIndex = 0
            Me.tabSuggestions.Text = "Suggestions"
            Me.tabSuggestions.UseVisualStyleBackColor = True
            '
            'dgvSuggestions
            '
            Me.dgvSuggestions.AllowUserToAddRows = False
            Me.dgvSuggestions.AllowUserToDeleteRows = False
            Me.dgvSuggestions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvSuggestions.Dock = System.Windows.Forms.DockStyle.Fill
            Me.dgvSuggestions.Location = New System.Drawing.Point(3, 43)
            Me.dgvSuggestions.Name = "dgvSuggestions"
            Me.dgvSuggestions.RowTemplate.Height = 25
            Me.dgvSuggestions.Size = New System.Drawing.Size(786, 526)
            Me.dgvSuggestions.TabIndex = 1
            '
            'pnlSuggestionsTop
            '
            Me.pnlSuggestionsTop.Controls.Add(Me.btnGenerate)
            Me.pnlSuggestionsTop.Controls.Add(Me.cmbStatusFilter)
            Me.pnlSuggestionsTop.Controls.Add(Me.lblFilter)
            Me.pnlSuggestionsTop.Dock = System.Windows.Forms.DockStyle.Top
            Me.pnlSuggestionsTop.Location = New System.Drawing.Point(3, 3)
            Me.pnlSuggestionsTop.Name = "pnlSuggestionsTop"
            Me.pnlSuggestionsTop.Size = New System.Drawing.Size(786, 40)
            Me.pnlSuggestionsTop.TabIndex = 0
            '
            'lblFilter
            '
            Me.lblFilter.AutoSize = True
            Me.lblFilter.Location = New System.Drawing.Point(14, 12)
            Me.lblFilter.Name = "lblFilter"
            Me.lblFilter.Size = New System.Drawing.Size(36, 15)
            Me.lblFilter.TabIndex = 0
            Me.lblFilter.Text = "Filter:"
            '
            'cmbStatusFilter
            '
            Me.cmbStatusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cmbStatusFilter.FormattingEnabled = True
            Me.cmbStatusFilter.Items.AddRange(New Object() {"Pending", "Accepted", "Dismissed", "All"})
            Me.cmbStatusFilter.Location = New System.Drawing.Point(56, 9)
            Me.cmbStatusFilter.Name = "cmbStatusFilter"
            Me.cmbStatusFilter.Size = New System.Drawing.Size(121, 23)
            Me.cmbStatusFilter.TabIndex = 1
            '
            'btnGenerate
            '
            Me.btnGenerate.Location = New System.Drawing.Point(195, 9)
            Me.btnGenerate.Name = "btnGenerate"
            Me.btnGenerate.Size = New System.Drawing.Size(150, 23)
            Me.btnGenerate.TabIndex = 2
            Me.btnGenerate.Text = "Generate Suggestions"
            Me.btnGenerate.UseVisualStyleBackColor = True
            '
            'tabConfiguration
            '
            Me.tabConfiguration.Controls.Add(Me.dgvConfigs)
            Me.tabConfiguration.Location = New System.Drawing.Point(4, 24)
            Me.tabConfiguration.Name = "tabConfiguration"
            Me.tabConfiguration.Padding = New System.Windows.Forms.Padding(3)
            Me.tabConfiguration.Size = New System.Drawing.Size(792, 572)
            Me.tabConfiguration.TabIndex = 1
            Me.tabConfiguration.Text = "Configuration"
            Me.tabConfiguration.UseVisualStyleBackColor = True
            '
            'dgvConfigs
            '
            Me.dgvConfigs.AllowUserToAddRows = False
            Me.dgvConfigs.AllowUserToDeleteRows = False
            Me.dgvConfigs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvConfigs.Dock = System.Windows.Forms.DockStyle.Fill
            Me.dgvConfigs.Location = New System.Drawing.Point(3, 3)
            Me.dgvConfigs.Name = "dgvConfigs"
            Me.dgvConfigs.ReadOnly = True
            Me.dgvConfigs.RowTemplate.Height = 25
            Me.dgvConfigs.Size = New System.Drawing.Size(786, 566)
            Me.dgvConfigs.TabIndex = 0
            '
            'ReorderSuggestionsView
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.tabControl)
            Me.Name = "ReorderSuggestionsView"
            Me.Size = New System.Drawing.Size(800, 600)
            Me.tabControl.ResumeLayout(False)
            Me.tabSuggestions.ResumeLayout(False)
            CType(Me.dgvSuggestions, System.ComponentModel.ISupportInitialize).EndInit()
            Me.pnlSuggestionsTop.ResumeLayout(False)
            Me.pnlSuggestionsTop.PerformLayout()
            Me.tabConfiguration.ResumeLayout(False)
            CType(Me.dgvConfigs, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents tabControl As System.Windows.Forms.TabControl
        Friend WithEvents tabSuggestions As System.Windows.Forms.TabPage
        Friend WithEvents tabConfiguration As System.Windows.Forms.TabPage
        Friend WithEvents pnlSuggestionsTop As System.Windows.Forms.Panel
        Friend WithEvents cmbStatusFilter As System.Windows.Forms.ComboBox
        Friend WithEvents lblFilter As System.Windows.Forms.Label
        Friend WithEvents btnGenerate As System.Windows.Forms.Button
        Friend WithEvents dgvSuggestions As System.Windows.Forms.DataGridView
        Friend WithEvents dgvConfigs As System.Windows.Forms.DataGridView
    End Class
End Namespace
