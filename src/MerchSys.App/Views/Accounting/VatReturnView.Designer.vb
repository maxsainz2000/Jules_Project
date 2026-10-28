Namespace Views.Accounting
    Partial Public Class VatReturnView
        Private components As System.ComponentModel.IContainer

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso (components IsNot Nothing) Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
            Me.pnlToolbar = New System.Windows.Forms.Panel()
            Me.cmbYear = New System.Windows.Forms.ComboBox()
            Me.cmbPeriod = New System.Windows.Forms.ComboBox()
            Me.rb2550M = New System.Windows.Forms.RadioButton()
            Me.rb2550Q = New System.Windows.Forms.RadioButton()
            Me.rb2551Q = New System.Windows.Forms.RadioButton()
            Me.btnGenerate = New System.Windows.Forms.Button()
            Me.btnFile = New System.Windows.Forms.Button()
            Me.btnAmend = New System.Windows.Forms.Button()
            Me.btnExportCsv = New System.Windows.Forms.Button()
            Me.btnExportPdf = New System.Windows.Forms.Button()
            Me.pnlWhatThisMeans = New System.Windows.Forms.Panel()
            Me.txtWhatThisMeans = New System.Windows.Forms.TextBox()
            Me.pnlSummary = New System.Windows.Forms.Panel()
            Me.lblTotalSales = New System.Windows.Forms.Label()
            Me.lblTotalPurchases = New System.Windows.Forms.Label()
            Me.lblOutputVat = New System.Windows.Forms.Label()
            Me.lblInputVat = New System.Windows.Forms.Label()
            Me.lblNetVatPayable = New System.Windows.Forms.Label()
            Me.lblFilingStatus = New System.Windows.Forms.Label()
            Me.lblError = New System.Windows.Forms.Label()
            Me.dgvLines = New System.Windows.Forms.DataGridView()
            Me.dgvHistory = New System.Windows.Forms.DataGridView()
            Me.pnlToolbar.SuspendLayout()
            Me.pnlWhatThisMeans.SuspendLayout()
            Me.pnlSummary.SuspendLayout()
            CType(Me.dgvLines, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.dgvHistory, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            
            ' pnlToolbar
            Me.pnlToolbar.Controls.Add(Me.rb2550M)
            Me.pnlToolbar.Controls.Add(Me.rb2550Q)
            Me.pnlToolbar.Controls.Add(Me.rb2551Q)
            Me.pnlToolbar.Controls.Add(Me.cmbYear)
            Me.pnlToolbar.Controls.Add(Me.cmbPeriod)
            Me.pnlToolbar.Controls.Add(Me.btnGenerate)
            Me.pnlToolbar.Controls.Add(Me.btnFile)
            Me.pnlToolbar.Controls.Add(Me.btnAmend)
            Me.pnlToolbar.Controls.Add(Me.btnExportCsv)
            Me.pnlToolbar.Controls.Add(Me.btnExportPdf)
            Me.pnlToolbar.Dock = System.Windows.Forms.DockStyle.Top
            Me.pnlToolbar.Height = 50
            
            ' rb2550M
            Me.rb2550M.Text = "2550M"
            Me.rb2550M.Location = New System.Drawing.Point(10, 15)
            Me.rb2550M.AutoSize = True
            
            ' rb2550Q
            Me.rb2550Q.Text = "2550Q"
            Me.rb2550Q.Location = New System.Drawing.Point(80, 15)
            Me.rb2550Q.AutoSize = True
            
            ' rb2551Q
            Me.rb2551Q.Text = "2551Q"
            Me.rb2551Q.Location = New System.Drawing.Point(150, 15)
            Me.rb2551Q.AutoSize = True
            
            ' cmbYear
            Me.cmbYear.Location = New System.Drawing.Point(220, 13)
            Me.cmbYear.Width = 80
            
            ' cmbPeriod
            Me.cmbPeriod.Location = New System.Drawing.Point(310, 13)
            Me.cmbPeriod.Width = 60
            
            ' btnGenerate
            Me.btnGenerate.Text = "Generate"
            Me.btnGenerate.Tag = "Accent"
            Me.btnGenerate.Location = New System.Drawing.Point(380, 12)
            
            ' btnFile
            Me.btnFile.Text = "File"
            Me.btnFile.Tag = "Accent"
            Me.btnFile.Location = New System.Drawing.Point(460, 12)
            
            ' btnAmend
            Me.btnAmend.Text = "Amend"
            Me.btnAmend.Tag = "Accent"
            Me.btnAmend.Location = New System.Drawing.Point(540, 12)
            
            ' btnExportCsv
            Me.btnExportCsv.Text = "Export CSV"
            Me.btnExportCsv.Tag = "Accent"
            Me.btnExportCsv.Location = New System.Drawing.Point(620, 12)
            
            ' btnExportPdf
            Me.btnExportPdf.Text = "Export PDF"
            Me.btnExportPdf.Tag = "Accent"
            Me.btnExportPdf.Location = New System.Drawing.Point(700, 12)
            
            ' pnlWhatThisMeans
            Me.pnlWhatThisMeans.Controls.Add(Me.txtWhatThisMeans)
            Me.pnlWhatThisMeans.Dock = System.Windows.Forms.DockStyle.Top
            Me.pnlWhatThisMeans.Height = 60
            
            ' txtWhatThisMeans
            Me.txtWhatThisMeans.Dock = System.Windows.Forms.DockStyle.Fill
            Me.txtWhatThisMeans.Multiline = True
            Me.txtWhatThisMeans.ReadOnly = True
            
            ' pnlSummary
            Me.pnlSummary.Controls.Add(Me.lblTotalSales)
            Me.pnlSummary.Controls.Add(Me.lblTotalPurchases)
            Me.pnlSummary.Controls.Add(Me.lblOutputVat)
            Me.pnlSummary.Controls.Add(Me.lblInputVat)
            Me.pnlSummary.Controls.Add(Me.lblNetVatPayable)
            Me.pnlSummary.Controls.Add(Me.lblFilingStatus)
            Me.pnlSummary.Controls.Add(Me.lblError)
            Me.pnlSummary.Dock = System.Windows.Forms.DockStyle.Top
            Me.pnlSummary.Height = 80
            Me.pnlSummary.Tag = "PrimaryMetricCard"
            
            ' Labels setup (simplified layout)
            Me.lblTotalSales.Location = New System.Drawing.Point(10, 10)
            Me.lblTotalSales.Tag = "SecondaryMetricValue"
            Me.lblTotalPurchases.Location = New System.Drawing.Point(150, 10)
            Me.lblTotalPurchases.Tag = "SecondaryMetricValue"
            Me.lblOutputVat.Location = New System.Drawing.Point(290, 10)
            Me.lblOutputVat.Tag = "SecondaryMetricValue"
            Me.lblInputVat.Location = New System.Drawing.Point(430, 10)
            Me.lblInputVat.Tag = "SecondaryMetricValue"
            Me.lblNetVatPayable.Location = New System.Drawing.Point(570, 10)
            Me.lblNetVatPayable.Tag = "PrimaryMetricValue"
            Me.lblFilingStatus.Location = New System.Drawing.Point(10, 40)
            Me.lblError.Location = New System.Drawing.Point(150, 40)
            Me.lblError.ForeColor = System.Drawing.Color.Red
            Me.lblError.Visible = False
            Me.lblError.AutoSize = True
            
            ' dgvLines
            Me.dgvLines.EnableHeadersVisualStyles = False
            Me.dgvLines.Dock = System.Windows.Forms.DockStyle.Fill
            
            ' dgvHistory
            Me.dgvHistory.EnableHeadersVisualStyles = False
            Me.dgvHistory.Dock = System.Windows.Forms.DockStyle.Right
            Me.dgvHistory.Width = 300
            
            ' VatReturnView
            Me.AutoScroll = True
            Me.Controls.Add(Me.dgvLines)
            Me.Controls.Add(Me.dgvHistory)
            Me.Controls.Add(Me.pnlSummary)
            Me.Controls.Add(Me.pnlWhatThisMeans)
            Me.Controls.Add(Me.pnlToolbar)
            Me.Size = New System.Drawing.Size(1000, 600)
            
            Me.pnlToolbar.ResumeLayout(False)
            Me.pnlWhatThisMeans.ResumeLayout(False)
            Me.pnlSummary.ResumeLayout(False)
            CType(Me.dgvLines, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.dgvHistory, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
        End Sub

        Friend WithEvents pnlToolbar As System.Windows.Forms.Panel
        Friend WithEvents cmbYear As System.Windows.Forms.ComboBox
        Friend WithEvents cmbPeriod As System.Windows.Forms.ComboBox
        Friend WithEvents rb2550M As System.Windows.Forms.RadioButton
        Friend WithEvents rb2550Q As System.Windows.Forms.RadioButton
        Friend WithEvents rb2551Q As System.Windows.Forms.RadioButton
        Friend WithEvents btnGenerate As System.Windows.Forms.Button
        Friend WithEvents btnFile As System.Windows.Forms.Button
        Friend WithEvents btnAmend As System.Windows.Forms.Button
        Friend WithEvents btnExportCsv As System.Windows.Forms.Button
        Friend WithEvents btnExportPdf As System.Windows.Forms.Button
        Friend WithEvents pnlWhatThisMeans As System.Windows.Forms.Panel
        Friend WithEvents txtWhatThisMeans As System.Windows.Forms.TextBox
        Friend WithEvents pnlSummary As System.Windows.Forms.Panel
        Friend WithEvents lblTotalSales As System.Windows.Forms.Label
        Friend WithEvents lblTotalPurchases As System.Windows.Forms.Label
        Friend WithEvents lblOutputVat As System.Windows.Forms.Label
        Friend WithEvents lblInputVat As System.Windows.Forms.Label
        Friend WithEvents lblNetVatPayable As System.Windows.Forms.Label
        Friend WithEvents lblFilingStatus As System.Windows.Forms.Label
        Friend WithEvents lblError As System.Windows.Forms.Label
        Friend WithEvents dgvLines As System.Windows.Forms.DataGridView
        Friend WithEvents dgvHistory As System.Windows.Forms.DataGridView
        Friend WithEvents pnlErrorState As MerchSys.App.Controls.ErrorStatePanel
    End Class
End Namespace
