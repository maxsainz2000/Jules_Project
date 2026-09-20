Imports System.Windows.Forms
Imports System.ComponentModel

Namespace Views
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class OwnerDashboardView
        Inherits System.Windows.Forms.UserControl

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

        Private components As System.ComponentModel.IContainer

        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.pnlLoading = New System.Windows.Forms.Panel()
            Me.lblLoading = New System.Windows.Forms.Label()
            Me.tableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()

            Me.pnlKpi1 = New System.Windows.Forms.Panel()
            Me.lblKpi1Title = New System.Windows.Forms.Label()
            Me.lblKpi1Value = New System.Windows.Forms.Label()
            Me.lblKpi1Interpretation = New System.Windows.Forms.Label()

            Me.pnlKpi2 = New System.Windows.Forms.Panel()
            Me.lblKpi2Title = New System.Windows.Forms.Label()
            Me.lblKpi2Value = New System.Windows.Forms.Label()
            Me.lblKpi2Interpretation = New System.Windows.Forms.Label()

            Me.pnlKpi3 = New System.Windows.Forms.Panel()
            Me.lblKpi3Title = New System.Windows.Forms.Label()
            Me.lblKpi3Value = New System.Windows.Forms.Label()
            Me.lblKpi3Interpretation = New System.Windows.Forms.Label()

            Me.pnlKpi4 = New System.Windows.Forms.Panel()
            Me.lblKpi4Title = New System.Windows.Forms.Label()
            Me.lblKpi4Value = New System.Windows.Forms.Label()
            Me.lblKpi4Interpretation = New System.Windows.Forms.Label()

            Me.pnlLoading.SuspendLayout()
            Me.tableLayoutPanel1.SuspendLayout()
            Me.pnlKpi1.SuspendLayout()
            Me.pnlKpi2.SuspendLayout()
            Me.pnlKpi3.SuspendLayout()
            Me.pnlKpi4.SuspendLayout()
            Me.SuspendLayout()
            '
            'pnlLoading
            '
            Me.pnlLoading.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
            Me.pnlLoading.Controls.Add(Me.lblLoading)
            Me.pnlLoading.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pnlLoading.Location = New System.Drawing.Point(0, 0)
            Me.pnlLoading.Name = "pnlLoading"
            Me.pnlLoading.Size = New System.Drawing.Size(600, 400)
            Me.pnlLoading.TabIndex = 1
            Me.pnlLoading.Visible = False
            '
            'lblLoading
            '
            Me.lblLoading.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.lblLoading.AutoSize = True
            Me.lblLoading.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblLoading.ForeColor = System.Drawing.Color.White
            Me.lblLoading.Location = New System.Drawing.Point(240, 180)
            Me.lblLoading.Name = "lblLoading"
            Me.lblLoading.Size = New System.Drawing.Size(120, 30)
            Me.lblLoading.TabIndex = 0
            Me.lblLoading.Text = "Loading..."
            '
            'tableLayoutPanel1
            '
            Me.tableLayoutPanel1.ColumnCount = 2
            Me.tableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.tableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.tableLayoutPanel1.Controls.Add(Me.pnlKpi1, 0, 0)
            Me.tableLayoutPanel1.Controls.Add(Me.pnlKpi2, 1, 0)
            Me.tableLayoutPanel1.Controls.Add(Me.pnlKpi3, 0, 1)
            Me.tableLayoutPanel1.Controls.Add(Me.pnlKpi4, 1, 1)
            Me.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.tableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
            Me.tableLayoutPanel1.Name = "tableLayoutPanel1"
            Me.tableLayoutPanel1.RowCount = 2
            Me.tableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.tableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.tableLayoutPanel1.Size = New System.Drawing.Size(600, 400)
            Me.tableLayoutPanel1.TabIndex = 0
            '
            'pnlKpi1
            '
            Me.pnlKpi1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.pnlKpi1.Controls.Add(Me.lblKpi1Interpretation)
            Me.pnlKpi1.Controls.Add(Me.lblKpi1Value)
            Me.pnlKpi1.Controls.Add(Me.lblKpi1Title)
            Me.pnlKpi1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pnlKpi1.Location = New System.Drawing.Point(3, 3)
            Me.pnlKpi1.Name = "pnlKpi1"
            Me.pnlKpi1.Size = New System.Drawing.Size(294, 194)
            Me.pnlKpi1.TabIndex = 0
            '
            'lblKpi1Title
            '
            Me.lblKpi1Title.AutoSize = True
            Me.lblKpi1Title.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblKpi1Title.Location = New System.Drawing.Point(10, 10)
            Me.lblKpi1Title.Name = "lblKpi1Title"
            Me.lblKpi1Title.Size = New System.Drawing.Size(120, 21)
            Me.lblKpi1Title.TabIndex = 0
            Me.lblKpi1Title.Text = "Gross Revenue"
            '
            'lblKpi1Value
            '
            Me.lblKpi1Value.AutoSize = True
            Me.lblKpi1Value.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me.lblKpi1Value.Location = New System.Drawing.Point(10, 40)
            Me.lblKpi1Value.Name = "lblKpi1Value"
            Me.lblKpi1Value.Size = New System.Drawing.Size(55, 30)
            Me.lblKpi1Value.TabIndex = 1
            Me.lblKpi1Value.Text = "$0.00"
            '
            'lblKpi1Interpretation
            '
            Me.lblKpi1Interpretation.AutoSize = True
            Me.lblKpi1Interpretation.Location = New System.Drawing.Point(10, 80)
            Me.lblKpi1Interpretation.Name = "lblKpi1Interpretation"
            Me.lblKpi1Interpretation.Size = New System.Drawing.Size(117, 15)
            Me.lblKpi1Interpretation.TabIndex = 2
            Me.lblKpi1Interpretation.Text = "What This Means: ..."
            '
            'pnlKpi2
            '
            Me.pnlKpi2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.pnlKpi2.Controls.Add(Me.lblKpi2Interpretation)
            Me.pnlKpi2.Controls.Add(Me.lblKpi2Value)
            Me.pnlKpi2.Controls.Add(Me.lblKpi2Title)
            Me.pnlKpi2.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pnlKpi2.Location = New System.Drawing.Point(303, 3)
            Me.pnlKpi2.Name = "pnlKpi2"
            Me.pnlKpi2.Size = New System.Drawing.Size(294, 194)
            Me.pnlKpi2.TabIndex = 1
            '
            'lblKpi2Title
            '
            Me.lblKpi2Title.AutoSize = True
            Me.lblKpi2Title.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblKpi2Title.Location = New System.Drawing.Point(10, 10)
            Me.lblKpi2Title.Name = "lblKpi2Title"
            Me.lblKpi2Title.Size = New System.Drawing.Size(130, 21)
            Me.lblKpi2Title.TabIndex = 0
            Me.lblKpi2Title.Text = "Inventory Value"
            '
            'lblKpi2Value
            '
            Me.lblKpi2Value.AutoSize = True
            Me.lblKpi2Value.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me.lblKpi2Value.Location = New System.Drawing.Point(10, 40)
            Me.lblKpi2Value.Name = "lblKpi2Value"
            Me.lblKpi2Value.Size = New System.Drawing.Size(55, 30)
            Me.lblKpi2Value.TabIndex = 1
            Me.lblKpi2Value.Text = "$0.00"
            '
            'lblKpi2Interpretation
            '
            Me.lblKpi2Interpretation.AutoSize = True
            Me.lblKpi2Interpretation.Location = New System.Drawing.Point(10, 80)
            Me.lblKpi2Interpretation.Name = "lblKpi2Interpretation"
            Me.lblKpi2Interpretation.Size = New System.Drawing.Size(117, 15)
            Me.lblKpi2Interpretation.TabIndex = 2
            Me.lblKpi2Interpretation.Text = "What This Means: ..."
            '
            'pnlKpi3
            '
            Me.pnlKpi3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.pnlKpi3.Controls.Add(Me.lblKpi3Interpretation)
            Me.pnlKpi3.Controls.Add(Me.lblKpi3Value)
            Me.pnlKpi3.Controls.Add(Me.lblKpi3Title)
            Me.pnlKpi3.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pnlKpi3.Location = New System.Drawing.Point(3, 203)
            Me.pnlKpi3.Name = "pnlKpi3"
            Me.pnlKpi3.Size = New System.Drawing.Size(294, 194)
            Me.pnlKpi3.TabIndex = 2
            '
            'lblKpi3Title
            '
            Me.lblKpi3Title.AutoSize = True
            Me.lblKpi3Title.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblKpi3Title.Location = New System.Drawing.Point(10, 10)
            Me.lblKpi3Title.Name = "lblKpi3Title"
            Me.lblKpi3Title.Size = New System.Drawing.Size(124, 21)
            Me.lblKpi3Title.TabIndex = 0
            Me.lblKpi3Title.Text = "Total Expenses"
            '
            'lblKpi3Value
            '
            Me.lblKpi3Value.AutoSize = True
            Me.lblKpi3Value.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me.lblKpi3Value.Location = New System.Drawing.Point(10, 40)
            Me.lblKpi3Value.Name = "lblKpi3Value"
            Me.lblKpi3Value.Size = New System.Drawing.Size(55, 30)
            Me.lblKpi3Value.TabIndex = 1
            Me.lblKpi3Value.Text = "$0.00"
            '
            'lblKpi3Interpretation
            '
            Me.lblKpi3Interpretation.AutoSize = True
            Me.lblKpi3Interpretation.Location = New System.Drawing.Point(10, 80)
            Me.lblKpi3Interpretation.Name = "lblKpi3Interpretation"
            Me.lblKpi3Interpretation.Size = New System.Drawing.Size(117, 15)
            Me.lblKpi3Interpretation.TabIndex = 2
            Me.lblKpi3Interpretation.Text = "What This Means: ..."
            '
            'pnlKpi4
            '
            Me.pnlKpi4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.pnlKpi4.Controls.Add(Me.lblKpi4Interpretation)
            Me.pnlKpi4.Controls.Add(Me.lblKpi4Value)
            Me.pnlKpi4.Controls.Add(Me.lblKpi4Title)
            Me.pnlKpi4.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pnlKpi4.Location = New System.Drawing.Point(303, 203)
            Me.pnlKpi4.Name = "pnlKpi4"
            Me.pnlKpi4.Size = New System.Drawing.Size(294, 194)
            Me.pnlKpi4.TabIndex = 3
            '
            'lblKpi4Title
            '
            Me.lblKpi4Title.AutoSize = True
            Me.lblKpi4Title.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblKpi4Title.Location = New System.Drawing.Point(10, 10)
            Me.lblKpi4Title.Name = "lblKpi4Title"
            Me.lblKpi4Title.Size = New System.Drawing.Size(89, 21)
            Me.lblKpi4Title.TabIndex = 0
            Me.lblKpi4Title.Text = "Net Profit"
            '
            'lblKpi4Value
            '
            Me.lblKpi4Value.AutoSize = True
            Me.lblKpi4Value.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me.lblKpi4Value.Location = New System.Drawing.Point(10, 40)
            Me.lblKpi4Value.Name = "lblKpi4Value"
            Me.lblKpi4Value.Size = New System.Drawing.Size(55, 30)
            Me.lblKpi4Value.TabIndex = 1
            Me.lblKpi4Value.Text = "$0.00"
            '
            'lblKpi4Interpretation
            '
            Me.lblKpi4Interpretation.AutoSize = True
            Me.lblKpi4Interpretation.Location = New System.Drawing.Point(10, 80)
            Me.lblKpi4Interpretation.Name = "lblKpi4Interpretation"
            Me.lblKpi4Interpretation.Size = New System.Drawing.Size(117, 15)
            Me.lblKpi4Interpretation.TabIndex = 2
            Me.lblKpi4Interpretation.Text = "What This Means: ..."
            '
            'OwnerDashboardView
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.pnlLoading)
            Me.Controls.Add(Me.tableLayoutPanel1)
            Me.Name = "OwnerDashboardView"
            Me.Size = New System.Drawing.Size(600, 400)
            Me.pnlLoading.ResumeLayout(False)
            Me.pnlLoading.PerformLayout()
            Me.tableLayoutPanel1.ResumeLayout(False)
            Me.pnlKpi1.ResumeLayout(False)
            Me.pnlKpi1.PerformLayout()
            Me.pnlKpi2.ResumeLayout(False)
            Me.pnlKpi2.PerformLayout()
            Me.pnlKpi3.ResumeLayout(False)
            Me.pnlKpi3.PerformLayout()
            Me.pnlKpi4.ResumeLayout(False)
            Me.pnlKpi4.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents pnlLoading As System.Windows.Forms.Panel
        Friend WithEvents lblLoading As System.Windows.Forms.Label
        Friend WithEvents tableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents pnlKpi1 As System.Windows.Forms.Panel
        Friend WithEvents lblKpi1Title As System.Windows.Forms.Label
        Friend WithEvents lblKpi1Value As System.Windows.Forms.Label
        Friend WithEvents lblKpi1Interpretation As System.Windows.Forms.Label
        Friend WithEvents pnlKpi2 As System.Windows.Forms.Panel
        Friend WithEvents lblKpi2Title As System.Windows.Forms.Label
        Friend WithEvents lblKpi2Value As System.Windows.Forms.Label
        Friend WithEvents lblKpi2Interpretation As System.Windows.Forms.Label
        Friend WithEvents pnlKpi3 As System.Windows.Forms.Panel
        Friend WithEvents lblKpi3Title As System.Windows.Forms.Label
        Friend WithEvents lblKpi3Value As System.Windows.Forms.Label
        Friend WithEvents lblKpi3Interpretation As System.Windows.Forms.Label
        Friend WithEvents pnlKpi4 As System.Windows.Forms.Panel
        Friend WithEvents lblKpi4Title As System.Windows.Forms.Label
        Friend WithEvents lblKpi4Value As System.Windows.Forms.Label
        Friend WithEvents lblKpi4Interpretation As System.Windows.Forms.Label
    End Class
End Namespace
