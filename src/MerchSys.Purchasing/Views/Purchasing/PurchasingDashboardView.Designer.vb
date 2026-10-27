Namespace Views.Purchasing
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class PurchasingDashboardView
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
            Me.components = New System.ComponentModel.Container()
            Me.pnlOutstandingAp = New System.Windows.Forms.Panel()
            Me.lblOutstandingApTitle = New System.Windows.Forms.Label()
            Me.lblOutstandingApValue = New System.Windows.Forms.Label()
            Me.pnlOverdueAp = New System.Windows.Forms.Panel()
            Me.lblOverdueApTitle = New System.Windows.Forms.Label()
            Me.lblOverdueApValue = New System.Windows.Forms.Label()
            Me.pnlPendingDeliveries = New System.Windows.Forms.Panel()
            Me.lblPendingDeliveriesTitle = New System.Windows.Forms.Label()
            Me.lblPendingDeliveriesValue = New System.Windows.Forms.Label()
            Me.pnlReorderSuggestions = New System.Windows.Forms.Panel()
            Me.lblReorderSuggestionsTitle = New System.Windows.Forms.Label()
            Me.lblReorderSuggestionsValue = New System.Windows.Forms.Label()
            Me.pnlActiveVendors = New System.Windows.Forms.Panel()
            Me.lblActiveVendorsTitle = New System.Windows.Forms.Label()
            Me.lblActiveVendorsValue = New System.Windows.Forms.Label()
            Me.pnlAvgLeadTime = New System.Windows.Forms.Panel()
            Me.lblAvgLeadTimeTitle = New System.Windows.Forms.Label()
            Me.lblAvgLeadTimeValue = New System.Windows.Forms.Label()
            Me.pnlToolbar = New System.Windows.Forms.Panel()
            Me.btnRefresh = New System.Windows.Forms.Button()
            Me.dgvPendingDeliveries = New System.Windows.Forms.DataGridView()
            Me.dgvTopVendors = New System.Windows.Forms.DataGridView()
            Me.lblPendingDeliveriesGridTitle = New System.Windows.Forms.Label()
            Me.lblTopVendorsGridTitle = New System.Windows.Forms.Label()
            Me.lblErrorMessage = New System.Windows.Forms.Label()

            Me.pnlOutstandingAp.SuspendLayout()
            Me.pnlOverdueAp.SuspendLayout()
            Me.pnlPendingDeliveries.SuspendLayout()
            Me.pnlReorderSuggestions.SuspendLayout()
            Me.pnlActiveVendors.SuspendLayout()
            Me.pnlAvgLeadTime.SuspendLayout()
            Me.pnlToolbar.SuspendLayout()
            CType(Me.dgvPendingDeliveries, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.dgvTopVendors, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()

            '
            ' pnlOutstandingAp
            '
            Me.pnlOutstandingAp.Controls.Add(Me.lblOutstandingApTitle)
            Me.pnlOutstandingAp.Controls.Add(Me.lblOutstandingApValue)
            Me.pnlOutstandingAp.Location = New System.Drawing.Point(10, 50)
            Me.pnlOutstandingAp.Name = "pnlOutstandingAp"
            Me.pnlOutstandingAp.Size = New System.Drawing.Size(250, 100)
            Me.pnlOutstandingAp.TabIndex = 1
            Me.pnlOutstandingAp.Tag = "PrimaryMetricCard"
            '
            ' lblOutstandingApTitle
            '
            Me.lblOutstandingApTitle.AutoSize = True
            Me.lblOutstandingApTitle.Location = New System.Drawing.Point(10, 10)
            Me.lblOutstandingApTitle.Name = "lblOutstandingApTitle"
            Me.lblOutstandingApTitle.Size = New System.Drawing.Size(86, 15)
            Me.lblOutstandingApTitle.TabIndex = 0
            Me.lblOutstandingApTitle.Tag = "SectionHeader"
            Me.lblOutstandingApTitle.Text = "Outstanding AP"
            '
            ' lblOutstandingApValue
            '
            Me.lblOutstandingApValue.AutoSize = True
            Me.lblOutstandingApValue.Location = New System.Drawing.Point(10, 40)
            Me.lblOutstandingApValue.Name = "lblOutstandingApValue"
            Me.lblOutstandingApValue.Size = New System.Drawing.Size(42, 15)
            Me.lblOutstandingApValue.TabIndex = 1
            Me.lblOutstandingApValue.Tag = "PrimaryMetricValue"
            Me.lblOutstandingApValue.Text = "₱0.00"
            '
            ' pnlOverdueAp
            '
            Me.pnlOverdueAp.Controls.Add(Me.lblOverdueApTitle)
            Me.pnlOverdueAp.Controls.Add(Me.lblOverdueApValue)
            Me.pnlOverdueAp.Location = New System.Drawing.Point(270, 50)
            Me.pnlOverdueAp.Name = "pnlOverdueAp"
            Me.pnlOverdueAp.Size = New System.Drawing.Size(200, 100)
            Me.pnlOverdueAp.TabIndex = 2
            Me.pnlOverdueAp.Tag = "SecondaryMetricCard"
            '
            ' lblOverdueApTitle
            '
            Me.lblOverdueApTitle.AutoSize = True
            Me.lblOverdueApTitle.Location = New System.Drawing.Point(10, 10)
            Me.lblOverdueApTitle.Name = "lblOverdueApTitle"
            Me.lblOverdueApTitle.Size = New System.Drawing.Size(69, 15)
            Me.lblOverdueApTitle.TabIndex = 0
            Me.lblOverdueApTitle.Tag = "Subtitle"
            Me.lblOverdueApTitle.Text = "Overdue AP"
            '
            ' lblOverdueApValue
            '
            Me.lblOverdueApValue.AutoSize = True
            Me.lblOverdueApValue.Location = New System.Drawing.Point(10, 40)
            Me.lblOverdueApValue.Name = "lblOverdueApValue"
            Me.lblOverdueApValue.Size = New System.Drawing.Size(13, 15)
            Me.lblOverdueApValue.TabIndex = 1
            Me.lblOverdueApValue.Tag = "SecondaryMetricValue"
            Me.lblOverdueApValue.Text = "0"
            '
            ' pnlPendingDeliveries
            '
            Me.pnlPendingDeliveries.Controls.Add(Me.lblPendingDeliveriesTitle)
            Me.pnlPendingDeliveries.Controls.Add(Me.lblPendingDeliveriesValue)
            Me.pnlPendingDeliveries.Location = New System.Drawing.Point(480, 50)
            Me.pnlPendingDeliveries.Name = "pnlPendingDeliveries"
            Me.pnlPendingDeliveries.Size = New System.Drawing.Size(200, 100)
            Me.pnlPendingDeliveries.TabIndex = 3
            Me.pnlPendingDeliveries.Tag = "SecondaryMetricCard"
            '
            ' lblPendingDeliveriesTitle
            '
            Me.lblPendingDeliveriesTitle.AutoSize = True
            Me.lblPendingDeliveriesTitle.Location = New System.Drawing.Point(10, 10)
            Me.lblPendingDeliveriesTitle.Name = "lblPendingDeliveriesTitle"
            Me.lblPendingDeliveriesTitle.Size = New System.Drawing.Size(102, 15)
            Me.lblPendingDeliveriesTitle.TabIndex = 0
            Me.lblPendingDeliveriesTitle.Tag = "Subtitle"
            Me.lblPendingDeliveriesTitle.Text = "Pending Deliveries"
            '
            ' lblPendingDeliveriesValue
            '
            Me.lblPendingDeliveriesValue.AutoSize = True
            Me.lblPendingDeliveriesValue.Location = New System.Drawing.Point(10, 40)
            Me.lblPendingDeliveriesValue.Name = "lblPendingDeliveriesValue"
            Me.lblPendingDeliveriesValue.Size = New System.Drawing.Size(13, 15)
            Me.lblPendingDeliveriesValue.TabIndex = 1
            Me.lblPendingDeliveriesValue.Tag = "SecondaryMetricValue"
            Me.lblPendingDeliveriesValue.Text = "0"
            '
            ' pnlReorderSuggestions
            '
            Me.pnlReorderSuggestions.Controls.Add(Me.lblReorderSuggestionsTitle)
            Me.pnlReorderSuggestions.Controls.Add(Me.lblReorderSuggestionsValue)
            Me.pnlReorderSuggestions.Location = New System.Drawing.Point(690, 50)
            Me.pnlReorderSuggestions.Name = "pnlReorderSuggestions"
            Me.pnlReorderSuggestions.Size = New System.Drawing.Size(200, 100)
            Me.pnlReorderSuggestions.TabIndex = 4
            Me.pnlReorderSuggestions.Tag = "SecondaryMetricCard"
            '
            ' lblReorderSuggestionsTitle
            '
            Me.lblReorderSuggestionsTitle.AutoSize = True
            Me.lblReorderSuggestionsTitle.Location = New System.Drawing.Point(10, 10)
            Me.lblReorderSuggestionsTitle.Name = "lblReorderSuggestionsTitle"
            Me.lblReorderSuggestionsTitle.Size = New System.Drawing.Size(116, 15)
            Me.lblReorderSuggestionsTitle.TabIndex = 0
            Me.lblReorderSuggestionsTitle.Tag = "Subtitle"
            Me.lblReorderSuggestionsTitle.Text = "Reorder Suggestions"
            '
            ' lblReorderSuggestionsValue
            '
            Me.lblReorderSuggestionsValue.AutoSize = True
            Me.lblReorderSuggestionsValue.Location = New System.Drawing.Point(10, 40)
            Me.lblReorderSuggestionsValue.Name = "lblReorderSuggestionsValue"
            Me.lblReorderSuggestionsValue.Size = New System.Drawing.Size(13, 15)
            Me.lblReorderSuggestionsValue.TabIndex = 1
            Me.lblReorderSuggestionsValue.Tag = "SecondaryMetricValue"
            Me.lblReorderSuggestionsValue.Text = "0"
            '
            ' pnlActiveVendors
            '
            Me.pnlActiveVendors.Controls.Add(Me.lblActiveVendorsTitle)
            Me.pnlActiveVendors.Controls.Add(Me.lblActiveVendorsValue)
            Me.pnlActiveVendors.Location = New System.Drawing.Point(10, 160)
            Me.pnlActiveVendors.Name = "pnlActiveVendors"
            Me.pnlActiveVendors.Size = New System.Drawing.Size(200, 100)
            Me.pnlActiveVendors.TabIndex = 5
            Me.pnlActiveVendors.Tag = "SecondaryMetricCard"
            '
            ' lblActiveVendorsTitle
            '
            Me.lblActiveVendorsTitle.AutoSize = True
            Me.lblActiveVendorsTitle.Location = New System.Drawing.Point(10, 10)
            Me.lblActiveVendorsTitle.Name = "lblActiveVendorsTitle"
            Me.lblActiveVendorsTitle.Size = New System.Drawing.Size(84, 15)
            Me.lblActiveVendorsTitle.TabIndex = 0
            Me.lblActiveVendorsTitle.Tag = "Subtitle"
            Me.lblActiveVendorsTitle.Text = "Active Vendors"
            '
            ' lblActiveVendorsValue
            '
            Me.lblActiveVendorsValue.AutoSize = True
            Me.lblActiveVendorsValue.Location = New System.Drawing.Point(10, 40)
            Me.lblActiveVendorsValue.Name = "lblActiveVendorsValue"
            Me.lblActiveVendorsValue.Size = New System.Drawing.Size(13, 15)
            Me.lblActiveVendorsValue.TabIndex = 1
            Me.lblActiveVendorsValue.Tag = "SecondaryMetricValue"
            Me.lblActiveVendorsValue.Text = "0"
            '
            ' pnlAvgLeadTime
            '
            Me.pnlAvgLeadTime.Controls.Add(Me.lblAvgLeadTimeTitle)
            Me.pnlAvgLeadTime.Controls.Add(Me.lblAvgLeadTimeValue)
            Me.pnlAvgLeadTime.Location = New System.Drawing.Point(220, 160)
            Me.pnlAvgLeadTime.Name = "pnlAvgLeadTime"
            Me.pnlAvgLeadTime.Size = New System.Drawing.Size(200, 100)
            Me.pnlAvgLeadTime.TabIndex = 6
            Me.pnlAvgLeadTime.Tag = "SecondaryMetricCard"
            '
            ' lblAvgLeadTimeTitle
            '
            Me.lblAvgLeadTimeTitle.AutoSize = True
            Me.lblAvgLeadTimeTitle.Location = New System.Drawing.Point(10, 10)
            Me.lblAvgLeadTimeTitle.Name = "lblAvgLeadTimeTitle"
            Me.lblAvgLeadTimeTitle.Size = New System.Drawing.Size(89, 15)
            Me.lblAvgLeadTimeTitle.TabIndex = 0
            Me.lblAvgLeadTimeTitle.Tag = "Subtitle"
            Me.lblAvgLeadTimeTitle.Text = "Avg. Lead Time"
            '
            ' lblAvgLeadTimeValue
            '
            Me.lblAvgLeadTimeValue.AutoSize = True
            Me.lblAvgLeadTimeValue.Location = New System.Drawing.Point(10, 40)
            Me.lblAvgLeadTimeValue.Name = "lblAvgLeadTimeValue"
            Me.lblAvgLeadTimeValue.Size = New System.Drawing.Size(13, 15)
            Me.lblAvgLeadTimeValue.TabIndex = 1
            Me.lblAvgLeadTimeValue.Tag = "SecondaryMetricValue"
            Me.lblAvgLeadTimeValue.Text = "0"
            '
            ' pnlToolbar
            '
            Me.pnlToolbar.Controls.Add(Me.btnRefresh)
            Me.pnlToolbar.Dock = System.Windows.Forms.DockStyle.Top
            Me.pnlToolbar.Location = New System.Drawing.Point(0, 0)
            Me.pnlToolbar.Name = "pnlToolbar"
            Me.pnlToolbar.Size = New System.Drawing.Size(900, 40)
            Me.pnlToolbar.TabIndex = 0
            Me.pnlToolbar.Tag = "FilterBar"
            '
            ' btnRefresh
            '
            Me.btnRefresh.Location = New System.Drawing.Point(10, 8)
            Me.btnRefresh.Name = "btnRefresh"
            Me.btnRefresh.Size = New System.Drawing.Size(75, 23)
            Me.btnRefresh.TabIndex = 0
            Me.btnRefresh.Tag = "FilterBarItem"
            Me.btnRefresh.Text = "Refresh"
            Me.btnRefresh.UseVisualStyleBackColor = True
            '
            ' lblPendingDeliveriesGridTitle
            '
            Me.lblPendingDeliveriesGridTitle.AutoSize = True
            Me.lblPendingDeliveriesGridTitle.Location = New System.Drawing.Point(10, 275)
            Me.lblPendingDeliveriesGridTitle.Name = "lblPendingDeliveriesGridTitle"
            Me.lblPendingDeliveriesGridTitle.Size = New System.Drawing.Size(127, 15)
            Me.lblPendingDeliveriesGridTitle.TabIndex = 7
            Me.lblPendingDeliveriesGridTitle.Tag = "SectionHeader"
            Me.lblPendingDeliveriesGridTitle.Text = "Top Pending Deliveries"
            '
            ' dgvPendingDeliveries
            '
            Me.dgvPendingDeliveries.AllowUserToAddRows = False
            Me.dgvPendingDeliveries.AllowUserToDeleteRows = False
            Me.dgvPendingDeliveries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvPendingDeliveries.Location = New System.Drawing.Point(10, 300)
            Me.dgvPendingDeliveries.Name = "dgvPendingDeliveries"
            Me.dgvPendingDeliveries.ReadOnly = True
            Me.dgvPendingDeliveries.Size = New System.Drawing.Size(550, 200)
            Me.dgvPendingDeliveries.TabIndex = 8
            Me.dgvPendingDeliveries.Tag = "DataGrid"
            '
            ' lblTopVendorsGridTitle
            '
            Me.lblTopVendorsGridTitle.AutoSize = True
            Me.lblTopVendorsGridTitle.Location = New System.Drawing.Point(580, 275)
            Me.lblTopVendorsGridTitle.Name = "lblTopVendorsGridTitle"
            Me.lblTopVendorsGridTitle.Size = New System.Drawing.Size(130, 15)
            Me.lblTopVendorsGridTitle.TabIndex = 9
            Me.lblTopVendorsGridTitle.Tag = "SectionHeader"
            Me.lblTopVendorsGridTitle.Text = "Top Vendors (by spend)"
            '
            ' dgvTopVendors
            '
            Me.dgvTopVendors.AllowUserToAddRows = False
            Me.dgvTopVendors.AllowUserToDeleteRows = False
            Me.dgvTopVendors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvTopVendors.Location = New System.Drawing.Point(580, 300)
            Me.dgvTopVendors.Name = "dgvTopVendors"
            Me.dgvTopVendors.ReadOnly = True
            Me.dgvTopVendors.Size = New System.Drawing.Size(300, 200)
            Me.dgvTopVendors.TabIndex = 10
            Me.dgvTopVendors.Tag = "DataGrid"
            '
            ' lblErrorMessage
            '
            Me.lblErrorMessage.AutoSize = True
            Me.lblErrorMessage.ForeColor = System.Drawing.Color.Red
            Me.lblErrorMessage.Location = New System.Drawing.Point(100, 15)
            Me.lblErrorMessage.Name = "lblErrorMessage"
            Me.lblErrorMessage.Size = New System.Drawing.Size(0, 15)
            Me.lblErrorMessage.TabIndex = 11
            Me.lblErrorMessage.Tag = "ErrorMessage"
            Me.lblErrorMessage.Visible = False
            '
            ' PurchasingDashboardView
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.AutoScroll = True
            Me.Controls.Add(Me.lblErrorMessage)
            Me.Controls.Add(Me.dgvTopVendors)
            Me.Controls.Add(Me.lblTopVendorsGridTitle)
            Me.Controls.Add(Me.dgvPendingDeliveries)
            Me.Controls.Add(Me.lblPendingDeliveriesGridTitle)
            Me.Controls.Add(Me.pnlAvgLeadTime)
            Me.Controls.Add(Me.pnlActiveVendors)
            Me.Controls.Add(Me.pnlReorderSuggestions)
            Me.Controls.Add(Me.pnlPendingDeliveries)
            Me.Controls.Add(Me.pnlOverdueAp)
            Me.Controls.Add(Me.pnlOutstandingAp)
            Me.Controls.Add(Me.pnlToolbar)
            Me.Name = "PurchasingDashboardView"
            Me.Size = New System.Drawing.Size(900, 520)
            Me.pnlOutstandingAp.ResumeLayout(False)
            Me.pnlOutstandingAp.PerformLayout()
            Me.pnlOverdueAp.ResumeLayout(False)
            Me.pnlOverdueAp.PerformLayout()
            Me.pnlPendingDeliveries.ResumeLayout(False)
            Me.pnlPendingDeliveries.PerformLayout()
            Me.pnlReorderSuggestions.ResumeLayout(False)
            Me.pnlReorderSuggestions.PerformLayout()
            Me.pnlActiveVendors.ResumeLayout(False)
            Me.pnlActiveVendors.PerformLayout()
            Me.pnlAvgLeadTime.ResumeLayout(False)
            Me.pnlAvgLeadTime.PerformLayout()
            Me.pnlToolbar.ResumeLayout(False)
            CType(Me.dgvPendingDeliveries, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.dgvTopVendors, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents pnlOutstandingAp As System.Windows.Forms.Panel
        Friend WithEvents lblOutstandingApTitle As System.Windows.Forms.Label
        Friend WithEvents lblOutstandingApValue As System.Windows.Forms.Label
        Friend WithEvents pnlOverdueAp As System.Windows.Forms.Panel
        Friend WithEvents lblOverdueApTitle As System.Windows.Forms.Label
        Friend WithEvents lblOverdueApValue As System.Windows.Forms.Label
        Friend WithEvents pnlPendingDeliveries As System.Windows.Forms.Panel
        Friend WithEvents lblPendingDeliveriesTitle As System.Windows.Forms.Label
        Friend WithEvents lblPendingDeliveriesValue As System.Windows.Forms.Label
        Friend WithEvents pnlReorderSuggestions As System.Windows.Forms.Panel
        Friend WithEvents lblReorderSuggestionsTitle As System.Windows.Forms.Label
        Friend WithEvents lblReorderSuggestionsValue As System.Windows.Forms.Label
        Friend WithEvents pnlActiveVendors As System.Windows.Forms.Panel
        Friend WithEvents lblActiveVendorsTitle As System.Windows.Forms.Label
        Friend WithEvents lblActiveVendorsValue As System.Windows.Forms.Label
        Friend WithEvents pnlAvgLeadTime As System.Windows.Forms.Panel
        Friend WithEvents lblAvgLeadTimeTitle As System.Windows.Forms.Label
        Friend WithEvents lblAvgLeadTimeValue As System.Windows.Forms.Label
        Friend WithEvents pnlToolbar As System.Windows.Forms.Panel
        Friend WithEvents btnRefresh As System.Windows.Forms.Button
        Friend WithEvents dgvPendingDeliveries As System.Windows.Forms.DataGridView
        Friend WithEvents dgvTopVendors As System.Windows.Forms.DataGridView
        Friend WithEvents lblPendingDeliveriesGridTitle As System.Windows.Forms.Label
        Friend WithEvents lblTopVendorsGridTitle As System.Windows.Forms.Label
        Friend WithEvents lblErrorMessage As System.Windows.Forms.Label

    End Class
End Namespace