Imports System.Windows.Forms

Namespace Views.Dialogs

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class ProductPriceHistoryDialog
        ''' <summary>
        ''' Required designer variable.
        ''' </summary>
        Private components As System.ComponentModel.IContainer = Nothing

        ''' <summary>
        ''' Clean up any resources being used.
        ''' </summary>
        ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso (components IsNot Nothing) Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
            Me.dgvHistory = New System.Windows.Forms.DataGridView()
            Me.btnClose = New System.Windows.Forms.Button()
            CType(Me.dgvHistory, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'dgvHistory
            '
            Me.dgvHistory.AllowUserToAddRows = False
            Me.dgvHistory.AllowUserToDeleteRows = False
            Me.dgvHistory.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.dgvHistory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
            Me.dgvHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvHistory.Location = New System.Drawing.Point(12, 12)
            Me.dgvHistory.Name = "dgvHistory"
            Me.dgvHistory.ReadOnly = True
            Me.dgvHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
            Me.dgvHistory.Size = New System.Drawing.Size(660, 308)
            Me.dgvHistory.TabIndex = 0
            '
            'btnClose
            '
            Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnClose.Location = New System.Drawing.Point(597, 326)
            Me.btnClose.Name = "btnClose"
            Me.btnClose.Size = New System.Drawing.Size(75, 23)
            Me.btnClose.TabIndex = 1
            Me.btnClose.Text = "Close"
            Me.btnClose.UseVisualStyleBackColor = True
            '
            'ProductPriceHistoryDialog
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(684, 361)
            Me.Controls.Add(Me.btnClose)
            Me.Controls.Add(Me.dgvHistory)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "ProductPriceHistoryDialog"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Price History"
            CType(Me.dgvHistory, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents dgvHistory As System.Windows.Forms.DataGridView
        Friend WithEvents btnClose As System.Windows.Forms.Button

    End Class

End Namespace
