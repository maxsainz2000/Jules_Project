Imports MerchSys.SharedKernel.Entities
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Interfaces

Namespace Services
    Public Class LoginSessionService
        Implements ISessionService

        Private _currentUserRole As UserRole?
        Private _currentUsername As String
        Private _currentUserId As Integer?

        Public Sub SetUser(user As UserAccount)
            If user Is Nothing Then
                ClearUser()
                Return
            End If

            ' Retain only the identity fields. Keeping the whole entity (including the
            ' password hash) alive in memory was unnecessary exposure (L3).
            _currentUserId = user.Id
            _currentUsername = user.Username
            _currentUserRole = user.Role
        End Sub

        Public ReadOnly Property CurrentUserId As Integer?
            Get
                Return _currentUserId
            End Get
        End Property

        Public ReadOnly Property CurrentUserRole As UserRole? Implements ISessionService.CurrentUserRole
            Get
                Return _currentUserRole
            End Get
        End Property

        Public ReadOnly Property CurrentUsername As String Implements ISessionService.CurrentUsername
            Get
                Return _currentUsername
            End Get
        End Property

        Public ReadOnly Property IsAuthenticated As Boolean Implements ISessionService.IsAuthenticated
            Get
                Return _currentUsername IsNot Nothing
            End Get
        End Property

        Public Sub ClearUser() Implements ISessionService.ClearUser
            _currentUserId = Nothing
            _currentUsername = Nothing
            _currentUserRole = Nothing
        End Sub
    End Class
End Namespace
