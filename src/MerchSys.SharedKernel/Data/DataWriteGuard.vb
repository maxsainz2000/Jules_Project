Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Exceptions
Imports MerchSys.SharedKernel.Interfaces

Namespace Data

    ''' <summary>
    ''' Enforces the same role/write-context policy as <see cref="RoleGuardInterceptor"/> for
    ''' raw-ADO write paths, which EF's SaveChanges interception cannot see.
    '''
    ''' The interceptor only fires on EF <c>SaveChanges</c>. Any service that writes through
    ''' <c>MySqlCommand.ExecuteNonQuery</c> must call this guard explicitly, otherwise the DA5
    ''' authorization guarantee is porous by construction.
    ''' </summary>
    Public NotInheritable Class DataWriteGuard

        Private Sub New()
        End Sub

        ''' <summary>
        ''' Throws <see cref="UnauthorizedWriteException"/> when the current session is an Owner
        ''' and the write is being made in the user (non-privileged) context.
        ''' </summary>
        ''' <param name="sessionService">Current session, used to read the role.</param>
        ''' <param name="writeContextScope">Current write context.</param>
        ''' <param name="operation">Short description used in the error message.</param>
        Public Shared Sub EnsureCanWrite(sessionService As ISessionService, writeContextScope As IWriteContextScope, operation As String)
            If sessionService Is Nothing Then Throw New ArgumentNullException(NameOf(sessionService))
            If writeContextScope Is Nothing Then Throw New ArgumentNullException(NameOf(writeContextScope))

            If writeContextScope.Current = WriteContextKind.User AndAlso sessionService.CurrentUserRole = UserRole.Owner Then
                Throw New UnauthorizedWriteException($"Owner accounts are not permitted to perform database writes ({operation}).")
            End If
        End Sub

    End Class

End Namespace
