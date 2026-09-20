Imports MerchSys.SharedKernel.Entities
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Interfaces

Namespace Services
    Public Class LoginSessionService
        Implements ISessionService

        Private _currentUser As UserAccount

        Public Sub SetUser(user As UserAccount)
            _currentUser = user
        End Sub

        Public ReadOnly Property CurrentUserRole As UserRole? Implements ISessionService.CurrentUserRole
            Get
                Return _currentUser?.Role
            End Get
        End Property

        Public ReadOnly Property CurrentUsername As String Implements ISessionService.CurrentUsername
            Get
                Return _currentUser?.Username
            End Get
        End Property

        Public ReadOnly Property IsAuthenticated As Boolean Implements ISessionService.IsAuthenticated
            Get
                Return _currentUser IsNot Nothing
            End Get
        End Property

        Public Sub ClearUser() Implements ISessionService.ClearUser
            _currentUser = Nothing
        End Sub
    End Class
End Namespace
