Imports System.Threading
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Diagnostics
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Exceptions
Imports MerchSys.SharedKernel.Interfaces

Namespace Data

    Public Class RoleGuardInterceptor
        Inherits SaveChangesInterceptor

        Private ReadOnly _sessionService As ISessionService
        Private ReadOnly _writeContextScope As IWriteContextScope

        Public Sub New(sessionService As ISessionService, writeContextScope As IWriteContextScope)
            _sessionService = sessionService
            _writeContextScope = writeContextScope
        End Sub

        Private Sub PerformCheck(eventData As DbContextEventData)
            If _writeContextScope.Current = WriteContextKind.User Then
                If _sessionService.CurrentUserRole = UserRole.Owner Then
                    Dim dbContext = eventData.Context
                    If dbContext IsNot Nothing Then
                        Dim hasChanges = dbContext.ChangeTracker.Entries().
                            Any(Function(e) e.State = EntityState.Added OrElse
                                            e.State = EntityState.Modified OrElse
                                            e.State = EntityState.Deleted)

                        If hasChanges Then
                            Throw New UnauthorizedWriteException("Owner accounts are not permitted to perform database writes in the User context.")
                        End If
                    End If
                End If
            End If
        End Sub

        Public Overrides Function SavingChanges(eventData As DbContextEventData, result As InterceptionResult(Of Integer)) As InterceptionResult(Of Integer)
            PerformCheck(eventData)
            Return MyBase.SavingChanges(eventData, result)
        End Function

        Public Overrides Function SavingChangesAsync(eventData As DbContextEventData, result As InterceptionResult(Of Integer), Optional cancellationToken As CancellationToken = Nothing) As ValueTask(Of InterceptionResult(Of Integer))
            PerformCheck(eventData)
            Return MyBase.SavingChangesAsync(eventData, result, cancellationToken)
        End Function

    End Class

End Namespace
