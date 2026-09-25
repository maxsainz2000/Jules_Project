Imports System.Threading
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Diagnostics
Imports MerchSys.SharedKernel.Exceptions
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.SharedKernel.Enums

Namespace Data
    Public Class RoleGuardInterceptor
        Inherits SaveChangesInterceptor

        Private ReadOnly _sessionService As ISessionService
        Private ReadOnly _writeContextScope As IWriteContextScope

        Public Sub New(sessionService As ISessionService, writeContextScope As IWriteContextScope)
            _sessionService = sessionService
            _writeContextScope = writeContextScope
        End Sub

        Public Overrides Function SavingChanges(eventData As DbContextEventData, result As InterceptionResult(Of Integer)) As InterceptionResult(Of Integer)
            ValidateWritePermissions(eventData.Context)
            Return MyBase.SavingChanges(eventData, result)
        End Function

        Public Overrides Function SavingChangesAsync(eventData As DbContextEventData, result As InterceptionResult(Of Integer), Optional cancellationToken As CancellationToken = Nothing) As ValueTask(Of InterceptionResult(Of Integer))
            ValidateWritePermissions(eventData.Context)
            Return MyBase.SavingChangesAsync(eventData, result, cancellationToken)
        End Function

        Private Sub ValidateWritePermissions(context As DbContext)
            If context Is Nothing Then Return

            If _writeContextScope.CurrentKind = WriteContextKind.System OrElse _writeContextScope.CurrentKind = WriteContextKind.AuthSelfService Then
                Return
            End If

            Dim isOwner = _sessionService.CurrentUserRole.HasValue AndAlso _sessionService.CurrentUserRole.Value = UserRole.Owner

            If isOwner Then
                Dim hasWrites = context.ChangeTracker.Entries().Any(Function(e) e.State = EntityState.Added OrElse e.State = EntityState.Modified OrElse e.State = EntityState.Deleted)
                If hasWrites Then
                    Throw New UnauthorizedWriteException("Owner role is restricted from performing write operations.")
                End If
            End If
        End Sub
    End Class
End Namespace
