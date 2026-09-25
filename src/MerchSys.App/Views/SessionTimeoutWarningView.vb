Imports System
Imports System.Windows.Forms

Namespace Views
    Public Partial Class SessionTimeoutWarningView
        Inherits Form
        Implements ISessionTimeoutWarningView

        Private _decisionMade As Boolean = False

        Public Event StaySignedInRequested As EventHandler Implements ISessionTimeoutWarningView.StaySignedInRequested
        Public Event SignOutRequested As EventHandler Implements ISessionTimeoutWarningView.SignOutRequested

        Public Sub New()
            InitializeComponent()
            Me.TopMost = True
            Me.StartPosition = FormStartPosition.CenterParent
            Me.BackColor = System.Drawing.Color.FromArgb(45, 45, 48)
            Me.ForeColor = System.Drawing.Color.White
        End Sub

        Public Sub SetCountdown(formattedTime As String) Implements ISessionTimeoutWarningView.SetCountdown
            lblCountdown.Text = formattedTime
        End Sub

        Public Sub CloseView() Implements ISessionTimeoutWarningView.CloseView
            If Not Me.IsDisposed AndAlso Not Me.Disposing Then
                Me.DialogResult = DialogResult.OK
                Me.Close()
            End If
        End Sub

        Public Shadows Function ShowDialog() As DialogResult Implements ISessionTimeoutWarningView.ShowDialog
            Return MyBase.ShowDialog()
        End Function

        Public Shadows Sub Show(owner As IWin32Window) Implements ISessionTimeoutWarningView.Show
            MyBase.Show(owner)
        End Sub

        Public Sub MarkDecisionMade()
            _decisionMade = True
        End Sub

        Private Sub btnStaySignedIn_Click(sender As Object, e As EventArgs) Handles btnStaySignedIn.Click
            MarkDecisionMade()
            ' Fire event. Coordinator handles scope disposal.
            ' Do NOT call CloseView() here directly to prevent ObjectDisposedException
            ' because the coordinator will dispose the DI scope, disposing the Form.
            RaiseEvent StaySignedInRequested(Me, EventArgs.Empty)
        End Sub

        Private Sub btnSignOut_Click(sender As Object, e As EventArgs) Handles btnSignOut.Click
            MarkDecisionMade()
            RaiseEvent SignOutRequested(Me, EventArgs.Empty)
        End Sub

        Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
            If Not _decisionMade AndAlso e.CloseReason = CloseReason.UserClosing Then
                MarkDecisionMade()
                RaiseEvent SignOutRequested(Me, EventArgs.Empty)
            End If
            MyBase.OnFormClosing(e)
        End Sub
    End Class
End Namespace
