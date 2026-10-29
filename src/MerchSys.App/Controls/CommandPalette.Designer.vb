Namespace Controls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class CommandPalette
        Inherits System.Windows.Forms.UserControl

        'UserControl overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(disposing As Boolean)
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
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.pnlBackdrop = New System.Windows.Forms.Panel()
            Me.pnlCard = New System.Windows.Forms.Panel()
            Me.txtSearch = New System.Windows.Forms.TextBox()
            Me.lstResults = New System.Windows.Forms.ListBox()
            Me.busyOverlay = New MerchSys.App.Controls.BusyOverlay()
            Me.emptyStatePanel = New MerchSys.App.Controls.EmptyStatePanel()
            Me.pnlBackdrop.SuspendLayout()
            Me.pnlCard.SuspendLayout()
            Me.SuspendLayout()
            '
            ' pnlBackdrop
            '
            Me.pnlBackdrop.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
            Me.pnlBackdrop.Controls.Add(Me.pnlCard)
            Me.pnlBackdrop.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pnlBackdrop.Location = New System.Drawing.Point(0, 0)
            Me.pnlBackdrop.Name = "pnlBackdrop"
            Me.pnlBackdrop.Size = New System.Drawing.Size(800, 600)
            Me.pnlBackdrop.TabIndex = 0
            '
            ' pnlCard
            '
            Me.pnlCard.BackColor = System.Drawing.SystemColors.Window
            Me.pnlCard.Controls.Add(Me.lstResults)
            Me.pnlCard.Controls.Add(Me.emptyStatePanel)
            Me.pnlCard.Controls.Add(Me.busyOverlay)
            Me.pnlCard.Controls.Add(Me.txtSearch)
            Me.pnlCard.Location = New System.Drawing.Point(150, 100)
            Me.pnlCard.Name = "pnlCard"
            Me.pnlCard.Size = New System.Drawing.Size(500, 400)
            Me.pnlCard.TabIndex = 0
            Me.pnlCard.Tag = "Card"
            '
            ' txtSearch
            '
            Me.txtSearch.Dock = System.Windows.Forms.DockStyle.Top
            Me.txtSearch.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me.txtSearch.Location = New System.Drawing.Point(0, 0)
            Me.txtSearch.Name = "txtSearch"
            Me.txtSearch.Size = New System.Drawing.Size(500, 29)
            Me.txtSearch.TabIndex = 0
            '
            ' lstResults
            '
            Me.lstResults.Dock = System.Windows.Forms.DockStyle.Fill
            Me.lstResults.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me.lstResults.FormattingEnabled = True
            Me.lstResults.ItemHeight = 20
            Me.lstResults.Location = New System.Drawing.Point(0, 29)
            Me.lstResults.Name = "lstResults"
            Me.lstResults.Size = New System.Drawing.Size(500, 371)
            Me.lstResults.TabIndex = 1
            '
            ' busyOverlay
            '
            Me.busyOverlay.Dock = System.Windows.Forms.DockStyle.Fill
            Me.busyOverlay.Location = New System.Drawing.Point(0, 29)
            Me.busyOverlay.Message = "Searching..."
            Me.busyOverlay.Name = "busyOverlay"
            Me.busyOverlay.Size = New System.Drawing.Size(500, 371)
            Me.busyOverlay.TabIndex = 2
            Me.busyOverlay.Visible = False
            '
            ' emptyStatePanel
            '
            Me.emptyStatePanel.Dock = System.Windows.Forms.DockStyle.Fill
            Me.emptyStatePanel.Location = New System.Drawing.Point(0, 29)
            Me.emptyStatePanel.Name = "emptyStatePanel"
            Me.emptyStatePanel.Size = New System.Drawing.Size(500, 371)
            Me.emptyStatePanel.TabIndex = 3
            Me.emptyStatePanel.Visible = False
            '
            ' CommandPalette
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.pnlBackdrop)
            Me.Name = "CommandPalette"
            Me.Size = New System.Drawing.Size(800, 600)
            Me.pnlBackdrop.ResumeLayout(False)
            Me.pnlCard.ResumeLayout(False)
            Me.pnlCard.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents pnlBackdrop As System.Windows.Forms.Panel
        Friend WithEvents pnlCard As System.Windows.Forms.Panel
        Friend WithEvents txtSearch As System.Windows.Forms.TextBox
        Friend WithEvents lstResults As System.Windows.Forms.ListBox
        Friend WithEvents busyOverlay As MerchSys.App.Controls.BusyOverlay
        Friend WithEvents emptyStatePanel As MerchSys.App.Controls.EmptyStatePanel

    End Class
End Namespace
