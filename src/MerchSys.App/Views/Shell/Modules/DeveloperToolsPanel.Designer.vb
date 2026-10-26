Namespace Views.Shell.Modules
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class DeveloperToolsPanel
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

        Friend WithEvents btnRunVatSchemaHarness As System.Windows.Forms.Button
        Friend WithEvents btnToggleTheme As System.Windows.Forms.Button

        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.btnRunVatSchemaHarness = New System.Windows.Forms.Button()
            Me.btnToggleTheme = New System.Windows.Forms.Button()
            Me.SuspendLayout()
            '
            'btnRunVatSchemaHarness
            '
            Me.btnRunVatSchemaHarness.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnRunVatSchemaHarness.Location = New System.Drawing.Point(10, 10)
            Me.btnRunVatSchemaHarness.Name = "btnRunVatSchemaHarness"
            Me.btnRunVatSchemaHarness.Size = New System.Drawing.Size(200, 30)
            Me.btnRunVatSchemaHarness.TabIndex = 0
            Me.btnRunVatSchemaHarness.Text = "Run VAT Schema Harness"
            Me.btnRunVatSchemaHarness.UseVisualStyleBackColor = True
            '
            'btnToggleTheme
            '
            Me.btnToggleTheme.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnToggleTheme.Location = New System.Drawing.Point(10, 50)
            Me.btnToggleTheme.Name = "btnToggleTheme"
            Me.btnToggleTheme.Size = New System.Drawing.Size(200, 30)
            Me.btnToggleTheme.TabIndex = 1
            Me.btnToggleTheme.Text = "Toggle Theme"
            Me.btnToggleTheme.UseVisualStyleBackColor = True
            '
            'DeveloperToolsPanel
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.btnToggleTheme)
            Me.Controls.Add(Me.btnRunVatSchemaHarness)
            Me.Name = "DeveloperToolsPanel"
            Me.Size = New System.Drawing.Size(220, 326)
            Me.ResumeLayout(False)
        End Sub
    End Class
End Namespace
