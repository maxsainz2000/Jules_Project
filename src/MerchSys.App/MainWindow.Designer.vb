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
    Friend WithEvents btnLogout As System.Windows.Forms.Button

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New System.ComponentModel.Container()
        Me.btnLogout = New System.Windows.Forms.Button()
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Text = "Form1"
        Me.btnLogout.Location = New System.Drawing.Point(12, 400)
        Me.btnLogout.Name = "btnLogout"
        Me.btnLogout.Size = New System.Drawing.Size(100, 30)
        Me.btnLogout.TabIndex = 0
        Me.btnLogout.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        Me.btnLogout.Text = "Log Out"
        Me.btnLogout.UseVisualStyleBackColor = True
        Me.Controls.Add(Me.btnLogout)
    End Sub

End Class
