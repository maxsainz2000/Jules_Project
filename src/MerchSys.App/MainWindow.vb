Public Class MainWindow
    Public Event LogoutRequested As EventHandler
    Public Event DashboardRequested As EventHandler

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
            Return btnDashboard.Visible
        End Get
        Set(value As Boolean)
            btnDashboard.Visible = value
        End Set
    End Property

    Public Sub ShowView(view As UserControl)
        pnlContent.Controls.Clear()
        view.Dock = DockStyle.Fill
        pnlContent.Controls.Add(view)
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs) Handles btnLogout.Click
        RaiseEvent LogoutRequested(Me, EventArgs.Empty)
    End Sub

    Private Sub btnDashboard_Click(sender As Object, e As EventArgs) Handles btnDashboard.Click
        RaiseEvent DashboardRequested(Me, EventArgs.Empty)
    End Sub
End Class
