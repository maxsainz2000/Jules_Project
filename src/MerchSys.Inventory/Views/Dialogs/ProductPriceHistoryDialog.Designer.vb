Imports System.Windows.Forms

Namespace Views.Dialogs

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class ProductPriceHistoryDialog
        Inherits Form

        Private components As System.ComponentModel.IContainer

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso (components IsNot Nothing) Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
            Me.dgvHistories = New System.Windows.Forms.DataGridView()
            Me.btnClose = New System.Windows.Forms.Button()
            Me.pnlBottom = New System.Windows.Forms.Panel()
            CType(Me.dgvHistories, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.pnlBottom.SuspendLayout()
            Me.SuspendLayout()
            '
            ' dgvHistories
            '
            Me.dgvHistories.AllowUserToAddRows = False
            Me.dgvHistories.AllowUserToDeleteRows = False
            Me.dgvHistories.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvHistories.Dock = System.Windows.Forms.DockStyle.Fill
            Me.dgvHistories.Location = New System.Drawing.Point(0, 0)
            Me.dgvHistories.Name = "dgvHistories"
            Me.dgvHistories.ReadOnly = True
            Me.dgvHistories.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
            Me.dgvHistories.Size = New System.Drawing.Size(600, 360)
            Me.dgvHistories.TabIndex = 0
            '
            ' pnlBottom
            '
            Me.pnlBottom.Controls.Add(Me.btnClose)
            Me.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom
            Me.pnlBottom.Location = New System.Drawing.Point(0, 360)
            Me.pnlBottom.Name = "pnlBottom"
            Me.pnlBottom.Size = New System.Drawing.Size(600, 40)
            Me.pnlBottom.TabIndex = 1
            '
            ' btnClose
            '
            Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnClose.Location = New System.Drawing.Point(513, 8)
            Me.btnClose.Name = "btnClose"
            Me.btnClose.Size = New System.Drawing.Size(75, 23)
            Me.btnClose.TabIndex = 0
            Me.btnClose.Text = "Close"
            Me.btnClose.UseVisualStyleBackColor = True
            '
            ' ProductPriceHistoryDialog
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(600, 400)
            Me.Controls.Add(Me.dgvHistories)
            Me.Controls.Add(Me.pnlBottom)
            Me.Name = "ProductPriceHistoryDialog"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Product Retail Price History"
            CType(Me.dgvHistories, System.ComponentModel.ISupportInitialize).EndInit()
            Me.pnlBottom.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents dgvHistories As DataGridView
        Friend WithEvents btnClose As Button
        Friend WithEvents pnlBottom As Panel

    End Class

End Namespace
