Imports MerchSys.POS.Data
Imports MerchSys.POS.Entities
Imports MerchSys.SharedKernel.Events
Imports Microsoft.EntityFrameworkCore
Imports MediatR

Namespace Services

    Public Class SalesReturnService
        Implements ISalesReturnService

        Private ReadOnly _dbContext As POSDbContext
        Private ReadOnly _mediator As IMediator

        Public Sub New(dbContext As POSDbContext, mediator As IMediator)
            _dbContext = dbContext
            _mediator = mediator
        End Sub

        Public Async Function ProcessReturnAsync(transactionId As Integer, returnQuantities As Dictionary(Of Integer, Integer), reason As String, restock As Boolean, user As String) As Task(Of SalesReturn) Implements ISalesReturnService.ProcessReturnAsync
            If String.IsNullOrWhiteSpace(reason) Then
                Throw New ArgumentException("Return reason is required.", NameOf(reason))
            End If

            If returnQuantities Is Nothing OrElse returnQuantities.Count = 0 Then
                Throw New ArgumentException("At least one item must be returned.", NameOf(returnQuantities))
            End If

            Dim transaction = Await _dbContext.SalesTransactions.
                Include(Function(t) t.Lines).
                Include(Function(t) t.CreditAccount).
                FirstOrDefaultAsync(Function(t) t.Id = transactionId AndAlso Not t.IsDeleted)

            If transaction Is Nothing Then
                Throw New InvalidOperationException("Transaction not found or has been deleted.")
            End If

            If transaction.IsVoid Then
                Throw New InvalidOperationException("Cannot return items from a voided transaction.")
            End If

            Dim existingReturnsProjection = Await _dbContext.SalesReturns.
                Where(Function(r) r.SalesTransactionId = transactionId).
                Select(Function(r) New With { .Id = r.Id }).
                ToListAsync()

            Dim returnRefundAmount As Decimal = 0

            ' Currently assuming return quantities are for lines that haven't been fully returned yet.
            ' In a complete robust system, we would check how many have already been returned
            ' across all previous returns for this transaction to enforce limits.
            ' Assuming here we don't have a SalesReturnLine entity yet in the Entities,
            ' we'll just check against the original line's maximum quantity.

            For Each kvp In returnQuantities
                Dim lineId = kvp.Key
                Dim returnQty = kvp.Value

                Dim line = transaction.Lines.FirstOrDefault(Function(l) l.Id = lineId)
                If line Is Nothing Then
                    Throw New InvalidOperationException($"Line {lineId} not found in transaction.")
                End If

                If returnQty > line.Quantity Then
                    Throw New InvalidOperationException($"Return quantity for product {line.ProductName} exceeds purchased quantity.")
                End If

                If returnQty <= 0 Then
                    Throw New InvalidOperationException($"Return quantity for product {line.ProductName} must be greater than zero.")
                End If

                returnRefundAmount += (line.UnitPrice * returnQty)
            Next

            If transaction.PaymentMethod = "Credit" AndAlso transaction.CreditAccount IsNot Nothing Then
                ' Adjust credit account balance by reducing the current balance.
                transaction.CreditAccount.CurrentBalance -= returnRefundAmount
                If transaction.CreditAccount.CurrentBalance < 0 Then
                    transaction.CreditAccount.CurrentBalance = 0
                End If

                ' Also recalculate blocked status
                If transaction.CreditAccount.CurrentBalance <= 0 Then
                    transaction.CreditAccount.IsBlocked = False
                End If

                transaction.CreditAccount.ModifiedAt = DateTime.UtcNow
                transaction.CreditAccount.ModifiedBy = user
            End If

            Dim newReturn As New SalesReturn With {
                .SalesTransactionId = transactionId,
                .Reason = reason,
                .IsRestocked = restock,
                .CreatedAt = DateTime.UtcNow,
                .CreatedBy = user,
                .ModifiedAt = DateTime.UtcNow,
                .ModifiedBy = user
            }

            _dbContext.SalesReturns.Add(newReturn)
            Await _dbContext.SaveChangesAsync()

            If restock Then
                Dim stockReturnedEvent As New StockReturnedEvent With {
                    .ReturnId = newReturn.Id,
                    .ReturnDate = DateTime.UtcNow
                }

                For Each kvp In returnQuantities
                    Dim lineId = kvp.Key
                    Dim returnQty = kvp.Value
                    Dim line = transaction.Lines.First(Function(l) l.Id = lineId)

                    stockReturnedEvent.Items.Add(New StockReturnedEvent.StockReturnedItem With {
                        .ProductId = line.ProductId,
                        .Quantity = returnQty
                    })
                Next

                Await _mediator.Publish(stockReturnedEvent)
            End If

            Return newReturn
        End Function

        Public Async Function GetReturnsForTransactionAsync(transactionId As Integer) As Task(Of List(Of SalesReturn)) Implements ISalesReturnService.GetReturnsForTransactionAsync
            Dim projections = Await _dbContext.SalesReturns.
                Where(Function(r) r.SalesTransactionId = transactionId).
                Select(Function(r) New With {
                    .Id = r.Id,
                    .SalesTransactionId = r.SalesTransactionId,
                    .Reason = r.Reason,
                    .IsRestocked = r.IsRestocked,
                    .CreatedAt = r.CreatedAt,
                    .CreatedBy = r.CreatedBy,
                    .ModifiedAt = r.ModifiedAt,
                    .ModifiedBy = r.ModifiedBy
                }).ToListAsync()

            Dim results As New List(Of SalesReturn)()
            For Each p In projections
                results.Add(New SalesReturn With {
                    .Id = p.Id,
                    .SalesTransactionId = p.SalesTransactionId,
                    .Reason = p.Reason,
                    .IsRestocked = p.IsRestocked,
                    .CreatedAt = p.CreatedAt,
                    .CreatedBy = p.CreatedBy,
                    .ModifiedAt = p.ModifiedAt,
                    .ModifiedBy = p.ModifiedBy
                })
            Next

            Return results
        End Function

        Public Async Function GetReturnHistoryAsync(startDate As DateTime, endDate As DateTime) As Task(Of List(Of SalesReturn)) Implements ISalesReturnService.GetReturnHistoryAsync
            Dim projections = Await _dbContext.SalesReturns.
                Where(Function(r) r.CreatedAt >= startDate AndAlso r.CreatedAt <= endDate).
                Select(Function(r) New With {
                    .Id = r.Id,
                    .SalesTransactionId = r.SalesTransactionId,
                    .Reason = r.Reason,
                    .IsRestocked = r.IsRestocked,
                    .CreatedAt = r.CreatedAt,
                    .CreatedBy = r.CreatedBy,
                    .ModifiedAt = r.ModifiedAt,
                    .ModifiedBy = r.ModifiedBy
                }).ToListAsync()

            Dim results As New List(Of SalesReturn)()
            For Each p In projections
                results.Add(New SalesReturn With {
                    .Id = p.Id,
                    .SalesTransactionId = p.SalesTransactionId,
                    .Reason = p.Reason,
                    .IsRestocked = p.IsRestocked,
                    .CreatedAt = p.CreatedAt,
                    .CreatedBy = p.CreatedBy,
                    .ModifiedAt = p.ModifiedAt,
                    .ModifiedBy = p.ModifiedBy
                })
            Next

            Return results
        End Function

    End Class

End Namespace
