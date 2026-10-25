Imports System.Data.Common
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Diagnostics
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Exceptions
Imports MerchSys.SharedKernel.Interfaces

Namespace Data

    Public Class RoleGuardInterceptor
        Inherits SaveChangesInterceptor
        Implements IDbCommandInterceptor

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

        Private Sub PerformCommandCheck()
            If _writeContextScope.Current = WriteContextKind.User AndAlso _sessionService.CurrentUserRole = UserRole.Owner Then
                Throw New UnauthorizedWriteException("Owner accounts are not permitted to perform database writes in the User context.")
            End If
        End Sub

        Public Function NonQueryExecuting(command As DbCommand, eventData As CommandEventData, result As InterceptionResult(Of Integer)) As InterceptionResult(Of Integer) Implements IDbCommandInterceptor.NonQueryExecuting
            PerformCommandCheck()
            Return result
        End Function

        Public Function NonQueryExecutingAsync(command As DbCommand, eventData As CommandEventData, result As InterceptionResult(Of Integer), Optional cancellationToken As CancellationToken = Nothing) As ValueTask(Of InterceptionResult(Of Integer)) Implements IDbCommandInterceptor.NonQueryExecutingAsync
            PerformCommandCheck()
            Return New ValueTask(Of InterceptionResult(Of Integer))(result)
        End Function

        Public Function CommandCreating(eventData As CommandCorrelatedEventData, result As InterceptionResult(Of DbCommand)) As InterceptionResult(Of DbCommand) Implements IDbCommandInterceptor.CommandCreating
            Return result
        End Function

        Public Function CommandCreated(eventData As CommandEndEventData, result As DbCommand) As DbCommand Implements IDbCommandInterceptor.CommandCreated
            Return result
        End Function

        Public Function CommandInitialized(eventData As CommandEndEventData, result As DbCommand) As DbCommand Implements IDbCommandInterceptor.CommandInitialized
            Return result
        End Function

        Public Function ReaderExecuting(command As DbCommand, eventData As CommandEventData, result As InterceptionResult(Of DbDataReader)) As InterceptionResult(Of DbDataReader) Implements IDbCommandInterceptor.ReaderExecuting
            Return result
        End Function

        Public Function ScalarExecuting(command As DbCommand, eventData As CommandEventData, result As InterceptionResult(Of Object)) As InterceptionResult(Of Object) Implements IDbCommandInterceptor.ScalarExecuting
            Return result
        End Function

        Public Function ReaderExecutingAsync(command As DbCommand, eventData As CommandEventData, result As InterceptionResult(Of DbDataReader), Optional cancellationToken As CancellationToken = Nothing) As ValueTask(Of InterceptionResult(Of DbDataReader)) Implements IDbCommandInterceptor.ReaderExecutingAsync
            Return New ValueTask(Of InterceptionResult(Of DbDataReader))(result)
        End Function

        Public Function ScalarExecutingAsync(command As DbCommand, eventData As CommandEventData, result As InterceptionResult(Of Object), Optional cancellationToken As CancellationToken = Nothing) As ValueTask(Of InterceptionResult(Of Object)) Implements IDbCommandInterceptor.ScalarExecutingAsync
            Return New ValueTask(Of InterceptionResult(Of Object))(result)
        End Function

        Public Function ReaderExecuted(command As DbCommand, eventData As CommandExecutedEventData, result As DbDataReader) As DbDataReader Implements IDbCommandInterceptor.ReaderExecuted
            Return result
        End Function

        Public Function ScalarExecuted(command As DbCommand, eventData As CommandExecutedEventData, result As Object) As Object Implements IDbCommandInterceptor.ScalarExecuted
            Return result
        End Function

        Public Function NonQueryExecuted(command As DbCommand, eventData As CommandExecutedEventData, result As Integer) As Integer Implements IDbCommandInterceptor.NonQueryExecuted
            Return result
        End Function

        Public Function ReaderExecutedAsync(command As DbCommand, eventData As CommandExecutedEventData, result As DbDataReader, Optional cancellationToken As CancellationToken = Nothing) As ValueTask(Of DbDataReader) Implements IDbCommandInterceptor.ReaderExecutedAsync
            Return New ValueTask(Of DbDataReader)(result)
        End Function

        Public Function ScalarExecutedAsync(command As DbCommand, eventData As CommandExecutedEventData, result As Object, Optional cancellationToken As CancellationToken = Nothing) As ValueTask(Of Object) Implements IDbCommandInterceptor.ScalarExecutedAsync
            Return New ValueTask(Of Object)(result)
        End Function

        Public Function NonQueryExecutedAsync(command As DbCommand, eventData As CommandExecutedEventData, result As Integer, Optional cancellationToken As CancellationToken = Nothing) As ValueTask(Of Integer) Implements IDbCommandInterceptor.NonQueryExecutedAsync
            Return New ValueTask(Of Integer)(result)
        End Function

        Public Sub CommandCanceled(command As DbCommand, eventData As CommandEndEventData) Implements IDbCommandInterceptor.CommandCanceled
        End Sub

        Public Function CommandCanceledAsync(command As DbCommand, eventData As CommandEndEventData, Optional cancellationToken As CancellationToken = Nothing) As Task Implements IDbCommandInterceptor.CommandCanceledAsync
            Return Task.CompletedTask
        End Function

        Public Sub CommandFailed(command As DbCommand, eventData As CommandErrorEventData) Implements IDbCommandInterceptor.CommandFailed
        End Sub

        Public Function CommandFailedAsync(command As DbCommand, eventData As CommandErrorEventData, Optional cancellationToken As CancellationToken = Nothing) As Task Implements IDbCommandInterceptor.CommandFailedAsync
            Return Task.CompletedTask
        End Function

        Public Function DataReaderClosing(command As DbCommand, eventData As DataReaderClosingEventData, result As InterceptionResult) As InterceptionResult Implements IDbCommandInterceptor.DataReaderClosing
            Return result
        End Function

        Public Function DataReaderClosingAsync(command As DbCommand, eventData As DataReaderClosingEventData, result As InterceptionResult) As ValueTask(Of InterceptionResult) Implements IDbCommandInterceptor.DataReaderClosingAsync
            Return New ValueTask(Of InterceptionResult)(result)
        End Function

        Public Function DataReaderDisposing(command As DbCommand, eventData As DataReaderDisposingEventData, result As InterceptionResult) As InterceptionResult Implements IDbCommandInterceptor.DataReaderDisposing
            Return result
        End Function

    End Class

End Namespace
