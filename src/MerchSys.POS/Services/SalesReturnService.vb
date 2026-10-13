Imports System.Text.Json
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

        ''' <summary>
        ''' Shape persisted in <see cref="SalesReturn.SerializedLines"/>.
        ''' </summary>
        Public Class ReturnedLineRecord
            Public Property ProductId As Integer
            Public Property Quantity As Integer
            Public Property UnitPrice As Decimal
        End Class

        Public Async Function ProcessReturnAsync(transactionId As Integer, returnQuantities As Dictionary(Of Integer, Integer), reason As String, isRestocked As Boolean) As Task(Of SalesReturn) Implements ISalesReturnService.ProcessReturnAsync
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

            ' Everything is aggregated by product, because product is the key the rest of the system uses:
            ' Inventory restocks by product and Accounting keys RevenueRecord on (sale, product).
            Dim originalByProduct = transaction.Lines.
                GroupBy(Function(l) l.ProductId).
                ToDictionary(Function(g) g.Key, Function(g) g.Sum(Function(l) l.Quantity))

            Dim previouslyReturnedByProduct = Await GetPreviouslyReturnedByProductAsync(transactionId)

            Dim requestedByProduct As New Dictionary(Of Integer, Integer)()
            Dim unitPriceByProduct As New Dictionary(Of Integer, Decimal)()

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

                If Not requestedByProduct.ContainsKey(line.ProductId) Then
                    requestedByProduct(line.ProductId) = 0
                    unitPriceByProduct(line.ProductId) = line.UnitPrice
                End If
                requestedByProduct(line.ProductId) += returnQty
            Next

            ' Previously this compared only against the original line quantity, so a customer could
            ' return more than they bought across several returns. SalesReturn now records its lines,
            ' so the cumulative figure is knowable and the limit can actually be enforced.
            For Each productId In requestedByProduct.Keys
                Dim alreadyReturned = If(previouslyReturnedByProduct.ContainsKey(productId), previouslyReturnedByProduct(productId), 0)
                Dim originalQuantity = If(originalByProduct.ContainsKey(productId), originalByProduct(productId), 0)

                If alreadyReturned + requestedByProduct(productId) > originalQuantity Then
                    Throw New InvalidOperationException(
                        $"Cannot return {requestedByProduct(productId)} of product {productId}: {originalQuantity} were purchased and {alreadyReturned} have already been returned.")
                End If
            Next

            Dim totalRefundAmount As Decimal = 0
            For Each productId In requestedByProduct.Keys
                totalRefundAmount += requestedByProduct(productId) * unitPriceByProduct(productId)
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

            Dim returnedLines = requestedByProduct.Keys.
                Select(Function(productId) New ReturnedLineRecord With {
                    .ProductId = productId,
                    .Quantity = requestedByProduct(productId),
                    .UnitPrice = unitPriceByProduct(productId)
                }).
                ToList()

            Dim salesReturn As New SalesReturn With {
                .SalesTransactionId = transactionId,
                .Reason = reason,
                .IsRestocked = isRestocked,
                .SerializedLines = JsonSerializer.Serialize(returnedLines)
            }

            _db.SalesReturns.Add(salesReturn)
            Await _db.SaveChangesAsync()

            ' Published only after the return is committed. It used to be published inside the loop
            ' above, before SaveChanges, so a failed save left stock restocked with no return record.
            Dim returnedEvent = New SaleReturnedEvent With {
                .SaleId = transactionId,
                .SalesReturnId = salesReturn.Id,
                .ReturnedAt = DateTime.UtcNow,
                .TotalRefundAmount = totalRefundAmount,
                .IsRestocked = isRestocked
            }

            For Each line In returnedLines
                returnedEvent.Items.Add(New SaleReturnedEvent.ReturnedItem With {
                    .ProductId = line.ProductId,
                    .QuantityReturned = line.Quantity,
                    .CumulativeQuantityReturned = line.Quantity + If(previouslyReturnedByProduct.ContainsKey(line.ProductId), previouslyReturnedByProduct(line.ProductId), 0),
                    .OriginalQuantitySold = If(originalByProduct.ContainsKey(line.ProductId), originalByProduct(line.ProductId), 0),
                    .UnitPrice = line.UnitPrice
                })
            Next

            Await _eventBus.PublishAsync(returnedEvent)

            Return salesReturn
        End Function

        ''' <summary>
        ''' Totals the quantities already returned for each product, read back from the line detail
        ''' stored on earlier <see cref="SalesReturn"/> rows.
        ''' </summary>
        Private Async Function GetPreviouslyReturnedByProductAsync(transactionId As Integer) As Task(Of Dictionary(Of Integer, Integer))
            Dim serialized = Await _db.SalesReturns _
                .Where(Function(r) r.SalesTransactionId = transactionId) _
                .Select(Function(r) r.SerializedLines) _
                .ToListAsync()

            Dim totals As New Dictionary(Of Integer, Integer)()

            For Each json In serialized
                If String.IsNullOrWhiteSpace(json) Then
                    Continue For
                End If

                Dim records = JsonSerializer.Deserialize(Of List(Of ReturnedLineRecord))(json)
                If records Is Nothing Then
                    Continue For
                End If

                For Each record In records
                    If Not totals.ContainsKey(record.ProductId) Then
                        totals(record.ProductId) = 0
                    End If
                    totals(record.ProductId) += record.Quantity
                Next
            Next

            Return totals
        End Function

        Public Async Function GetReturnsForTransactionAsync(transactionId As Integer) As Task(Of List(Of SalesReturn)) Implements ISalesReturnService.GetReturnsForTransactionAsync
            Dim returnList = Await _db.SalesReturns _
                .Where(Function(r) r.SalesTransactionId = transactionId) _
                .Select(Function(r) New With {
                    .Id = r.Id,
                    .SalesTransactionId = r.SalesTransactionId,
                    .Reason = r.Reason,
                    .IsRestocked = r.IsRestocked,
                    .SerializedLines = r.SerializedLines,
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
                .SerializedLines = r.SerializedLines,
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
