Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Namespace Views.Accounting

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Public Class FinancialOverviewView
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
            Me.pnlVatTile = New System.Windows.Forms.Panel()
            Me.lblVatPayable = New System.Windows.Forms.Label()
            Me.lblVatDueDate = New System.Windows.Forms.Label()
            Me.lblTitle = New System.Windows.Forms.Label()
            Me.btnRefresh = New System.Windows.Forms.Button()
            Me.lblLastRefreshed = New System.Windows.Forms.Label()
            Me.lblLoading = New System.Windows.Forms.Label()
            Me.pnlWhatThisMeans = New System.Windows.Forms.Panel()
            Me.lblWhatThisMeansIcon = New System.Windows.Forms.Label()
            Me.lblWhatThisMeans = New System.Windows.Forms.Label()
            Me.dgvTopProducts = New System.Windows.Forms.DataGridView()
            Me.dgvMonthlyTrends = New System.Windows.Forms.DataGridView()
            Me.lblTodayRevenue = New System.Windows.Forms.Label()
            Me.lblMtdRevenue = New System.Windows.Forms.Label()
            Me.lblYtdRevenue = New System.Windows.Forms.Label()
            Me.lblTotalAR = New System.Windows.Forms.Label()
            Me.lblTotalAP = New System.Windows.Forms.Label()
            Me.lblInventoryValue = New System.Windows.Forms.Label()
            Me.lblGrossMarginToday = New System.Windows.Forms.Label()
            Me.lblOverdueARCount = New System.Windows.Forms.Label()
            Me.lblOverdueAPCount = New System.Windows.Forms.Label()
            Me.lblLowStockAlertCount = New System.Windows.Forms.Label()
            Me.pnlWhatThisMeans.SuspendLayout()
            CType(Me.dgvTopProducts, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.dgvMonthlyTrends, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'lblTitle
            '
            Me.lblTitle.AutoSize = True
            Me.lblTitle.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Bold)
            Me.lblTitle.Location = New System.Drawing.Point(12, 12)
            Me.lblTitle.Name = "lblTitle"
            Me.lblTitle.Size = New System.Drawing.Size(200, 30)
            Me.lblTitle.TabIndex = 0
            Me.lblTitle.Text = "Financial Overview"
            '
            'btnRefresh
            '
            Me.btnRefresh.Location = New System.Drawing.Point(850, 15)
            Me.btnRefresh.Tag = "Accent"
            Me.btnRefresh.Name = "btnRefresh"
            Me.btnRefresh.Size = New System.Drawing.Size(75, 23)
            Me.btnRefresh.TabIndex = 1
            Me.btnRefresh.Text = "Refresh"
            Me.btnRefresh.UseVisualStyleBackColor = True
            '
            'lblLastRefreshed
            '
            Me.lblLastRefreshed.AutoSize = True
            Me.lblLastRefreshed.Location = New System.Drawing.Point(850, 45)
            Me.lblLastRefreshed.Name = "lblLastRefreshed"
            Me.lblLastRefreshed.Size = New System.Drawing.Size(0, 15)
            Me.lblLastRefreshed.TabIndex = 2
            '
            'lblLoading
            '
            Me.lblLoading.AutoSize = True
            Me.lblLoading.ForeColor = System.Drawing.Color.Blue
            Me.lblLoading.Location = New System.Drawing.Point(220, 24)
            Me.lblLoading.Name = "lblLoading"
            Me.lblLoading.Size = New System.Drawing.Size(59, 15)
            Me.lblLoading.TabIndex = 3
            Me.lblLoading.Text = "Loading..."
            Me.lblLoading.Visible = False
            '
            'pnlWhatThisMeans
            '
            Me.pnlWhatThisMeans.BackColor = System.Drawing.Color.FromArgb(CType(CType(235, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(251, Byte), Integer))
            Me.pnlWhatThisMeans.Controls.Add(Me.lblWhatThisMeansIcon)
            Me.pnlWhatThisMeans.Controls.Add(Me.lblWhatThisMeans)
            Me.pnlWhatThisMeans.Location = New System.Drawing.Point(17, 60)
            Me.pnlWhatThisMeans.Name = "pnlWhatThisMeans"
            Me.pnlWhatThisMeans.Size = New System.Drawing.Size(908, 60)
            Me.pnlWhatThisMeans.TabIndex = 4
            '
            'lblWhatThisMeansIcon
            '
            Me.lblWhatThisMeansIcon.AutoSize = True
            Me.lblWhatThisMeansIcon.Font = New System.Drawing.Font("Segoe UI", 16.0!)
            Me.lblWhatThisMeansIcon.Location = New System.Drawing.Point(10, 15)
            Me.lblWhatThisMeansIcon.Name = "lblWhatThisMeansIcon"
            Me.lblWhatThisMeansIcon.Size = New System.Drawing.Size(35, 30)
            Me.lblWhatThisMeansIcon.TabIndex = 0
            Me.lblWhatThisMeansIcon.Text = ""
            '
            'lblWhatThisMeans
            '
            Me.lblWhatThisMeans.Location = New System.Drawing.Point(50, 10)
            Me.lblWhatThisMeans.Name = "lblWhatThisMeans"
            Me.lblWhatThisMeans.Size = New System.Drawing.Size(840, 40)
            Me.lblWhatThisMeans.TabIndex = 1
            Me.lblWhatThisMeans.Text = ""
            '
            'dgvTopProducts
            '
            Me.dgvTopProducts.AllowUserToAddRows = False
            Me.dgvTopProducts.AllowUserToDeleteRows = False
            Me.dgvTopProducts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvTopProducts.Location = New System.Drawing.Point(17, 240)
            Me.dgvTopProducts.Name = "dgvTopProducts"
            Me.dgvTopProducts.EnableHeadersVisualStyles = False
            Me.dgvTopProducts.ReadOnly = True
            Me.dgvTopProducts.Size = New System.Drawing.Size(440, 200)
            Me.dgvTopProducts.TabIndex = 5
            '
            'dgvMonthlyTrends
            '
            Me.dgvMonthlyTrends.AllowUserToAddRows = False
            Me.dgvMonthlyTrends.AllowUserToDeleteRows = False
            Me.dgvMonthlyTrends.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvMonthlyTrends.Location = New System.Drawing.Point(485, 240)
            Me.dgvMonthlyTrends.Name = "dgvMonthlyTrends"
            Me.dgvMonthlyTrends.EnableHeadersVisualStyles = False
            Me.dgvMonthlyTrends.ReadOnly = True
            Me.dgvMonthlyTrends.Size = New System.Drawing.Size(440, 200)
            Me.dgvMonthlyTrends.TabIndex = 6
            '
            'lblTodayRevenue
            '
            Me.lblTodayRevenue.AutoSize = True
            Me.lblTodayRevenue.Location = New System.Drawing.Point(17, 140)
            Me.lblTodayRevenue.Name = "lblTodayRevenue"
            Me.lblTodayRevenue.Size = New System.Drawing.Size(0, 15)
            Me.lblTodayRevenue.TabIndex = 7
            Me.lblTodayRevenue.Tag = "SecondaryMetricValue"
            '
            'lblMtdRevenue
            '
            Me.lblMtdRevenue.AutoSize = True
            Me.lblMtdRevenue.Location = New System.Drawing.Point(130, 140)
            Me.lblMtdRevenue.Name = "lblMtdRevenue"
            Me.lblMtdRevenue.Size = New System.Drawing.Size(0, 15)
            Me.lblMtdRevenue.TabIndex = 8
            Me.lblMtdRevenue.Tag = "PrimaryMetricValue"
            '
            'lblYtdRevenue
            '
            Me.lblYtdRevenue.AutoSize = True
            Me.lblYtdRevenue.Location = New System.Drawing.Point(250, 140)
            Me.lblYtdRevenue.Name = "lblYtdRevenue"
            Me.lblYtdRevenue.Size = New System.Drawing.Size(0, 15)
            Me.lblYtdRevenue.TabIndex = 9
            Me.lblYtdRevenue.Tag = "MetricCaption"
            '
            'lblGrossMarginToday
            '
            Me.lblGrossMarginToday.AutoSize = True
            Me.lblGrossMarginToday.Location = New System.Drawing.Point(370, 140)
            Me.lblGrossMarginToday.Name = "lblGrossMarginToday"
            Me.lblGrossMarginToday.Size = New System.Drawing.Size(0, 15)
            Me.lblGrossMarginToday.TabIndex = 10
            Me.lblGrossMarginToday.Tag = "SecondaryMetricValue"
            '
            'lblTotalAR
            '
            Me.lblTotalAR.AutoSize = True
            Me.lblTotalAR.Location = New System.Drawing.Point(490, 140)
            Me.lblTotalAR.Name = "lblTotalAR"
            Me.lblTotalAR.Size = New System.Drawing.Size(0, 15)
            Me.lblTotalAR.TabIndex = 11
            Me.lblTotalAR.Tag = "SecondaryMetricValue"
            '
            'lblTotalAP
            '
            Me.lblTotalAP.AutoSize = True
            Me.lblTotalAP.Location = New System.Drawing.Point(610, 140)
            Me.lblTotalAP.Name = "lblTotalAP"
            Me.lblTotalAP.Size = New System.Drawing.Size(0, 15)
            Me.lblTotalAP.TabIndex = 12
            Me.lblTotalAP.Tag = "SecondaryMetricValue"
            '
            'lblInventoryValue
            '
            Me.lblInventoryValue.AutoSize = True
            Me.lblInventoryValue.Location = New System.Drawing.Point(730, 140)
            Me.lblInventoryValue.Name = "lblInventoryValue"
            Me.lblInventoryValue.Size = New System.Drawing.Size(0, 15)
            Me.lblInventoryValue.TabIndex = 13
            Me.lblInventoryValue.Tag = "MetricCaption"
            '
            'lblOverdueARCount
            '
            Me.lblOverdueARCount.AutoSize = True
            Me.lblOverdueARCount.ForeColor = System.Drawing.Color.Red
            Me.lblOverdueARCount.Location = New System.Drawing.Point(17, 190)
            Me.lblOverdueARCount.Name = "lblOverdueARCount"
            Me.lblOverdueARCount.Size = New System.Drawing.Size(0, 15)
            Me.lblOverdueARCount.TabIndex = 14
            '
            'lblOverdueAPCount
            '
            Me.lblOverdueAPCount.AutoSize = True
            Me.lblOverdueAPCount.ForeColor = System.Drawing.Color.Red
            Me.lblOverdueAPCount.Location = New System.Drawing.Point(200, 190)
            Me.lblOverdueAPCount.Name = "lblOverdueAPCount"
            Me.lblOverdueAPCount.Size = New System.Drawing.Size(0, 15)
            Me.lblOverdueAPCount.TabIndex = 15
            '
            'lblLowStockAlertCount
            '
            Me.lblLowStockAlertCount.AutoSize = True
            Me.lblLowStockAlertCount.ForeColor = System.Drawing.Color.Orange
            Me.lblLowStockAlertCount.Location = New System.Drawing.Point(400, 190)
            Me.lblLowStockAlertCount.Name = "lblLowStockAlertCount"
            Me.lblLowStockAlertCount.Size = New System.Drawing.Size(0, 15)
            Me.lblLowStockAlertCount.TabIndex = 16
            '
            'pnlVatTile
            '
            ' ARCHITECTURAL INTEGRATION & DATAFLOW:
            ' Data Source: Populated via FinancialOverviewPresenter from the VatEnrichedFinancialOverviewService
            '              decorator which wraps FinancialOverviewService with IKpiProvider (VatPayableKpiProvider).
            ' Role Guarding: Restricted navigation. Only UserRole.Manager and UserRole.Developer are permitted to navigate.
            '                UserRole.Owner displays the tile and severity, but clicking suppresses navigation.
            ' Navigation Chain: pnlVatTile.Click / label clicks -> RaiseEvent VatTileClicked
            '                   -> FinancialOverviewPresenter.OnVatTileClicked -> RaiseEvent NavigateToVatReturnRequested
            '                   -> FinancialOverviewView.OnNavigateToVatReturnRequested -> shell.Presenter.NavigateTo("VAT Returns").
            Me.pnlVatTile.BackColor = System.Drawing.Color.LightGray
            Me.pnlVatTile.Controls.Add(Me.lblVatPayable)
            Me.pnlVatTile.Controls.Add(Me.lblVatDueDate)
            Me.pnlVatTile.Location = New System.Drawing.Point(600, 180)
            Me.pnlVatTile.Name = "pnlVatTile"
            Me.pnlVatTile.Size = New System.Drawing.Size(200, 50)
            Me.pnlVatTile.TabIndex = 17
            Me.pnlVatTile.Cursor = System.Windows.Forms.Cursors.Hand
            Me.pnlVatTile.Tag = "TertiaryMetricCard"
            '
            'lblVatPayable
            '
            Me.lblVatPayable.AutoSize = True
            Me.lblVatPayable.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
            Me.lblVatPayable.Location = New System.Drawing.Point(5, 5)
            Me.lblVatPayable.Name = "lblVatPayable"
            Me.lblVatPayable.Size = New System.Drawing.Size(0, 19)
            Me.lblVatPayable.TabIndex = 0
            Me.lblVatPayable.Cursor = System.Windows.Forms.Cursors.Hand
            '
            'lblVatDueDate
            '
            Me.lblVatDueDate.AutoSize = True
            Me.lblVatDueDate.Location = New System.Drawing.Point(5, 25)
            Me.lblVatDueDate.Name = "lblVatDueDate"
            Me.lblVatDueDate.Size = New System.Drawing.Size(0, 15)
            Me.lblVatDueDate.TabIndex = 1
            Me.lblVatDueDate.Cursor = System.Windows.Forms.Cursors.Hand
            '
            'FinancialOverviewView
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.AutoScroll = True
            Me.Controls.Add(Me.pnlVatTile)
            Me.Controls.Add(Me.lblLowStockAlertCount)
            Me.Controls.Add(Me.lblOverdueAPCount)
            Me.Controls.Add(Me.lblOverdueARCount)
            Me.Controls.Add(Me.lblInventoryValue)
            Me.Controls.Add(Me.lblTotalAP)
            Me.Controls.Add(Me.lblTotalAR)
            Me.Controls.Add(Me.lblGrossMarginToday)
            Me.Controls.Add(Me.lblYtdRevenue)
            Me.Controls.Add(Me.lblMtdRevenue)
            Me.Controls.Add(Me.lblTodayRevenue)
            Me.Controls.Add(Me.dgvMonthlyTrends)
            Me.Controls.Add(Me.dgvTopProducts)
            Me.Controls.Add(Me.pnlWhatThisMeans)
            Me.Controls.Add(Me.lblLoading)
            Me.Controls.Add(Me.lblLastRefreshed)
            Me.Controls.Add(Me.btnRefresh)
            Me.Controls.Add(Me.lblTitle)
            Me.Name = "FinancialOverviewView"
            Me.Size = New System.Drawing.Size(950, 480)
            Me.pnlWhatThisMeans.ResumeLayout(False)
            Me.pnlWhatThisMeans.PerformLayout()
            CType(Me.dgvTopProducts, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.dgvMonthlyTrends, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents lblTitle As System.Windows.Forms.Label
        Friend WithEvents btnRefresh As System.Windows.Forms.Button
        Friend WithEvents lblLastRefreshed As System.Windows.Forms.Label
        Friend WithEvents lblLoading As System.Windows.Forms.Label
        Friend WithEvents pnlWhatThisMeans As System.Windows.Forms.Panel
        Friend WithEvents lblWhatThisMeansIcon As System.Windows.Forms.Label
        Friend WithEvents lblWhatThisMeans As System.Windows.Forms.Label
        Friend WithEvents dgvTopProducts As System.Windows.Forms.DataGridView
        Friend WithEvents dgvMonthlyTrends As System.Windows.Forms.DataGridView
        Friend WithEvents lblTodayRevenue As System.Windows.Forms.Label
        Friend WithEvents lblMtdRevenue As System.Windows.Forms.Label
        Friend WithEvents lblYtdRevenue As System.Windows.Forms.Label
        Friend WithEvents lblTotalAR As System.Windows.Forms.Label
        Friend WithEvents lblTotalAP As System.Windows.Forms.Label
        Friend WithEvents lblInventoryValue As System.Windows.Forms.Label
        Friend WithEvents lblGrossMarginToday As System.Windows.Forms.Label
        Friend WithEvents lblOverdueARCount As System.Windows.Forms.Label
        Friend WithEvents lblOverdueAPCount As System.Windows.Forms.Label
        Friend WithEvents lblLowStockAlertCount As System.Windows.Forms.Label
        Friend WithEvents pnlVatTile As System.Windows.Forms.Panel
        Friend WithEvents lblVatPayable As System.Windows.Forms.Label
        Friend WithEvents lblVatDueDate As System.Windows.Forms.Label

    End Class

End Namespace
