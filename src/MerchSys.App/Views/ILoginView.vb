Namespace Views
    Public Interface ILoginView
        Property Username As String
        Property Password As String
        Property NewPassword As String
        Property ConfirmNewPassword As String
        Property ErrorMessage As String

        Sub ShowError(message As String)
        Sub ClearError()
        Sub ShowFirstLoginPanel()
        Sub EnableControls(enabled As Boolean)
        Sub CloseWithSuccess()

        Event LoginClicked As EventHandler
        Event ChangePasswordClicked As EventHandler
    End Interface
End Namespace
