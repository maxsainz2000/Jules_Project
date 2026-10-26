Imports System.Windows.Forms

Namespace Views.Accounting
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class VatReliefReportView
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
            Me.pnlHeader = New System.Windows.Forms.Panel()
            Me.btnRefresh = New System.Windows.Forms.Button()
            Me.btnExportPdf = New System.Windows.Forms.Button()
            Me.cboMonth = New System.Windows.Forms.ComboBox()
            Me.lblMonth = New System.Windows.Forms.Label()
            Me.cboYear = New System.Windows.Forms.ComboBox()
            Me.lblYear = New System.Windows.Forms.Label()
            Me.lblTitle = New System.Windows.Forms.Label()
            Me.pnlNetVatBanner = New System.Windows.Forms.Panel()
            Me.lblNetVatPayable = New System.Windows.Forms.Label()
            Me.lblNetVatTitle = New System.Windows.Forms.Label()
            Me.pnlSummaries = New System.Windows.Forms.TableLayoutPanel()
            Me.pnlSalesCard = New System.Windows.Forms.Panel()
            Me.lblOutputVat = New System.Windows.Forms.Label()
            Me.lblTotalSales = New System.Windows.Forms.Label()
            Me.lblZeroRatedSales = New System.Windows.Forms.Label()
            Me.lblVatExemptSales = New System.Windows.Forms.Label()
            Me.lblVatableSales = New System.Windows.Forms.Label()
            Me.lblSalesTitle = New System.Windows.Forms.Label()
            Me.pnlPurchasesCard = New System.Windows.Forms.Panel()
            Me.lblInputVat = New System.Windows.Forms.Label()
            Me.lblTotalPurchases = New System.Windows.Forms.Label()
            Me.lblZeroRatedPurchases = New System.Windows.Forms.Label()
            Me.lblVatExemptPurchases = New System.Windows.Forms.Label()
            Me.lblVatablePurchases = New System.Windows.Forms.Label()
            Me.lblPurchasesTitle = New System.Windows.Forms.Label()
            Me.dgvTrailingMonths = New System.Windows.Forms.DataGridView()
            Me.pnlWhatThisMeans = New System.Windows.Forms.Panel()
            Me.lblWhatThisMeansText = New System.Windows.Forms.Label()
            Me.lblWhatThisMeansTitle = New System.Windows.Forms.Label()
            Me.pnlHeader.SuspendLayout()
            Me.pnlNetVatBanner.SuspendLayout()
            Me.pnlSummaries.SuspendLayout()
            Me.pnlSalesCard.SuspendLayout()
            Me.pnlPurchasesCard.SuspendLayout()
            CType(Me.dgvTrailingMonths, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.pnlWhatThisMeans.SuspendLayout()
            Me.SuspendLayout()
            '
            'pnlHeader
            '
            Me.pnlHeader.Controls.Add(Me.btnRefresh)
            Me.pnlHeader.Controls.Add(Me.btnExportPdf)
            Me.pnlHeader.Controls.Add(Me.cboMonth)
            Me.pnlHeader.Controls.Add(Me.lblMonth)
            Me.pnlHeader.Controls.Add(Me.cboYear)
            Me.pnlHeader.Controls.Add(Me.lblYear)
            Me.pnlHeader.Controls.Add(Me.lblTitle)
            Me.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top
            Me.pnlHeader.Location = New System.Drawing.Point(0, 0)
            Me.pnlHeader.Name = "pnlHeader"
            Me.pnlHeader.Size = New System.Drawing.Size(900, 60)
            Me.pnlHeader.TabIndex = 0
            '
            'btnRefresh
            '
            Me.btnRefresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnRefresh.Location = New System.Drawing.Point(800, 18)
            Me.btnRefresh.Name = "btnRefresh"
            Me.btnRefresh.Size = New System.Drawing.Size(75, 25)
            Me.btnRefresh.TabIndex = 5
            Me.btnRefresh.Text = "Refresh"
            Me.btnRefresh.UseVisualStyleBackColor = True
            '
            'btnExportPdf
            '
            Me.btnExportPdf.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnExportPdf.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnExportPdf.Location = New System.Drawing.Point(880, 18)
            Me.btnExportPdf.Name = "btnExportPdf"
            Me.btnExportPdf.Size = New System.Drawing.Size(75, 25)
            Me.btnExportPdf.TabIndex = 6
            Me.btnExportPdf.Text = "Export PDF"
            Me.btnExportPdf.UseVisualStyleBackColor = True
            '
            'cboMonth
            '
            Me.cboMonth.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.cboMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cboMonth.FormattingEnabled = True
            Me.cboMonth.Location = New System.Drawing.Point(670, 20)
            Me.cboMonth.Name = "cboMonth"
            Me.cboMonth.Size = New System.Drawing.Size(110, 23)
            Me.cboMonth.TabIndex = 4
            '
            'lblMonth
            '
            Me.lblMonth.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblMonth.AutoSize = True
            Me.lblMonth.Location = New System.Drawing.Point(620, 23)
            Me.lblMonth.Name = "lblMonth"
            Me.lblMonth.Size = New System.Drawing.Size(46, 15)
            Me.lblMonth.TabIndex = 3
            Me.lblMonth.Text = "Month:"
            '
            'cboYear
            '
            Me.cboYear.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.cboYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cboYear.FormattingEnabled = True
            Me.cboYear.Location = New System.Drawing.Point(520, 20)
            Me.cboYear.Name = "cboYear"
            Me.cboYear.Size = New System.Drawing.Size(80, 23)
            Me.cboYear.TabIndex = 2
            '
            'lblYear
            '
            Me.lblYear.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblYear.AutoSize = True
            Me.lblYear.Location = New System.Drawing.Point(480, 23)
            Me.lblYear.Name = "lblYear"
            Me.lblYear.Size = New System.Drawing.Size(32, 15)
            Me.lblYear.TabIndex = 1
            Me.lblYear.Text = "Year:"
            '
            'lblTitle
            '
            Me.lblTitle.AutoSize = True
            Me.lblTitle.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblTitle.Location = New System.Drawing.Point(20, 15)
            Me.lblTitle.Name = "lblTitle"
            Me.lblTitle.Size = New System.Drawing.Size(199, 30)
            Me.lblTitle.TabIndex = 0
            Me.lblTitle.Text = "VAT Relief Report"
            '
            'pnlNetVatBanner
            '
            Me.pnlNetVatBanner.Controls.Add(Me.lblNetVatPayable)
            Me.pnlNetVatBanner.Controls.Add(Me.lblNetVatTitle)
            Me.pnlNetVatBanner.Dock = System.Windows.Forms.DockStyle.Top
            Me.pnlNetVatBanner.Location = New System.Drawing.Point(0, 60)
            Me.pnlNetVatBanner.Name = "pnlNetVatBanner"
            Me.pnlNetVatBanner.Size = New System.Drawing.Size(900, 60)
            Me.pnlNetVatBanner.TabIndex = 1
            '
            'lblNetVatPayable
            '
            Me.lblNetVatPayable.AutoSize = True
            Me.lblNetVatPayable.Font = New System.Drawing.Font("Segoe UI", 18.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblNetVatPayable.Location = New System.Drawing.Point(180, 15)
            Me.lblNetVatPayable.Name = "lblNetVatPayable"
            Me.lblNetVatPayable.Size = New System.Drawing.Size(76, 32)
            Me.lblNetVatPayable.TabIndex = 1
            Me.lblNetVatPayable.Text = "₱0.00"
            '
            'lblNetVatTitle
            '
            Me.lblNetVatTitle.AutoSize = True
            Me.lblNetVatTitle.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me.lblNetVatTitle.Location = New System.Drawing.Point(20, 20)
            Me.lblNetVatTitle.Name = "lblNetVatTitle"
            Me.lblNetVatTitle.Size = New System.Drawing.Size(155, 25)
            Me.lblNetVatTitle.TabIndex = 0
            Me.lblNetVatTitle.Text = "Net VAT Payable:"
            '
            'pnlSummaries
            '
            Me.pnlSummaries.ColumnCount = 2
            Me.pnlSummaries.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.pnlSummaries.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.pnlSummaries.Controls.Add(Me.pnlSalesCard, 0, 0)
            Me.pnlSummaries.Controls.Add(Me.pnlPurchasesCard, 1, 0)
            Me.pnlSummaries.Dock = System.Windows.Forms.DockStyle.Top
            Me.pnlSummaries.Location = New System.Drawing.Point(0, 120)
            Me.pnlSummaries.Name = "pnlSummaries"
            Me.pnlSummaries.RowCount = 1
            Me.pnlSummaries.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.pnlSummaries.Size = New System.Drawing.Size(900, 200)
            Me.pnlSummaries.TabIndex = 2
            '
            'pnlSalesCard
            '
            Me.pnlSalesCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.pnlSalesCard.Controls.Add(Me.lblOutputVat)
            Me.pnlSalesCard.Controls.Add(Me.lblTotalSales)
            Me.pnlSalesCard.Controls.Add(Me.lblZeroRatedSales)
            Me.pnlSalesCard.Controls.Add(Me.lblVatExemptSales)
            Me.pnlSalesCard.Controls.Add(Me.lblVatableSales)
            Me.pnlSalesCard.Controls.Add(Me.lblSalesTitle)
            Me.pnlSalesCard.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pnlSalesCard.Location = New System.Drawing.Point(10, 10)
            Me.pnlSalesCard.Margin = New System.Windows.Forms.Padding(10)
            Me.pnlSalesCard.Name = "pnlSalesCard"
            Me.pnlSalesCard.Size = New System.Drawing.Size(430, 180)
            Me.pnlSalesCard.TabIndex = 0
            '
            'lblOutputVat
            '
            Me.lblOutputVat.AutoSize = True
            Me.lblOutputVat.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
            Me.lblOutputVat.Location = New System.Drawing.Point(20, 140)
            Me.lblOutputVat.Name = "lblOutputVat"
            Me.lblOutputVat.Size = New System.Drawing.Size(94, 19)
            Me.lblOutputVat.TabIndex = 5
            Me.lblOutputVat.Text = "Output VAT: "
            '
            'lblTotalSales
            '
            Me.lblTotalSales.AutoSize = True
            Me.lblTotalSales.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
            Me.lblTotalSales.Location = New System.Drawing.Point(20, 115)
            Me.lblTotalSales.Name = "lblTotalSales"
            Me.lblTotalSales.Size = New System.Drawing.Size(89, 19)
            Me.lblTotalSales.TabIndex = 4
            Me.lblTotalSales.Text = "Total Sales: "
            '
            'lblZeroRatedSales
            '
            Me.lblZeroRatedSales.AutoSize = True
            Me.lblZeroRatedSales.Location = New System.Drawing.Point(20, 90)
            Me.lblZeroRatedSales.Name = "lblZeroRatedSales"
            Me.lblZeroRatedSales.Size = New System.Drawing.Size(99, 15)
            Me.lblZeroRatedSales.TabIndex = 3
            Me.lblZeroRatedSales.Text = "Zero-Rated Sales:"
            '
            'lblVatExemptSales
            '
            Me.lblVatExemptSales.AutoSize = True
            Me.lblVatExemptSales.Location = New System.Drawing.Point(20, 65)
            Me.lblVatExemptSales.Name = "lblVatExemptSales"
            Me.lblVatExemptSales.Size = New System.Drawing.Size(103, 15)
            Me.lblVatExemptSales.TabIndex = 2
            Me.lblVatExemptSales.Text = "VAT-Exempt Sales:"
            '
            'lblVatableSales
            '
            Me.lblVatableSales.AutoSize = True
            Me.lblVatableSales.Location = New System.Drawing.Point(20, 40)
            Me.lblVatableSales.Name = "lblVatableSales"
            Me.lblVatableSales.Size = New System.Drawing.Size(78, 15)
            Me.lblVatableSales.TabIndex = 1
            Me.lblVatableSales.Text = "Vatable Sales:"
            '
            'lblSalesTitle
            '
            Me.lblSalesTitle.AutoSize = True
            Me.lblSalesTitle.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblSalesTitle.Location = New System.Drawing.Point(15, 10)
            Me.lblSalesTitle.Name = "lblSalesTitle"
            Me.lblSalesTitle.Size = New System.Drawing.Size(49, 21)
            Me.lblSalesTitle.TabIndex = 0
            Me.lblSalesTitle.Text = "Sales"
            '
            'pnlPurchasesCard
            '
            Me.pnlPurchasesCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.pnlPurchasesCard.Controls.Add(Me.lblInputVat)
            Me.pnlPurchasesCard.Controls.Add(Me.lblTotalPurchases)
            Me.pnlPurchasesCard.Controls.Add(Me.lblZeroRatedPurchases)
            Me.pnlPurchasesCard.Controls.Add(Me.lblVatExemptPurchases)
            Me.pnlPurchasesCard.Controls.Add(Me.lblVatablePurchases)
            Me.pnlPurchasesCard.Controls.Add(Me.lblPurchasesTitle)
            Me.pnlPurchasesCard.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pnlPurchasesCard.Location = New System.Drawing.Point(460, 10)
            Me.pnlPurchasesCard.Margin = New System.Windows.Forms.Padding(10)
            Me.pnlPurchasesCard.Name = "pnlPurchasesCard"
            Me.pnlPurchasesCard.Size = New System.Drawing.Size(430, 180)
            Me.pnlPurchasesCard.TabIndex = 1
            '
            'lblInputVat
            '
            Me.lblInputVat.AutoSize = True
            Me.lblInputVat.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
            Me.lblInputVat.Location = New System.Drawing.Point(20, 140)
            Me.lblInputVat.Name = "lblInputVat"
            Me.lblInputVat.Size = New System.Drawing.Size(81, 19)
            Me.lblInputVat.TabIndex = 11
            Me.lblInputVat.Text = "Input VAT: "
            '
            'lblTotalPurchases
            '
            Me.lblTotalPurchases.AutoSize = True
            Me.lblTotalPurchases.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
            Me.lblTotalPurchases.Location = New System.Drawing.Point(20, 115)
            Me.lblTotalPurchases.Name = "lblTotalPurchases"
            Me.lblTotalPurchases.Size = New System.Drawing.Size(121, 19)
            Me.lblTotalPurchases.TabIndex = 10
            Me.lblTotalPurchases.Text = "Total Purchases: "
            '
            'lblZeroRatedPurchases
            '
            Me.lblZeroRatedPurchases.AutoSize = True
            Me.lblZeroRatedPurchases.Location = New System.Drawing.Point(20, 90)
            Me.lblZeroRatedPurchases.Name = "lblZeroRatedPurchases"
            Me.lblZeroRatedPurchases.Size = New System.Drawing.Size(126, 15)
            Me.lblZeroRatedPurchases.TabIndex = 9
            Me.lblZeroRatedPurchases.Text = "Zero-Rated Purchases:"
            '
            'lblVatExemptPurchases
            '
            Me.lblVatExemptPurchases.AutoSize = True
            Me.lblVatExemptPurchases.Location = New System.Drawing.Point(20, 65)
            Me.lblVatExemptPurchases.Name = "lblVatExemptPurchases"
            Me.lblVatExemptPurchases.Size = New System.Drawing.Size(130, 15)
            Me.lblVatExemptPurchases.TabIndex = 8
            Me.lblVatExemptPurchases.Text = "VAT-Exempt Purchases:"
            '
            'lblVatablePurchases
            '
            Me.lblVatablePurchases.AutoSize = True
            Me.lblVatablePurchases.Location = New System.Drawing.Point(20, 40)
            Me.lblVatablePurchases.Name = "lblVatablePurchases"
            Me.lblVatablePurchases.Size = New System.Drawing.Size(105, 15)
            Me.lblVatablePurchases.TabIndex = 7
            Me.lblVatablePurchases.Text = "Vatable Purchases:"
            '
            'lblPurchasesTitle
            '
            Me.lblPurchasesTitle.AutoSize = True
            Me.lblPurchasesTitle.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblPurchasesTitle.Location = New System.Drawing.Point(15, 10)
            Me.lblPurchasesTitle.Name = "lblPurchasesTitle"
            Me.lblPurchasesTitle.Size = New System.Drawing.Size(86, 21)
            Me.lblPurchasesTitle.TabIndex = 6
            Me.lblPurchasesTitle.Text = "Purchases"
            '
            'dgvTrailingMonths
            '
            Me.dgvTrailingMonths.AllowUserToAddRows = False
            Me.dgvTrailingMonths.AllowUserToDeleteRows = False
            Me.dgvTrailingMonths.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
            Me.dgvTrailingMonths.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvTrailingMonths.Dock = System.Windows.Forms.DockStyle.Fill
            Me.dgvTrailingMonths.Location = New System.Drawing.Point(0, 320)
            Me.dgvTrailingMonths.Name = "dgvTrailingMonths"
            Me.dgvTrailingMonths.ReadOnly = True
            Me.dgvTrailingMonths.RowHeadersVisible = False
            Me.dgvTrailingMonths.RowTemplate.Height = 25
            Me.dgvTrailingMonths.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
            Me.dgvTrailingMonths.Size = New System.Drawing.Size(900, 200)
            Me.dgvTrailingMonths.TabIndex = 3
            '
            'pnlWhatThisMeans
            '
            Me.pnlWhatThisMeans.Controls.Add(Me.lblWhatThisMeansText)
            Me.pnlWhatThisMeans.Controls.Add(Me.lblWhatThisMeansTitle)
            Me.pnlWhatThisMeans.Dock = System.Windows.Forms.DockStyle.Bottom
            Me.pnlWhatThisMeans.Location = New System.Drawing.Point(0, 520)
            Me.pnlWhatThisMeans.Name = "pnlWhatThisMeans"
            Me.pnlWhatThisMeans.Size = New System.Drawing.Size(900, 80)
            Me.pnlWhatThisMeans.TabIndex = 4
            '
            'lblWhatThisMeansText
            '
            Me.lblWhatThisMeansText.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblWhatThisMeansText.Location = New System.Drawing.Point(20, 35)
            Me.lblWhatThisMeansText.Name = "lblWhatThisMeansText"
            Me.lblWhatThisMeansText.Size = New System.Drawing.Size(860, 35)
            Me.lblWhatThisMeansText.TabIndex = 1
            Me.lblWhatThisMeansText.Text = "..."
            '
            'lblWhatThisMeansTitle
            '
            Me.lblWhatThisMeansTitle.AutoSize = True
            Me.lblWhatThisMeansTitle.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblWhatThisMeansTitle.Location = New System.Drawing.Point(15, 10)
            Me.lblWhatThisMeansTitle.Name = "lblWhatThisMeansTitle"
            Me.lblWhatThisMeansTitle.Size = New System.Drawing.Size(123, 19)
            Me.lblWhatThisMeansTitle.TabIndex = 0
            Me.lblWhatThisMeansTitle.Text = "What This Means"
            '
            'VatReliefReportView
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.dgvTrailingMonths)
            Me.Controls.Add(Me.pnlWhatThisMeans)
            Me.Controls.Add(Me.pnlSummaries)
            Me.Controls.Add(Me.pnlNetVatBanner)
            Me.Controls.Add(Me.pnlHeader)
            Me.Name = "VatReliefReportView"
            Me.Size = New System.Drawing.Size(900, 600)
            Me.pnlHeader.ResumeLayout(False)
            Me.pnlHeader.PerformLayout()
            Me.pnlNetVatBanner.ResumeLayout(False)
            Me.pnlNetVatBanner.PerformLayout()
            Me.pnlSummaries.ResumeLayout(False)
            Me.pnlSalesCard.ResumeLayout(False)
            Me.pnlSalesCard.PerformLayout()
            Me.pnlPurchasesCard.ResumeLayout(False)
            Me.pnlPurchasesCard.PerformLayout()
            CType(Me.dgvTrailingMonths, System.ComponentModel.ISupportInitialize).EndInit()
            Me.pnlWhatThisMeans.ResumeLayout(False)
            Me.pnlWhatThisMeans.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents pnlHeader As Panel
        Friend WithEvents lblTitle As Label
        Friend WithEvents btnRefresh As Button
        Friend WithEvents btnExportPdf As Button
        Friend WithEvents cboMonth As ComboBox
        Friend WithEvents lblMonth As Label
        Friend WithEvents cboYear As ComboBox
        Friend WithEvents lblYear As Label
        Friend WithEvents pnlNetVatBanner As Panel
        Friend WithEvents lblNetVatTitle As Label
        Friend WithEvents lblNetVatPayable As Label
        Friend WithEvents pnlSummaries As TableLayoutPanel
        Friend WithEvents pnlSalesCard As Panel
        Friend WithEvents pnlPurchasesCard As Panel
        Friend WithEvents lblSalesTitle As Label
        Friend WithEvents lblVatableSales As Label
        Friend WithEvents lblVatExemptSales As Label
        Friend WithEvents lblZeroRatedSales As Label
        Friend WithEvents lblTotalSales As Label
        Friend WithEvents lblOutputVat As Label
        Friend WithEvents lblPurchasesTitle As Label
        Friend WithEvents lblVatablePurchases As Label
        Friend WithEvents lblVatExemptPurchases As Label
        Friend WithEvents lblZeroRatedPurchases As Label
        Friend WithEvents lblTotalPurchases As Label
        Friend WithEvents lblInputVat As Label
        Friend WithEvents dgvTrailingMonths As DataGridView
        Friend WithEvents pnlWhatThisMeans As Panel
        Friend WithEvents lblWhatThisMeansTitle As Label
        Friend WithEvents lblWhatThisMeansText As Label
    End Class
End Namespace
