Imports System.Windows.Forms

Namespace Views
    Public Class StockDashboardView
        Inherits UserControl

        Public Sub New()
            Dim lbl As New Label()
            lbl.Text = "Stock Dashboard"
            lbl.Dock = DockStyle.Fill
            lbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            Me.Controls.Add(lbl)
            Me.BackColor = System.Drawing.Color.White
        End Sub
    End Class
End Namespace