Imports MerchSys.App.Services.Theming

Namespace Controls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class CommandPalette
        Inherits System.Windows.Forms.UserControl

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

        Private components As System.ComponentModel.IContainer

        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.busyOverlay = New MerchSys.App.Controls.BusyOverlay()
            Me.emptyStatePanel = New MerchSys.App.Controls.EmptyStatePanel()
            Me.pnlBackdrop = New System.Windows.Forms.Panel()
            Me.pnlCard = New System.Windows.Forms.Panel()
            Me.txtSearch = New System.Windows.Forms.TextBox()
            Me.lstResults = New System.Windows.Forms.ListBox()

            Me.pnlBackdrop.SuspendLayout()
            Me.pnlCard.SuspendLayout()
            Me.SuspendLayout()

            ' busyOverlay
            Me.busyOverlay.Dock = System.Windows.Forms.DockStyle.Fill
            Me.busyOverlay.Visible = False

            ' emptyStatePanel
            Me.emptyStatePanel.Dock = System.Windows.Forms.DockStyle.Fill
            Me.emptyStatePanel.Visible = False

            ' pnlBackdrop
            Me.pnlBackdrop.BackColor = System.Drawing.Color.FromArgb(128, 0, 0, 0)
            Me.pnlBackdrop.Controls.Add(Me.pnlCard)
            Me.pnlBackdrop.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pnlBackdrop.Location = New System.Drawing.Point(0, 0)
            Me.pnlBackdrop.Name = "pnlBackdrop"
            Me.pnlBackdrop.Size = New System.Drawing.Size(800, 600)
            Me.pnlBackdrop.TabIndex = 0

            ' pnlCard
            Me.pnlCard.BackColor = System.Drawing.SystemColors.Window
            Me.pnlCard.Controls.Add(Me.busyOverlay)
            Me.pnlCard.Controls.Add(Me.emptyStatePanel)
            Me.pnlCard.Controls.Add(Me.lstResults)
            Me.pnlCard.Controls.Add(Me.txtSearch)
            Me.pnlCard.Location = New System.Drawing.Point(100, 50)
            Me.pnlCard.Name = "pnlCard"
            Me.pnlCard.Size = New System.Drawing.Size(600, 400)
            Me.pnlCard.TabIndex = 0
            Me.pnlCard.Anchor = System.Windows.Forms.AnchorStyles.None

            ' txtSearch
            Me.txtSearch.Dock = System.Windows.Forms.DockStyle.Top
            Me.txtSearch.Font = New System.Drawing.Font("Segoe UI", 14.0!)
            Me.txtSearch.Location = New System.Drawing.Point(0, 0)
            Me.txtSearch.Name = "txtSearch"
            Me.txtSearch.Size = New System.Drawing.Size(600, 32)
            Me.txtSearch.TabIndex = 0

            ' lstResults
            Me.lstResults.Dock = System.Windows.Forms.DockStyle.Fill
            Me.lstResults.Font = New System.Drawing.Font("Segoe UI", 12.0!)
            Me.lstResults.ItemHeight = 21
            Me.lstResults.Location = New System.Drawing.Point(0, 32)
            Me.lstResults.Name = "lstResults"
            Me.lstResults.Size = New System.Drawing.Size(600, 368)
            Me.lstResults.TabIndex = 1
            Me.lstResults.BorderStyle = System.Windows.Forms.BorderStyle.None

            ' CommandPalette
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.pnlBackdrop)
            Me.Name = "CommandPalette"
            Me.Size = New System.Drawing.Size(800, 600)

            Me.pnlCard.ResumeLayout(False)
            Me.pnlCard.PerformLayout()
            Me.pnlBackdrop.ResumeLayout(False)
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
