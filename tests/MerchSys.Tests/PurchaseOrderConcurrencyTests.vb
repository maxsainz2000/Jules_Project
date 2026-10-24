Imports Xunit
Imports System
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Purchasing.Entities
Imports MerchSys.Purchasing.Services
Imports MerchSys.SharedKernel.Enums
Imports MerchSys.SharedKernel.Exceptions

''' <summary>
''' DB-backed coverage for C6: <c>PurchaseOrder</c> carries an optimistic concurrency token, so the
''' status transitions can no longer be check-then-act.
'''
''' Needs a live MariaDB; builds and drops its own throwaway database via
''' <see cref="TestDatabaseFixture"/>. Run with:
'''   dotnet test --filter "Category=Integration"
''' </summary>
<Trait("Category", "Integration")>
<Collection("IntegrationDatabase")>
Public Class PurchaseOrderConcurrencyTests
    Implements IClassFixture(Of TestDatabaseFixture)

    Private ReadOnly _fixture As TestDatabaseFixture

    Public Sub New(fixture As TestDatabaseFixture)
        _fixture = fixture
    End Sub

    ''' <summary>
    ''' Closing an order books an accounts-payable entry. With no token the status check was
    ''' check-then-act, so two clients could both pass it and the liability was counted twice.
    ''' </summary>
    <Fact>
    Public Async Function ClosingAnOrderAnotherClientAlreadyClosedSurfacesTheConflict() As Task
        Dim orderId = Await CreateOrderAsync(PurchaseOrderStatus.Verified)

        Using ctxA = _fixture.NewPurchasing()
            Using ctxB = _fixture.NewPurchasing()
                ' Both clients read the order before either writes — the lost-update setup. The token
                ' only guards a write based on a stale read, so the reads have to come first.
                Dim orderA = Await ctxA.PurchaseOrders.FirstAsync(Function(p) p.Id = orderId)
                Dim orderB = Await ctxB.PurchaseOrders.FirstAsync(Function(p) p.Id = orderId)
                Assert.Equal(PurchaseOrderStatus.Verified, orderA.Status)
                Assert.Equal(PurchaseOrderStatus.Verified, orderB.Status)

                Await New PurchaseOrderService(ctxA).CloseAsync(orderId)

                Await Assert.ThrowsAsync(Of ConcurrentModificationException)(
                    Function() New PurchaseOrderService(ctxB).CloseAsync(orderId))
            End Using
        End Using

        Using ctx = _fixture.NewPurchasing()
            Dim order = Await ctx.PurchaseOrders.FirstAsync(Function(p) p.Id = orderId)
            Assert.Equal(PurchaseOrderStatus.Closed, order.Status)

            ' The losing client must not have booked a second liability.
            Assert.Equal(1, Await ctx.Set(Of AccountsPayableEntry)().
                Where(Function(a) a.PurchaseOrderId.HasValue AndAlso a.PurchaseOrderId.Value = orderId).
                CountAsync())
        End Using
    End Function

    ''' <summary>Any transition refuses a write based on a stale read, not just <c>Close</c>.</summary>
    <Fact>
    Public Async Function SubmittingAnOrderAnotherClientAlreadySubmittedSurfacesTheConflict() As Task
        Dim orderId = Await CreateOrderAsync(PurchaseOrderStatus.Draft)

        Using ctxA = _fixture.NewPurchasing()
            Using ctxB = _fixture.NewPurchasing()
                Dim orderA = Await ctxA.PurchaseOrders.FirstAsync(Function(p) p.Id = orderId)
                Dim orderB = Await ctxB.PurchaseOrders.FirstAsync(Function(p) p.Id = orderId)
                Assert.Equal(PurchaseOrderStatus.Draft, orderA.Status)
                Assert.Equal(PurchaseOrderStatus.Draft, orderB.Status)

                Await New PurchaseOrderService(ctxA).SubmitAsync(orderId)

                Await Assert.ThrowsAsync(Of ConcurrentModificationException)(
                    Function() New PurchaseOrderService(ctxB).SubmitAsync(orderId))
            End Using
        End Using

        Using ctx = _fixture.NewPurchasing()
            Assert.Equal(PurchaseOrderStatus.Submitted,
                         (Await ctx.PurchaseOrders.FirstAsync(Function(p) p.Id = orderId)).Status)
        End Using
    End Function

    Private Async Function CreateOrderAsync(status As PurchaseOrderStatus) As Task(Of Integer)
        Using ctx = _fixture.NewPurchasing()
            Dim order As New PurchaseOrder With {
                .OrderNumber = "PO-CONC-" & Guid.NewGuid().ToString("N").Substring(0, 8),
                .VendorId = 1,
                .Status = status,
                .TotalAmount = 100D,
                .CreatedAt = DateTime.UtcNow
            }
            ctx.PurchaseOrders.Add(order)
            Await ctx.SaveChangesAsync()
            Return order.Id
        End Using
    End Function

End Class
