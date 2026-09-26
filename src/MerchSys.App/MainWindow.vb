Imports MerchSys.App.Views
Imports MerchSys.App.Presenters
Imports MerchSys.App.Models
Imports System.Windows.Forms

Public Class MainWindow
    Implements IMainWindowView
    Public Event LogoutRequested As EventHandler
    Public Event DashboardRequested As EventHandler
    Public Event ModuleSelected As EventHandler(Of AppModule)
    Public Event NavigationRequested As EventHandler(Of NavigationItem) Implements IMainWindowView.NavigationRequested

    Public Sub New()
        InitializeComponent()
        Me.KeyPreview = True
        pnlSidebar.BringToFront()
        pnlContent.BringToFront()
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If keyData = (Keys.Control Or Keys.D1) Then
            RaiseEvent ModuleSelected(Me, AppModule.Purchasing)
            Return True
        ElseIf keyData = (Keys.Control Or Keys.D2) Then
            RaiseEvent ModuleSelected(Me, AppModule.Inventory)
            Return True
        ElseIf keyData = (Keys.Control Or Keys.D3) Then
            RaiseEvent ModuleSelected(Me, AppModule.POS)
            Return True
        ElseIf keyData = (Keys.Control Or Keys.D4) Then
            RaiseEvent ModuleSelected(Me, AppModule.Accounting)
            Return True
        ElseIf keyData = (Keys.Control Or Keys.D0) Then
#If DEBUG Then
            RaiseEvent ModuleSelected(Me, AppModule.DeveloperTools)
            Return True
#End If
        End If
        
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub OnModuleSelected(sender As Object, moduleId As AppModule)
        RaiseEvent ModuleSelected(Me, moduleId)
    End Sub

    Public Sub SyncActiveModuleVisuals(moduleId As AppModule)
        ' Left here to satisfy presenter dependency
    End Sub

    <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
    Public Property UserIdentityText As String
        Get
            Return lblUserIdentity.Text
        End Get
        Set(value As String)
            lblUserIdentity.Text = value
        End Set
    End Property

    <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
    Public Property DashboardButtonVisible As Boolean
        Get
            Return False ' Deprecated in this UI layout but required to compile based on previous usage
        End Get
        Set(value As Boolean)
            ' Deprecated
        End Set
    End Property

    Public Sub ShowView(view As UserControl) Implements IMainWindowView.ShowView
        pnlContent.Controls.Clear()
        view.Dock = DockStyle.Fill
        pnlContent.Controls.Add(view)
    End Sub

    Public Sub RenderNavigation(groups As Dictionary(Of String, List(Of NavigationItem))) Implements IMainWindowView.RenderNavigation
        pnlSidebar.Controls.Clear()
        Dim topPosition As Integer = 10
        For Each kvp In groups
            Dim groupLabel As New Label()
            groupLabel.Text = kvp.Key
            groupLabel.ForeColor = System.Drawing.Color.LightGray
            groupLabel.Font = New System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
            groupLabel.Location = New System.Drawing.Point(10, topPosition)
            groupLabel.AutoSize = True
            pnlSidebar.Controls.Add(groupLabel)
            topPosition += 30

            For Each item In kvp.Value
                Dim btn As New Button()
                btn.Text = item.Name
                btn.Tag = item
                btn.FlatStyle = FlatStyle.Flat
                btn.FlatAppearance.BorderSize = 0
                btn.ForeColor = System.Drawing.Color.White
                btn.Font = New System.Drawing.Font("Segoe UI", 9)
                btn.Location = New System.Drawing.Point(10, topPosition)
                btn.Size = New System.Drawing.Size(180, 30)
                btn.Cursor = Cursors.Hand
                btn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
                AddHandler btn.Click, AddressOf NavButton_Click
                pnlSidebar.Controls.Add(btn)
                topPosition += 35
            Next
            topPosition += 10
        Next

        Dim btnLogout As New Button()
        btnLogout.Text = "Logout"
        btnLogout.FlatStyle = FlatStyle.Flat
        btnLogout.FlatAppearance.BorderSize = 0
        btnLogout.ForeColor = System.Drawing.Color.White
        btnLogout.Font = New System.Drawing.Font("Segoe UI", 9)
        btnLogout.Dock = DockStyle.Bottom
        btnLogout.Height = 40
        btnLogout.Cursor = Cursors.Hand
        AddHandler btnLogout.Click, AddressOf btnLogout_Click
        pnlSidebar.Controls.Add(btnLogout)
    End Sub

    Private Sub NavButton_Click(sender As Object, e As EventArgs)
        Dim btn = DirectCast(sender, Button)
        Dim item = DirectCast(btn.Tag, NavigationItem)
        RaiseEvent NavigationRequested(Me, item)
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs)
        RaiseEvent LogoutRequested(Me, EventArgs.Empty)
    End Sub
End Class
