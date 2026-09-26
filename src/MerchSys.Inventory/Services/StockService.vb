Imports System.Data
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Configuration
Imports MySqlConnector
Imports MerchSys.Inventory.Data
Imports MerchSys.Inventory.Entities

Namespace Services

    Public Class StockService
        Implements IStockService

        Private ReadOnly _dbContext As InventoryDbContext
        Private ReadOnly _configuration As IConfiguration

        Public Sub New(dbContext As InventoryDbContext, configuration As IConfiguration)
            _dbContext = dbContext
            _configuration = configuration
        End Sub

        Public Async Function AddStockBatchAsync(productId As Integer, quantity As Integer, unitCost As Decimal, receiptDate As DateTime, sourcePurchaseOrderId As Integer, expiryDate As DateTime?) As Task Implements IStockService.AddStockBatchAsync
            Dim batch = New StockBatch With {
                .ProductId = productId,
                .QuantityReceived = quantity,
                .QuantityRemaining = quantity,
                .UnitCost = unitCost,
                .ReceiptDate = receiptDate,
                .SourcePurchaseOrderId = sourcePurchaseOrderId,
                .ExpiryDate = expiryDate
            }
            _dbContext.StockBatches.Add(batch)
            Await _dbContext.SaveChangesAsync()
        End Function

        Public Async Function DeductStockFIFOAsync(productId As Integer, quantity As Integer) As Task(Of FIFODeductionResult) Implements IStockService.DeductStockFIFOAsync
            Dim batches = Await _dbContext.StockBatches.
                Where(Function(b) b.ProductId = productId AndAlso b.QuantityRemaining > 0).
                OrderBy(Function(b) b.ReceiptDate).
                ToListAsync()

            Dim totalAvailable = batches.Sum(Function(b) b.QuantityRemaining)
            If totalAvailable < quantity Then
                Throw New InsufficientStockException($"Insufficient stock for product {productId}. Requested: {quantity}, Available: {totalAvailable}")
            End If

            Dim remainingToDeduct = quantity
            Dim deducted = 0

            For Each batch In batches
                If remainingToDeduct <= 0 Then
                    Exit For
                End If

                Dim deductAmount = Math.Min(batch.QuantityRemaining, remainingToDeduct)
                batch.QuantityRemaining -= deductAmount
                remainingToDeduct -= deductAmount
                deducted += deductAmount
            Next

            Await _dbContext.SaveChangesAsync()

            Return New FIFODeductionResult With {
                .ProductId = productId,
                .DeductedQuantity = deducted,
                .RemainingQuantityNeeded = remainingToDeduct,
                .Success = remainingToDeduct = 0
            }
        End Function

        Public Async Function GetCurrentStockLevelsAsync() As Task(Of List(Of StockLevelDto)) Implements IStockService.GetCurrentStockLevelsAsync
            Dim resultList = New List(Of StockLevelDto)()

            Dim connectionString = _configuration.GetSection("Sync")("MariaDbConnection")
            If String.IsNullOrWhiteSpace(connectionString) Then
                Throw New InvalidOperationException("MariaDbConnection string is not configured.")
            End If

            Dim query = "
                SELECT
                    p.Id AS ProductId,
                    COALESCE(SUM(b.QuantityRemaining), 0) AS CurrentQuantity,
                    p.MinimumThreshold
                FROM Inv_Products p
                LEFT JOIN Inv_StockBatches b ON p.Id = b.ProductId AND b.QuantityRemaining > 0 AND b.IsDeleted = 0
                WHERE p.IsDeleted = 0
                GROUP BY p.Id, p.MinimumThreshold"

            Using connection = New MySqlConnection(connectionString)
                Await connection.OpenAsync()
                Using command = New MySqlCommand(query, connection)
                    Using reader = Await command.ExecuteReaderAsync()
                        While Await reader.ReadAsync()
                            Dim productId = reader.GetInt32("ProductId")
                            Dim currentQuantity = reader.GetInt32("CurrentQuantity")
                            Dim minimumThreshold = reader.GetInt32("MinimumThreshold")

                            resultList.Add(New StockLevelDto With {
                                .ProductId = productId,
                                .CurrentQuantity = currentQuantity,
                                .ThresholdQuantity = minimumThreshold,
                                .IsBelowThreshold = currentQuantity < minimumThreshold
                            })
                        End While
                    End Using
                End Using
            End Using

            Return resultList
        End Function
    End Class

    Public Class InsufficientStockException
        Inherits Exception

        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
    End Class

End Namespace
