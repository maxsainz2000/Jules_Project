Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.POS.Data
Imports MerchSys.POS.Entities
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.SharedKernel.Events

Namespace Services
    Public Class SalesReturnService
        Implements ISalesReturnService

        Private ReadOnly _db As POSDbContext
        Private ReadOnly _eventBus As IEventBus

        Public Sub New(db As POSDbContext, eventBus As IEventBus)
            _db = db
            _eventBus = eventBus
        End Sub

        Public Async Function ProcessReturnAsync(transactionId As Integer, returnQuantities As Dictionary(Of Integer, Integer), reason As String, isRestocked As Boolean) As Task(Of SalesReturn) Implements ISalesReturnService.ProcessReturnAsync
            ' Note: Using anonymous projection for ToListAsync bug is only necessary for ToListAsync.
            ' FirstOrDefaultAsync can be used normally.
            Dim transactionData = Await _db.SalesTransactions _
                .Where(Function(t) t.Id = transactionId) _
                .Select(Function(t) New With {
                    .Id = t.Id,
                    .TransactionNumber = t.TransactionNumber,
                    .TransactionDate = t.TransactionDate,
                    .TotalAmount = t.TotalAmount,
                    .PaymentMethod = t.PaymentMethod,
                    .IsVoid = t.IsVoid,
                    .CreditAccount = t.CreditAccount,
                    .Lines = t.Lines
                }) _
                .FirstOrDefaultAsync()

            If transactionData Is Nothing Then
                Throw New ArgumentException($"Transaction with ID {transactionId} not found.")
            End If

            Dim transaction = New SalesTransaction With {
                .Id = transactionData.Id,
                .TransactionNumber = transactionData.TransactionNumber,
                .TransactionDate = transactionData.TransactionDate,
                .TotalAmount = transactionData.TotalAmount,
                .PaymentMethod = transactionData.PaymentMethod,
                .IsVoid = transactionData.IsVoid,
                .CreditAccount = transactionData.CreditAccount,
                .Lines = transactionData.Lines
            }

            If transaction.IsVoid Then
                Throw New InvalidOperationException("Cannot process a return for a voided transaction.")
            End If

            Dim totalRefundAmount As Decimal = 0

            For Each kvp In returnQuantities
                Dim lineId = kvp.Key
                Dim returnQty = kvp.Value

                If returnQty <= 0 Then
                    Continue For
                End If

                Dim line = transaction.Lines.FirstOrDefault(Function(l) l.Id = lineId)
                If line Is Nothing Then
                    Throw New ArgumentException($"Line ID {lineId} does not belong to transaction ID {transactionId}.")
                End If

                ' To enforce return limits correctly we would need to check past returns for quantities.
                ' Since SalesReturn entity does not store returned quantities per line currently,
                ' we will validate against the original line quantity, assuming no previous returns for simplicity unless we extend SalesReturn.
                ' According to instructions, SalesReturn does not store individual returned line items, validation must be handled in-memory.
                If returnQty > line.Quantity Then
                    Throw New InvalidOperationException($"Cannot return {returnQty} of product {line.ProductName}. Only {line.Quantity} were purchased.")
                End If

                totalRefundAmount += returnQty * line.UnitPrice

                If isRestocked Then
                    Dim evt As New StockReturnedEvent(line.ProductId, returnQty)
                    Await _eventBus.PublishAsync(evt)
                End If
            Next

            If totalRefundAmount <= 0 Then
                Throw New InvalidOperationException("No items were returned.")
            End If

            If transaction.PaymentMethod = MerchSys.SharedKernel.Enums.PaymentMethod.Credit.ToString() AndAlso transaction.CreditAccount IsNot Nothing Then
                transaction.CreditAccount.CurrentBalance -= totalRefundAmount
                If transaction.CreditAccount.CurrentBalance < 0 Then
                    transaction.CreditAccount.CurrentBalance = 0
                End If
                _db.CreditAccounts.Update(transaction.CreditAccount)
            End If

            Dim salesReturn As New SalesReturn With {
                .SalesTransactionId = transactionId,
                .Reason = reason,
                .IsRestocked = isRestocked
            }

            _db.SalesReturns.Add(salesReturn)
            Await _db.SaveChangesAsync()

            Return salesReturn
        End Function

        Public Async Function GetReturnsForTransactionAsync(transactionId As Integer) As Task(Of List(Of SalesReturn)) Implements ISalesReturnService.GetReturnsForTransactionAsync
            Dim returnList = Await _db.SalesReturns _
                .Where(Function(r) r.SalesTransactionId = transactionId) _
                .Select(Function(r) New With {
                    .Id = r.Id,
                    .SalesTransactionId = r.SalesTransactionId,
                    .Reason = r.Reason,
                    .IsRestocked = r.IsRestocked,
                    .SalesTransaction = r.SalesTransaction,
                    .CreatedAt = r.CreatedAt,
                    .CreatedBy = r.CreatedBy,
                    .ModifiedAt = r.ModifiedAt,
                    .ModifiedBy = r.ModifiedBy
                }) _
                .ToListAsync()

            Return returnList.Select(Function(r) New SalesReturn With {
                .Id = r.Id,
                .SalesTransactionId = r.SalesTransactionId,
                .Reason = r.Reason,
                .IsRestocked = r.IsRestocked,
                .SalesTransaction = r.SalesTransaction,
                .CreatedAt = r.CreatedAt,
                .CreatedBy = r.CreatedBy,
                .ModifiedAt = r.ModifiedAt,
                .ModifiedBy = r.ModifiedBy
            }).ToList()
        End Function

        Public Async Function GetReturnHistoryAsync() As Task(Of List(Of SalesReturn)) Implements ISalesReturnService.GetReturnHistoryAsync
            Dim returnList = Await _db.SalesReturns _
                .Select(Function(r) New With {
                    .Id = r.Id,
                    .SalesTransactionId = r.SalesTransactionId,
                    .Reason = r.Reason,
                    .IsRestocked = r.IsRestocked,
                    .SalesTransaction = r.SalesTransaction,
                    .CreatedAt = r.CreatedAt,
                    .CreatedBy = r.CreatedBy,
                    .ModifiedAt = r.ModifiedAt,
                    .ModifiedBy = r.ModifiedBy
                }) _
                .ToListAsync()

            Return returnList.Select(Function(r) New SalesReturn With {
                .Id = r.Id,
                .SalesTransactionId = r.SalesTransactionId,
                .Reason = r.Reason,
                .IsRestocked = r.IsRestocked,
                .SalesTransaction = r.SalesTransaction,
                .CreatedAt = r.CreatedAt,
                .CreatedBy = r.CreatedBy,
                .ModifiedAt = r.ModifiedAt,
                .ModifiedBy = r.ModifiedBy
            }).ToList()
        End Function
    End Class
End Namespace