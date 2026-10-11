Imports MerchSys.App.Presenters
Imports MerchSys.App.Models
Imports System.Windows.Forms
Imports System.Drawing

Namespace Views
    Public Class MainWindow
        Implements IMainWindowView

        Public Event LogoutRequested As EventHandler Implements IMainWindowView.LogoutRequested
        Public Event ModuleSelected As EventHandler(Of AppModule)
        Public Event NavigationRequested As EventHandler(Of NavigationItem) Implements IMainWindowView.NavigationRequested

        Private _renderedItems As List(Of NavigationItem)

        ''' <summary>
        ''' Explicit interface implementation of <see cref="IMainWindowView.Load"/>.
        ''' Uses the Custom Event pattern (as OwnerDashboardView does) instead of
        ''' shadowing the inherited WinForms <c>Form.Load</c> member (L7).
        ''' </summary>
        Public Shadows Custom Event Load As EventHandler Implements IMainWindowView.Load
            AddHandler(value As EventHandler)
                AddHandler MyBase.Load, value
            End AddHandler
            RemoveHandler(value As EventHandler)
                RemoveHandler MyBase.Load, value
            End RemoveHandler
            RaiseEvent(sender As Object, e As EventArgs)
                ' The base Form Load event is raised automatically.
            End RaiseEvent
        End Event

        Public Sub New()
            InitializeComponent()
            Me.KeyPreview = True
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

        ''' <summary>
        ''' Sidebar header text showing the signed-in user and role (verification Test 20).
        ''' Previously this wrote to the window title, which no test checks (L5).
        ''' </summary>
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property UserIdentityText As String Implements IMainWindowView.UserIdentityText
            Get
                Return lblUserIdentity.Text
            End Get
            Set(value As String)
                lblUserIdentity.Text = value
            End Set
        End Property

        ''' <summary>Hosts the connection status indicator at the bottom of the sidebar (H3).</summary>
        Public Sub AddStatusIndicator(indicator As Control) Implements IMainWindowView.AddStatusIndicator
            pnlStatusHost.Controls.Clear()
            indicator.Dock = DockStyle.Fill
            pnlStatusHost.Controls.Add(indicator)
        End Sub

        ''' <summary>Closes the shell window so the login flow can restart (H7).</summary>
        Public Sub CloseWindow() Implements IMainWindowView.CloseWindow
            Me.Close()
        End Sub

        Public Sub RenderView(view As UserControl) Implements IMainWindowView.RenderView
            pnlContent.Controls.Clear()
            view.Dock = DockStyle.Fill
            pnlContent.Controls.Add(view)
        End Sub

        Public Sub AddNavigationButtons(items As List(Of NavigationItem)) Implements IMainWindowView.AddNavigationButtons
            ' When the item set is unchanged (the common case while navigating), only
            ' refresh the active-state styling instead of disposing and recreating every
            ' button, which caused flicker and handler churn (L8).
            If _renderedItems IsNot Nothing AndAlso SameItems(_renderedItems, items) Then
                For Each ctrl As Control In pnlSidebar.Controls
                    Dim existing = TryCast(ctrl, Button)
                    Dim navItem = If(existing IsNot Nothing, TryCast(existing.Tag, NavigationItem), Nothing)
                    If existing IsNot Nothing AndAlso navItem IsNot Nothing Then
                        existing.BackColor = If(navItem.IsActive, Color.FromArgb(52, 73, 94), Color.Transparent)
                    End If
                Next
                _renderedItems = items
                Return
            End If

            For Each ctrl As Control In pnlSidebar.Controls
                ctrl.Dispose()
            Next
            pnlSidebar.Controls.Clear()

            Dim currentY As Integer = 10

            For Each item In items
                Dim btn As New Button()
                btn.Text = item.Name
                btn.Tag = item
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

            _renderedItems = items
        End Sub

        Private Shared Function SameItems(left As List(Of NavigationItem), right As List(Of NavigationItem)) As Boolean
            If left Is Nothing OrElse right Is Nothing OrElse left.Count <> right.Count Then Return False
            For index = 0 To left.Count - 1
                If Not Object.ReferenceEquals(left(index), right(index)) Then Return False
            Next
            Return True
        End Function

        Private Sub NavButton_Click(sender As Object, e As EventArgs)
            Dim btn = DirectCast(sender, Button)
            Dim item = TryCast(btn.Tag, NavigationItem)
            If item IsNot Nothing Then
                RaiseEvent NavigationRequested(Me, item)
            End If
        End Sub

        Private Sub btnLogout_Click(sender As Object, e As EventArgs)
            RaiseEvent LogoutRequested(Me, EventArgs.Empty)
        End Sub
    End Class
End Namespace
