Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Purchasing.Data
Imports MerchSys.Purchasing.Entities

Namespace Services
    Public Class AccountsPayableService
        Implements IAccountsPayableService

        Private ReadOnly _dbContext As PurchasingDbContext

        Public Sub New(dbContext As PurchasingDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function CreateAsync(dto As CreateAccountsPayableDto) As Task(Of Integer) Implements IAccountsPayableService.CreateAsync
            Dim entry As New AccountsPayableEntry With {
                .VendorId = dto.VendorId,
                .PurchaseOrderId = dto.PurchaseOrderId,
                .InvoiceDate = dto.InvoiceDate,
                .DueDate = dto.DueDate,
                .TotalAmount = dto.TotalAmount,
                .AmountPaid = 0,
                .IsPaid = False
            }

            _dbContext.AccountsPayable.Add(entry)
            Await _dbContext.SaveChangesAsync()

            Return entry.Id
        End Function

        Public Async Function RecordPaymentAsync(id As Integer, amount As Decimal) As Task Implements IAccountsPayableService.RecordPaymentAsync
            Dim entry = Await _dbContext.AccountsPayable.FindAsync(id)
            If entry Is Nothing Then Throw New InvalidOperationException("Accounts payable entry not found.")

            entry.AmountPaid += amount
            If entry.AmountPaid >= entry.TotalAmount Then
                entry.IsPaid = True
            End If

            Await _dbContext.SaveChangesAsync()
        End Function

        Private Function GetProjectedQuery() As IQueryable(Of AccountsPayableDto)
            Return _dbContext.AccountsPayable.
                GroupJoin(_dbContext.Vendors, Function(ap) ap.VendorId, Function(v) v.Id, Function(ap, v) New With {ap, v}).
                SelectMany(Function(x) x.v.DefaultIfEmpty(), Function(x, v) New With {x.ap, v}).
                GroupJoin(_dbContext.PurchaseOrders, Function(x) x.ap.PurchaseOrderId, Function(po) CType(po.Id, Integer?), Function(x, po) New With {x.ap, x.v, po}).
                SelectMany(Function(x) x.po.DefaultIfEmpty(), Function(x, po) New AccountsPayableDto With {
                    .Id = x.ap.Id,
                    .VendorId = x.ap.VendorId,
                    .VendorName = If(x.v Is Nothing, String.Empty, x.v.Name),
                    .PurchaseOrderId = x.ap.PurchaseOrderId,
                    .OrderNumber = If(po Is Nothing, String.Empty, po.OrderNumber),
                    .InvoiceDate = x.ap.InvoiceDate,
                    .DueDate = x.ap.DueDate,
                    .TotalAmount = x.ap.TotalAmount,
                    .AmountPaid = x.ap.AmountPaid,
                    .IsPaid = x.ap.IsPaid
                })
        End Function

        Public Async Function GetOutstandingAsync() As Task(Of List(Of AccountsPayableDto)) Implements IAccountsPayableService.GetOutstandingAsync
            Return Await GetProjectedQuery().Where(Function(e) Not e.IsPaid).ToListAsync()
        End Function

        Public Async Function GetOverdueAsync() As Task(Of List(Of AccountsPayableDto)) Implements IAccountsPayableService.GetOverdueAsync
            Return Await GetProjectedQuery().Where(Function(e) Not e.IsPaid AndAlso e.DueDate < DateTime.UtcNow).ToListAsync()
        End Function

        Public Async Function GetByVendorAsync(vendorId As Integer) As Task(Of List(Of AccountsPayableDto)) Implements IAccountsPayableService.GetByVendorAsync
            Return Await GetProjectedQuery().Where(Function(e) e.VendorId = vendorId).ToListAsync()
        End Function

        Public Async Function GetAllAsync() As Task(Of List(Of AccountsPayableDto)) Implements IAccountsPayableService.GetAllAsync
            Return Await GetProjectedQuery().OrderByDescending(Function(e) e.InvoiceDate).ToListAsync()
        End Function
    End Class
End Namespace
