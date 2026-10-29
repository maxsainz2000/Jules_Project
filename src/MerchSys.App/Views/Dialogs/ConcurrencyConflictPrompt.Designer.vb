Namespace Views.Dialogs
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class ConcurrencyConflictPrompt
        Inherits System.Windows.Forms.Form

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
            Me.lblMessage = New System.Windows.Forms.Label()
            Me.btnRefresh = New System.Windows.Forms.Button()
            Me.btnCancel = New System.Windows.Forms.Button()
            Me.SuspendLayout()
            '
            'lblMessage
            '
            Me.lblMessage.AutoSize = False
            Me.lblMessage.Location = New System.Drawing.Point(20, 20)
            Me.lblMessage.Name = "lblMessage"
            Me.lblMessage.Size = New System.Drawing.Size(360, 40)
            Me.lblMessage.TabIndex = 0
            Me.lblMessage.Text = "Data changed elsewhere — refresh and retry"
            Me.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'btnRefresh
            '
            Me.btnRefresh.DialogResult = System.Windows.Forms.DialogResult.Retry
            Me.btnRefresh.Location = New System.Drawing.Point(100, 80)
            Me.btnRefresh.Name = "btnRefresh"
            Me.btnRefresh.Size = New System.Drawing.Size(90, 30)
            Me.btnRefresh.TabIndex = 1
            Me.btnRefresh.Tag = "Accent"
            Me.btnRefresh.Text = "&Refresh"
            Me.btnRefresh.UseVisualStyleBackColor = True
            '
            'btnCancel
            '
            Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.btnCancel.Location = New System.Drawing.Point(210, 80)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(90, 30)
            Me.btnCancel.TabIndex = 2
            Me.btnCancel.Text = "&Cancel"
            Me.btnCancel.UseVisualStyleBackColor = True
            '
            'ConcurrencyConflictPrompt
            '
            Me.AcceptButton = Me.btnRefresh
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.btnCancel
            Me.ClientSize = New System.Drawing.Size(400, 130)
            Me.Controls.Add(Me.btnCancel)
            Me.Controls.Add(Me.btnRefresh)
            Me.Controls.Add(Me.lblMessage)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "ConcurrencyConflictPrompt"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Concurrency Conflict"
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents lblMessage As System.Windows.Forms.Label
        Friend WithEvents btnRefresh As System.Windows.Forms.Button
        Friend WithEvents btnCancel As System.Windows.Forms.Button
    End Class
End Namespace
