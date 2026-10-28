Imports System.Windows.Forms
Imports System.Drawing

Namespace Views.Accounting

    Partial Class SalesSummaryView
        Inherits UserControl

        Private components As System.ComponentModel.IContainer

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso (components IsNot Nothing) Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
            Me.pnlErrorState = New MerchSys.App.Controls.ErrorStatePanel()
            Me.lblTitle = New System.Windows.Forms.Label()
            Me.rbDaily = New System.Windows.Forms.RadioButton()
            Me.rbWeekly = New System.Windows.Forms.RadioButton()
            Me.rbMonthly = New System.Windows.Forms.RadioButton()
            Me.dtpDate = New System.Windows.Forms.DateTimePicker()
            Me.cboYear = New System.Windows.Forms.ComboBox()
            Me.cboMonth = New System.Windows.Forms.ComboBox()
            Me.btnRefresh = New System.Windows.Forms.Button()
            Me.lblLoading = New System.Windows.Forms.Label()
            Me.pnlWhatThisMeans = New System.Windows.Forms.Panel()
            Me.lblWarning = New System.Windows.Forms.Label()
            Me.lblWhatThisMeans = New System.Windows.Forms.Label()
            Me.pnlKpiCards = New System.Windows.Forms.FlowLayoutPanel()
            Me.pnlNetSales = New System.Windows.Forms.Panel()
            Me.lblNetSalesTitle = New System.Windows.Forms.Label()
            Me.lblNetSalesValue = New System.Windows.Forms.Label()
            Me.pnlTransactions = New System.Windows.Forms.Panel()
            Me.lblTransactionsTitle = New System.Windows.Forms.Label()
            Me.lblTransactionsValue = New System.Windows.Forms.Label()
            Me.pnlAvgTransaction = New System.Windows.Forms.Panel()
            Me.lblAvgTransactionTitle = New System.Windows.Forms.Label()
            Me.lblAvgTransactionValue = New System.Windows.Forms.Label()
            Me.pnlReturns = New System.Windows.Forms.Panel()
            Me.lblReturnsTitle = New System.Windows.Forms.Label()
            Me.lblReturnsValue = New System.Windows.Forms.Label()
            Me.tabControl1 = New System.Windows.Forms.TabControl()
            Me.tabPage1 = New System.Windows.Forms.TabPage()
            Me.dgvPaymentBreakdowns = New System.Windows.Forms.DataGridView()
            Me.lblTotalGrossSales = New System.Windows.Forms.Label()
            Me.lblTotalTransactions = New System.Windows.Forms.Label()
            Me.tabPage2 = New System.Windows.Forms.TabPage()
            Me.dgvDailyBreakdown = New System.Windows.Forms.DataGridView()
            Me.tabPage3 = New System.Windows.Forms.TabPage()
            Me.dgvPaymentTrends = New System.Windows.Forms.DataGridView()
            Me.pnlWhatThisMeans.SuspendLayout()
            Me.pnlKpiCards.SuspendLayout()
            Me.pnlNetSales.SuspendLayout()
            Me.pnlTransactions.SuspendLayout()
            Me.pnlAvgTransaction.SuspendLayout()
            Me.pnlReturns.SuspendLayout()
            Me.tabControl1.SuspendLayout()
            Me.tabPage1.SuspendLayout()
            CType(Me.dgvPaymentBreakdowns, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.tabPage2.SuspendLayout()
            CType(Me.dgvDailyBreakdown, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.tabPage3.SuspendLayout()
            CType(Me.dgvPaymentTrends, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'lblTitle
            '
            Me.lblTitle.AutoSize = True
            Me.lblTitle.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblTitle.Location = New System.Drawing.Point(13, 13)
            Me.lblTitle.Name = "lblTitle"
            Me.lblTitle.Size = New System.Drawing.Size(170, 30)
            Me.lblTitle.TabIndex = 0
            Me.lblTitle.Text = "Sales Summary"
            '
            'rbDaily
            '
            Me.rbDaily.AutoSize = True
            Me.rbDaily.Checked = True
            Me.rbDaily.Location = New System.Drawing.Point(200, 22)
            Me.rbDaily.Name = "rbDaily"
            Me.rbDaily.Size = New System.Drawing.Size(51, 19)
            Me.rbDaily.TabIndex = 1
            Me.rbDaily.TabStop = True
            Me.rbDaily.Text = "Daily"
            Me.rbDaily.UseVisualStyleBackColor = True
            '
            'rbWeekly
            '
            Me.rbWeekly.AutoSize = True
            Me.rbWeekly.Location = New System.Drawing.Point(257, 22)
            Me.rbWeekly.Name = "rbWeekly"
            Me.rbWeekly.Size = New System.Drawing.Size(63, 19)
            Me.rbWeekly.TabIndex = 2
            Me.rbWeekly.Text = "Weekly"
            Me.rbWeekly.UseVisualStyleBackColor = True
            '
            'rbMonthly
            '
            Me.rbMonthly.AutoSize = True
            Me.rbMonthly.Location = New System.Drawing.Point(326, 22)
            Me.rbMonthly.Name = "rbMonthly"
            Me.rbMonthly.Size = New System.Drawing.Size(70, 19)
            Me.rbMonthly.TabIndex = 3
            Me.rbMonthly.Text = "Monthly"
            Me.rbMonthly.UseVisualStyleBackColor = True
            '
            'dtpDate
            '
            Me.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
            Me.dtpDate.Location = New System.Drawing.Point(402, 20)
            Me.dtpDate.Name = "dtpDate"
            Me.dtpDate.Size = New System.Drawing.Size(120, 23)
            Me.dtpDate.TabIndex = 4
            '
            'cboYear
            '
            Me.cboYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cboYear.FormattingEnabled = True
            Me.cboYear.Location = New System.Drawing.Point(402, 20)
            Me.cboYear.Name = "cboYear"
            Me.cboYear.Size = New System.Drawing.Size(70, 23)
            Me.cboYear.TabIndex = 5
            Me.cboYear.Visible = False
            '
            'cboMonth
            '
            Me.cboMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cboMonth.FormattingEnabled = True
            Me.cboMonth.Location = New System.Drawing.Point(478, 20)
            Me.cboMonth.Name = "cboMonth"
            Me.cboMonth.Size = New System.Drawing.Size(50, 23)
            Me.cboMonth.TabIndex = 6
            Me.cboMonth.Visible = False
            '
            'btnRefresh
            '
            Me.btnRefresh.Location = New System.Drawing.Point(540, 20)
            Me.btnRefresh.Tag = "Accent"
            Me.btnRefresh.Name = "btnRefresh"
            Me.btnRefresh.Size = New System.Drawing.Size(75, 23)
            Me.btnRefresh.TabIndex = 7
            Me.btnRefresh.Text = "Refresh"
            Me.btnRefresh.UseVisualStyleBackColor = True
            '
            'lblLoading
            '
            Me.lblLoading.AutoSize = True
            Me.lblLoading.ForeColor = System.Drawing.Color.Blue
            Me.lblLoading.Location = New System.Drawing.Point(621, 24)
            Me.lblLoading.Name = "lblLoading"
            Me.lblLoading.Size = New System.Drawing.Size(59, 15)
            Me.lblLoading.TabIndex = 8
            Me.lblLoading.Text = "Loading..."
            Me.lblLoading.Visible = False
            '
            'pnlWhatThisMeans
            '
            Me.pnlWhatThisMeans.BackColor = System.Drawing.Color.FromArgb(CType(CType(235, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(251, Byte), Integer))
            Me.pnlWhatThisMeans.Controls.Add(Me.lblWarning)
            Me.pnlWhatThisMeans.Controls.Add(Me.lblWhatThisMeans)
            Me.pnlWhatThisMeans.Location = New System.Drawing.Point(18, 55)
            Me.pnlWhatThisMeans.Name = "pnlWhatThisMeans"
            Me.pnlWhatThisMeans.Size = New System.Drawing.Size(760, 60)
            Me.pnlWhatThisMeans.TabIndex = 9
            '
            'lblWarning
            '
            Me.lblWarning.AutoSize = True
            Me.lblWarning.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblWarning.ForeColor = System.Drawing.Color.Red
            Me.lblWarning.Location = New System.Drawing.Point(10, 10)
            Me.lblWarning.Name = "lblWarning"
            Me.lblWarning.Size = New System.Drawing.Size(199, 15)
            Me.lblWarning.TabIndex = 1
            Me.lblWarning.Text = "Credit Alert - High AR Exposure"
            Me.lblWarning.Visible = False
            '
            'lblWhatThisMeans
            '
            Me.lblWhatThisMeans.AutoSize = True
            Me.lblWhatThisMeans.Location = New System.Drawing.Point(10, 30)
            Me.lblWhatThisMeans.Name = "lblWhatThisMeans"
            Me.lblWhatThisMeans.Size = New System.Drawing.Size(102, 15)
            Me.lblWhatThisMeans.TabIndex = 0
            Me.lblWhatThisMeans.Text = "What This Means..."
            '
            'pnlKpiCards
            '
            Me.pnlKpiCards.Controls.Add(Me.pnlNetSales)
            Me.pnlKpiCards.Controls.Add(Me.pnlTransactions)
            Me.pnlKpiCards.Controls.Add(Me.pnlAvgTransaction)
            Me.pnlKpiCards.Controls.Add(Me.pnlReturns)
            Me.pnlKpiCards.Location = New System.Drawing.Point(18, 125)
            Me.pnlKpiCards.Name = "pnlKpiCards"
            Me.pnlKpiCards.Size = New System.Drawing.Size(760, 90)
            Me.pnlKpiCards.TabIndex = 10
            '
            'pnlNetSales
            '
            Me.pnlNetSales.BackColor = System.Drawing.Color.White
            Me.pnlNetSales.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.pnlNetSales.Controls.Add(Me.lblNetSalesTitle)
            Me.pnlNetSales.Controls.Add(Me.lblNetSalesValue)
            Me.pnlNetSales.Location = New System.Drawing.Point(3, 3)
            Me.pnlNetSales.Name = "pnlNetSales"
            Me.pnlNetSales.Size = New System.Drawing.Size(180, 80)
            Me.pnlNetSales.TabIndex = 0
            Me.pnlNetSales.Tag = "PrimaryMetricCard"
            '
            'lblNetSalesTitle
            '
            Me.lblNetSalesTitle.AutoSize = True
            Me.lblNetSalesTitle.ForeColor = System.Drawing.Color.Gray
            Me.lblNetSalesTitle.Location = New System.Drawing.Point(10, 10)
            Me.lblNetSalesTitle.Name = "lblNetSalesTitle"
            Me.lblNetSalesTitle.Size = New System.Drawing.Size(54, 15)
            Me.lblNetSalesTitle.TabIndex = 0
            Me.lblNetSalesTitle.Text = "Net Sales"
            Me.lblNetSalesTitle.Tag = "MetricCaption"
            '
            'lblNetSalesValue
            '
            Me.lblNetSalesValue.AutoSize = True
            Me.lblNetSalesValue.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblNetSalesValue.Location = New System.Drawing.Point(10, 35)
            Me.lblNetSalesValue.Name = "lblNetSalesValue"
            Me.lblNetSalesValue.Size = New System.Drawing.Size(50, 21)
            Me.lblNetSalesValue.TabIndex = 1
            Me.lblNetSalesValue.Text = "₱0.00"
            Me.lblNetSalesValue.Tag = "PrimaryMetricValue"
            '
            'pnlTransactions
            '
            Me.pnlTransactions.BackColor = System.Drawing.Color.White
            Me.pnlTransactions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.pnlTransactions.Controls.Add(Me.lblTransactionsTitle)
            Me.pnlTransactions.Controls.Add(Me.lblTransactionsValue)
            Me.pnlTransactions.Location = New System.Drawing.Point(189, 3)
            Me.pnlTransactions.Name = "pnlTransactions"
            Me.pnlTransactions.Size = New System.Drawing.Size(180, 80)
            Me.pnlTransactions.TabIndex = 1
            Me.pnlTransactions.Tag = "SecondaryMetricCard"
            '
            'lblTransactionsTitle
            '
            Me.lblTransactionsTitle.AutoSize = True
            Me.lblTransactionsTitle.ForeColor = System.Drawing.Color.Gray
            Me.lblTransactionsTitle.Location = New System.Drawing.Point(10, 10)
            Me.lblTransactionsTitle.Name = "lblTransactionsTitle"
            Me.lblTransactionsTitle.Size = New System.Drawing.Size(72, 15)
            Me.lblTransactionsTitle.TabIndex = 0
            Me.lblTransactionsTitle.Text = "Transactions"
            Me.lblTransactionsTitle.Tag = "MetricCaption"
            '
            'lblTransactionsValue
            '
            Me.lblTransactionsValue.AutoSize = True
            Me.lblTransactionsValue.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblTransactionsValue.Location = New System.Drawing.Point(10, 35)
            Me.lblTransactionsValue.Name = "lblTransactionsValue"
            Me.lblTransactionsValue.Size = New System.Drawing.Size(19, 21)
            Me.lblTransactionsValue.TabIndex = 1
            Me.lblTransactionsValue.Text = "0"
            Me.lblTransactionsValue.Tag = "SecondaryMetricValue"
            '
            'pnlAvgTransaction
            '
            Me.pnlAvgTransaction.BackColor = System.Drawing.Color.White
            Me.pnlAvgTransaction.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.pnlAvgTransaction.Controls.Add(Me.lblAvgTransactionTitle)
            Me.pnlAvgTransaction.Controls.Add(Me.lblAvgTransactionValue)
            Me.pnlAvgTransaction.Location = New System.Drawing.Point(375, 3)
            Me.pnlAvgTransaction.Name = "pnlAvgTransaction"
            Me.pnlAvgTransaction.Size = New System.Drawing.Size(180, 80)
            Me.pnlAvgTransaction.TabIndex = 2
            Me.pnlAvgTransaction.Tag = "SecondaryMetricCard"
            '
            'lblAvgTransactionTitle
            '
            Me.lblAvgTransactionTitle.AutoSize = True
            Me.lblAvgTransactionTitle.ForeColor = System.Drawing.Color.Gray
            Me.lblAvgTransactionTitle.Location = New System.Drawing.Point(10, 10)
            Me.lblAvgTransactionTitle.Name = "lblAvgTransactionTitle"
            Me.lblAvgTransactionTitle.Size = New System.Drawing.Size(91, 15)
            Me.lblAvgTransactionTitle.TabIndex = 0
            Me.lblAvgTransactionTitle.Text = "Avg Transaction"
            Me.lblAvgTransactionTitle.Tag = "MetricCaption"
            '
            'lblAvgTransactionValue
            '
            Me.lblAvgTransactionValue.AutoSize = True
            Me.lblAvgTransactionValue.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblAvgTransactionValue.Location = New System.Drawing.Point(10, 35)
            Me.lblAvgTransactionValue.Name = "lblAvgTransactionValue"
            Me.lblAvgTransactionValue.Size = New System.Drawing.Size(50, 21)
            Me.lblAvgTransactionValue.TabIndex = 1
            Me.lblAvgTransactionValue.Text = "₱0.00"
            Me.lblAvgTransactionValue.Tag = "SecondaryMetricValue"
            '
            'pnlReturns
            '
            Me.pnlReturns.BackColor = System.Drawing.Color.White
            Me.pnlReturns.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.pnlReturns.Controls.Add(Me.lblReturnsTitle)
            Me.pnlReturns.Controls.Add(Me.lblReturnsValue)
            Me.pnlReturns.Location = New System.Drawing.Point(561, 3)
            Me.pnlReturns.Name = "pnlReturns"
            Me.pnlReturns.Size = New System.Drawing.Size(180, 80)
            Me.pnlReturns.TabIndex = 3
            Me.pnlReturns.Tag = "SecondaryMetricCard"
            '
            'lblReturnsTitle
            '
            Me.lblReturnsTitle.AutoSize = True
            Me.lblReturnsTitle.ForeColor = System.Drawing.Color.Gray
            Me.lblReturnsTitle.Location = New System.Drawing.Point(10, 10)
            Me.lblReturnsTitle.Name = "lblReturnsTitle"
            Me.lblReturnsTitle.Size = New System.Drawing.Size(47, 15)
            Me.lblReturnsTitle.TabIndex = 0
            Me.lblReturnsTitle.Text = "Returns"
            Me.lblReturnsTitle.Tag = "MetricCaption"
            '
            'lblReturnsValue
            '
            Me.lblReturnsValue.AutoSize = True
            Me.lblReturnsValue.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblReturnsValue.Location = New System.Drawing.Point(10, 35)
            Me.lblReturnsValue.Name = "lblReturnsValue"
            Me.lblReturnsValue.Size = New System.Drawing.Size(50, 21)
            Me.lblReturnsValue.TabIndex = 1
            Me.lblReturnsValue.Text = "₱0.00"
            Me.lblReturnsValue.Tag = "SecondaryMetricValue"
            '
            'tabControl1
            '
            Me.tabControl1.Controls.Add(Me.tabPage1)
            Me.tabControl1.Controls.Add(Me.tabPage2)
            Me.tabControl1.Controls.Add(Me.tabPage3)
            Me.tabControl1.Location = New System.Drawing.Point(18, 225)
            Me.tabControl1.Name = "tabControl1"
            Me.tabControl1.SelectedIndex = 0
            Me.tabControl1.Size = New System.Drawing.Size(760, 300)
            Me.tabControl1.TabIndex = 11
            '
            'tabPage1
            '
            Me.tabPage1.Controls.Add(Me.dgvPaymentBreakdowns)
            Me.tabPage1.Controls.Add(Me.lblTotalGrossSales)
            Me.tabPage1.Controls.Add(Me.lblTotalTransactions)
            Me.tabPage1.Location = New System.Drawing.Point(4, 24)
            Me.tabPage1.Name = "tabPage1"
            Me.tabPage1.Padding = New System.Windows.Forms.Padding(3)
            Me.tabPage1.Size = New System.Drawing.Size(752, 272)
            Me.tabPage1.TabIndex = 0
            Me.tabPage1.Text = "Payment Method Breakdown"
            Me.tabPage1.UseVisualStyleBackColor = True
            '
            'dgvPaymentBreakdowns
            '
            Me.dgvPaymentBreakdowns.AllowUserToAddRows = False
            Me.dgvPaymentBreakdowns.AllowUserToDeleteRows = False
            Me.dgvPaymentBreakdowns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvPaymentBreakdowns.Location = New System.Drawing.Point(6, 6)
            Me.dgvPaymentBreakdowns.Name = "dgvPaymentBreakdowns"
            Me.dgvPaymentBreakdowns.EnableHeadersVisualStyles = False
            Me.dgvPaymentBreakdowns.ReadOnly = True
            Me.dgvPaymentBreakdowns.RowTemplate.Height = 25
            Me.dgvPaymentBreakdowns.Size = New System.Drawing.Size(740, 230)
            Me.dgvPaymentBreakdowns.TabIndex = 0
            '
            'lblTotalGrossSales
            '
            Me.lblTotalGrossSales.AutoSize = True
            Me.lblTotalGrossSales.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblTotalGrossSales.Location = New System.Drawing.Point(6, 245)
            Me.lblTotalGrossSales.Name = "lblTotalGrossSales"
            Me.lblTotalGrossSales.Size = New System.Drawing.Size(124, 15)
            Me.lblTotalGrossSales.TabIndex = 1
            Me.lblTotalGrossSales.Text = "Total Gross Sales: ₱0.00"
            '
            'lblTotalTransactions
            '
            Me.lblTotalTransactions.AutoSize = True
            Me.lblTotalTransactions.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblTotalTransactions.Location = New System.Drawing.Point(200, 245)
            Me.lblTotalTransactions.Name = "lblTotalTransactions"
            Me.lblTotalTransactions.Size = New System.Drawing.Size(117, 15)
            Me.lblTotalTransactions.TabIndex = 2
            Me.lblTotalTransactions.Text = "Total Transactions: 0"
            '
            'tabPage2
            '
            Me.tabPage2.Controls.Add(Me.dgvDailyBreakdown)
            Me.tabPage2.Location = New System.Drawing.Point(4, 24)
            Me.tabPage2.Name = "tabPage2"
            Me.tabPage2.Padding = New System.Windows.Forms.Padding(3)
            Me.tabPage2.Size = New System.Drawing.Size(752, 272)
            Me.tabPage2.TabIndex = 1
            Me.tabPage2.Text = "Daily Breakdown"
            Me.tabPage2.UseVisualStyleBackColor = True
            '
            'dgvDailyBreakdown
            '
            Me.dgvDailyBreakdown.AllowUserToAddRows = False
            Me.dgvDailyBreakdown.AllowUserToDeleteRows = False
            Me.dgvDailyBreakdown.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvDailyBreakdown.Dock = System.Windows.Forms.DockStyle.Fill
            Me.dgvDailyBreakdown.Location = New System.Drawing.Point(3, 3)
            Me.dgvDailyBreakdown.Name = "dgvDailyBreakdown"
            Me.dgvDailyBreakdown.EnableHeadersVisualStyles = False
            Me.dgvDailyBreakdown.ReadOnly = True
            Me.dgvDailyBreakdown.RowTemplate.Height = 25
            Me.dgvDailyBreakdown.Size = New System.Drawing.Size(746, 266)
            Me.dgvDailyBreakdown.TabIndex = 0
            '
            'tabPage3
            '
            Me.tabPage3.Controls.Add(Me.dgvPaymentTrends)
            Me.tabPage3.Location = New System.Drawing.Point(4, 24)
            Me.tabPage3.Name = "tabPage3"
            Me.tabPage3.Padding = New System.Windows.Forms.Padding(3)
            Me.tabPage3.Size = New System.Drawing.Size(752, 272)
            Me.tabPage3.TabIndex = 2
            Me.tabPage3.Text = "Payment Trends"
            Me.tabPage3.UseVisualStyleBackColor = True
            '
            'dgvPaymentTrends
            '
            Me.dgvPaymentTrends.AllowUserToAddRows = False
            Me.dgvPaymentTrends.AllowUserToDeleteRows = False
            Me.dgvPaymentTrends.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvPaymentTrends.Dock = System.Windows.Forms.DockStyle.Fill
            Me.dgvPaymentTrends.Location = New System.Drawing.Point(3, 3)
            Me.dgvPaymentTrends.Name = "dgvPaymentTrends"
            Me.dgvPaymentTrends.EnableHeadersVisualStyles = False
            Me.dgvPaymentTrends.ReadOnly = True
            Me.dgvPaymentTrends.RowTemplate.Height = 25
            Me.dgvPaymentTrends.Size = New System.Drawing.Size(746, 266)
            Me.dgvPaymentTrends.TabIndex = 0
            '
            'SalesSummaryView
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.tabControl1)
            Me.Controls.Add(Me.pnlKpiCards)
            Me.Controls.Add(Me.pnlWhatThisMeans)
            Me.Controls.Add(Me.lblLoading)
            Me.Controls.Add(Me.btnRefresh)
            Me.Controls.Add(Me.cboMonth)
            Me.Controls.Add(Me.cboYear)
            Me.Controls.Add(Me.dtpDate)
            Me.Controls.Add(Me.rbMonthly)
            Me.Controls.Add(Me.rbWeekly)
            Me.Controls.Add(Me.rbDaily)
            Me.Controls.Add(Me.lblTitle)
            Me.Name = "SalesSummaryView"
            Me.Size = New System.Drawing.Size(800, 540)
            Me.pnlWhatThisMeans.ResumeLayout(False)
            Me.pnlWhatThisMeans.PerformLayout()
            Me.pnlKpiCards.ResumeLayout(False)
            Me.pnlNetSales.ResumeLayout(False)
            Me.pnlNetSales.PerformLayout()
            Me.pnlTransactions.ResumeLayout(False)
            Me.pnlTransactions.PerformLayout()
            Me.pnlAvgTransaction.ResumeLayout(False)
            Me.pnlAvgTransaction.PerformLayout()
            Me.pnlReturns.ResumeLayout(False)
            Me.pnlReturns.PerformLayout()
            Me.tabControl1.ResumeLayout(False)
            Me.tabPage1.ResumeLayout(False)
            Me.tabPage1.PerformLayout()
            CType(Me.dgvPaymentBreakdowns, System.ComponentModel.ISupportInitialize).EndInit()
            Me.tabPage2.ResumeLayout(False)
            CType(Me.dgvDailyBreakdown, System.ComponentModel.ISupportInitialize).EndInit()
            Me.tabPage3.ResumeLayout(False)
            CType(Me.dgvPaymentTrends, System.ComponentModel.ISupportInitialize).EndInit()

            '
            ' pnlErrorState
            '
            Me.pnlErrorState.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pnlErrorState.Location = New System.Drawing.Point(0, 0)
            Me.pnlErrorState.Name = "pnlErrorState"
            Me.pnlErrorState.TabIndex = 26
            Me.pnlErrorState.Visible = False
            Me.Controls.Add(Me.pnlErrorState)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents lblTitle As Label
        Friend WithEvents rbDaily As RadioButton
        Friend WithEvents rbWeekly As RadioButton
        Friend WithEvents rbMonthly As RadioButton
        Friend WithEvents dtpDate As DateTimePicker
        Friend WithEvents cboYear As ComboBox
        Friend WithEvents cboMonth As ComboBox
        Friend WithEvents btnRefresh As Button
        Friend WithEvents lblLoading As Label
        Friend WithEvents pnlWhatThisMeans As Panel
        Friend WithEvents lblWarning As Label
        Friend WithEvents lblWhatThisMeans As Label
        Friend WithEvents pnlKpiCards As FlowLayoutPanel
        Friend WithEvents pnlNetSales As Panel
        Friend WithEvents lblNetSalesTitle As Label
        Friend WithEvents lblNetSalesValue As Label
        Friend WithEvents pnlTransactions As Panel
        Friend WithEvents lblTransactionsTitle As Label
        Friend WithEvents lblTransactionsValue As Label
        Friend WithEvents pnlAvgTransaction As Panel
        Friend WithEvents lblAvgTransactionTitle As Label
        Friend WithEvents lblAvgTransactionValue As Label
        Friend WithEvents pnlReturns As Panel
        Friend WithEvents lblReturnsTitle As Label
        Friend WithEvents lblReturnsValue As Label
        Friend WithEvents tabControl1 As TabControl
        Friend WithEvents tabPage1 As TabPage
        Friend WithEvents dgvPaymentBreakdowns As DataGridView
        Friend WithEvents lblTotalGrossSales As Label
        Friend WithEvents lblTotalTransactions As Label
        Friend WithEvents tabPage2 As TabPage
        Friend WithEvents dgvDailyBreakdown As DataGridView
        Friend WithEvents tabPage3 As TabPage
        Friend WithEvents dgvPaymentTrends As DataGridView
        Friend WithEvents pnlErrorState As MerchSys.App.Controls.ErrorStatePanel

    End Class

End Namespace
