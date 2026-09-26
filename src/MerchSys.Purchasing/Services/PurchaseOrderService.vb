Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Purchasing.Data
Imports MerchSys.Purchasing.Entities
Imports System.Threading.Tasks

Namespace Services
    Public Class PurchaseOrderService
        Private ReadOnly _dbContext As PurchasingDbContext

        Public Sub New(dbContext As PurchasingDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function CreateDraftAsync() As Task(Of PurchaseOrder)
            ' Find the highest order number including soft-deleted items so we don't duplicate sequence numbers.
            ' Policy A: Sequence skips deleted (OrderNumber is deterministic auto-generated).
            ' Note: Keep pre-existing hotfix... (There wasn't one found, but we maintain deterministic increment).
            Dim maxOrderStr = Await _dbContext.Set(Of PurchaseOrder)().
                IgnoreQueryFilters().
                OrderByDescending(Function(p) p.OrderNumber).
                Select(Function(p) p.OrderNumber).
                FirstOrDefaultAsync()

            Dim maxOrderNumber As Long = 0
            If Not String.IsNullOrEmpty(maxOrderStr) Then
                Long.TryParse(maxOrderStr, maxOrderNumber)
            End If

            Dim newOrderNumber As Long = maxOrderNumber + 1

            Dim newPo As New PurchaseOrder With {
                .OrderNumber = newOrderNumber.ToString()
            }

            _dbContext.Set(Of PurchaseOrder)().Add(newPo)
            Await _dbContext.SaveChangesAsync()

            Return newPo
        End Function
    End Class
End Namespace
