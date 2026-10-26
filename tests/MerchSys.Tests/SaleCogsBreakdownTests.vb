Imports System.Threading
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Diagnostics
Imports Moq
Imports Xunit
Imports MediatR
Imports MerchSys.Inventory.Data
Imports MerchSys.Inventory.Entities
Imports MerchSys.Inventory.Handlers
Imports MerchSys.SharedKernel.Queries

Namespace Tests

    Public Class SaleCogsBreakdownTests

        <Fact>
        Public Async Function GetSaleCogsBreakdownQueryHandler_ShouldReturnCorrectBreakdown() As Task
            ' Arrange
            Dim options = New DbContextOptionsBuilder(Of InventoryDbContext)().
                UseInMemoryDatabase(databaseName:="InventoryDb_" & Guid.NewGuid().ToString()).
                Options

            Using context = New InventoryDbContext(options)
                context.SaleCogsRecords.Add(New SaleCogsRecord With {
                    .SaleId = 1,
                    .ProductId = 10,
                    .BatchId = 100,
                    .QuantityDeducted = 2,
                    .UnitCost = 15.0D,
                    .TotalCost = 30.0D,
                    .DeductedAt = DateTime.UtcNow
                })
                context.SaleCogsRecords.Add(New SaleCogsRecord With {
                    .SaleId = 1,
                    .ProductId = 10,
                    .BatchId = 101,
                    .QuantityDeducted = 3,
                    .UnitCost = 16.0D,
                    .TotalCost = 48.0D,
                    .DeductedAt = DateTime.UtcNow
                })
                context.SaleCogsRecords.Add(New SaleCogsRecord With {
                    .SaleId = 2, ' Different sale
                    .ProductId = 10,
                    .BatchId = 102,
                    .QuantityDeducted = 1,
                    .UnitCost = 17.0D,
                    .TotalCost = 17.0D,
                    .DeductedAt = DateTime.UtcNow
                })
                Await context.SaveChangesAsync()
            End Using

            ' Act
            Dim result As List(Of SaleCogsBreakdownDto)
            Using context = New InventoryDbContext(options)
                Dim handler = New GetSaleCogsBreakdownQueryHandler(context)
                result = Await handler.Handle(New GetSaleCogsBreakdownQuery With {.SaleId = 1}, CancellationToken.None)
            End Using

            ' Assert
            Assert.NotNull(result)
            Assert.Equal(2, result.Count)
            Assert.Equal(78.0D, result.Sum(Function(r) r.TotalCost))
        End Function



        <Fact>
        Public Async Function StockDeductedAccountingHandler_ShouldUseBreakdownWhenAvailable() As Task
            ' Arrange
            Dim options = New DbContextOptionsBuilder(Of Accounting.Data.AccountingDbContext)().
                UseInMemoryDatabase(databaseName:="AccountingDb_" & Guid.NewGuid().ToString()).
                ConfigureWarnings(Function(w) w.Ignore(InMemoryEventId.TransactionIgnoredWarning)).
                Options

            Dim mockMediator = New Mock(Of IMediator)()
            Dim breakdown = New List(Of SaleCogsBreakdownDto) From {
                New SaleCogsBreakdownDto With {.ProductId = 10, .TotalCost = 15.0D},
                New SaleCogsBreakdownDto With {.ProductId = 10, .TotalCost = 25.0D} ' Total 40.0D
            }
            mockMediator.Setup(Function(m) m.Send(It.IsAny(Of GetSaleCogsBreakdownQuery)(), It.IsAny(Of CancellationToken)())).
                ReturnsAsync(breakdown)

            Dim handlerEvent = New SharedKernel.Events.StockDeductedEvent With {
                .SaleId = 1,
                .DeductedAt = DateTime.UtcNow
            }
            handlerEvent.Items.Add(New SharedKernel.Events.StockDeductedEvent.StockDeductedItem With {
                .ProductId = 10,
                .QuantityDeducted = 2,
                .TotalCost = 35.0D ' Fallback cost
            })

            Using context = New Accounting.Data.AccountingDbContext(options)
                Dim handler = New Accounting.Handlers.StockDeductedAccountingHandler(context, mockMediator.Object)

                ' Act
                Await handler.Handle(handlerEvent, CancellationToken.None)
            End Using

            ' Assert
            Using context = New Accounting.Data.AccountingDbContext(options)
                Dim expense = Await context.ExpenseRecords.FirstOrDefaultAsync(Function(e) e.SourceReferenceId.HasValue AndAlso e.SourceReferenceId.Value = 1 AndAlso e.Category = "COGS")
                Assert.NotNull(expense)
                Assert.Equal(40.0D, expense.Amount) ' Uses exact breakdown

                Dim revenue = Await context.RevenueRecords.FirstOrDefaultAsync(Function(r) r.SourceTransactionId = 1 AndAlso r.ProductId = 10)
                Assert.NotNull(revenue)
                Assert.Equal(40.0D, revenue.TotalCogs)
                Assert.Equal(20.0D, revenue.UnitCost)
            End Using
        End Function

        <Fact>
        Public Async Function StockDeductedAccountingHandler_ShouldFallbackToItemCostWhenBreakdownFails() As Task
            ' Arrange
            Dim options = New DbContextOptionsBuilder(Of Accounting.Data.AccountingDbContext)().
                UseInMemoryDatabase(databaseName:="AccountingDb_" & Guid.NewGuid().ToString()).
                ConfigureWarnings(Function(w) w.Ignore(InMemoryEventId.TransactionIgnoredWarning)).
                Options

            Dim mockMediator = New Mock(Of IMediator)()
            mockMediator.Setup(Function(m) m.Send(It.IsAny(Of GetSaleCogsBreakdownQuery)(), It.IsAny(Of CancellationToken)())).
                ThrowsAsync(New Exception("Simulated Failure"))

            Dim handlerEvent = New SharedKernel.Events.StockDeductedEvent With {
                .SaleId = 2,
                .DeductedAt = DateTime.UtcNow
            }
            handlerEvent.Items.Add(New SharedKernel.Events.StockDeductedEvent.StockDeductedItem With {
                .ProductId = 20,
                .QuantityDeducted = 3,
                .TotalCost = 45.0D ' Fallback cost
            })

            Using context = New Accounting.Data.AccountingDbContext(options)
                Dim handler = New Accounting.Handlers.StockDeductedAccountingHandler(context, mockMediator.Object)

                ' Act
                Await handler.Handle(handlerEvent, CancellationToken.None)
            End Using

            ' Assert
            Using context = New Accounting.Data.AccountingDbContext(options)
                Dim expense = Await context.ExpenseRecords.FirstOrDefaultAsync(Function(e) e.SourceReferenceId.HasValue AndAlso e.SourceReferenceId.Value = 2 AndAlso e.Category = "COGS")
                Assert.NotNull(expense)
                Assert.Equal(45.0D, expense.Amount) ' Uses fallback

                Dim revenue = Await context.RevenueRecords.FirstOrDefaultAsync(Function(r) r.SourceTransactionId = 2 AndAlso r.ProductId = 20)
                Assert.NotNull(revenue)
                Assert.Equal(45.0D, revenue.TotalCogs)
                Assert.Equal(15.0D, revenue.UnitCost)
            End Using
        End Function
    End Class
End Namespace
