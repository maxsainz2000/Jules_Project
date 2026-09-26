Imports System.Drawing
Imports System.Windows.Forms
Imports System.ComponentModel

Namespace Views

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class ExpiryMonitorView
        Inherits UserControl

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
            Me.lblNearExpiry = New System.Windows.Forms.Label()
            Me.lblExpired = New System.Windows.Forms.Label()
            Me.lblTotalValueAtRisk = New System.Windows.Forms.Label()
            Me.numThreshold = New System.Windows.Forms.NumericUpDown()
            Me.tabControl = New System.Windows.Forms.TabControl()
            Me.tabNearExpiry = New System.Windows.Forms.TabPage()
            Me.dgvNearExpiry = New System.Windows.Forms.DataGridView()
            Me.tabExpired = New System.Windows.Forms.TabPage()
            Me.dgvExpired = New System.Windows.Forms.DataGridView()
            Me.btnRefresh = New System.Windows.Forms.Button()
            Me.lblThreshold = New System.Windows.Forms.Label()
            CType(Me.numThreshold, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.tabControl.SuspendLayout()
            Me.tabNearExpiry.SuspendLayout()
            CType(Me.dgvNearExpiry, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.tabExpired.SuspendLayout()
            CType(Me.dgvExpired, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'lblNearExpiry
            '
            Me.lblNearExpiry.AutoSize = True
            Me.lblNearExpiry.Location = New System.Drawing.Point(15, 15)
            Me.lblNearExpiry.Name = "lblNearExpiry"
            Me.lblNearExpiry.Size = New System.Drawing.Size(125, 15)
            Me.lblNearExpiry.TabIndex = 0
            Me.lblNearExpiry.Text = "Near Expiry Count: 0"
            '
            'lblExpired
            '
            Me.lblExpired.AutoSize = True
            Me.lblExpired.Location = New System.Drawing.Point(200, 15)
            Me.lblExpired.Name = "lblExpired"
            Me.lblExpired.Size = New System.Drawing.Size(100, 15)
            Me.lblExpired.TabIndex = 1
            Me.lblExpired.Text = "Expired Count: 0"
            '
            'lblTotalValueAtRisk
            '
            Me.lblTotalValueAtRisk.AutoSize = True
            Me.lblTotalValueAtRisk.Location = New System.Drawing.Point(385, 15)
            Me.lblTotalValueAtRisk.Name = "lblTotalValueAtRisk"
            Me.lblTotalValueAtRisk.Size = New System.Drawing.Size(130, 15)
            Me.lblTotalValueAtRisk.TabIndex = 2
            Me.lblTotalValueAtRisk.Text = "Total Value At Risk: $0"
            '
            'lblThreshold
            '
            Me.lblThreshold.AutoSize = True
            Me.lblThreshold.Location = New System.Drawing.Point(550, 15)
            Me.lblThreshold.Name = "lblThreshold"
            Me.lblThreshold.Size = New System.Drawing.Size(95, 15)
            Me.lblThreshold.TabIndex = 6
            Me.lblThreshold.Text = "Threshold Days:"
            '
            'numThreshold
            '
            Me.numThreshold.Location = New System.Drawing.Point(650, 13)
            Me.numThreshold.Name = "numThreshold"
            Me.numThreshold.Size = New System.Drawing.Size(60, 23)
            Me.numThreshold.TabIndex = 3
            '
            'btnRefresh
            '
            Me.btnRefresh.Location = New System.Drawing.Point(730, 13)
            Me.btnRefresh.Name = "btnRefresh"
            Me.btnRefresh.Size = New System.Drawing.Size(75, 23)
            Me.btnRefresh.TabIndex = 7
            Me.btnRefresh.Text = "Refresh"
            Me.btnRefresh.UseVisualStyleBackColor = True
            '
            'tabControl
            '
            Me.tabControl.Controls.Add(Me.tabNearExpiry)
            Me.tabControl.Controls.Add(Me.tabExpired)
            Me.tabControl.Location = New System.Drawing.Point(15, 50)
            Me.tabControl.Name = "tabControl"
            Me.tabControl.SelectedIndex = 0
            Me.tabControl.Size = New System.Drawing.Size(790, 430)
            Me.tabControl.TabIndex = 4
            '
            'tabNearExpiry
            '
            Me.tabNearExpiry.Controls.Add(Me.dgvNearExpiry)
            Me.tabNearExpiry.Location = New System.Drawing.Point(4, 24)
            Me.tabNearExpiry.Name = "tabNearExpiry"
            Me.tabNearExpiry.Padding = New System.Windows.Forms.Padding(3)
            Me.tabNearExpiry.Size = New System.Drawing.Size(782, 402)
            Me.tabNearExpiry.TabIndex = 0
            Me.tabNearExpiry.Text = "Near-Expiry"
            Me.tabNearExpiry.UseVisualStyleBackColor = True
            '
            'dgvNearExpiry
            '
            Me.dgvNearExpiry.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvNearExpiry.Dock = System.Windows.Forms.DockStyle.Fill
            Me.dgvNearExpiry.Location = New System.Drawing.Point(3, 3)
            Me.dgvNearExpiry.Name = "dgvNearExpiry"
            Me.dgvNearExpiry.RowTemplate.Height = 25
            Me.dgvNearExpiry.Size = New System.Drawing.Size(776, 396)
            Me.dgvNearExpiry.TabIndex = 0
            '
            'tabExpired
            '
            Me.tabExpired.Controls.Add(Me.dgvExpired)
            Me.tabExpired.Location = New System.Drawing.Point(4, 24)
            Me.tabExpired.Name = "tabExpired"
            Me.tabExpired.Padding = New System.Windows.Forms.Padding(3)
            Me.tabExpired.Size = New System.Drawing.Size(782, 402)
            Me.tabExpired.TabIndex = 1
            Me.tabExpired.Text = "Expired"
            Me.tabExpired.UseVisualStyleBackColor = True
            '
            'dgvExpired
            '
            Me.dgvExpired.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvExpired.Dock = System.Windows.Forms.DockStyle.Fill
            Me.dgvExpired.Location = New System.Drawing.Point(3, 3)
            Me.dgvExpired.Name = "dgvExpired"
            Me.dgvExpired.RowTemplate.Height = 25
            Me.dgvExpired.Size = New System.Drawing.Size(776, 396)
            Me.dgvExpired.TabIndex = 0
            '
            'ExpiryMonitorView
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.btnRefresh)
            Me.Controls.Add(Me.lblThreshold)
            Me.Controls.Add(Me.tabControl)
            Me.Controls.Add(Me.numThreshold)
            Me.Controls.Add(Me.lblTotalValueAtRisk)
            Me.Controls.Add(Me.lblExpired)
            Me.Controls.Add(Me.lblNearExpiry)
            Me.Name = "ExpiryMonitorView"
            Me.Size = New System.Drawing.Size(820, 500)
            CType(Me.numThreshold, System.ComponentModel.ISupportInitialize).EndInit()
            Me.tabControl.ResumeLayout(False)
            Me.tabNearExpiry.ResumeLayout(False)
            CType(Me.dgvNearExpiry, System.ComponentModel.ISupportInitialize).EndInit()
            Me.tabExpired.ResumeLayout(False)
            CType(Me.dgvExpired, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents lblNearExpiry As Label
        Friend WithEvents lblExpired As Label
        Friend WithEvents lblTotalValueAtRisk As Label
        Friend WithEvents numThreshold As NumericUpDown
        Friend WithEvents tabControl As TabControl
        Friend WithEvents tabNearExpiry As TabPage
        Friend WithEvents dgvNearExpiry As DataGridView
        Friend WithEvents tabExpired As TabPage
        Friend WithEvents dgvExpired As DataGridView
        Friend WithEvents lblThreshold As Label
        Friend WithEvents btnRefresh As Button
    End Class

End Namespace
