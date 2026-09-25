Namespace Views.Shell
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class ActivityRail
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
        Friend WithEvents pnlIcons As System.Windows.Forms.FlowLayoutPanel

        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.pnlIcons = New System.Windows.Forms.FlowLayoutPanel()
            Me.SuspendLayout()
            '
            'pnlIcons
            '
            Me.pnlIcons.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pnlIcons.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
            Me.pnlIcons.Location = New System.Drawing.Point(0, 0)
            Me.pnlIcons.Name = "pnlIcons"
            Me.pnlIcons.Size = New System.Drawing.Size(60, 450)
            Me.pnlIcons.TabIndex = 0
            '
            'ActivityRail
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.pnlIcons)
            Me.Name = "ActivityRail"
            Me.Size = New System.Drawing.Size(60, 450)
            Me.ResumeLayout(False)
        End Sub
    End Class
End Namespace
