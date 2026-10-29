Namespace Controls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class CommandPalette
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
            Me.busyOverlay = New MerchSys.App.Controls.BusyOverlay()
            Me.emptyStatePanel = New MerchSys.App.Controls.EmptyStatePanel()
            Me.pnlCard = New System.Windows.Forms.Panel()
            Me.txtSearch = New System.Windows.Forms.TextBox()
            Me.lstResults = New System.Windows.Forms.ListBox()
            Me.pnlCard.SuspendLayout()
            Me.SuspendLayout()
            '
            ' busyOverlay
            '
            Me.busyOverlay.Dock = System.Windows.Forms.DockStyle.Fill
            Me.busyOverlay.Visible = False
            '
            ' emptyStatePanel
            '
            Me.emptyStatePanel.Dock = System.Windows.Forms.DockStyle.Fill
            Me.emptyStatePanel.Visible = False
            '
            ' pnlCard
            '
            Me.pnlCard.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.pnlCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.pnlCard.Controls.Add(Me.busyOverlay)
            Me.pnlCard.Controls.Add(Me.emptyStatePanel)
            Me.pnlCard.Controls.Add(Me.lstResults)
            Me.pnlCard.Controls.Add(Me.txtSearch)
            Me.pnlCard.Location = New System.Drawing.Point(100, 50)
            Me.pnlCard.Name = "pnlCard"
            Me.pnlCard.Size = New System.Drawing.Size(400, 300)
            Me.pnlCard.TabIndex = 0
            '
            ' txtSearch
            '
            Me.txtSearch.Dock = System.Windows.Forms.DockStyle.Top
            Me.txtSearch.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me.txtSearch.Location = New System.Drawing.Point(0, 0)
            Me.txtSearch.Name = "txtSearch"
            Me.txtSearch.Size = New System.Drawing.Size(398, 29)
            Me.txtSearch.TabIndex = 0
            Me.txtSearch.Text = ""
            '
            ' lstResults
            '
            Me.lstResults.Dock = System.Windows.Forms.DockStyle.Fill
            Me.lstResults.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.lstResults.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me.lstResults.FormattingEnabled = True
            Me.lstResults.ItemHeight = 40
            Me.lstResults.Location = New System.Drawing.Point(0, 29)
            Me.lstResults.Name = "lstResults"
            Me.lstResults.Size = New System.Drawing.Size(398, 269)
            Me.lstResults.TabIndex = 1
            '
            ' CommandPalette
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
            Me.Controls.Add(Me.pnlCard)
            Me.Name = "CommandPalette"
            Me.Size = New System.Drawing.Size(600, 400)
            Me.pnlCard.ResumeLayout(False)
            Me.pnlCard.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents busyOverlay As MerchSys.App.Controls.BusyOverlay
        Friend WithEvents emptyStatePanel As MerchSys.App.Controls.EmptyStatePanel
        Friend WithEvents pnlCard As System.Windows.Forms.Panel
        Friend WithEvents txtSearch As System.Windows.Forms.TextBox
        Friend WithEvents lstResults As System.Windows.Forms.ListBox
    End Class
End Namespace
