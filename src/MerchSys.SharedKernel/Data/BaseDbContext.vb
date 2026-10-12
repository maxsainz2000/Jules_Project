Imports System.Linq.Expressions
Imports System.Threading
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports Microsoft.EntityFrameworkCore.Metadata.Conventions
Imports MerchSys.SharedKernel.Interfaces

Namespace Data

    Public MustInherit Class BaseDbContext
        Inherits DbContext

        Private ReadOnly _currentUserProvider As ICurrentUserProvider

        Public Sub New(options As DbContextOptions, Optional currentUserProvider As ICurrentUserProvider = Nothing)
            MyBase.New(options)
            _currentUserProvider = currentUserProvider
        End Sub

        ''' <summary>
        ''' Identity stamped into audit columns. Falls back to "System" only when there is no
        ''' active session (e.g. startup seeding), so real user changes record who made them (H4).
        ''' </summary>
        Private Function ResolveActor() As String
            Dim name = If(_currentUserProvider IsNot Nothing, _currentUserProvider.Username, Nothing)
            If String.IsNullOrWhiteSpace(name) Then Return "System"
            Return name
        End Function

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

        ''' <summary>
        ''' Applies the module table prefix to every mapped entity, skipping any entity whose
        ''' table name already carries the prefix. Call this from <c>OnModelCreating</c>
        ''' <b>after</b> <c>ApplyConfigurationsFromAssembly</c>, because the configurations are
        ''' what declare the base table names.
        '''
        ''' The idempotence matters: a blanket prefix turns a configuration that already declares
        ''' <c>ToTable("Pos_SalesTransactions")</c> into <c>Pos_Pos_SalesTransactions</c>, which no
        ''' longer matches the names the raw-ADO queries and the operator runbooks use.
        ''' </summary>
        Protected Shared Sub ApplyTablePrefix(modelBuilder As ModelBuilder, prefix As String)
            For Each entityType In modelBuilder.Model.GetEntityTypes()
                Dim currentName = entityType.GetTableName()

                ' Owned, view-mapped and keyless types have no table name of their own.
                If currentName Is Nothing Then Continue For

                If Not currentName.StartsWith(prefix, StringComparison.Ordinal) Then
                    entityType.SetTableName(prefix & currentName)
                End If
            Next
        End Sub

        Private Sub ApplySecondaryIndexes(modelBuilder As ModelBuilder)
            For Each entityType In modelBuilder.Model.GetEntityTypes()
                ' Only large, paginated transactional tables need the (CreatedAt, Id) seek
                ' index. Applying it to every auditable entity (including lookup tables)
                ' wasted space and could duplicate configured indexes (M12).
                If Not GetType(IPaginatedEntity).IsAssignableFrom(entityType.ClrType) Then Continue For

                Dim idProp = entityType.FindProperty("Id")
                Dim createdAtProp = entityType.FindProperty(NameOf(IAuditable.CreatedAt))
                If idProp Is Nothing OrElse createdAtProp Is Nothing Then Continue For

                ' Skip if an index covering (CreatedAt, Id) is already declared.
                Dim alreadyIndexed = entityType.GetIndexes().Any(
                    Function(ix)
                        Dim props = ix.Properties
                        Return props.Count = 2 AndAlso
                               props(0).Name = NameOf(IAuditable.CreatedAt) AndAlso
                               props(1).Name = "Id"
                    End Function)
                If alreadyIndexed Then Continue For

                entityType.AddIndex({createdAtProp, idProp})
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
            Dim actor = ResolveActor()

            For Each dbEntry In entries
                If TypeOf dbEntry.Entity Is IAuditable Then
                    Dim auditable = DirectCast(dbEntry.Entity, IAuditable)
                    If dbEntry.State = EntityState.Added Then
                        auditable.CreatedAt = DateTime.UtcNow
                        auditable.CreatedBy = actor
                    ElseIf dbEntry.State = EntityState.Modified Then
                        auditable.ModifiedAt = DateTime.UtcNow
                        auditable.ModifiedBy = actor
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
