Namespace Views
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainWindow
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Me.components = New System.ComponentModel.Container()
        Me.pnlSidebar = New System.Windows.Forms.Panel()
        Me.pnlContent = New System.Windows.Forms.Panel()
        Me.pnlLeft = New System.Windows.Forms.Panel()
        Me.pnlHeader = New System.Windows.Forms.Panel()
        Me.lblUserIdentity = New System.Windows.Forms.Label()
        Me.btnToggleThemeShell = New System.Windows.Forms.Button()
        Me.pnlStatusHost = New System.Windows.Forms.Panel()
        Me.pnlLeft.SuspendLayout()
        Me.pnlHeader.SuspendLayout()
        Me.SuspendLayout()
        '
        ' pnlSidebar
        '
        Me.pnlSidebar.BackColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlSidebar.Location = New System.Drawing.Point(0, 48)
        Me.pnlSidebar.Name = "pnlSidebar"
        Me.pnlSidebar.Size = New System.Drawing.Size(200, 354)
        Me.pnlSidebar.TabIndex = 1
        Me.pnlSidebar.AutoScroll = True
        '
        ' pnlHeader
        '
        Me.pnlHeader.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(44, Byte), Integer), CType(CType(58, Byte), Integer))
        Me.pnlHeader.Controls.Add(Me.btnToggleThemeShell)
        Me.pnlHeader.Controls.Add(Me.lblUserIdentity)
        Me.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlHeader.Location = New System.Drawing.Point(0, 0)
        Me.pnlHeader.Name = "pnlHeader"
        Me.pnlHeader.Size = New System.Drawing.Size(200, 48)
        Me.pnlHeader.TabIndex = 3
        '
        ' lblUserIdentity
        '
        Me.lblUserIdentity.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblUserIdentity.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblUserIdentity.ForeColor = System.Drawing.Color.White
        Me.lblUserIdentity.Location = New System.Drawing.Point(0, 0)
        Me.lblUserIdentity.Name = "lblUserIdentity"
        Me.lblUserIdentity.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.lblUserIdentity.Size = New System.Drawing.Size(120, 48)
        Me.lblUserIdentity.TabIndex = 0
        Me.lblUserIdentity.Text = "Not signed in"
        Me.lblUserIdentity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        ' btnToggleThemeShell
        '
        Me.btnToggleThemeShell.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnToggleThemeShell.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnToggleThemeShell.FlatAppearance.BorderSize = 0
        Me.btnToggleThemeShell.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnToggleThemeShell.ForeColor = System.Drawing.Color.White
        Me.btnToggleThemeShell.Location = New System.Drawing.Point(120, 0)
        Me.btnToggleThemeShell.Name = "btnToggleThemeShell"
        Me.btnToggleThemeShell.Size = New System.Drawing.Size(80, 48)
        Me.btnToggleThemeShell.TabIndex = 1
        Me.btnToggleThemeShell.Text = "☀️ Light"
        Me.btnToggleThemeShell.UseVisualStyleBackColor = True
        '
        ' pnlStatusHost
        '
        Me.pnlStatusHost.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(44, Byte), Integer), CType(CType(58, Byte), Integer))
        Me.pnlStatusHost.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlStatusHost.Location = New System.Drawing.Point(0, 402)
        Me.pnlStatusHost.Name = "pnlStatusHost"
        Me.pnlStatusHost.Size = New System.Drawing.Size(200, 48)
        Me.pnlStatusHost.TabIndex = 4
        '
        ' pnlLeft
        '
        Me.pnlLeft.Controls.Add(Me.pnlSidebar)
        Me.pnlLeft.Controls.Add(Me.pnlHeader)
        Me.pnlLeft.Controls.Add(Me.pnlStatusHost)
        Me.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left
        Me.pnlLeft.Location = New System.Drawing.Point(0, 0)
        Me.pnlLeft.Name = "pnlLeft"
        Me.pnlLeft.Size = New System.Drawing.Size(200, 450)
        Me.pnlLeft.TabIndex = 0
        '
        ' pnlContent
        '
        Me.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlContent.Location = New System.Drawing.Point(200, 0)
        Me.pnlContent.Name = "pnlContent"
        Me.pnlContent.Size = New System.Drawing.Size(600, 450)
        Me.pnlContent.TabIndex = 2
        '
        'MainWindow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.pnlContent)
        Me.Controls.Add(Me.pnlLeft)
        Me.Name = "MainWindow"
        Me.Text = "MainWindow"
        Me.pnlHeader.ResumeLayout(False)
        Me.pnlLeft.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlSidebar As System.Windows.Forms.Panel
    Friend WithEvents pnlContent As System.Windows.Forms.Panel
    Friend WithEvents pnlLeft As System.Windows.Forms.Panel
    Friend WithEvents pnlHeader As System.Windows.Forms.Panel
    Friend WithEvents lblUserIdentity As System.Windows.Forms.Label
    Friend WithEvents pnlStatusHost As System.Windows.Forms.Panel
    Friend WithEvents btnToggleThemeShell As System.Windows.Forms.Button

End Class
End Namespace
