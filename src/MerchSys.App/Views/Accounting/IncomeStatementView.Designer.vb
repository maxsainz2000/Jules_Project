Namespace Views.Accounting

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IncomeStatementView
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
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.pnlHeader = New System.Windows.Forms.Panel()
        Me.rbAnnual = New System.Windows.Forms.RadioButton()
        Me.rbQuarterly = New System.Windows.Forms.RadioButton()
        Me.rbMonthly = New System.Windows.Forms.RadioButton()
        Me.cboYear = New System.Windows.Forms.ComboBox()
        Me.cboMonth = New System.Windows.Forms.ComboBox()
        Me.cboQuarter = New System.Windows.Forms.ComboBox()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.btnExportPdf = New System.Windows.Forms.Button()
        Me.lblLoading = New System.Windows.Forms.Label()
        Me.pnlWhatThisMeans = New System.Windows.Forms.Panel()
        Me.lblWhatThisMeansIcon = New System.Windows.Forms.Label()
        Me.lblWhatThisMeans = New System.Windows.Forms.Label()
        Me.tabControl1 = New System.Windows.Forms.TabControl()
        Me.tabPageSummary = New System.Windows.Forms.TabPage()
        Me.lblNetSales = New System.Windows.Forms.Label()
        Me.lblCostOfGoodsSold = New System.Windows.Forms.Label()
        Me.lblGrossProfit = New System.Windows.Forms.Label()
        Me.lblGrossMarginPercent = New System.Windows.Forms.Label()
        Me.lblOperatingExpenses = New System.Windows.Forms.Label()
        Me.lblNetIncome = New System.Windows.Forms.Label()
        Me.lblNetMarginPercent = New System.Windows.Forms.Label()
        Me.dgvOperatingExpenses = New System.Windows.Forms.DataGridView()
        Me.tabPageProductMargins = New System.Windows.Forms.TabPage()
        Me.dgvProductMargins = New System.Windows.Forms.DataGridView()
        Me.colCategory = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colAmount = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colProduct = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colUnitsSold = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colRevenue = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colCogs = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colGrossProfit = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colMarginPercent = New System.Windows.Forms.DataGridViewTextBoxColumn()

        Me.pnlHeader.SuspendLayout()
        Me.pnlWhatThisMeans.SuspendLayout()
        Me.tabControl1.SuspendLayout()
        Me.tabPageSummary.SuspendLayout()
        CType(Me.dgvOperatingExpenses, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabPageProductMargins.SuspendLayout()
        CType(Me.dgvProductMargins, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.Location = New System.Drawing.Point(12, 9)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(262, 30)
        Me.lblTitle.TabIndex = 0
        Me.lblTitle.Text = "Income Statement (P&&L)"
        '
        'pnlHeader
        '
        Me.pnlHeader.Controls.Add(Me.rbAnnual)
        Me.pnlHeader.Controls.Add(Me.rbQuarterly)
        Me.pnlHeader.Controls.Add(Me.rbMonthly)
        Me.pnlHeader.Controls.Add(Me.cboYear)
        Me.pnlHeader.Controls.Add(Me.cboMonth)
        Me.pnlHeader.Controls.Add(Me.cboQuarter)
        Me.pnlHeader.Controls.Add(Me.btnRefresh)
        Me.pnlHeader.Controls.Add(Me.btnExportPdf)
        Me.pnlHeader.Controls.Add(Me.lblLoading)
        Me.pnlHeader.Location = New System.Drawing.Point(17, 50)
        Me.pnlHeader.Name = "pnlHeader"
        Me.pnlHeader.Size = New System.Drawing.Size(950, 40)
        Me.pnlHeader.TabIndex = 1
        '
        'rbMonthly
        '
        Me.rbMonthly.AutoSize = True
        Me.rbMonthly.Location = New System.Drawing.Point(10, 10)
        Me.rbMonthly.Name = "rbMonthly"
        Me.rbMonthly.Size = New System.Drawing.Size(66, 19)
        Me.rbMonthly.TabIndex = 0
        Me.rbMonthly.Text = "Monthly"
        Me.rbMonthly.UseVisualStyleBackColor = True
        '
        'rbQuarterly
        '
        Me.rbQuarterly.AutoSize = True
        Me.rbQuarterly.Location = New System.Drawing.Point(85, 10)
        Me.rbQuarterly.Name = "rbQuarterly"
        Me.rbQuarterly.Size = New System.Drawing.Size(73, 19)
        Me.rbQuarterly.TabIndex = 1
        Me.rbQuarterly.Text = "Quarterly"
        Me.rbQuarterly.UseVisualStyleBackColor = True
        '
        'rbAnnual
        '
        Me.rbAnnual.AutoSize = True
        Me.rbAnnual.Location = New System.Drawing.Point(165, 10)
        Me.rbAnnual.Name = "rbAnnual"
        Me.rbAnnual.Size = New System.Drawing.Size(61, 19)
        Me.rbAnnual.TabIndex = 2
        Me.rbAnnual.Text = "Annual"
        Me.rbAnnual.UseVisualStyleBackColor = True
        '
        'cboYear
        '
        Me.cboYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboYear.FormattingEnabled = True
        Me.cboYear.Location = New System.Drawing.Point(240, 8)
        Me.cboYear.Name = "cboYear"
        Me.cboYear.Size = New System.Drawing.Size(80, 23)
        Me.cboYear.TabIndex = 3
        '
        'cboMonth
        '
        Me.cboMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboMonth.FormattingEnabled = True
        Me.cboMonth.Location = New System.Drawing.Point(330, 8)
        Me.cboMonth.Name = "cboMonth"
        Me.cboMonth.Size = New System.Drawing.Size(100, 23)
        Me.cboMonth.TabIndex = 4
        '
        'cboQuarter
        '
        Me.cboQuarter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboQuarter.FormattingEnabled = True
        Me.cboQuarter.Location = New System.Drawing.Point(440, 8)
        Me.cboQuarter.Name = "cboQuarter"
        Me.cboQuarter.Size = New System.Drawing.Size(100, 23)
        Me.cboQuarter.TabIndex = 5
        '
        'btnRefresh
        '
        Me.btnRefresh.Location = New System.Drawing.Point(550, 7)
        Me.btnRefresh.Tag = "Accent"
            Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(75, 25)
        Me.btnRefresh.TabIndex = 6
        Me.btnRefresh.Text = "Refresh"
        Me.btnRefresh.UseVisualStyleBackColor = True
        '
        'btnExportPdf
        '
        Me.btnExportPdf.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnExportPdf.Location = New System.Drawing.Point(630, 7)
        Me.btnExportPdf.Tag = "Accent"
            Me.btnExportPdf.Name = "btnExportPdf"
        Me.btnExportPdf.Size = New System.Drawing.Size(85, 25)
        Me.btnExportPdf.TabIndex = 7
        Me.btnExportPdf.Text = "Export PDF"
        Me.btnExportPdf.UseVisualStyleBackColor = True
        '
        'lblLoading
        '
        Me.lblLoading.AutoSize = True
        Me.lblLoading.ForeColor = System.Drawing.Color.DimGray
        Me.lblLoading.Location = New System.Drawing.Point(635, 12)
        Me.lblLoading.Name = "lblLoading"
        Me.lblLoading.Size = New System.Drawing.Size(59, 15)
        Me.lblLoading.TabIndex = 7
        Me.lblLoading.Text = "Loading..."
        Me.lblLoading.Visible = False
        '
        'pnlWhatThisMeans
        '
        Me.pnlWhatThisMeans.BackColor = System.Drawing.Color.FromArgb(CType(CType(235, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(251, Byte), Integer))
        Me.pnlWhatThisMeans.Controls.Add(Me.lblWhatThisMeansIcon)
        Me.pnlWhatThisMeans.Controls.Add(Me.lblWhatThisMeans)
        Me.pnlWhatThisMeans.Location = New System.Drawing.Point(17, 100)
        Me.pnlWhatThisMeans.Name = "pnlWhatThisMeans"
        Me.pnlWhatThisMeans.Size = New System.Drawing.Size(950, 70)
        Me.pnlWhatThisMeans.TabIndex = 2
        '
        'lblWhatThisMeansIcon
        '
        Me.lblWhatThisMeansIcon.AutoSize = True
        Me.lblWhatThisMeansIcon.Font = New System.Drawing.Font("Segoe UI", 20.0!)
        Me.lblWhatThisMeansIcon.Location = New System.Drawing.Point(10, 15)
        Me.lblWhatThisMeansIcon.Name = "lblWhatThisMeansIcon"
        Me.lblWhatThisMeansIcon.Size = New System.Drawing.Size(43, 37)
        Me.lblWhatThisMeansIcon.TabIndex = 0
        Me.lblWhatThisMeansIcon.Text = ""
        '
        'lblWhatThisMeans
        '
        Me.lblWhatThisMeans.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.lblWhatThisMeans.Location = New System.Drawing.Point(60, 10)
        Me.lblWhatThisMeans.Name = "lblWhatThisMeans"
        Me.lblWhatThisMeans.Size = New System.Drawing.Size(870, 50)
        Me.lblWhatThisMeans.TabIndex = 1
        Me.lblWhatThisMeans.Text = "Interpretation will appear here."
        '
        'tabControl1
        '
        Me.tabControl1.Controls.Add(Me.tabPageSummary)
        Me.tabControl1.Controls.Add(Me.tabPageProductMargins)
        Me.tabControl1.Location = New System.Drawing.Point(17, 180)
        Me.tabControl1.Name = "tabControl1"
        Me.tabControl1.SelectedIndex = 0
        Me.tabControl1.Size = New System.Drawing.Size(950, 400)
        Me.tabControl1.TabIndex = 3
        '
        'tabPageSummary
        '
        Me.tabPageSummary.Controls.Add(Me.lblNetSales)
        Me.tabPageSummary.Controls.Add(Me.lblCostOfGoodsSold)
        Me.tabPageSummary.Controls.Add(Me.lblGrossProfit)
        Me.tabPageSummary.Controls.Add(Me.lblGrossMarginPercent)
        Me.tabPageSummary.Controls.Add(Me.lblOperatingExpenses)
        Me.tabPageSummary.Controls.Add(Me.lblNetIncome)
        Me.tabPageSummary.Controls.Add(Me.lblNetMarginPercent)
        Me.tabPageSummary.Controls.Add(Me.dgvOperatingExpenses)
        Me.tabPageSummary.Location = New System.Drawing.Point(4, 24)
        Me.tabPageSummary.Name = "tabPageSummary"
        Me.tabPageSummary.Padding = New System.Windows.Forms.Padding(3)
        Me.tabPageSummary.Size = New System.Drawing.Size(942, 372)
        Me.tabPageSummary.TabIndex = 0
        Me.tabPageSummary.Text = "Income Statement"
        Me.tabPageSummary.UseVisualStyleBackColor = True
        '
        'lblNetSales
        '
        Me.lblNetSales.AutoSize = True
        Me.lblNetSales.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.lblNetSales.Location = New System.Drawing.Point(20, 20)
        Me.lblNetSales.Name = "lblNetSales"
        Me.lblNetSales.Size = New System.Drawing.Size(126, 21)
        Me.lblNetSales.TabIndex = 0
        Me.lblNetSales.Text = "Net Sales: ₱0.00"
        Me.lblNetSales.Tag = "SecondaryMetricValue"
        '
        'lblCostOfGoodsSold
        '
        Me.lblCostOfGoodsSold.AutoSize = True
        Me.lblCostOfGoodsSold.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.lblCostOfGoodsSold.Location = New System.Drawing.Point(20, 50)
        Me.lblCostOfGoodsSold.Name = "lblCostOfGoodsSold"
        Me.lblCostOfGoodsSold.Size = New System.Drawing.Size(111, 21)
        Me.lblCostOfGoodsSold.TabIndex = 1
        Me.lblCostOfGoodsSold.Text = "COGS: -₱0.00"
        Me.lblCostOfGoodsSold.Tag = "SecondaryMetricValue"
        '
        'lblGrossProfit
        '
        Me.lblGrossProfit.AutoSize = True
        Me.lblGrossProfit.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblGrossProfit.Location = New System.Drawing.Point(20, 80)
        Me.lblGrossProfit.Name = "lblGrossProfit"
        Me.lblGrossProfit.Size = New System.Drawing.Size(149, 21)
        Me.lblGrossProfit.TabIndex = 2
        Me.lblGrossProfit.Text = "Gross Profit: ₱0.00"
        Me.lblGrossProfit.Tag = "SecondaryMetricValue"
        '
        'lblGrossMarginPercent
        '
        Me.lblGrossMarginPercent.AutoSize = True
        Me.lblGrossMarginPercent.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblGrossMarginPercent.Location = New System.Drawing.Point(20, 110)
        Me.lblGrossMarginPercent.Name = "lblGrossMarginPercent"
        Me.lblGrossMarginPercent.Size = New System.Drawing.Size(124, 19)
        Me.lblGrossMarginPercent.TabIndex = 3
        Me.lblGrossMarginPercent.Text = "Gross Margin: 0.0%"
        Me.lblGrossMarginPercent.Tag = "SecondaryMetricValue"
        '
        'lblOperatingExpenses
        '
        Me.lblOperatingExpenses.AutoSize = True
        Me.lblOperatingExpenses.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.lblOperatingExpenses.Location = New System.Drawing.Point(20, 150)
        Me.lblOperatingExpenses.Name = "lblOperatingExpenses"
        Me.lblOperatingExpenses.Size = New System.Drawing.Size(207, 21)
        Me.lblOperatingExpenses.TabIndex = 4
        Me.lblOperatingExpenses.Text = "Operating Expenses: -₱0.00"
        Me.lblOperatingExpenses.Tag = "SecondaryMetricValue"
        '
        'lblNetIncome
        '
        Me.lblNetIncome.AutoSize = True
        Me.lblNetIncome.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
        Me.lblNetIncome.Location = New System.Drawing.Point(20, 190)
        Me.lblNetIncome.Name = "lblNetIncome"
        Me.lblNetIncome.Size = New System.Drawing.Size(170, 25)
        Me.lblNetIncome.TabIndex = 5
        Me.lblNetIncome.Text = "Net Income: ₱0.00"
        Me.lblNetIncome.Tag = "PrimaryMetricValue"
        '
        'lblNetMarginPercent
        '
        Me.lblNetMarginPercent.AutoSize = True
        Me.lblNetMarginPercent.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblNetMarginPercent.Location = New System.Drawing.Point(20, 220)
        Me.lblNetMarginPercent.Name = "lblNetMarginPercent"
        Me.lblNetMarginPercent.Size = New System.Drawing.Size(115, 19)
        Me.lblNetMarginPercent.TabIndex = 6
        Me.lblNetMarginPercent.Text = "Net Margin: 0.0%"
        Me.lblNetMarginPercent.Tag = "SecondaryMetricValue"
        '
        'dgvOperatingExpenses
        '
        Me.dgvOperatingExpenses.AllowUserToAddRows = False
        Me.dgvOperatingExpenses.AllowUserToDeleteRows = False
        Me.dgvOperatingExpenses.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvOperatingExpenses.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colCategory, Me.colAmount})
        Me.dgvOperatingExpenses.Location = New System.Drawing.Point(400, 20)
        Me.dgvOperatingExpenses.Name = "dgvOperatingExpenses"
            Me.dgvOperatingExpenses.EnableHeadersVisualStyles = False
        Me.dgvOperatingExpenses.ReadOnly = True
        Me.dgvOperatingExpenses.RowHeadersVisible = False
        Me.dgvOperatingExpenses.Size = New System.Drawing.Size(350, 320)
        Me.dgvOperatingExpenses.TabIndex = 7
        '
        'colCategory
        '
        Me.colCategory.HeaderText = "Expense Category"
        Me.colCategory.Name = "colCategory"
        Me.colCategory.ReadOnly = True
        Me.colCategory.Width = 200
        '
        'colAmount
        '
        Me.colAmount.HeaderText = "Amount"
        Me.colAmount.Name = "colAmount"
        Me.colAmount.ReadOnly = True
        Me.colAmount.Width = 120
        '
        'tabPageProductMargins
        '
        Me.tabPageProductMargins.Controls.Add(Me.dgvProductMargins)
        Me.tabPageProductMargins.Location = New System.Drawing.Point(4, 24)
        Me.tabPageProductMargins.Name = "tabPageProductMargins"
        Me.tabPageProductMargins.Padding = New System.Windows.Forms.Padding(3)
        Me.tabPageProductMargins.Size = New System.Drawing.Size(942, 372)
        Me.tabPageProductMargins.TabIndex = 1
        Me.tabPageProductMargins.Text = "Product Margins"
        Me.tabPageProductMargins.UseVisualStyleBackColor = True
        '
        'dgvProductMargins
        '
        Me.dgvProductMargins.AllowUserToAddRows = False
        Me.dgvProductMargins.AllowUserToDeleteRows = False
        Me.dgvProductMargins.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvProductMargins.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colProduct, Me.colUnitsSold, Me.colRevenue, Me.colCogs, Me.colGrossProfit, Me.colMarginPercent})
        Me.dgvProductMargins.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvProductMargins.Location = New System.Drawing.Point(3, 3)
        Me.dgvProductMargins.Name = "dgvProductMargins"
            Me.dgvProductMargins.EnableHeadersVisualStyles = False
        Me.dgvProductMargins.ReadOnly = True
        Me.dgvProductMargins.RowHeadersVisible = False
        Me.dgvProductMargins.Size = New System.Drawing.Size(936, 366)
        Me.dgvProductMargins.TabIndex = 0
        '
        'colProduct
        '
        Me.colProduct.HeaderText = "Product"
        Me.colProduct.Name = "colProduct"
        Me.colProduct.ReadOnly = True
        Me.colProduct.Width = 250
        '
        'colUnitsSold
        '
        Me.colUnitsSold.HeaderText = "Units Sold"
        Me.colUnitsSold.Name = "colUnitsSold"
        Me.colUnitsSold.ReadOnly = True
        Me.colUnitsSold.Width = 100
        '
        'colRevenue
        '
        Me.colRevenue.HeaderText = "Revenue"
        Me.colRevenue.Name = "colRevenue"
        Me.colRevenue.ReadOnly = True
        Me.colRevenue.Width = 120
        '
        'colCogs
        '
        Me.colCogs.HeaderText = "COGS"
        Me.colCogs.Name = "colCogs"
        Me.colCogs.ReadOnly = True
        Me.colCogs.Width = 120
        '
        'colGrossProfit
        '
        Me.colGrossProfit.HeaderText = "Gross Profit"
        Me.colGrossProfit.Name = "colGrossProfit"
        Me.colGrossProfit.ReadOnly = True
        Me.colGrossProfit.Width = 120
        '
        'colMarginPercent
        '
        Me.colMarginPercent.HeaderText = "Margin %"
        Me.colMarginPercent.Name = "colMarginPercent"
        Me.colMarginPercent.ReadOnly = True
        Me.colMarginPercent.Width = 100
        '
        'IncomeStatementView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        '
        'pnlErrorState
        '
        Me.pnlErrorState.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlErrorState.Location = New System.Drawing.Point(0, 0)
        Me.pnlErrorState.Name = "pnlErrorState"
        Me.pnlErrorState.TabIndex = 26
        Me.pnlErrorState.Visible = False

        Me.Controls.Add(Me.pnlErrorState)
        Me.Controls.Add(Me.tabControl1)
        Me.Controls.Add(Me.pnlWhatThisMeans)
        Me.Controls.Add(Me.pnlHeader)
        Me.Controls.Add(Me.lblTitle)
        Me.Name = "IncomeStatementView"
        Me.Size = New System.Drawing.Size(984, 600)
        Me.pnlHeader.ResumeLayout(False)
        Me.pnlHeader.PerformLayout()
        Me.pnlWhatThisMeans.ResumeLayout(False)
        Me.pnlWhatThisMeans.PerformLayout()
        Me.tabControl1.ResumeLayout(False)
        Me.tabPageSummary.ResumeLayout(False)
        Me.tabPageSummary.PerformLayout()
        CType(Me.dgvOperatingExpenses, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabPageProductMargins.ResumeLayout(False)
        CType(Me.dgvProductMargins, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents pnlHeader As System.Windows.Forms.Panel
    Friend WithEvents rbAnnual As System.Windows.Forms.RadioButton
    Friend WithEvents rbQuarterly As System.Windows.Forms.RadioButton
    Friend WithEvents rbMonthly As System.Windows.Forms.RadioButton
    Friend WithEvents cboYear As System.Windows.Forms.ComboBox
    Friend WithEvents cboMonth As System.Windows.Forms.ComboBox
    Friend WithEvents cboQuarter As System.Windows.Forms.ComboBox
    Friend WithEvents btnRefresh As System.Windows.Forms.Button
    Friend WithEvents btnExportPdf As System.Windows.Forms.Button
    Friend WithEvents lblLoading As System.Windows.Forms.Label
    Friend WithEvents pnlWhatThisMeans As System.Windows.Forms.Panel
    Friend WithEvents lblWhatThisMeansIcon As System.Windows.Forms.Label
    Friend WithEvents lblWhatThisMeans As System.Windows.Forms.Label
    Friend WithEvents tabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents tabPageSummary As System.Windows.Forms.TabPage
    Friend WithEvents tabPageProductMargins As System.Windows.Forms.TabPage
    Friend WithEvents lblNetSales As System.Windows.Forms.Label
    Friend WithEvents lblCostOfGoodsSold As System.Windows.Forms.Label
    Friend WithEvents lblGrossProfit As System.Windows.Forms.Label
    Friend WithEvents lblGrossMarginPercent As System.Windows.Forms.Label
    Friend WithEvents lblOperatingExpenses As System.Windows.Forms.Label
    Friend WithEvents lblNetIncome As System.Windows.Forms.Label
    Friend WithEvents lblNetMarginPercent As System.Windows.Forms.Label
    Friend WithEvents dgvOperatingExpenses As System.Windows.Forms.DataGridView
    Friend WithEvents dgvProductMargins As System.Windows.Forms.DataGridView
    Friend WithEvents colCategory As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colAmount As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colProduct As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colUnitsSold As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colRevenue As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colCogs As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colGrossProfit As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colMarginPercent As System.Windows.Forms.DataGridViewTextBoxColumn

    Friend WithEvents pnlErrorState As MerchSys.App.Controls.ErrorStatePanel
    End Class

End Namespace