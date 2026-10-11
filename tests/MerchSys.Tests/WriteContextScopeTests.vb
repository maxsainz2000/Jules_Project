Imports Xunit
Imports MerchSys.SharedKernel.Data
Imports MerchSys.SharedKernel.Interfaces

Public Class WriteContextScopeTests

    <Fact>
    Public Sub DefaultsToUserContext()
        Dim scope = New WriteContextScope()
        Assert.Equal(WriteContextKind.User, scope.Current)
    End Sub

    <Fact>
    Public Sub EnterElevatesAndDisposeRestores()
        Dim scope = New WriteContextScope()

        Using scope.Enter(WriteContextKind.System)
            Assert.Equal(WriteContextKind.System, scope.Current)
        End Using

        Assert.Equal(WriteContextKind.User, scope.Current)
    End Sub

    <Fact>
    Public Sub NestedScopesRestoreTheParentFrame()
        Dim scope = New WriteContextScope()

        Using scope.Enter(WriteContextKind.System)
            Using scope.Enter(WriteContextKind.AuthSelfService)
                Assert.Equal(WriteContextKind.AuthSelfService, scope.Current)
            End Using
            Assert.Equal(WriteContextKind.System, scope.Current)
        End Using

        Assert.Equal(WriteContextKind.User, scope.Current)
    End Sub
End Class
