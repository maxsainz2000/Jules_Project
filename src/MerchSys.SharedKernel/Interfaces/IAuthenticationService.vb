Imports MerchSys.SharedKernel.Entities

Namespace Interfaces

    Public Class AuthenticationResult
        Public Property Success As Boolean
        Public Property ErrorMessage As String
        Public Property User As UserAccount
        Public Property RequiresPasswordChange As Boolean
    End Class

    Public Class PasswordChangeResult
        Public Property Success As Boolean
        Public Property ErrorMessage As String
    End Class

    Public Interface IAuthenticationService
        Function AuthenticateAsync(username As String, password As String) As Task(Of AuthenticationResult)
        Function ChangePasswordAsync(userId As Integer, newPassword As String) As Task(Of PasswordChangeResult)
        Function ChangePasswordForFirstLoginAsync(username As String, currentPassword As String, newPassword As String) As Task(Of PasswordChangeResult)
    End Interface

End Namespace
