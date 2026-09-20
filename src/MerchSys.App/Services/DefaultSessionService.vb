Imports MerchSys.SharedKernel.Entities
Imports MerchSys.SharedKernel.Interfaces

Namespace Services
    ''' <summary>
    ''' DEBUG-bypass only, never ships to production.
    ''' </summary>
    Public Class DefaultSessionService
        Implements ISessionService

        Private _currentUser As Object

        Public Property CurrentUser As Object Implements ISessionService.CurrentUser
            Get
                Return _currentUser
            End Get
            Set(value As Object)
                _currentUser = value
            End Set
        End Property

        Public ReadOnly Property CurrentRole As SharedKernel.Enums.UserRole? Implements ISessionService.CurrentRole
            Get
                Return SharedKernel.Enums.UserRole.Manager
            End Get
        End Property

        Public Sub ClearUser() Implements ISessionService.ClearUser
            _currentUser = Nothing
        End Sub
    End Class
End Namespace
