Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Diagnostics
Imports MerchSys.SharedKernel.Exceptions
Imports System.Threading
Imports System.Threading.Tasks
Imports MerchSys.POS.Entities

Namespace Data.Interceptors
    Public Class ImmutableReceiptInterceptor
        Inherits SaveChangesInterceptor

        Public Shared Property ArchivalBypass As New AsyncLocal(Of Boolean)

        Public Overrides Function SavingChanges(eventData As DbContextEventData, result As InterceptionResult(Of Integer)) As InterceptionResult(Of Integer)
            CheckImmutability(eventData.Context)
            Return MyBase.SavingChanges(eventData, result)
        End Function

        Public Overrides Function SavingChangesAsync(eventData As DbContextEventData, result As InterceptionResult(Of Integer), Optional cancellationToken As CancellationToken = Nothing) As ValueTask(Of InterceptionResult(Of Integer))
            CheckImmutability(eventData.Context)
            Return MyBase.SavingChangesAsync(eventData, result, cancellationToken)
        End Function

        Private Sub CheckImmutability(context As DbContext)
            If context Is Nothing Then Return
            If ArchivalBypass.Value Then Return

            For Each dbEntry In context.ChangeTracker.Entries()
                If IsImmutable(dbEntry.Entity) AndAlso
                   (dbEntry.State = EntityState.Modified OrElse dbEntry.State = EntityState.Deleted) Then

                    Dim entityType = dbEntry.Entity.GetType().Name
                    Dim entityKey = "Unknown"
                    If dbEntry.Properties.Any(Function(p) p.Metadata.IsPrimaryKey()) Then
                        entityKey = dbEntry.Properties.First(Function(p) p.Metadata.IsPrimaryKey()).CurrentValue?.ToString()
                    End If

                    Throw New ImmutableEntityException(entityType, entityKey)
                End If
            Next
        End Sub

        ''' <summary>
        ''' The receipt entities that must never be modified or deleted once written.
        '''
        ''' The two archive entities are included so that BIR-retention rows cannot be edited or
        ''' removed through EF either. They are the application-side counterpart to the
        ''' <c>Pos_OfficialReceiptArchive</c> database triggers — which, until now, were the *only*
        ''' enforcement for the archive, because this interceptor covered just the two live tables
        ''' (H4). The archival service still works: it only ever <c>Add</c>s archive rows, and it
        ''' runs with <see cref="ArchivalBypass"/> set.
        ''' </summary>
        Private Shared Function IsImmutable(entity As Object) As Boolean
            Return TypeOf entity Is OfficialReceipt OrElse
                   TypeOf entity Is ReceiptIntegrity OrElse
                   TypeOf entity Is OfficialReceiptArchive OrElse
                   TypeOf entity Is ReceiptIntegrityArchive
        End Function
    End Class
End Namespace