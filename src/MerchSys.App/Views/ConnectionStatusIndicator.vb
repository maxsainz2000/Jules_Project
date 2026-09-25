Imports System.Windows.Forms
Imports System.Drawing

Namespace Views
    Public Partial Class ConnectionStatusIndicator
        Inherits UserControl

        Public Event RetryClicked As EventHandler

        Public Sub New()
            InitializeComponent()
        End Sub

        Public Sub SetState(statusText As String, indicatorColor As Color, isRetryVisible As Boolean)
            If Me.InvokeRequired Then
                Me.Invoke(Sub() SetState(statusText, indicatorColor, isRetryVisible))
                Return
            End If

            lblStatus.Text = statusText
            pnlIndicator.BackColor = indicatorColor
            btnRetry.Visible = isRetryVisible
        End Sub

        Private Sub btnRetry_Click(sender As Object, e As EventArgs) Handles btnRetry.Click
            RaiseEvent RetryClicked(Me, EventArgs.Empty)
        End Sub
    End Class
End Namespace
