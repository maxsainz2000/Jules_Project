Public Class MainWindow
    Public Event LogoutRequested As EventHandler

    Private Sub btnLogout_Click(sender As Object, e As EventArgs) Handles btnLogout.Click
        RaiseEvent LogoutRequested(Me, EventArgs.Empty)
    End Sub
End Class
