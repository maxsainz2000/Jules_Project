Imports MerchSys.App.Presenters
Imports MerchSys.App.Models
Imports System.Windows.Forms
Imports System.Drawing

Namespace Views
    Public Class MainWindow
        Implements IMainWindowView

        Public Event LogoutRequested As EventHandler Implements IMainWindowView.LogoutRequested
        Public Event DashboardRequested As EventHandler
        Public Event ModuleSelected As EventHandler(Of AppModule)
        Public Event NavigationRequested As EventHandler(Of Type) Implements IMainWindowView.NavigationRequested
        Public Shadows Event Load As EventHandler Implements IMainWindowView.Load

        Public Sub New()
            InitializeComponent()
            Me.KeyPreview = True

            AddHandler MyBase.Load, Sub(sender, e) RaiseEvent Load(sender, e)
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

        Public Sub SyncActiveModuleVisuals(moduleId As AppModule)
            ' No longer applicable in new layout
        End Sub

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property UserIdentityText As String
            Get
                Return Me.Text
            End Get
            Set(value As String)
                Me.Text = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property DashboardButtonVisible As Boolean
            Get
                Return False ' Deprecated
            End Get
            Set(value As Boolean)
                ' Deprecated
            End Set
        End Property

        Public Sub RenderView(view As UserControl) Implements IMainWindowView.RenderView
            pnlContent.Controls.Clear()
            view.Dock = DockStyle.Fill
            pnlContent.Controls.Add(view)
        End Sub

        Public Sub AddNavigationButtons(items As List(Of NavigationItem)) Implements IMainWindowView.AddNavigationButtons
            For Each ctrl As Control In pnlSidebar.Controls
                ctrl.Dispose()
            Next
            pnlSidebar.Controls.Clear()

            Dim currentY As Integer = 10

            For Each item In items
                Dim btn As New Button()
                btn.Text = item.Name
                btn.Tag = item.ViewType
                btn.Width = pnlSidebar.Width - 20
                btn.Height = 40
                btn.Location = New Point(10, currentY)
                btn.FlatStyle = FlatStyle.Flat
                btn.ForeColor = Color.White
                btn.BackColor = If(item.IsActive, Color.FromArgb(52, 73, 94), Color.Transparent)
                btn.FlatAppearance.BorderSize = 0
                btn.TextAlign = ContentAlignment.MiddleLeft
                btn.Padding = New Padding(10, 0, 0, 0)

                AddHandler btn.Click, AddressOf NavButton_Click

                pnlSidebar.Controls.Add(btn)
                currentY += 45
            Next

            ' Add a logout button at the bottom
            Dim btnLogout As New Button()
            btnLogout.Text = "Logout"
            btnLogout.Width = pnlSidebar.Width - 20
            btnLogout.Height = 40
            btnLogout.Location = New Point(10, Math.Max(currentY, pnlSidebar.Height - 50))
            btnLogout.FlatStyle = FlatStyle.Flat
            btnLogout.ForeColor = Color.White
            btnLogout.BackColor = Color.FromArgb(192, 57, 43)
            btnLogout.FlatAppearance.BorderSize = 0
            btnLogout.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right

            AddHandler btnLogout.Click, AddressOf btnLogout_Click

            pnlSidebar.Controls.Add(btnLogout)
        End Sub

        Private Sub NavButton_Click(sender As Object, e As EventArgs)
            Dim btn = DirectCast(sender, Button)
            Dim viewType = TryCast(btn.Tag, Type)
            If viewType IsNot Nothing Then
                RaiseEvent NavigationRequested(Me, viewType)
            End If
        End Sub

        Private Sub btnLogout_Click(sender As Object, e As EventArgs)
            RaiseEvent LogoutRequested(Me, EventArgs.Empty)
        End Sub
    End Class
End Namespace
