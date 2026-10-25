Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Interfaces

''' <summary>Minimal in-memory <see cref="ISessionService"/> for tests.</summary>
Friend Class FakeSessionService
    Implements ISessionService

    Public Property Role As UserRole?
    Public Property Name As String

    Public ReadOnly Property CurrentUserRole As UserRole? Implements ISessionService.CurrentUserRole
        Get
            Return Role
        End Get
    End Property

    Public ReadOnly Property CurrentUsername As String Implements ISessionService.CurrentUsername
        Get
            Return Name
        End Get
    End Property

    Public ReadOnly Property IsAuthenticated As Boolean Implements ISessionService.IsAuthenticated
        Get
            Return Name IsNot Nothing
        End Get
    End Property

    Public Sub ClearUser() Implements ISessionService.ClearUser
        Name = Nothing
        Role = Nothing
    End Sub
End Class
