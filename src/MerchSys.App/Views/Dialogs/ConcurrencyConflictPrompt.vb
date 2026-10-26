Imports System.Windows.Forms

Namespace Views.Dialogs
    Public Class ConcurrencyConflictPrompt
        Inherits Form

        Public Sub New(entityName As String)
            InitializeComponent()
            lblMessage.Text = $"Data changed elsewhere — refresh and retry ({entityName})"
        End Sub

    End Class
End Namespace
