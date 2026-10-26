Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Diagnostics
Imports Xunit
Imports MediatR
Imports Moq
Imports MerchSys.Inventory.Data
Imports MerchSys.Inventory.Entities
Imports MerchSys.Inventory.Handlers
Imports MerchSys.SharedKernel.Queries

Public Class SaleCogsBreakdownTests
    <Fact>
    Public Async Function GetSaleCogsBreakdownQueryHandler_ReturnsExactDtos() As Task
        ' Arrange
        Dim options = New DbContextOptionsBuilder(Of InventoryDbContext)().
            UseInMemoryDatabase(databaseName:="TestInventoryDb_" & Guid.NewGuid().ToString()).
            ConfigureWarnings(Function(w) w.Ignore(InMemoryEventId.TransactionIgnoredWarning)).
            Options

        Using dbContext = New InventoryDbContext(options)
            dbContext.SaleCogsRecords.Add(New SaleCogsRecord With {
                .SaleId = 100, .ProductId = 1, .BatchId = 10, .QuantityDeducted = 2, .UnitCost = 10.0D, .TotalCost = 20.0D, .DeductedAt = DateTime.UtcNow
            })
            dbContext.SaleCogsRecords.Add(New SaleCogsRecord With {
                .SaleId = 100, .ProductId = 1, .BatchId = 11, .QuantityDeducted = 3, .UnitCost = 12.0D, .TotalCost = 36.0D, .DeductedAt = DateTime.UtcNow
            })
            dbContext.SaleCogsRecords.Add(New SaleCogsRecord With {
                .SaleId = 200, .ProductId = 1, .BatchId = 10, .QuantityDeducted = 1, .UnitCost = 10.0D, .TotalCost = 10.0D, .DeductedAt = DateTime.UtcNow
            })
            Await dbContext.SaveChangesAsync()
        End Using

        Dim result As List(Of SaleCogsBreakdownDto)
        Using dbContext = New InventoryDbContext(options)
            Dim handler = New GetSaleCogsBreakdownQueryHandler(dbContext)
            Dim query = New GetSaleCogsBreakdownQuery With {.SaleId = 100}

            ' Act
            result = Await handler.Handle(query, CancellationToken.None)
        End Using

        ' Assert
        Assert.NotNull(result)
        Assert.Equal(2, result.Count)
        Assert.All(result, Sub(r) Assert.Equal(100, r.SaleId))
        Assert.Equal(56.0D, result.Sum(Function(r) r.TotalCost))
    End Function

    <Fact>
    Public Async Function StockDeductedAccountingHandler_ComputesExactCogsWhenBreakdownReturned() As Task
        ' Arrange
        Dim options = New DbContextOptionsBuilder(Of MerchSys.Accounting.Data.AccountingDbContext)().
            UseInMemoryDatabase(databaseName:="TestAccDb_" & Guid.NewGuid().ToString()).
            ConfigureWarnings(Function(w) w.Ignore(InMemoryEventId.TransactionIgnoredWarning)).
            Options

        Dim mediatorMock = New Mock(Of IMediator)()
        Dim breakdown = New List(Of SaleCogsBreakdownDto) From {
            New SaleCogsBreakdownDto With {.SaleId = 300, .ProductId = 2, .BatchId = 20, .QuantityDeducted = 1, .UnitCost = 15.0D, .TotalCost = 15.0D},
            New SaleCogsBreakdownDto With {.SaleId = 300, .ProductId = 2, .BatchId = 21, .QuantityDeducted = 2, .UnitCost = 20.0D, .TotalCost = 40.0D}
        }
        mediatorMock.Setup(Function(m) m.Send(It.IsAny(Of GetSaleCogsBreakdownQuery)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(breakdown)

        Dim evt = New MerchSys.SharedKernel.Events.StockDeductedEvent With {
            .SaleId = 300,
            .DeductedAt = DateTime.UtcNow
        }
        evt.Items.Add(New MerchSys.SharedKernel.Events.StockDeductedEvent.StockDeductedItem With {
            .ProductId = 2, .QuantityDeducted = 3, .TotalCost = 50.0D ' 50 vs true 55
        })

        Using dbContext = New MerchSys.Accounting.Data.AccountingDbContext(options)
            Dim handler = New MerchSys.Accounting.Handlers.StockDeductedAccountingHandler(dbContext, mediatorMock.Object)

            ' Act
            Await handler.Handle(evt, CancellationToken.None)
        End Using

        Using dbContext = New MerchSys.Accounting.Data.AccountingDbContext(options)
            Dim expense = Await dbContext.ExpenseRecords.FirstOrDefaultAsync()
            Dim revenue = Await dbContext.RevenueRecords.FirstOrDefaultAsync()

            ' Assert
            Assert.NotNull(expense)
            Assert.Equal(55.0D, expense.Amount)

            Assert.NotNull(revenue)
            Assert.Equal(55.0D, revenue.TotalCogs)
        End Using
    End Function

    <Fact>
    Public Async Function StockDeductedAccountingHandler_FallsBackWhenBreakdownEmpty() As Task
        ' Arrange
        Dim options = New DbContextOptionsBuilder(Of MerchSys.Accounting.Data.AccountingDbContext)().
            UseInMemoryDatabase(databaseName:="TestAccDb_" & Guid.NewGuid().ToString()).
            ConfigureWarnings(Function(w) w.Ignore(InMemoryEventId.TransactionIgnoredWarning)).
            Options

        Dim mediatorMock = New Mock(Of IMediator)()
        mediatorMock.Setup(Function(m) m.Send(It.IsAny(Of GetSaleCogsBreakdownQuery)(), It.IsAny(Of CancellationToken)())).ReturnsAsync(New List(Of SaleCogsBreakdownDto)()) ' Empty

        Dim evt = New MerchSys.SharedKernel.Events.StockDeductedEvent With {
            .SaleId = 400,
            .DeductedAt = DateTime.UtcNow
        }
        evt.Items.Add(New MerchSys.SharedKernel.Events.StockDeductedEvent.StockDeductedItem With {
            .ProductId = 3, .QuantityDeducted = 2, .TotalCost = 45.0D
        })

        Using dbContext = New MerchSys.Accounting.Data.AccountingDbContext(options)
            Dim handler = New MerchSys.Accounting.Handlers.StockDeductedAccountingHandler(dbContext, mediatorMock.Object)

            ' Act
            Await handler.Handle(evt, CancellationToken.None)
        End Using

        Using dbContext = New MerchSys.Accounting.Data.AccountingDbContext(options)
            Dim expense = Await dbContext.ExpenseRecords.FirstOrDefaultAsync()
            Dim revenue = Await dbContext.RevenueRecords.FirstOrDefaultAsync()

            ' Assert
            Assert.NotNull(expense)
            Assert.Equal(45.0D, expense.Amount)

            Assert.NotNull(revenue)
            Assert.Equal(45.0D, revenue.TotalCogs)
        End Using
    End Function
End Class
