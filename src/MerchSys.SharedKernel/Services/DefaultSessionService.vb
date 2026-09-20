Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Interfaces

Namespace Services
    ''' <summary>
    ''' DEBUG-bypass only, never ships to production.
    ''' </summary>
    Public Class DefaultSessionService
        Implements ISessionService

        Public ReadOnly Property CurrentUserRole As UserRole? Implements ISessionService.CurrentUserRole
            Get
                Return UserRole.Manager
            End Get
        End Property

        Public ReadOnly Property CurrentUsername As String Implements ISessionService.CurrentUsername
            Get
                Return "DebugUser"
            End Get
        End Property

        Public ReadOnly Property IsAuthenticated As Boolean Implements ISessionService.IsAuthenticated
            Get
                Return True
            End Get
        End Property

        Public Sub ClearUser() Implements ISessionService.ClearUser
            ' Debug bypass: nothing to clear
        End Sub
    End Class
End Namespace
