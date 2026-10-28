Imports System.Windows.Forms
Imports MerchSys.App.Services.Theming
Imports System.ComponentModel

Namespace Controls
    Public Class ErrorStatePanel
        Inherits UserControl

        Private _title As String = "An error occurred"
        Private _message As String = ""
        Private _retryText As String = "Retry"

        Public Event RetryClicked As EventHandler

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property Title As String
            Get
                Return _title
            End Get
            Set(value As String)
                _title = value
                lblTitle.Text = value
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

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property RetryText As String
            Get
                Return _retryText
            End Get
            Set(value As String)
                _retryText = value
                btnRetry.Text = value
            End Set
        End Property

        Public Sub New()
            InitializeComponent()
            lblTitle.Text = _title
            lblMessage.Text = _message
            btnRetry.Text = _retryText

            AddHandler btnRetry.Click, Sub(sender, e) RaiseEvent RetryClicked(Me, EventArgs.Empty)
        End Sub

        Public Sub ApplyTheme(tokens As ThemeTokens)
            If tokens IsNot Nothing Then
                Me.BackColor = tokens.SurfaceBackground
                lblTitle.ForeColor = tokens.Danger
                lblMessage.ForeColor = tokens.SecondaryText
            End If
        End Sub
    End Class
End Namespace
