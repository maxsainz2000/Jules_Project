Imports System.Windows.Forms
Imports MerchSys.App.Services.Theming
Imports System.ComponentModel

Namespace Controls
    Public Class EmptyStatePanel
        Inherits UserControl

        Private _title As String = "No items found"
        Private _description As String = "There are no records matching the selected criteria."
        Private _actionText As String = ""

        Public Event ActionClicked As EventHandler

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
        Public Property Description As String
            Get
                Return _description
            End Get
            Set(value As String)
                _description = value
                lblDescription.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ActionText As String
            Get
                Return _actionText
            End Get
            Set(value As String)
                _actionText = value
                btnAction.Text = value
                btnAction.Visible = Not String.IsNullOrEmpty(value)
            End Set
        End Property

        Public Sub New()
            InitializeComponent()
            lblTitle.Text = _title
            lblDescription.Text = _description
            btnAction.Text = _actionText
            btnAction.Visible = Not String.IsNullOrEmpty(_actionText)

            AddHandler btnAction.Click, Sub(sender, e) RaiseEvent ActionClicked(Me, EventArgs.Empty)
        End Sub

        Public Sub ApplyTheme(tokens As ThemeTokens)
            If tokens IsNot Nothing Then
                Me.BackColor = tokens.SurfaceBackground
                lblTitle.ForeColor = tokens.PrimaryText
                lblDescription.ForeColor = tokens.SecondaryText
            End If
        End Sub

    End Class
End Namespace
