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
                If (TypeOf dbEntry.Entity Is OfficialReceipt OrElse TypeOf dbEntry.Entity Is ReceiptIntegrity) AndAlso
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
    End Class
End Namespace