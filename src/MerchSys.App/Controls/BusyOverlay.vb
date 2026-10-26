Imports System.Windows.Forms
Imports MerchSys.App.Services.Theming
Imports System.ComponentModel

Namespace Controls
    Public Class BusyOverlay
        Inherits UserControl

        Private _isBusy As Boolean = False
        Private _message As String = "Loading..."

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property IsBusy As Boolean
            Get
                Return _isBusy
            End Get
            Set(value As Boolean)
                If _isBusy <> value Then
                    _isBusy = value
                    UpdateState()
                End If
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property Message As String
            Get
                Return _message
            End Get
            Set(value As String)
                _message = value
                lblMessage.Text = value
            End Set
        End Property

        Public Sub New()
            InitializeComponent()
            lblMessage.Text = _message
            UpdateState()
        End Sub

        Private Sub UpdateState()
            If _isBusy Then
                Me.Visible = True
                Me.BringToFront()
                progressBar.Style = ProgressBarStyle.Marquee
            Else
                Me.Visible = False
                progressBar.Style = ProgressBarStyle.Blocks
            End If
        End Sub

        Public Sub ApplyTheme(tokens As ThemeTokens)
            If tokens IsNot Nothing Then
                Me.BackColor = System.Drawing.Color.FromArgb(200, tokens.SurfaceBackground.R, tokens.SurfaceBackground.G, tokens.SurfaceBackground.B)
                panelContent.BackColor = tokens.CardBackground
                lblMessage.ForeColor = tokens.PrimaryText
            End If
        End Sub

    End Class
End Namespace
