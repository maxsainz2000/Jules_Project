Imports System.Data.Common
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Storage
Imports MerchSys.Inventory.Data
Imports MerchSys.Inventory.Entities
Imports MerchSys.SharedKernel.Data

Namespace Services

    ''' <summary>
    ''' The pessimistic half of the FIFO critical section: InnoDB row locks on a product's in-stock
    ''' batches, so two clients cannot both read the same quantities and both write.
    '''
    ''' Shared by stock deduction and shrinkage recording because both decrement
    ''' <c>QuantityRemaining</c> on the same rows. They must take the lock in the same order, and a
    ''' single helper is what keeps that true as the two call sites evolve.
    ''' </summary>
    Friend Module StockBatchLock

        ''' <summary>
        ''' Locks every batch of <paramref name="productId"/> that still holds stock, ordered by
        ''' <c>(ReceiptDate, Id)</c> so concurrent callers queue instead of deadlocking — two clients
        ''' taking the same locks in different orders is the classic deadlock.
        '''
        ''' Requires an open transaction: the lock is held until that transaction ends, which is what
        ''' makes it serialize anything. The rows are locked but not consumed — callers re-read them
        ''' through EF, which then sees the state the lock protects.
        '''
        ''' EF Core cannot express <c>FOR UPDATE</c>, so this is raw ADO, driven through
        ''' <see cref="DbConnection"/>/<see cref="DbCommand"/> because the connection from
        ''' <c>Database.GetDbConnection()</c> belongs to <c>MySql.Data</c> and casting it to
        ''' <c>MySqlConnector.MySqlConnection</c> throws.
        '''
        ''' The table name comes from the EF model, so the <c>Inv_</c> prefix is applied in exactly
        ''' one place.
        ''' </summary>
        Friend Async Function LockInStockBatchesAsync(dbContext As InventoryDbContext, productId As Integer) As Task
            Dim connection = dbContext.Database.GetDbConnection()
            Dim currentTransaction = dbContext.Database.CurrentTransaction
            Dim tableName = dbContext.Model.FindEntityType(GetType(StockBatch)).GetTableName()

            Using command = connection.CreateCommand()
                ' Enlist the lock in the caller's transaction, or it is released immediately and
                ' serializes nothing.
                If currentTransaction IsNot Nothing Then
                    command.Transaction = currentTransaction.GetDbTransaction()
                End If

                command.CommandText =
                    $"SELECT Id FROM `{tableName}` " &
                    "WHERE ProductId = @productId AND QuantityRemaining > 0 " &
                    "ORDER BY ReceiptDate, Id FOR UPDATE"
                command.AddParameter("@productId", productId)

                Using reader = Await command.ExecuteReaderAsync()
                    While Await reader.ReadAsync()
                        ' Reading the row is what acquires the lock; there is nothing to consume.
                    End While
                End Using
            End Using
        End Function

    End Module

End Namespace
