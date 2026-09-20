Namespace Interfaces
    Public Interface ISessionService
        ReadOnly Property CurrentUserRole As Enums.UserRole?
        ReadOnly Property CurrentUsername As String
        ReadOnly Property IsAuthenticated As Boolean
        Sub ClearUser()
    End Interface
End Namespace
