Namespace Views.Shell
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class ModuleDetailPanel
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
        Friend WithEvents lblModuleName As System.Windows.Forms.Label
        Friend WithEvents pnlItems As System.Windows.Forms.Panel
        Friend WithEvents ConnectionStatusSlot As System.Windows.Forms.Panel
        Friend WithEvents btnLogout As System.Windows.Forms.Button

        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.lblModuleName = New System.Windows.Forms.Label()
            Me.pnlItems = New System.Windows.Forms.Panel()
            Me.ConnectionStatusSlot = New System.Windows.Forms.Panel()
            Me.btnLogout = New System.Windows.Forms.Button()
            Me.SuspendLayout()
            '
            'lblModuleName
            '
            Me.lblModuleName.Dock = System.Windows.Forms.DockStyle.Top
            Me.lblModuleName.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblModuleName.ForeColor = System.Drawing.Color.White
            Me.lblModuleName.Location = New System.Drawing.Point(0, 0)
            Me.lblModuleName.Name = "lblModuleName"
            Me.lblModuleName.Size = New System.Drawing.Size(220, 50)
            Me.lblModuleName.TabIndex = 0
            Me.lblModuleName.Text = "Module Name"
            Me.lblModuleName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'pnlItems
            '
            Me.pnlItems.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pnlItems.Location = New System.Drawing.Point(0, 50)
            Me.pnlItems.Name = "pnlItems"
            Me.pnlItems.Size = New System.Drawing.Size(220, 326)
            Me.pnlItems.TabIndex = 1
            '
            'ConnectionStatusSlot
            '
            Me.ConnectionStatusSlot.Dock = System.Windows.Forms.DockStyle.Bottom
            Me.ConnectionStatusSlot.Location = New System.Drawing.Point(0, 426)
            Me.ConnectionStatusSlot.Name = "ConnectionStatusSlot"
            Me.ConnectionStatusSlot.Size = New System.Drawing.Size(220, 24)
            Me.ConnectionStatusSlot.TabIndex = 3
            '
            'btnLogout
            '
            Me.btnLogout.Dock = System.Windows.Forms.DockStyle.Bottom
            Me.btnLogout.FlatAppearance.BorderSize = 0
            Me.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnLogout.ForeColor = System.Drawing.Color.White
            Me.btnLogout.Location = New System.Drawing.Point(0, 376)
            Me.btnLogout.Name = "btnLogout"
            Me.btnLogout.Size = New System.Drawing.Size(220, 50)
            Me.btnLogout.TabIndex = 2
            Me.btnLogout.Text = "Log Out"
            Me.btnLogout.UseVisualStyleBackColor = True
            '
            'ModuleDetailPanel
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.pnlItems)
            Me.Controls.Add(Me.lblModuleName)
            Me.Controls.Add(Me.btnLogout)
            Me.Controls.Add(Me.ConnectionStatusSlot)
            Me.Name = "ModuleDetailPanel"
            Me.Size = New System.Drawing.Size(220, 450)
            Me.ResumeLayout(False)
        End Sub
    End Class
End Namespace
