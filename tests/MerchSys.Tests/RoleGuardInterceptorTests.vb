Imports Xunit
Imports Microsoft.EntityFrameworkCore.Diagnostics
Imports MerchSys.SharedKernel.Data
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Exceptions

Public Class RoleGuardInterceptorTests

    <Fact>
    Public Sub OwnerExecutingNonQueryInUserContext_IsRejected()
        Dim session = New FakeSessionService With {.Role = UserRole.Owner, .Name = "owner"}
        Dim scope = New WriteContextScope()
        Dim interceptor = New RoleGuardInterceptor(session, scope)

        Dim command = New FakeDbCommand()
        Dim result = New InterceptionResult(Of Integer)()

        ' We can pass Nothing for CommandEventData since NonQueryExecuting only checks scope/session
        Assert.Throws(Of UnauthorizedWriteException)(Sub() interceptor.NonQueryExecuting(command, Nothing, result))
    End Sub

End Class

Public Class FakeDbCommand
    Inherits System.Data.Common.DbCommand

    Public Overrides Property CommandText As String
    Public Overrides Property CommandTimeout As Integer
    Public Overrides Property CommandType As System.Data.CommandType
    Public Overrides Property DesignTimeVisible As Boolean
    Public Overrides Property UpdatedRowSource As System.Data.UpdateRowSource
    Protected Overrides Property DbConnection As System.Data.Common.DbConnection
    Protected Overrides ReadOnly Property DbParameterCollection As System.Data.Common.DbParameterCollection
        Get
            Return Nothing
        End Get
    End Property
    Protected Overrides Property DbTransaction As System.Data.Common.DbTransaction

    Public Overrides Sub Cancel()
    End Sub

    Protected Overrides Function CreateDbParameter() As System.Data.Common.DbParameter
        Return Nothing
    End Function

    Protected Overrides Function ExecuteDbDataReader(behavior As System.Data.CommandBehavior) As System.Data.Common.DbDataReader
        Return Nothing
    End Function

    Public Overrides Function ExecuteNonQuery() As Integer
        Return 0
    End Function

    Public Overrides Function ExecuteScalar() As Object
        Return Nothing
    End Function

    Public Overrides Sub Prepare()
    End Sub
End Class
