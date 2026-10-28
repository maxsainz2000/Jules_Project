Namespace Views.POS
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class VatSettingsView
        Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
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
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.chkIsVatRegistered = New System.Windows.Forms.CheckBox()
        Me.txtTin = New System.Windows.Forms.TextBox()
        Me.txtBusinessName = New System.Windows.Forms.TextBox()
        Me.txtBusinessAddress = New System.Windows.Forms.TextBox()
        Me.nudVatRate = New System.Windows.Forms.NumericUpDown()
        Me.nudNonVatRate = New System.Windows.Forms.NumericUpDown()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnReload = New System.Windows.Forms.Button()
        Me.lblTin = New System.Windows.Forms.Label()
        Me.lblVatRate = New System.Windows.Forms.Label()
        Me.lblNonVatRate = New System.Windows.Forms.Label()
        Me.lblBusinessName = New System.Windows.Forms.Label()
        Me.lblBusinessAddress = New System.Windows.Forms.Label()
        CType(Me.nudVatRate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudNonVatRate, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'chkIsVatRegistered
        '
        Me.chkIsVatRegistered.AutoSize = True
        Me.chkIsVatRegistered.Location = New System.Drawing.Point(30, 30)
        Me.chkIsVatRegistered.Name = "chkIsVatRegistered"
        Me.chkIsVatRegistered.Size = New System.Drawing.Size(126, 19)
        Me.chkIsVatRegistered.TabIndex = 0
        Me.chkIsVatRegistered.Text = "Is VAT Registered?"
        Me.chkIsVatRegistered.UseVisualStyleBackColor = True
        '
        'lblTin
        '
        Me.lblTin.AutoSize = True
        Me.lblTin.Location = New System.Drawing.Point(30, 70)
        Me.lblTin.Name = "lblTin"
        Me.lblTin.Size = New System.Drawing.Size(28, 15)
        Me.lblTin.TabIndex = 1
        Me.lblTin.Text = "TIN:"
        '
        'txtTin
        '
        Me.txtTin.Location = New System.Drawing.Point(150, 67)
        Me.txtTin.Name = "txtTin"
        Me.txtTin.Size = New System.Drawing.Size(200, 23)
        Me.txtTin.TabIndex = 2
        '
        'lblVatRate
        '
        Me.lblVatRate.AutoSize = True
        Me.lblVatRate.Location = New System.Drawing.Point(30, 100)
        Me.lblVatRate.Name = "lblVatRate"
        Me.lblVatRate.Size = New System.Drawing.Size(89, 15)
        Me.lblVatRate.TabIndex = 3
        Me.lblVatRate.Text = "VAT Rate (%):"
        '
        'nudVatRate
        '
        Me.nudVatRate.DecimalPlaces = 2
        Me.nudVatRate.Location = New System.Drawing.Point(150, 98)
        Me.nudVatRate.Name = "nudVatRate"
        Me.nudVatRate.Size = New System.Drawing.Size(120, 23)
        Me.nudVatRate.TabIndex = 4
        '
        'lblNonVatRate
        '
        Me.lblNonVatRate.AutoSize = True
        Me.lblNonVatRate.Location = New System.Drawing.Point(30, 130)
        Me.lblNonVatRate.Name = "lblNonVatRate"
        Me.lblNonVatRate.Size = New System.Drawing.Size(115, 15)
        Me.lblNonVatRate.TabIndex = 5
        Me.lblNonVatRate.Text = "Non-VAT Rate (%):"
        '
        'nudNonVatRate
        '
        Me.nudNonVatRate.DecimalPlaces = 2
        Me.nudNonVatRate.Location = New System.Drawing.Point(150, 128)
        Me.nudNonVatRate.Name = "nudNonVatRate"
        Me.nudNonVatRate.Size = New System.Drawing.Size(120, 23)
        Me.nudNonVatRate.TabIndex = 6
        '
        'lblBusinessName
        '
        Me.lblBusinessName.AutoSize = True
        Me.lblBusinessName.Location = New System.Drawing.Point(30, 160)
        Me.lblBusinessName.Name = "lblBusinessName"
        Me.lblBusinessName.Size = New System.Drawing.Size(94, 15)
        Me.lblBusinessName.TabIndex = 7
        Me.lblBusinessName.Text = "Business Name:"
        '
        'txtBusinessName
        '
        Me.txtBusinessName.Location = New System.Drawing.Point(150, 157)
        Me.txtBusinessName.Name = "txtBusinessName"
        Me.txtBusinessName.Size = New System.Drawing.Size(200, 23)
        Me.txtBusinessName.TabIndex = 8
        '
        'lblBusinessAddress
        '
        Me.lblBusinessAddress.AutoSize = True
        Me.lblBusinessAddress.Location = New System.Drawing.Point(30, 190)
        Me.lblBusinessAddress.Name = "lblBusinessAddress"
        Me.lblBusinessAddress.Size = New System.Drawing.Size(103, 15)
        Me.lblBusinessAddress.TabIndex = 9
        Me.lblBusinessAddress.Text = "Business Address:"
        '
        'txtBusinessAddress
        '
        Me.txtBusinessAddress.Location = New System.Drawing.Point(150, 187)
        Me.txtBusinessAddress.Name = "txtBusinessAddress"
        Me.txtBusinessAddress.Size = New System.Drawing.Size(200, 23)
        Me.txtBusinessAddress.TabIndex = 10
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(150, 230)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(75, 23)
        Me.btnSave.TabIndex = 11
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnReload
        '
        Me.btnReload.Location = New System.Drawing.Point(231, 230)
        Me.btnReload.Name = "btnReload"
        Me.btnReload.Size = New System.Drawing.Size(75, 23)
        Me.btnReload.TabIndex = 12
        Me.btnReload.Text = "Reload"
        Me.btnReload.UseVisualStyleBackColor = True
        '
        'lblError
        '
        '
        'VatSettingsView
        Me.AutoScroll = True
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        '
        'pnlErrorState
        '
        Me.pnlErrorState.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlErrorState.Location = New System.Drawing.Point(0, 0)
        Me.pnlErrorState.Name = "pnlErrorState"
        Me.pnlErrorState.TabIndex = 26
        Me.pnlErrorState.Visible = False

        Me.Controls.Add(Me.pnlErrorState)
        Me.Controls.Add(Me.btnReload)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.txtBusinessAddress)
        Me.Controls.Add(Me.lblBusinessAddress)
        Me.Controls.Add(Me.txtBusinessName)
        Me.Controls.Add(Me.lblBusinessName)
        Me.Controls.Add(Me.nudNonVatRate)
        Me.Controls.Add(Me.lblNonVatRate)
        Me.Controls.Add(Me.nudVatRate)
        Me.Controls.Add(Me.lblVatRate)
        Me.Controls.Add(Me.txtTin)
        Me.Controls.Add(Me.lblTin)
        Me.Controls.Add(Me.chkIsVatRegistered)
        Me.Name = "VatSettingsView"
        Me.Size = New System.Drawing.Size(400, 310)
        CType(Me.nudVatRate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudNonVatRate, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents chkIsVatRegistered As System.Windows.Forms.CheckBox
    Friend WithEvents lblTin As System.Windows.Forms.Label
    Friend WithEvents txtTin As System.Windows.Forms.TextBox
    Friend WithEvents lblVatRate As System.Windows.Forms.Label
    Friend WithEvents nudVatRate As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblNonVatRate As System.Windows.Forms.Label
    Friend WithEvents nudNonVatRate As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblBusinessName As System.Windows.Forms.Label
    Friend WithEvents txtBusinessName As System.Windows.Forms.TextBox
    Friend WithEvents lblBusinessAddress As System.Windows.Forms.Label
    Friend WithEvents txtBusinessAddress As System.Windows.Forms.TextBox
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnReload As System.Windows.Forms.Button
    Friend WithEvents pnlErrorState As MerchSys.App.Controls.ErrorStatePanel
End Class
End Namespace
