Namespace Interfaces
    Public Interface ISessionService
        Property CurrentUser As Object
        ReadOnly Property CurrentRole As Enums.UserRole?
        Sub ClearUser()
    End Interface
End Namespace
