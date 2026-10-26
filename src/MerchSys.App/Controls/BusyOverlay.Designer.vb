Namespace Controls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class BusyOverlay
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
            Me.panelContent = New System.Windows.Forms.Panel()
            Me.progressBar = New System.Windows.Forms.ProgressBar()
            Me.lblMessage = New System.Windows.Forms.Label()
            Me.panelContent.SuspendLayout()
            Me.SuspendLayout()
            '
            'panelContent
            '
            Me.panelContent.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.panelContent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.panelContent.Controls.Add(Me.progressBar)
            Me.panelContent.Controls.Add(Me.lblMessage)
            Me.panelContent.Location = New System.Drawing.Point(50, 50)
            Me.panelContent.Name = "panelContent"
            Me.panelContent.Size = New System.Drawing.Size(200, 100)
            Me.panelContent.TabIndex = 0
            '
            'progressBar
            '
            Me.progressBar.Location = New System.Drawing.Point(20, 55)
            Me.progressBar.Name = "progressBar"
            Me.progressBar.Size = New System.Drawing.Size(160, 20)
            Me.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee
            Me.progressBar.TabIndex = 1
            '
            'lblMessage
            '
            Me.lblMessage.AutoSize = False
            Me.lblMessage.Location = New System.Drawing.Point(20, 20)
            Me.lblMessage.Name = "lblMessage"
            Me.lblMessage.Size = New System.Drawing.Size(160, 20)
            Me.lblMessage.TabIndex = 0
            Me.lblMessage.Text = "Loading..."
            Me.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'BusyOverlay
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.panelContent)
            Me.Name = "BusyOverlay"
            Me.Size = New System.Drawing.Size(300, 200)
            Me.panelContent.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents panelContent As System.Windows.Forms.Panel
        Friend WithEvents progressBar As System.Windows.Forms.ProgressBar
        Friend WithEvents lblMessage As System.Windows.Forms.Label
    End Class
End Namespace
