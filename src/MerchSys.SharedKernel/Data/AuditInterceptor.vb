Imports System.Threading
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Diagnostics
Imports MerchSys.SharedKernel.Interfaces

Namespace Data

    Public Class AuditInterceptor
        Inherits SaveChangesInterceptor

        Public Overrides Function SavingChanges(eventData As DbContextEventData, result As InterceptionResult(Of Integer)) As InterceptionResult(Of Integer)
            ApplyAuditAndSoftDelete(eventData.Context)
            Return MyBase.SavingChanges(eventData, result)
        End Function

        Public Overrides Function SavingChangesAsync(eventData As DbContextEventData, result As InterceptionResult(Of Integer), Optional cancellationToken As CancellationToken = Nothing) As ValueTask(Of InterceptionResult(Of Integer))
            ApplyAuditAndSoftDelete(eventData.Context)
            Return MyBase.SavingChangesAsync(eventData, result, cancellationToken)
        End Function

        Private Sub ApplyAuditAndSoftDelete(context As DbContext)
            If context Is Nothing Then Return

            Dim entries = context.ChangeTracker.Entries()

            For Each dbEntry In entries
                If TypeOf dbEntry.Entity Is IAuditable Then
                    Dim auditable = DirectCast(dbEntry.Entity, IAuditable)
                    If dbEntry.State = EntityState.Added Then
                        auditable.CreatedAt = DateTime.UtcNow
                        auditable.CreatedBy = "System"
                    ElseIf dbEntry.State = EntityState.Modified Then
                        auditable.ModifiedAt = DateTime.UtcNow
                        auditable.ModifiedBy = "System"
                    End If
                End If

                If TypeOf dbEntry.Entity Is ISoftDeletable AndAlso dbEntry.State = EntityState.Deleted Then
                    dbEntry.State = EntityState.Modified
                    DirectCast(dbEntry.Entity, ISoftDeletable).IsDeleted = True
                End If
            Next
        End Sub

    End Class

End Namespace
