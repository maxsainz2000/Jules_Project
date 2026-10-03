Imports System.Collections.Concurrent
Imports MerchSys.POS.Data
Imports MerchSys.POS.Entities
Imports Microsoft.EntityFrameworkCore

Namespace Services

    Public Class CartService
        Implements ICartService

        Private ReadOnly _db As POSDbContext
        Private Shared ReadOnly _carts As New ConcurrentDictionary(Of String, CartDto)()

        Public Sub New(db As POSDbContext)
            _db = db
        End Sub

        Public Function GetCart(cartId As String) As CartDto Implements ICartService.GetCart
            Return _carts.GetOrAdd(cartId, Function(id) New CartDto() With {.CartId = id})
        End Function

        Public Sub AddToCart(cartId As String, productId As Integer, productName As String, unitPrice As Decimal, quantity As Integer) Implements ICartService.AddToCart
            Dim cart = GetCart(cartId)
            SyncLock cart
                Dim existingLine = cart.Lines.FirstOrDefault(Function(l) l.ProductId = productId)
                If existingLine IsNot Nothing Then
                    existingLine.Quantity += quantity
                    existingLine.LineTotal = existingLine.UnitPrice * existingLine.Quantity
                Else
                    cart.Lines.Add(New CartLineDto With {
                        .ProductId = productId,
                        .ProductName = productName,
                        .UnitPrice = unitPrice,
                        .Quantity = quantity,
                        .LineTotal = unitPrice * quantity
                    })
                End If
                RecalculateCart(cart)
            End SyncLock
        End Sub

        Public Sub RemoveFromCart(cartId As String, productId As Integer) Implements ICartService.RemoveFromCart
            Dim cart = GetCart(cartId)
            SyncLock cart
                Dim lineToRemove = cart.Lines.FirstOrDefault(Function(l) l.ProductId = productId)
                If lineToRemove IsNot Nothing Then
                    cart.Lines.Remove(lineToRemove)
                    RecalculateCart(cart)
                End If
            End SyncLock
        End Sub

        Public Sub ClearCart(cartId As String) Implements ICartService.ClearCart
            Dim cart As CartDto = Nothing
            _carts.TryRemove(cartId, cart)
        End Sub

        Private Sub RecalculateCart(cart As CartDto)
            cart.TotalAmount = cart.Lines.Sum(Function(l) l.LineTotal)
            cart.VatableSales = Math.Round(cart.TotalAmount / 1.12D, 2)
            cart.VatAmount = cart.TotalAmount - cart.VatableSales
        End Sub

        Public Async Function FinalizeTransactionAsync(cartId As String, paymentMethod As String, Optional creditAccountId As Integer? = Nothing) As Task(Of SalesTransaction) Implements ICartService.FinalizeTransactionAsync
            Dim cart = GetCart(cartId)

            If Not cart.Lines.Any() Then
                Throw New InvalidOperationException("Cannot finalize an empty cart.")
            End If

            Dim creditAccount As CreditAccount = Nothing

            If creditAccountId.HasValue Then
                creditAccount = Await _db.CreditAccounts.FindAsync(creditAccountId.Value)
                If creditAccount Is Nothing Then
                    Throw New InvalidOperationException("Credit account not found.")
                End If
                If creditAccount.IsBlocked Then
                    Throw New InvalidOperationException("Credit account is blocked.")
                End If
            End If

            Dim nextTxNumber = Await GenerateNextTransactionNumberAsync()

            Dim transaction = New SalesTransaction With {
                .TransactionNumber = nextTxNumber,
                .TransactionDate = DateTime.Now,
                .TotalAmount = cart.TotalAmount,
                .PaymentMethod = paymentMethod,
                .IsVoid = False,
                .CreditAccount = creditAccount
            }

            For Each line In cart.Lines
                transaction.Lines.Add(New SalesTransactionLine With {
                    .ProductName = line.ProductName,
                    .UnitPrice = line.UnitPrice,
                    .Quantity = line.Quantity,
                    .LineTotal = line.LineTotal
                })
            Next

            If creditAccount IsNot Nothing Then
                creditAccount.CurrentBalance += cart.TotalAmount
                If creditAccount.CurrentBalance > 0 Then
                    creditAccount.IsBlocked = True
                End If
            End If

            _db.SalesTransactions.Add(transaction)
            Await _db.SaveChangesAsync()

            ClearCart(cartId)

            Return transaction
        End Function

        Public Async Function VoidTransactionAsync(transactionId As Integer) As Task Implements ICartService.VoidTransactionAsync
            Dim transaction = Await _db.SalesTransactions.Include(Function(t) t.CreditAccount).FirstOrDefaultAsync(Function(t) t.Id = transactionId)

            If transaction Is Nothing Then
                Throw New InvalidOperationException("Transaction not found.")
            End If

            If transaction.IsVoid Then
                Throw New InvalidOperationException("Transaction is already voided.")
            End If

            transaction.IsVoid = True

            If transaction.CreditAccount IsNot Nothing Then
                transaction.CreditAccount.CurrentBalance -= transaction.TotalAmount
                If transaction.CreditAccount.CurrentBalance <= 0 Then
                    transaction.CreditAccount.IsBlocked = False
                End If
            End If

            Await _db.SaveChangesAsync()
        End Function

        Public Async Function GetTransactionHistoryAsync() As Task(Of List(Of SalesTransaction)) Implements ICartService.GetTransactionHistoryAsync
            Dim transactionsAnonymous = Await _db.SalesTransactions _
                .Select(Function(t) New With {
                    t.Id,
                    t.TransactionNumber,
                    t.TransactionDate,
                    t.TotalAmount,
                    t.PaymentMethod,
                    t.IsVoid,
                    .CreditAccount = If(t.CreditAccount IsNot Nothing, New With { t.CreditAccount.Id, t.CreditAccount.CustomerName }, Nothing)
                }) _
                .OrderByDescending(Function(t) t.TransactionDate) _
                .ToListAsync()

            Dim transactions = New List(Of SalesTransaction)()
            For Each ta In transactionsAnonymous
                Dim t = New SalesTransaction With {
                    .Id = ta.Id,
                    .TransactionNumber = ta.TransactionNumber,
                    .TransactionDate = ta.TransactionDate,
                    .TotalAmount = ta.TotalAmount,
                    .PaymentMethod = ta.PaymentMethod,
                    .IsVoid = ta.IsVoid
                }
                If ta.CreditAccount IsNot Nothing Then
                    t.CreditAccount = New CreditAccount With {
                        .Id = ta.CreditAccount.Id,
                        .CustomerName = ta.CreditAccount.CustomerName
                    }
                End If
                transactions.Add(t)
            Next

            Return transactions
        End Function

        Private Async Function GenerateNextTransactionNumberAsync() As Task(Of String)
            Dim yearStr = DateTime.Now.Year.ToString()
            Dim prefix = $"TX-{yearStr}-"

            Dim latestTx = Await _db.SalesTransactions _
                .Where(Function(t) t.TransactionNumber.StartsWith(prefix)) _
                .OrderByDescending(Function(t) t.TransactionNumber) _
                .Select(Function(t) New With { t.TransactionNumber }) _
                .FirstOrDefaultAsync()

            Dim nextNum As Integer = 1
            If latestTx IsNot Nothing Then
                Dim lastNumStr = latestTx.TransactionNumber.Substring(latestTx.TransactionNumber.LastIndexOf("-"c) + 1)
                If Integer.TryParse(lastNumStr, nextNum) Then
                    nextNum += 1
                End If
            End If

            Return $"{prefix}{nextNum:D4}"
        End Function

    End Class

End Namespace
