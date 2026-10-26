Namespace Controls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class EmptyStatePanel
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
            Me.lblDescription = New System.Windows.Forms.Label()
            Me.btnAction = New System.Windows.Forms.Button()
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
            Me.lblTitle.Text = "No items found"
            Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            Me.lblTitle.Tag = "SectionHeader"
            '
            'lblDescription
            '
            Me.lblDescription.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblDescription.Location = New System.Drawing.Point(20, 50)
            Me.lblDescription.Name = "lblDescription"
            Me.lblDescription.Size = New System.Drawing.Size(260, 50)
            Me.lblDescription.TabIndex = 1
            Me.lblDescription.Text = "There are no records matching the selected criteria."
            Me.lblDescription.TextAlign = System.Drawing.ContentAlignment.TopCenter
            Me.lblDescription.Tag = "Subtitle"
            '
            'btnAction
            '
            Me.btnAction.Anchor = System.Windows.Forms.AnchorStyles.Bottom
            Me.btnAction.Location = New System.Drawing.Point(100, 110)
            Me.btnAction.Name = "btnAction"
            Me.btnAction.Size = New System.Drawing.Size(100, 30)
            Me.btnAction.TabIndex = 2
            Me.btnAction.Tag = "Primary"
            Me.btnAction.Text = "Action"
            Me.btnAction.UseVisualStyleBackColor = True
            Me.btnAction.Visible = False
            '
            'EmptyStatePanel
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.btnAction)
            Me.Controls.Add(Me.lblDescription)
            Me.Controls.Add(Me.lblTitle)
            Me.Name = "EmptyStatePanel"
            Me.Size = New System.Drawing.Size(300, 160)
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents lblTitle As System.Windows.Forms.Label
        Friend WithEvents lblDescription As System.Windows.Forms.Label
        Friend WithEvents btnAction As System.Windows.Forms.Button
    End Class
End Namespace
