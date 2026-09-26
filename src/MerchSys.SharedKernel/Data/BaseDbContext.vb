Imports System.Linq.Expressions
Imports System.Threading
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports Microsoft.EntityFrameworkCore.Metadata.Conventions
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

            ' Fix for WinForms DataGridView Boolean binding (MariaDB TINYINT(1))
            configurationBuilder.Properties(Of Boolean)().HaveConversion(Of Integer)()

            ' Prevent phantom concurrency exceptions caused by WinForms DateTimePicker truncating ms
            configurationBuilder.Properties(Of DateTime)().HaveColumnType("DATETIME(6)")

            configurationBuilder.Conventions.Add(Function(sp) New IgnoreNonTokenRowVersionConvention())
        End Sub

        Protected Overrides Sub OnModelCreating(modelBuilder As ModelBuilder)
            MyBase.OnModelCreating(modelBuilder)
            ApplySoftDeleteFilters(modelBuilder)
            ApplySecondaryIndexes(modelBuilder)
        End Sub

        Private Sub ApplySecondaryIndexes(modelBuilder As ModelBuilder)
            For Each entityType In modelBuilder.Model.GetEntityTypes()
                If GetType(IAuditable).IsAssignableFrom(entityType.ClrType) Then
                    Dim idProp = entityType.FindProperty("Id")
                    Dim createdAtProp = entityType.FindProperty(NameOf(IAuditable.CreatedAt))
                    If idProp IsNot Nothing AndAlso createdAtProp IsNot Nothing Then
                        entityType.AddIndex({createdAtProp, idProp})
                    End If
                End If
            Next
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

        ''' <summary>
        ''' A model finalizing convention that removes the 'RowVersion' property from entity types
        ''' unless it has been explicitly mapped as a concurrency token (IsRowVersion).
        ''' </summary>
        Private Class IgnoreNonTokenRowVersionConvention
            Implements IModelFinalizingConvention

            Public Sub ProcessModelFinalizing(modelBuilder As IConventionModelBuilder, context As IConventionContext(Of IConventionModelBuilder)) Implements IModelFinalizingConvention.ProcessModelFinalizing
                For Each entityType In modelBuilder.Metadata.GetEntityTypes()
                    Dim rowVersionProp = entityType.FindProperty("RowVersion")
                    If rowVersionProp IsNot Nothing AndAlso Not rowVersionProp.IsConcurrencyToken Then
                        entityType.RemoveProperty("RowVersion")
                    End If
                Next
            End Sub
        End Class

    End Class

End Namespace
