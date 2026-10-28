Namespace Controls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class ErrorStatePanel
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
            Me.lblTitle = New System.Windows.Forms.Label()
            Me.lblMessage = New System.Windows.Forms.Label()
            Me.btnRetry = New System.Windows.Forms.Button()
            Me.SuspendLayout()
            '
            'lblTitle
            '
            Me.lblTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblTitle.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblTitle.Location = New System.Drawing.Point(20, 20)
            Me.lblTitle.Name = "lblTitle"
            Me.lblTitle.Size = New System.Drawing.Size(260, 30)
            Me.lblTitle.TabIndex = 0
            Me.lblTitle.Text = "Unable to load data"
            Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            Me.lblTitle.Tag = "DangerHeader"
            '
            'lblMessage
            '
            Me.lblMessage.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblMessage.Location = New System.Drawing.Point(20, 50)
            Me.lblMessage.Name = "lblMessage"
            Me.lblMessage.Size = New System.Drawing.Size(260, 50)
            Me.lblMessage.TabIndex = 1
            Me.lblMessage.Text = "An error occurred while communicating with the database. Please check your connection and try again."
            Me.lblMessage.TextAlign = System.Drawing.ContentAlignment.TopCenter
            Me.lblMessage.Tag = "Subtitle"
            '
            'btnRetry
            '
            Me.btnRetry.Anchor = System.Windows.Forms.AnchorStyles.Bottom
            Me.btnRetry.Location = New System.Drawing.Point(100, 110)
            Me.btnRetry.Name = "btnRetry"
            Me.btnRetry.Size = New System.Drawing.Size(100, 30)
            Me.btnRetry.TabIndex = 2
            Me.btnRetry.Tag = "Primary"
            Me.btnRetry.Text = "Retry"
            Me.btnRetry.UseVisualStyleBackColor = True
            Me.btnRetry.Visible = True
            '
            'ErrorStatePanel
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.btnRetry)
            Me.Controls.Add(Me.lblMessage)
            Me.Controls.Add(Me.lblTitle)
            Me.Name = "ErrorStatePanel"
            Me.Size = New System.Drawing.Size(300, 160)
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents lblTitle As System.Windows.Forms.Label
        Friend WithEvents lblMessage As System.Windows.Forms.Label
        Friend WithEvents btnRetry As System.Windows.Forms.Button
    End Class
End Namespace
