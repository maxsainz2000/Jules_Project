Imports System.Windows.Forms
Imports System.Drawing

Namespace Views
    Public Partial Class ConnectionStatusIndicator
        Inherits UserControl
        Implements IConnectionStatusView

        Public Event RetryClicked As EventHandler Implements IConnectionStatusView.RetryClicked

        Public Sub New()
            InitializeComponent()
        End Sub

        Public ReadOnly Property Control As Control Implements IConnectionStatusView.Control
            Get
                Return Me
            End Get
        End Property

        Public Sub SetState(statusText As String, indicatorColor As Color, isRetryVisible As Boolean, foreColor As Color) Implements IConnectionStatusView.SetState
            If Me.InvokeRequired Then
                Me.Invoke(Sub() SetState(statusText, indicatorColor, isRetryVisible, foreColor))
                Return
            End If

            lblStatus.Text = statusText
            lblStatus.ForeColor = foreColor
            pnlIndicator.BackColor = indicatorColor
            btnRetry.Visible = isRetryVisible
        End Sub

        Private Sub btnRetry_Click(sender As Object, e As EventArgs) Handles btnRetry.Click
            RaiseEvent RetryClicked(Me, EventArgs.Empty)
        End Sub
    End Class
End Namespace
