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
            Me.pnlBusy = New System.Windows.Forms.Panel()
            Me.lblBusy = New System.Windows.Forms.Label()
            Me.pnlEmpty = New System.Windows.Forms.Panel()
            Me.lblEmpty = New System.Windows.Forms.Label()
            Me.pnlBackdrop.SuspendLayout()
            Me.pnlCard.SuspendLayout()
            Me.pnlBusy.SuspendLayout()
            Me.pnlEmpty.SuspendLayout()
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
            Me.pnlCard.Controls.Add(Me.pnlEmpty)
            Me.pnlCard.Controls.Add(Me.pnlBusy)
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
            ' pnlBusy
            '
            Me.pnlBusy.Controls.Add(Me.lblBusy)
            Me.pnlBusy.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pnlBusy.Location = New System.Drawing.Point(0, 29)
            Me.pnlBusy.Name = "pnlBusy"
            Me.pnlBusy.Size = New System.Drawing.Size(500, 371)
            Me.pnlBusy.TabIndex = 2
            Me.pnlBusy.Visible = False
            '
            ' lblBusy
            '
            Me.lblBusy.Dock = System.Windows.Forms.DockStyle.Fill
            Me.lblBusy.Location = New System.Drawing.Point(0, 0)
            Me.lblBusy.Name = "lblBusy"
            Me.lblBusy.Size = New System.Drawing.Size(500, 371)
            Me.lblBusy.TabIndex = 0
            Me.lblBusy.Text = "Searching..."
            Me.lblBusy.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            ' pnlEmpty
            '
            Me.pnlEmpty.Controls.Add(Me.lblEmpty)
            Me.pnlEmpty.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pnlEmpty.Location = New System.Drawing.Point(0, 29)
            Me.pnlEmpty.Name = "pnlEmpty"
            Me.pnlEmpty.Size = New System.Drawing.Size(500, 371)
            Me.pnlEmpty.TabIndex = 3
            Me.pnlEmpty.Visible = False
            '
            ' lblEmpty
            '
            Me.lblEmpty.Dock = System.Windows.Forms.DockStyle.Fill
            Me.lblEmpty.Location = New System.Drawing.Point(0, 0)
            Me.lblEmpty.Name = "lblEmpty"
            Me.lblEmpty.Size = New System.Drawing.Size(500, 371)
            Me.lblEmpty.TabIndex = 0
            Me.lblEmpty.Text = "No matching screens or products found"
            Me.lblEmpty.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
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
            Me.pnlBusy.ResumeLayout(False)
            Me.pnlEmpty.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents pnlBackdrop As System.Windows.Forms.Panel
        Friend WithEvents pnlCard As System.Windows.Forms.Panel
        Friend WithEvents txtSearch As System.Windows.Forms.TextBox
        Friend WithEvents lstResults As System.Windows.Forms.ListBox
        Friend WithEvents pnlBusy As System.Windows.Forms.Panel
        Friend WithEvents lblBusy As System.Windows.Forms.Label
        Friend WithEvents pnlEmpty As System.Windows.Forms.Panel
        Friend WithEvents lblEmpty As System.Windows.Forms.Label

    End Class
End Namespace
