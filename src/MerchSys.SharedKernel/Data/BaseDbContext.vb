Imports System.Data.Common
Imports System.Linq.Expressions
Imports System.Text.Json
Imports System.Threading
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.ChangeTracking
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports Microsoft.EntityFrameworkCore.Metadata.Conventions
Imports Microsoft.EntityFrameworkCore.Storage
Imports MerchSys.SharedKernel.Interfaces

Namespace Data

    Public MustInherit Class BaseDbContext
        Inherits DbContext

        Private ReadOnly _currentUserProvider As ICurrentUserProvider
        Private ReadOnly _writeContextScope As IWriteContextScope

        Public Sub New(options As DbContextOptions, Optional currentUserProvider As ICurrentUserProvider = Nothing, Optional writeContextScope As IWriteContextScope = Nothing)
            MyBase.New(options)
            _currentUserProvider = currentUserProvider
            _writeContextScope = writeContextScope
        End Sub

        ''' <summary>
        ''' Short module tag written into the audit log. Each module context overrides this; the
        ''' default is the type name so a context that forgets still logs something identifiable.
        ''' </summary>
        Protected Overridable ReadOnly Property AuditModuleTag As String
            Get
                Return Me.GetType().Name
            End Get
        End Property

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

        Public Overrides Async Function SaveChangesAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of Integer)
            ' Materialised: the audit rows are built from this list, and the tracker changes underneath.
            Dim entries = ChangeTracker.Entries().ToList()
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

            ' Captured BEFORE the save: afterwards OriginalValue equals CurrentValue and the previous
            ' values — the whole point of the log — are gone.
            Dim auditRows = BuildAuditRows(entries, actor)

            If auditRows.Count = 0 Then
                Return Await MyBase.SaveChangesAsync(cancellationToken)
            End If

            ' The audit rows must commit with the change they describe. EF's implicit per-save
            ' transaction is created inside MyBase.SaveChangesAsync, so a statement outside it would not
            ' be atomic — and a log that can disagree with the data is worse than no log. Begin an
            ' explicit transaction unless the caller already has one (the FIFO deduction does).
            Dim ownsTransaction = Database.CurrentTransaction Is Nothing
            Dim transaction As IDbContextTransaction = Nothing
            If ownsTransaction Then
                transaction = Await Database.BeginTransactionAsync(cancellationToken)
            End If

            Dim affected As Integer = 0
            Dim failure As Exception = Nothing
            Try
                affected = Await MyBase.SaveChangesAsync(cancellationToken)

                For Each row In auditRows
                    Await InsertAuditRowAsync(row, cancellationToken)
                Next

                If transaction IsNot Nothing Then
                    Await transaction.CommitAsync(cancellationToken)
                End If
            Catch ex As Exception
                failure = ex
            End Try

            ' Disposing without committing rolls back, so a failed log write takes the change with it.
            If transaction IsNot Nothing Then transaction.Dispose()

            If failure IsNot Nothing Then
                Throw failure
            End If

            Return affected
        End Function

        ''' <summary>
        ''' The audit rows a save will produce — one per changed <see cref="IAuditedEntity"/>.
        ''' </summary>
        Private Function BuildAuditRows(entries As List(Of EntityEntry), actor As String) As List(Of AuditRow)
            Dim writeContext As String = "User"
            If _writeContextScope IsNot Nothing Then
                writeContext = _writeContextScope.Current.ToString()
            End If

            Dim rows As New List(Of AuditRow)()

            For Each dbEntry In entries
                If Not GetType(IAuditedEntity).IsAssignableFrom(dbEntry.Entity.GetType()) Then Continue For
                If dbEntry.State <> EntityState.Added AndAlso
                   dbEntry.State <> EntityState.Modified AndAlso
                   dbEntry.State <> EntityState.Deleted Then Continue For

                rows.Add(New AuditRow With {
                    .OccurredAt = DateTime.UtcNow,
                    .Actor = actor,
                    .WriteContext = writeContext,
                    .ModuleTag = AuditModuleTag,
                    .EntityType = dbEntry.Entity.GetType().Name,
                    .Entry = dbEntry,
                    .AuditAction = ActionNameOf(dbEntry.State),
                    .ChangesJson = ChangesJsonFor(dbEntry)
                })
            Next

            Return rows
        End Function

        ''' <summary>
        ''' Writes one audit row on this context's own connection, enlisted in its current transaction.
        '''
        ''' Raw ADO rather than an EF entity on purpose: the audit table is written by all four module
        ''' contexts, and a table mapped into more than one context breaks <c>CreateTables()</c>, which
        ''' is not idempotent. The connection comes from <c>Database.GetDbConnection()</c> — a MySql.Data
        ''' connection — so it is driven through the provider-agnostic <see cref="DbConnection"/> API.
        ''' </summary>
        Private Async Function InsertAuditRowAsync(row As AuditRow, cancellationToken As CancellationToken) As Task
            Dim connection = Database.GetDbConnection()
            Dim currentTransaction = Database.CurrentTransaction

            Using command = connection.CreateCommand()
                If currentTransaction IsNot Nothing Then
                    command.Transaction = currentTransaction.GetDbTransaction()
                End If

                command.CommandText =
                    "INSERT INTO `" & AuditLogSchema.TableName & "` " &
                    "(`OccurredAt`, `Actor`, `WriteContext`, `Module`, `EntityType`, `EntityId`, `Action`, `ChangesJson`) " &
                    "VALUES (@occurredAt, @actor, @writeContext, @module, @entityType, @entityId, @action, @changesJson)"
                command.AddParameter("@occurredAt", row.OccurredAt)
                command.AddParameter("@actor", row.Actor)
                command.AddParameter("@writeContext", row.WriteContext)
                command.AddParameter("@module", row.ModuleTag)
                command.AddParameter("@entityType", row.EntityType)
                command.AddParameter("@entityId", PrimaryKeyOf(row.Entry))
                command.AddParameter("@action", row.AuditAction)
                command.AddParameter("@changesJson", row.ChangesJson)

                Await command.ExecuteNonQueryAsync(cancellationToken)
            End Using
        End Function

        Private Shared Function ActionNameOf(state As EntityState) As String
            Select Case state
                Case EntityState.Added
                    Return "Insert"
                Case EntityState.Deleted
                    Return "Delete"
                Case Else
                    Return "Update"
            End Select
        End Function

        ''' <summary>
        ''' Primary key as text, so any key type fits. Empty when the entity has no key.
        '''
        ''' Read after the save, so a store-generated key is the assigned one rather than 0.
        ''' </summary>
        Private Shared Function PrimaryKeyOf(dbEntry As EntityEntry) As String
            Dim keyProperty = dbEntry.Properties.FirstOrDefault(Function(p) p.Metadata.IsPrimaryKey())
            Return If(keyProperty?.CurrentValue?.ToString(), String.Empty)
        End Function

        ''' <summary>
        ''' Serialises what the row will no longer hold — the only thing the row cannot tell you
        ''' afterwards.
        ''' <list type="bullet">
        '''   <item><b>Update</b> — the changed columns only, with their previous and new values. The
        '''     unchanged columns are still in the row; the previous values exist nowhere else.</item>
        '''   <item><b>Delete</b> — every column, because the row is about to disappear entirely.</item>
        '''   <item><b>Insert</b> — nothing beyond the action: the values are all in the row, and the key
        '''     is recorded in <c>EntityId</c>.</item>
        ''' </list>
        ''' </summary>
        Private Shared Function ChangesJsonFor(dbEntry As EntityEntry) As String
            Dim changes As New Dictionary(Of String, Object)()

            If dbEntry.State = EntityState.Added Then
                Return JsonSerializer.Serialize(changes)
            End If

            For Each propertyEntry In dbEntry.Properties
                If propertyEntry.Metadata.IsPrimaryKey() Then Continue For
                If propertyEntry.Metadata.IsShadowProperty() Then Continue For

                If dbEntry.State = EntityState.Deleted Then
                    changes(propertyEntry.Metadata.Name) = propertyEntry.OriginalValue
                ElseIf propertyEntry.IsModified Then
                    changes(propertyEntry.Metadata.Name) = New Dictionary(Of String, Object) From {
                        {"from", propertyEntry.OriginalValue},
                        {"to", propertyEntry.CurrentValue}
                    }
                End If
            Next

            Return JsonSerializer.Serialize(changes)
        End Function

        ''' <summary>One audit row, before it is written.</summary>
        Private Class AuditRow
            Public Property OccurredAt As DateTime
            Public Property Actor As String
            Public Property WriteContext As String
            Public Property ModuleTag As String
            Public Property EntityType As String

            ''' <summary>
            ''' Held rather than resolved eagerly: the key of a store-generated entity is only assigned
            ''' by the save, and the audit insert runs after it.
            ''' </summary>
            Public Property Entry As EntityEntry

            Public Property AuditAction As String
            Public Property ChangesJson As String
        End Class

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
