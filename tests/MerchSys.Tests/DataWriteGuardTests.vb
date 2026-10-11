Imports Xunit
Imports MerchSys.SharedKernel.Data
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Exceptions
Imports MerchSys.SharedKernel.Interfaces

''' <summary>
''' Proves the DA5 authorization policy for raw-ADO write paths (H5): the guard rejects an
''' Owner write in the user context and permits privileged/system contexts.
''' </summary>
Public Class DataWriteGuardTests

    <Fact>
    Public Sub OwnerInUserContext_IsRejected()
        Dim session = New FakeSessionService With {.Role = UserRole.Owner, .Name = "owner"}
        Dim scope = New WriteContextScope()

        Assert.Throws(Of UnauthorizedWriteException)(
            Sub() DataWriteGuard.EnsureCanWrite(session, scope, "unit test write"))
    End Sub

    <Fact>
    Public Sub ManagerInUserContext_IsAllowed()
        Dim session = New FakeSessionService With {.Role = UserRole.Manager, .Name = "manager"}
        Dim scope = New WriteContextScope()

        DataWriteGuard.EnsureCanWrite(session, scope, "unit test write")
    End Sub

    <Fact>
    Public Sub OwnerInSystemContext_IsAllowed()
        Dim session = New FakeSessionService With {.Role = UserRole.Owner, .Name = "owner"}
        Dim scope = New WriteContextScope()

        Using scope.Enter(WriteContextKind.System)
            DataWriteGuard.EnsureCanWrite(session, scope, "startup seeding")
        End Using
    End Sub

    <Fact>
    Public Sub OwnerInAuthSelfServiceContext_IsAllowed()
        Dim session = New FakeSessionService With {.Role = UserRole.Owner, .Name = "owner"}
        Dim scope = New WriteContextScope()

        Using scope.Enter(WriteContextKind.AuthSelfService)
            DataWriteGuard.EnsureCanWrite(session, scope, "own password change")
        End Using
    End Sub
End Class
