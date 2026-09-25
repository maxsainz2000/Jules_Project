Imports System.Windows.Forms
Imports System.Drawing
Imports System.ComponentModel
Imports MerchSys.App.Presenters.Shell
Imports MerchSys.App.Models

Namespace Views.Shell
    Public Class ActivityRail
        Inherits UserControl

        Public ReadOnly Presenter As ActivityRailPresenter
        Public Event ModuleSelected As EventHandler(Of AppModule)

        Public Sub New(presenter As ActivityRailPresenter)
            InitializeComponent()
            Me.Presenter = presenter
            Me.Presenter.View = Me
            
            Dock = DockStyle.Left
            Width = 60
            BackColor = Color.FromArgb(36, 51, 66) ' #243342

            RenderIcons()
        End Sub

        Private Sub RenderIcons()
            pnlIcons.Controls.Clear()
            For Each item In Presenter.Items
                Dim btn As New Button()
                btn.Text = item.Abbreviation
                btn.Tag = item
                btn.Width = 60
                btn.Height = 60
                btn.FlatStyle = FlatStyle.Flat
                btn.FlatAppearance.BorderSize = 0
                btn.ForeColor = Color.White
                btn.Font = New Font("Segoe UI", 10, FontStyle.Bold)
                btn.Cursor = Cursors.Hand
                btn.Margin = New Padding(0)
                
                AddHandler btn.Click, AddressOf Icon_Click
                AddHandler item.PropertyChanged, AddressOf OnItemPropertyChanged
                
                UpdateButtonVisuals(btn, item)
                pnlIcons.Controls.Add(btn)
            Next
        End Sub

        Private Sub Icon_Click(sender As Object, e As EventArgs)
            Dim btn = DirectCast(sender, Button)
            Dim item = DirectCast(btn.Tag, RailItem)
            RaiseEvent ModuleSelected(Me, item.ModuleId)
        End Sub

        Private Sub OnItemPropertyChanged(sender As Object, e As PropertyChangedEventArgs)
            If e.PropertyName = NameOf(RailItem.IsActive) Then
                Dim item = DirectCast(sender, RailItem)
                For Each ctrl As Control In pnlIcons.Controls
                    If TypeOf ctrl Is Button AndAlso ctrl.Tag Is item Then
                        UpdateButtonVisuals(DirectCast(ctrl, Button), item)
                    End If
                Next
            End If
        End Sub

        Private Sub UpdateButtonVisuals(btn As Button, item As RailItem)
            If item.IsActive Then
                btn.BackColor = Color.FromArgb(45, 45, 48)
                ' We simulate the left accent bar with a Paint event on the button if active
                AddHandler btn.Paint, AddressOf DrawActiveAccent
            Else
                btn.BackColor = Color.Transparent
                RemoveHandler btn.Paint, AddressOf DrawActiveAccent
            End If
            btn.Invalidate()
        End Sub

        Private Sub DrawActiveAccent(sender As Object, e As PaintEventArgs)
            Dim btn = DirectCast(sender, Button)
            Using brush As New SolidBrush(Color.FromArgb(41, 128, 185)) ' #2980B9
                e.Graphics.FillRectangle(brush, 0, 0, 3, btn.Height)
            End Using
        End Sub
    End Class
End Namespace
