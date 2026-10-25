Imports System.Data.Common
Imports System.Text.Json
Imports System.Threading
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Logging
Imports MerchSys.Inventory.Data
Imports MerchSys.Inventory.Services
Imports MerchSys.SharedKernel.Events
Imports MerchSys.SharedKernel.Exceptions

Namespace Handlers

    ''' <summary>
    ''' Deducts stock for a completed sale.
    '''
    ''' The whole item set runs in <b>one</b> transaction. It previously saved each item
    ''' independently, so a failure on item 3 left items 1 and 2 deducted with no rollback — and
    ''' because <c>StockDeductedEvent</c> is only published after the loop, those deductions were
    ''' never reported to Accounting either, leaving their revenue with no COGS against it. Wrapping
    ''' the loop and publishing after the commit fixes both halves.
    ''' </summary>
    Public Class SaleCompletedHandler
        Implements INotificationHandler(Of SaleCompletedEvent)

        Private ReadOnly _stockService As IStockService
        Private ReadOnly _dbContext As InventoryDbContext
        Private ReadOnly _logger As ILogger(Of SaleCompletedHandler)
        Private ReadOnly _mediator As IMediator

        Public Sub New(stockService As IStockService, dbContext As InventoryDbContext, logger As ILogger(Of SaleCompletedHandler), mediator As IMediator)
            _stockService = stockService
            _dbContext = dbContext
            _logger = logger
            _mediator = mediator
        End Sub

        Public Async Function Handle(notification As SaleCompletedEvent, cancellationToken As CancellationToken) As Task Implements INotificationHandler(Of SaleCompletedEvent).Handle
            Dim deductedEvent = New StockDeductedEvent With {
                .SaleId = notification.SaleId,
                .DeductedAt = DateTime.UtcNow
            }

            Dim failureException As Exception = Nothing
            Dim failedProductId As Integer = 0
            Dim failedQuantity As Integer = 0

            ' DeductStockFIFOAsync joins this transaction instead of starting its own, so every item
            ' shares one unit of work. Disposing without committing is the rollback path — that is
            ' what makes a failure on any item undo the whole set.
            Try
                Using transaction = Await _dbContext.Database.BeginTransactionAsync(cancellationToken)
                    For Each item In notification.Items
                        Dim result As FIFODeductionResult
                        Try
                            result = Await _stockService.DeductStockFIFOAsync(item.ProductId, item.QuantitySold)
                        Catch ex As DbUpdateConcurrencyException
                            ' AGENTS.md: surface the conflict, never silently overwrite. With FOR UPDATE in
                            ' place this should not happen for the batch rows any more, but a writer that
                            ' does not take the lock can still move them.
                            _logger.LogError(ex, "Concurrent modification while deducting stock for Sale {SaleId}, Product {ProductId}", notification.SaleId, item.ProductId)
                            failedProductId = item.ProductId
                            failedQuantity = item.QuantitySold
                            failureException = New ConcurrentModificationException($"Stock for product {item.ProductId} was changed by another client while processing sale {notification.SaleId}.")
                            Exit For
                        Catch ex As InsufficientStockException
                            _logger.LogError(ex, "Failed to deduct stock for Sale {SaleId}, Product {ProductId}", notification.SaleId, item.ProductId)
                            failedProductId = item.ProductId
                            failedQuantity = item.QuantitySold
                            failureException = ex
                            Exit For
                        End Try

                        ' Capture the cost of what was actually consumed. This is the only point at which
                        ' the true FIFO cost of the sale is knowable — nothing records which batches a
                        ' sale consumed, so once the batches are decremented it cannot be reconstructed.
                        deductedEvent.Items.Add(New StockDeductedEvent.StockDeductedItem With {
                            .ProductId = item.ProductId,
                            .QuantityDeducted = result.DeductedQuantity,
                            .TotalCost = result.TotalCost
                        })
                    Next

                    If failureException Is Nothing Then
                        Await transaction.CommitAsync(cancellationToken)
                    End If
                End Using
            Finally
                ' The transaction is rolled back by the Using block if we exited with an exception.
            End Try

            If failureException IsNot Nothing Then
                Dim connection = _dbContext.Database.GetDbConnection()
                If connection.State <> System.Data.ConnectionState.Open Then
                    Await connection.OpenAsync(cancellationToken)
                End If

                Using command = connection.CreateCommand()
                    command.CommandText = "INSERT INTO App_AuditLog (OccurredAt, Actor, WriteContext, Module, EntityType, EntityId, Action, ChangesJson) " &
                                          "VALUES (@OccurredAt, @Actor, @WriteContext, @Module, @EntityType, @EntityId, @Action, @ChangesJson);"

                    Dim pOccurredAt = command.CreateParameter()
                    pOccurredAt.ParameterName = "@OccurredAt"
                    pOccurredAt.Value = DateTime.UtcNow
                    command.Parameters.Add(pOccurredAt)

                    Dim pActor = command.CreateParameter()
                    pActor.ParameterName = "@Actor"
                    pActor.Value = "System"
                    command.Parameters.Add(pActor)

                    Dim pWriteContext = command.CreateParameter()
                    pWriteContext.ParameterName = "@WriteContext"
                    pWriteContext.Value = "System"
                    command.Parameters.Add(pWriteContext)

                    Dim pModule = command.CreateParameter()
                    pModule.ParameterName = "@Module"
                    pModule.Value = "Inv"
                    command.Parameters.Add(pModule)

                    Dim pEntityType = command.CreateParameter()
                    pEntityType.ParameterName = "@EntityType"
                    pEntityType.Value = "SalesTransaction"
                    command.Parameters.Add(pEntityType)

                    Dim pEntityId = command.CreateParameter()
                    pEntityId.ParameterName = "@EntityId"
                    pEntityId.Value = notification.SaleId.ToString()
                    command.Parameters.Add(pEntityId)

                    Dim pAction = command.CreateParameter()
                    pAction.ParameterName = "@Action"
                    pAction.Value = "DeductFail"
                    command.Parameters.Add(pAction)

                    Dim pChangesJson = command.CreateParameter()
                    pChangesJson.ParameterName = "@ChangesJson"
                    pChangesJson.Value = JsonSerializer.Serialize(New With {
                        .Reason = failureException.Message,
                        .ProductId = failedProductId,
                        .Quantity = failedQuantity
                    })
                    command.Parameters.Add(pChangesJson)

                    Await command.ExecuteNonQueryAsync(cancellationToken)
                End Using

                Throw failureException
            End If

            ' Published only after the commit: a deduction that rolled back must never be announced.
            If deductedEvent.Items.Count > 0 Then
                Await _mediator.Publish(deductedEvent, cancellationToken)
            End If
        End Function
    End Class

End Namespace
