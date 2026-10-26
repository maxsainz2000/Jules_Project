Imports System.Windows.Forms

Namespace Services
    Public Class ValidationHelper
        Private ReadOnly _errorProvider As ErrorProvider

        Public Sub New()
            _errorProvider = New ErrorProvider()
            _errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink
        End Sub

        Public Sub SetError(control As Control, errorMessage As String)
            _errorProvider.SetError(control, errorMessage)
        End Sub

        Public Sub ClearError(control As Control)
            _errorProvider.SetError(control, String.Empty)
        End Sub

        Public Sub ClearAll()
            _errorProvider.Clear()
        End Sub
    End Class
End Namespace
