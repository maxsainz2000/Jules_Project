Imports MerchSys.App.Views
Imports MerchSys.App.Presenters
Imports MerchSys.App.Views.Shell
Imports MerchSys.App.Models
Imports System.Windows.Forms

Public Class MainWindow
    Public Event LogoutRequested As EventHandler
    Public Event DashboardRequested As EventHandler
    Public Event ModuleSelected As EventHandler(Of AppModule)

    Private ReadOnly _activityRail As ActivityRail
    Private ReadOnly _moduleDetailPanel As ModuleDetailPanel

    Public Sub New(activityRail As ActivityRail, moduleDetailPanel As ModuleDetailPanel)
        InitializeComponent()
        
        Me.KeyPreview = True
        
        _activityRail = activityRail
        _moduleDetailPanel = moduleDetailPanel

        ' Set up layout
        Controls.Add(_moduleDetailPanel)
        Controls.Add(_activityRail)
        
        _activityRail.BringToFront()
        _moduleDetailPanel.BringToFront()
        pnlContent.BringToFront()
        
        ' Wire up events
        AddHandler _activityRail.ModuleSelected, AddressOf OnModuleSelected
        
        ' ModuleDetailPanel owns btnLogout now
        AddHandler _moduleDetailPanel.btnLogout.Click, AddressOf btnLogout_Click
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
        _activityRail.Presenter.SyncActiveModule(moduleId)
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

    Public Sub ShowView(view As UserControl)
        pnlContent.Controls.Clear()
        view.Dock = DockStyle.Fill
        pnlContent.Controls.Add(view)
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs)
        RaiseEvent LogoutRequested(Me, EventArgs.Empty)
    End Sub
End Class
