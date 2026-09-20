Imports MerchSys.SharedKernel.Entities
Imports MerchSys.SharedKernel.Interfaces

Namespace Services
    Public Class LoginSessionService
        Implements ISessionService

        Private _currentUser As UserAccount

        Public Property CurrentUser As Object Implements ISessionService.CurrentUser
            Get
                Return _currentUser
            End Get
            Set(value As Object)
                _currentUser = DirectCast(value, UserAccount)
            End Set
        End Property

        Public ReadOnly Property CurrentRole As SharedKernel.Enums.UserRole? Implements ISessionService.CurrentRole
            Get
                If _currentUser IsNot Nothing Then
                    Return _currentUser.Role
                End If
                Return Nothing
            End Get
        End Property

        Public Sub ClearUser() Implements ISessionService.ClearUser
            _currentUser = Nothing
        End Sub
    End Class
End Namespace
