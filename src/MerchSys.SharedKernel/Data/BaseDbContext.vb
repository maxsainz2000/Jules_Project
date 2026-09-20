Imports System.Linq.Expressions
Imports System.Threading
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.SharedKernel.Interfaces

Namespace Data

    Public MustInherit Class BaseDbContext
        Inherits DbContext

        Public Sub New(options As DbContextOptions)
            MyBase.New(options)
        End Sub

        Protected Overrides Sub ConfigureConventions(configurationBuilder As ModelConfigurationBuilder)
            MyBase.ConfigureConventions(configurationBuilder)
            configurationBuilder.Properties(Of String)().
                HaveMaxLength(256)
        End Sub

        Protected Overrides Sub OnModelCreating(modelBuilder As ModelBuilder)
            MyBase.OnModelCreating(modelBuilder)
            ApplySoftDeleteFilters(modelBuilder)
        End Sub

        Private Sub ApplySoftDeleteFilters(modelBuilder As ModelBuilder)
            For Each entityType In modelBuilder.Model.GetEntityTypes()
                If GetType(ISoftDeletable).IsAssignableFrom(entityType.ClrType) Then
                    Dim param = Expression.Parameter(entityType.ClrType, "e")
                    Dim prop = Expression.Property(param, NameOf(ISoftDeletable.IsDeleted))
                    Dim condition = Expression.Equal(prop, Expression.Constant(False))
                    Dim lambda = Expression.Lambda(condition, param)
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda)
                End If
            Next
        End Sub

        Public Overrides Function SaveChangesAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of Integer)
            Dim entries = ChangeTracker.Entries()

            For Each dbEntry In entries
                If TypeOf dbEntry.Entity Is IAuditable Then
                    Dim auditable = DirectCast(dbEntry.Entity, IAuditable)
                    If dbEntry.State = EntityState.Added Then
                        auditable.CreatedAt = DateTime.UtcNow
                        auditable.CreatedBy = "System" ' Or fetch from some current user service
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

            Return MyBase.SaveChangesAsync(cancellationToken)
        End Function

    End Class

End Namespace
